public static class GameSessionRewardManager
{
    public static bool IsRewardConfirmed { get; private set; }

    public static int LastEarnedBranchCount { get; private set; }
    public static int LastTotalBranchCount { get; private set; }

    public static void Initialize()
    {
        IsRewardConfirmed = false;
        LastEarnedBranchCount = 0;
        LastTotalBranchCount = PlayerProfile.BranchCount;
    }

    public static bool ConfirmRewards()
    {
        if (IsRewardConfirmed) return false;

        int earnedBranchCount = GameSessionData.CollectedBranchCount;

        PlayerProfile.AddBranch(earnedBranchCount);
        LastEarnedBranchCount = earnedBranchCount;
        LastTotalBranchCount = PlayerProfile.BranchCount;

        SaveManager.Save();

        IsRewardConfirmed = true;

        GameSessionData.Clear();

        return true;
    }

    public static void DiscardReward()
    {
        if (IsRewardConfirmed) return;

        GameSessionData.Clear();

        LastEarnedBranchCount = 0;
        LastTotalBranchCount = PlayerProfile.BranchCount;
    }
}
