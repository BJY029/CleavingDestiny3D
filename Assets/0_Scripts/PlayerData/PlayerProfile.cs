using System;
using System.Collections.Generic;
using System.Linq;

public static class PlayerProfile
{
    public static int BranchCount { get; private set; }
    public static IReadOnlyList<string> OwnedAxeSkinIds => ownedAxeSkinIds;
    public static string EquippedAxeSkinId { get; private set; }
    public static event Action<int> OnBranchCountChanged;
    private static readonly List<string> ownedAxeSkinIds = new();

    public static void Initialize(PlayerSaveData saveData)
    {
        BranchCount = saveData.branchCount;

        ownedAxeSkinIds.Clear();
        ownedAxeSkinIds.AddRange(saveData.ownedAxeSkinIdx);

        EquippedAxeSkinId = saveData.equippedAxeSkinId;

        OnBranchCountChanged?.Invoke(BranchCount);
    }

    public static PlayerSaveData CreateSaveData()
    {
        return new PlayerSaveData
        {
            branchCount = BranchCount,
            ownedAxeSkinIdx = new List<string>(ownedAxeSkinIds),
            equippedAxeSkinId = EquippedAxeSkinId
        };
    }

    public static void AddBranch(int amount)
    {
        if (amount <= 0) return;

        BranchCount += amount;

        OnBranchCountChanged?.Invoke(BranchCount);
    }

    public static bool TrySpendBranch(int amount)
    {
        if (amount <= 0) return false;

        if (BranchCount < amount) return false;

        BranchCount -= amount;

        OnBranchCountChanged?.Invoke(BranchCount);
        return true;
    }

    public static bool OwnsAxeSkin(string skinId)
    {
        return OwnedAxeSkinIds.Contains(skinId);
    }

    public static bool AddAxeSkin(string skinId)
    {
        if (string.IsNullOrEmpty(skinId)) return false;

        if (ownedAxeSkinIds.Contains(skinId)) return false;

        ownedAxeSkinIds.Add(skinId);
        return true;
    }

    public static bool EquipAxeSkin(string skinId)
    {
        if (!ownedAxeSkinIds.Contains(skinId)) return false;

        EquippedAxeSkinId = skinId;
        return true;
    }
}
