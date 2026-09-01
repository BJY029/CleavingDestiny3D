public static class GameSessionData
{
    public static int CollectedBranchCount { get; private set; }

    public static void Initialize()
    {
        CollectedBranchCount = 0;
    }

    public static void AddBranch(int amount = 1)
    {
        if (amount <= 0) return;

        CollectedBranchCount += amount;
    }

    public static void Clear()
    {
        CollectedBranchCount = 0;
    }
}
