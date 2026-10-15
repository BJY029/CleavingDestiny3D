using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Potan.CoreUtils;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

// UGS 초기화, 로그인, 지갑·가격 조회, 보상·구매 정산을 한 싱글톤에서 처리합니다.
public class UGSManager : MonoSingleton<UGSManager>
{
    // 서버 잔액과 이번 요청의 실제 지급량은 서로 구분해서 사용합니다.
    [Serializable]
    public class WalletResponse
    {
        public bool ready;
        public bool accepted;
        public string rejectionReason;
        public string settlementId;
        // 응답에 잔액이 없는 경우를 실제 잔액 0과 구분하기 위한 기본값입니다.
        public int coins = -1;
        public int sticks = -1;
        public int awardedCoins, awardedSticks;
        // 보상 정산 응답에는 없을 수 있으므로 누락 시 기존 구매 목록을 유지합니다.
        public string[] ownedItems;
    }

    // PurchaseSkin은 ready/accepted 대신 success/purchased/status를 반환합니다.
    [Serializable]
    public class PurchaseResponse
    {
        public bool success, purchased;
        public string status, skinId, currency;
        public int price = -1;
        public int coins = -1;
        public int sticks = -1;
        public string[] ownedItems;
    }

    private struct ShopAttributes { }
    private Task catalogLoading;
    private Dictionary<string, AxeShopPrice> skinPrices = new();
    public bool IsWalletReady { get; private set; }
    public event Action WalletChanged;

    // 재시도할 때도 ID, 시각, 획득량과 승패를 그대로 유지해야 중복 지급을 막을 수 있습니다.
    [Serializable]
    public class RewardRequest
    {
        public string settlementId, submittedAt, result;
        public int collectedSticks;
    }

    // 서버 응답을 받지 못한 요청 목록이며 재화 잔액의 원본이 아닙니다.
    [Serializable]
    private class PendingRewards
    {
        public List<RewardRequest> requests = new();
    }

    public string PlayerId { get; private set; }
    // 서버에서 확인한 잔액의 메모리 캐시입니다.
    public Dictionary<string, int> PlayerData { get; private set; } = new();
    public static HashSet<string> PlayerDataKeys { get; } = new() { "coins", "sticks", "ownedItems" };
    // 지급 승인뿐 아니라 서버가 보상을 거부한 경우에도 호출됩니다.
    public event Action<WalletResponse> RewardSettled;

    // 여러 호출자가 같은 초기화 작업을 기다려 중복 초기화와 로그인을 피합니다.
    private Task initialization;
    // 정산과 조회를 순서대로 처리해 대기 목록과 잔액 캐시의 갱신이 겹치지 않도록 합니다.
    private readonly SemaphoreSlim settlementGate = new(1, 1);
    private PendingRewards pending;
    // 현재 실행 중 이미 처리한 ID는 서버 재호출 없이 이전 응답을 반환합니다.
    private readonly Dictionary<string, WalletResponse> completed = new();
    // 다른 계정의 대기 요청이 섞이지 않도록 PlayerId별로 로컬 저장 키를 나눕니다.
    private string PendingKey => "UGS.PendingRewards." + PlayerId;

    private async void Start()
    {
        if (!IsValidInstance) return;
        try { await EnsureInitializedAsync(); }
        catch (Exception e) { Debug.LogException(e); }
    }

    // 완료된 초기화는 재사용하고, 실패하거나 취소된 경우에만 다시 시작합니다.
    public Task EnsureInitializedAsync()
    {
        if (initialization == null || initialization.IsFaulted || initialization.IsCanceled)
            initialization = InitializeAsync();
        return initialization;
    }

    private async Task InitializeAsync()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        PlayerId = AuthenticationService.Instance.PlayerId;

        // 로그인한 계정의 미완료 요청을 복구합니다. 손상된 데이터는 덮어쓰지 않습니다.
        string json = PlayerPrefs.GetString(PendingKey, "");
        pending = string.IsNullOrEmpty(json) ? new PendingRewards() : JsonUtility.FromJson<PendingRewards>(json);

        if (pending == null || pending.requests == null)
            throw new InvalidOperationException("Pending reward data is invalid; do not overwrite it.");

        // 초기화 응답에 잔액이 포함되면 별도 Cloud Save 조회 없이 캐시를 갱신합니다.
        var response = await CloudCodeService.Instance.CallEndpointAsync<WalletResponse>(
            "InitWallet", new Dictionary<string, object>());

        if (response == null || !response.ready)
            throw new InvalidOperationException("Wallet initialization failed.");

