using System;
using System.Collections.Generic;
using System.Linq;

public static class PlayerProfile
{
    public static int BranchCount { get; private set; }
    public static IReadOnlyList<string> OwnedAxeSkinIds => ownedAxeSkinIds;
    public static string EquippedAxeSkinId { get; private set; }

    public static event Action<int> OnBranchCountChanged;
    public static event Action OnAxeSkinChanged;


    private static readonly List<string> ownedAxeSkinIds = new();

    public static void Initialize(PlayerSaveData saveData)
    {
        BranchCount = saveData.branchCount;

        ownedAxeSkinIds.Clear();

        if (saveData.ownedAxeSkinIdx != null)
            ownedAxeSkinIds.AddRange(saveData.ownedAxeSkinIdx);

        EquippedAxeSkinId = saveData.equippedAxeSkinId;

        OnBranchCountChanged?.Invoke(BranchCount);
        OnAxeSkinChanged?.Invoke();
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

    public static void InitBranch()
    {
        BranchCount = 0;

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
        if (string.IsNullOrWhiteSpace(skinId)) return false;

        return OwnedAxeSkinIds.Contains(skinId);
    }

    public static bool AddAxeSkin(string skinId)
    {
        if (string.IsNullOrEmpty(skinId)) return false;

        if (ownedAxeSkinIds.Contains(skinId)) return false;

        ownedAxeSkinIds.Add(skinId);
        return true;
    }

    // 서버 잔액과 구매 목록을 로컬 표시용으로 반영합니다. 로컬 데이터는 서버로 업로드하지 않습니다.
    public static void ApplyServerWallet(int sticks, IReadOnlyList<string> ownedItems)
    {
        if (sticks < 0) throw new ArgumentOutOfRangeException(nameof(sticks));
        List<string> serverSkins = null;
        if (ownedItems != null)
        {
            if (ownedItems.Any(string.IsNullOrWhiteSpace))
                throw new InvalidOperationException("Invalid server-owned item ID.");
            serverSkins = ownedItems.Distinct(StringComparer.Ordinal).ToList();
            // 기본 도끼는 무료 기본 외형으로 항상 사용할 수 있습니다.
            if (!serverSkins.Contains("axe_basic")) serverSkins.Insert(0, "axe_basic");
        }
        bool balanceChanged = BranchCount != sticks;
        bool skinsChanged = serverSkins != null && !ownedAxeSkinIds.SequenceEqual(serverSkins);
        BranchCount = sticks;
        if (serverSkins != null)
        {
            ownedAxeSkinIds.Clear();
            ownedAxeSkinIds.AddRange(serverSkins);
            if (!ownedAxeSkinIds.Contains(EquippedAxeSkinId))
            {
                EquippedAxeSkinId = "axe_basic";
                skinsChanged = true;
            }
        }
        if (balanceChanged) OnBranchCountChanged?.Invoke(BranchCount);
        if (skinsChanged) OnAxeSkinChanged?.Invoke();
    }

    public static bool EquipAxeSkin(string skinId)
    {
        if (string.IsNullOrWhiteSpace(skinId)) return false;
        if (!ownedAxeSkinIds.Contains(skinId)) return false;
        if (EquippedAxeSkinId == skinId) return false;

        EquippedAxeSkinId = skinId;

        OnAxeSkinChanged?.Invoke();

        return true;
    }
}
