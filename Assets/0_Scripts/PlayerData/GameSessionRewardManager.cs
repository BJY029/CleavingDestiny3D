using System;
using Photon.Pun;
using UnityEngine;

public static class GameSessionRewardManager
{
    public static bool IsRewardConfirmed { get; private set; }
    public static bool IsRewardPending { get; private set; }
    public static string RewardError { get; private set; }
    public static int LastEarnedBranchCount { get; private set; }
    public static int LastTotalBranchCount { get; private set; }
    public static int LastEarnedCoins { get; private set; }
    public static event Action RewardStateChanged;

    private static string settlementId;
    private static UGSManager.RewardRequest request;
    private static bool isSubmitting;
    private static UGSManager subscribedManager;

    public static void Initialize()
    {
        settlementId = Guid.NewGuid().ToString("N");
        request = null;
        isSubmitting = false;
        IsRewardConfirmed = false;
        IsRewardPending = false;
        RewardError = null;
        LastEarnedBranchCount = 0;
        LastEarnedCoins = 0;
        LastTotalBranchCount = PlayerProfile.BranchCount;
        if (subscribedManager != null) subscribedManager.RewardSettled -= OnRewardSettled;
        subscribedManager = UGSManager.Instance;
        subscribedManager.RewardSettled += OnRewardSettled;
    }

    public static async void ConfirmRewards(int loserActorNumber)
    {
        if (IsRewardConfirmed || isSubmitting) return;
        request ??= new UGSManager.RewardRequest
        {
            settlementId = settlementId,
            submittedAt = DateTime.UtcNow.ToString("O"),
            collectedSticks = GameSessionData.CollectedBranchCount,
            result = loserActorNumber == -1 ? "draw" :
                loserActorNumber == PhotonNetwork.LocalPlayer.ActorNumber ? "loss" : "win"
        };
        string submittingId = request.settlementId;
        isSubmitting = true;
        IsRewardPending = true;
        RewardError = null;
        RewardStateChanged?.Invoke();
        try
        {
            var response = await UGSManager.Instance.SubmitMatchRewardAsync(request);
            if (response != null && !IsRewardConfirmed) OnRewardSettled(response);
        }
        catch (Exception e)
        {
            if (settlementId == submittingId)
            {
                RewardError = "정산 실패 · 로비에서 재시도";
                Debug.LogException(e);
            }
        }
        finally
        {
            if (settlementId == submittingId)
            {
                isSubmitting = false;
                RewardStateChanged?.Invoke();
            }
        }
    }

    private static void OnRewardSettled(UGSManager.WalletResponse response)
    {
        if (response.settlementId != settlementId) return;
        IsRewardPending = false;
        IsRewardConfirmed = true;
        LastEarnedBranchCount = response.awardedSticks;
        LastEarnedCoins = response.awardedCoins;
        LastTotalBranchCount = response.sticks;
        RewardError = response.accepted ? null : "보상 미지급: " + response.rejectionReason;
        GameSessionData.Clear();
        RewardStateChanged?.Invoke();
    }

    public static void RetryCurrentReward()
    {
        // Covers a result created before the first successful authentication.
        // ConfirmRewards keeps the original request, including its result and ID.
        if (request != null && !IsRewardConfirmed && !isSubmitting) ConfirmRewards(0);
    }

    public static void DiscardReward()
    {
        // Leaving a completed match must not erase its pending settlement.
        if (IsRewardConfirmed || IsRewardPending) return;
        GameSessionData.Clear();
        LastEarnedBranchCount = 0;
        LastTotalBranchCount = PlayerProfile.BranchCount;
    }
}