        // 잔액을 반환하지 않는 이전 InitWallet 스크립트와의 호환 처리입니다.
        if (response.coins == -1 || response.sticks == -1 || response.ownedItems == null)
        {
            Debug.LogWarning("[UGS] InitWallet 응답에 잔액 또는 ownedItems가 없습니다. Protected 데이터에서 읽습니다.");
            await ReadWalletAsync();
        }
        else ApplyWallet(response);

    }

    // 요청을 먼저 로컬에 저장한 뒤 전송해 응답 유실 시 같은 ID로 재시도할 수 있게 합니다.
    public async Task<WalletResponse> SubmitMatchRewardAsync(RewardRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.settlementId))
            throw new ArgumentException("A reward request with a stable settlement ID is required.", nameof(request));
        try { await EnsureInitializedAsync(); }
        catch
        {
            // 인증 이후 지갑 초기화에 실패한 경우에도 정산 요청은 보관합니다.
            if (pending != null && !string.IsNullOrEmpty(PlayerId)) QueueReward(request);
            throw;
        }
        await settlementGate.WaitAsync();
        try
        {
            if (completed.TryGetValue(request.settlementId, out var previous)) return previous;
            QueueReward(request);
            return await ProcessPendingAsync(request.settlementId);
        }
        finally { settlementGate.Release(); }
    }

    // 앱을 종료하거나 네트워크가 끊겨도 남은 요청을 재시도합니다. 통신 예외가 발생하면 중단하고 다음 기회에 다시 시도합니다.
    public async Task RetryPendingRewardsAsync()
    {
        await EnsureInitializedAsync();
        await settlementGate.WaitAsync();
        try { await ProcessPendingAsync(); }
        finally { settlementGate.Release(); }
    }

    // 오래된 요청부터 처리하며 통신 예외가 발생하면 해당 요청을 남기고 중단합니다.
    private async Task<WalletResponse> ProcessPendingAsync(string requestedId = null)
    {
        WalletResponse requestedResponse = null;
        while (pending.requests.Count > 0)
        {
            var request = pending.requests[0];
            var response = await CallMatchRewardAsync(request);
            if (response == null || response.settlementId != request.settlementId)
                throw new InvalidOperationException("Unexpected reward settlement response.");
            ApplyWallet(response);

            // 서버가 지급하거나 거부한 요청은 처리 완료입니다. 통신 실패 요청만 재시도 대상으로 남깁니다.
            pending.requests.RemoveAt(0);
            SavePending();
            completed[request.settlementId] = response;
            if (request.settlementId == requestedId) requestedResponse = response;
            RewardSettled?.Invoke(response);
        }
        return requestedResponse;
    }

    // 서버는 로그인한 플레이어를 식별하므로 PlayerId나 최종 잔액을 입력으로 보내지 않습니다.
    private Task<WalletResponse> CallMatchRewardAsync(RewardRequest request)
    {
        return CloudCodeService.Instance.CallEndpointAsync<WalletResponse>(
            "SubmitMatchReward", new Dictionary<string, object>
            {
                { "settlementId", request.settlementId },
                { "submittedAt", request.submittedAt },
                { "collectedSticks", request.collectedSticks },
                { "result", request.result }
            });
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    // 중복 지급 테스트는 캐시를 우회해 서버에 다시 요청하되 로컬 상점 보상은 다시 더하지 않습니다.
    public async UniTask<WalletResponse> ReplayCompletedRewardForTestAsync(RewardRequest request)
    {
        await EnsureInitializedAsync();
        await settlementGate.WaitAsync();
        try
        {
            if (request == null || !completed.ContainsKey(request.settlementId))
                throw new InvalidOperationException("Submit this reward successfully before replaying it.");
            var response = await CallMatchRewardAsync(request);
            if (response == null || response.settlementId != request.settlementId)
                throw new InvalidOperationException("Unexpected reward settlement response.");
            ApplyWallet(response);
            return response;
        }
        finally { settlementGate.Release(); }
    }
#endif

    // 잘못된 응답으로 정상 잔액 캐시가 덮어써지지 않도록 검증 후 함께 반영합니다.
    private void ApplyWallet(WalletResponse response)
    {
        if (response == null || !response.ready || response.coins < 0 || response.sticks < 0 ||
            response.awardedCoins < 0 || response.awardedSticks < 0)
            throw new InvalidOperationException("Invalid wallet response.");

        SaveManager.Initialize();
        PlayerProfile.ApplyServerWallet(response.sticks, response.ownedItems);
        PlayerData["coins"] = response.coins;
        PlayerData["sticks"] = response.sticks;
        IsWalletReady = true;
        SaveManager.Save();
        WalletChanged?.Invoke();
    }

    // 가격표는 상점 첫 진입에 한 번만 받아 캐시합니다. 실패한 경우 다음 진입 때 재시도합니다.
    public Task LoadSkinCatalogAsync()
    {
        if (catalogLoading == null || catalogLoading.IsFaulted || catalogLoading.IsCanceled)
            catalogLoading = FetchSkinCatalogAsync();
        return catalogLoading;
    }

    private async Task FetchSkinCatalogAsync()
    {
        await EnsureInitializedAsync();
        var config = await RemoteConfigService.Instance.FetchConfigsAsync(new ShopAttributes(), new ShopAttributes());
        skinPrices = AxeShopPrice.ParseCatalog(config.GetJson("SKIN_CATALOG"));
    }

    public bool TryGetSkinPrice(string skinId, out AxeShopPrice price) => skinPrices.TryGetValue(skinId, out price);

    // 가격·재화·플레이어 ID는 보내지 않습니다. 서버의 SKIN_CATALOG와 로그인 계정으로 처리합니다.
    public async Task<PurchaseResponse> PurchaseSkinAsync(string skinId)
    {
        if (string.IsNullOrWhiteSpace(skinId)) throw new ArgumentException("Skin ID is required.", nameof(skinId));
        await EnsureInitializedAsync();
        await settlementGate.WaitAsync();
        try
        {
            var response = await CloudCodeService.Instance.CallEndpointAsync<PurchaseResponse>(
                "PurchaseSkin", new Dictionary<string, object> { { "skinId", skinId } });
            if (response == null || response.skinId != skinId)
                throw new InvalidOperationException("Unexpected PurchaseSkin response.");
            // skin_not_found 응답에는 잔액과 인벤토리가 포함되지 않습니다.
            if (response.status == "skin_not_found" && !response.success && !response.purchased)
            {
                skinPrices.Remove(skinId);
                return response;
            }
            bool validStatus = (response.status == "purchased" && response.success && response.purchased) ||
                (response.status == "already_owned" && response.success && !response.purchased) ||
                (response.status == "insufficient_funds" && !response.success && !response.purchased);
            if (!validStatus || response.ownedItems == null || response.price < 0 ||
                (response.currency != "coins" && response.currency != "sticks") ||
                (response.success && !Array.Exists(response.ownedItems, id => id == skinId)))
                throw new InvalidOperationException("Invalid PurchaseSkin result.");
            skinPrices[skinId] = new AxeShopPrice(response.currency, response.price);
            ApplyWallet(new WalletResponse
            {
                ready = true, coins = response.coins, sticks = response.sticks, ownedItems = response.ownedItems
            });
            return response;
        }
        finally { settlementGate.Release(); }
    }

    // 앱을 종료해도 요청이 남도록 즉시 저장합니다. PlayerPrefs는 신뢰할 수 있는 서버 기록이 아닙니다.
    private void SavePending()
    {
        PlayerPrefs.SetString(PendingKey, JsonUtility.ToJson(pending));
        PlayerPrefs.Save();
    }

    // 같은 정산 ID의 요청을 대기 목록에 중복으로 넣지 않습니다.
    private void QueueReward(RewardRequest request)
    {
        if (pending.requests.Exists(item => item.settlementId == request.settlementId)) return;
        pending.requests.Add(request);
        SavePending();
    }

    // 명시적인 잔액 조회는 초기화 스크립트 응답 대신 실제 저장된 Protected 값을 읽습니다.
    public async UniTask LoadPlayerData()
    {
        await EnsureInitializedAsync();

        await settlementGate.WaitAsync();

        try { await ReadWalletAsync(); }
        finally { settlementGate.Release(); }
    }

    // 필수 키가 없으면 0으로 처리하지 않고 계정이나 환경 설정을 확인할 수 있도록 오류를 냅니다.
    private async UniTask ReadWalletAsync()
    {
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(
            PlayerDataKeys, new LoadOptions(new ProtectedReadAccessClassOptions()));

        if (!data.TryGetValue("coins", out var coins) || !data.TryGetValue("sticks", out var sticks) ||
            !data.TryGetValue("ownedItems", out var ownedItems))
            throw new InvalidOperationException("Protected coins/sticks/ownedItems are missing. Check InitWallet and the signed-in player/environment.");

        ApplyWallet(new WalletResponse
        {
            ready = true,
            coins = coins.Value.GetAs<int>(),
            sticks = sticks.Value.GetAs<int>(),
            ownedItems = ownedItems.Value.GetAs<string[]>()
        });
    }
}
