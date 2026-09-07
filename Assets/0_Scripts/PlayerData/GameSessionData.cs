using System;

public static class GameSessionData
{
    public static int CollectedBranchCount { get; private set; }

    public static event Action<int> OnBranchCountChanged;

    public static void Initialize()
    {
        CollectedBranchCount = 0;
        OnBranchCountChanged?.Invoke(CollectedBranchCount);
    }

    public static void AddBranch(int amount = 1)
    {
        if (amount <= 0) return;

        CollectedBranchCount += amount;
        OnBranchCountChanged?.Invoke(CollectedBranchCount);
    }

    public static void Clear()
    {
        CollectedBranchCount = 0;
        OnBranchCountChanged?.Invoke(CollectedBranchCount);
    }
}
