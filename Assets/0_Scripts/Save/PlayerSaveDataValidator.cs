using System.Collections.Generic;

public static class PlayerSaveDataValidator
{
    private const int CurrentVersion = 1;
    private const string DefaultAxeSkinId = "axe_basic";

    public static bool ValidateAndRepair(PlayerSaveData data)
    {
        if (data == null) return false;

        if (data.version <= 0) data.version = CurrentVersion;

        if (data.branchCount < 0) data.branchCount = 0;

        data.ownedAxeSkinIdx ??= new List<string>();

        RemoveInvalidAndDuplicateSkinIds(data.ownedAxeSkinIdx);

        if (!data.ownedAxeSkinIdx.Contains(DefaultAxeSkinId))
        {
            data.ownedAxeSkinIdx.Add(DefaultAxeSkinId);
        }

        if (string.IsNullOrWhiteSpace(data.equippedAxeSkinId))
        {
            data.equippedAxeSkinId = DefaultAxeSkinId;
        }

        if (!data.ownedAxeSkinIdx.Contains(data.equippedAxeSkinId))
        {
            data.equippedAxeSkinId = DefaultAxeSkinId;
        }

        return true;
    }

    private static void RemoveInvalidAndDuplicateSkinIds(List<string> skinIds)
    {
        HashSet<string> uniqueIds = new();

        for (int i = skinIds.Count - 1; i >= 0; i--)
        {
            string skinId = skinIds[i];

            //빈 문자열일 경우 제거
            if (string.IsNullOrWhiteSpace(skinId))
            {
                skinIds.RemoveAt(i);
                continue;
            }

            //중복된 id값인 경우 제거
            if (!uniqueIds.Add(skinId))
            {
                skinIds.RemoveAt(i);
            }
        }
    }
}
