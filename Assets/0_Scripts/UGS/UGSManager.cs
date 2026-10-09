using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Potan.CoreUtils;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Core;
using UnityEngine;

public class UGSManager : MonoSingleton<UGSManager>
{
    [Serializable]
    public class WalletResponse
    {
        public bool ready;
        public bool accepted;
        public string rejectionReason;
        public string settlementId;
        // Missing JSON balances must not silently become a valid zero wallet.
        public int coins = -1;
        public int sticks = -1;
        public int awardedCoins, awardedSticks;
    }

    [Serializable]
    public class RewardRequest
    {
        public string settlementId, submittedAt, result;
        public int collectedSticks;
    }

    [Serializable]
    private class PendingRewards
    {
        public List<RewardRequest> requests = new();
    }

    public string PlayerId { get; private set; }
    public Dictionary<string, int> PlayerData { get; private set; } = new();
    public static HashSet<string> PlayerDataKeys { get; } = new() { "coins", "sticks" };
    public event Action<WalletResponse> RewardSettled;

    private Task initialization;
    private readonly SemaphoreSlim settlementGate = new(1, 1);
    private PendingRewards pending;
    private readonly Dictionary<string, WalletResponse> completed = new();
    private string PendingKey => "UGS.PendingRewards." + PlayerId;

    private async void Start()
    {
        if (!IsValidInstance) return;
        try { await EnsureInitializedAsync(); }
        catch (Exception e) { Debug.LogException(e); }
    }

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
        string json = PlayerPrefs.GetString(PendingKey, "");
        pending = string.IsNullOrEmpty(json) ? new PendingRewards() : JsonUtility.FromJson<PendingRewards>(json);
        if (pending == null || pending.requests == null)
            throw new InvalidOperationException("Pending reward data is invalid; do not overwrite it.");
        var response = await CloudCodeService.Instance.CallEndpointAsync<WalletResponse>(
            "InitWallet", new Dictionary<string, object>());
        if (response == null || !response.ready)
            throw new InvalidOperationException("Wallet initialization failed.");
        if (response.coins == -1 || response.sticks == -1)
        {
            Debug.LogWarning("[UGS] InitWallet response is missing coins/sticks. Publish the updated InitWallet.js. Reading Protected wallet directly for compatibility.");
            await ReadWalletAsync();
        }
        else ApplyWallet(response);

    }

    public async Task<WalletResponse> SubmitMatchRewardAsync(RewardRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.settlementId))
            throw new ArgumentException("A reward request with a stable settlement ID is required.", nameof(request));
        try { await EnsureInitializedAsync(); }
        catch
        {
            // Preserve the result even if wallet initialization failed after authentication.
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

    public async Task RetryPendingRewardsAsync()
    {
        await EnsureInitializedAsync();
        await settlementGate.WaitAsync();
        try { await ProcessPendingAsync(); }
        finally { settlementGate.Release(); }
    }

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

            // ponytail: cosmetic shop spending remains local; migrate purchases before treating it as a secure wallet.
            if (response.accepted && response.awardedSticks > 0)
            {
                SaveManager.Initialize();
                PlayerProfile.AddBranch(response.awardedSticks);
                SaveManager.Save();
            }

            pending.requests.RemoveAt(0);
            SavePending();
            completed[request.settlementId] = response;
            if (request.settlementId == requestedId) requestedResponse = response;
            RewardSettled?.Invoke(response);
        }
        return requestedResponse;
    }

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
    // Reach the server again instead of returning the client cache. Never credit the local shop again.
    public async Task<WalletResponse> ReplayCompletedRewardForTestAsync(RewardRequest request)
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

    private void ApplyWallet(WalletResponse response)
    {
        if (response == null || !response.ready || response.coins < 0 || response.sticks < 0 ||
            response.awardedCoins < 0 || response.awardedSticks < 0)
            throw new InvalidOperationException("Invalid wallet response.");
        PlayerData["coins"] = response.coins;
        PlayerData["sticks"] = response.sticks;
    }

    private void SavePending()
    {
        PlayerPrefs.SetString(PendingKey, JsonUtility.ToJson(pending));
        PlayerPrefs.Save();
    }

    private void QueueReward(RewardRequest request)
    {
        if (pending.requests.Exists(item => item.settlementId == request.settlementId)) return;
        pending.requests.Add(request);
        SavePending();
    }

    // Explicit refresh reads persisted balances, independent of the initialization script's response.
    public async Awaitable LoadPlayerData()
    {
        await EnsureInitializedAsync();
        await settlementGate.WaitAsync();
        try { await ReadWalletAsync(); }
        finally { settlementGate.Release(); }
    }

    private async Task ReadWalletAsync()
    {
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(
            PlayerDataKeys, new LoadOptions(new ProtectedReadAccessClassOptions()));
        if (!data.TryGetValue("coins", out var coins) || !data.TryGetValue("sticks", out var sticks))
            throw new InvalidOperationException("Protected coins/sticks are missing for the signed-in player. Check the Cloud Save player and environment.");
        ApplyWallet(new WalletResponse
        {
            ready = true,
            coins = coins.Value.GetAs<int>(),
            sticks = sticks.Value.GetAs<int>()
        });
    }
}
