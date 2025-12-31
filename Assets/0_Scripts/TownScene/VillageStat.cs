using Photon.Pun;
using UnityEngine;

public static class VillageStat
{
    // 특정 건물의 현재 레벨을 안전하게 가져오기
    public static int GetUpgradeLevel(VillageUpgradeIndex type)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKeys.VillageUpgrades, out object village))
        {
            int[] upgradeList = (int[])village;
            // 배열 인덱스 안전 체크
            if (upgradeList != null && upgradeList.Length > (int)type)
            {
                return upgradeList[(int)type];
            }
        }
        return 0;
    }

    // 특정 건물의 다음 레벨 업그레이드 비용 계산
    public static int GetLevelUpgradedCost(VillageUpgradeIndex facilityType)
    {
        int currentLevel = GetUpgradeLevel(facilityType);
        // 공식: baseCost * (multiplier ^ currentLevel)
        return (int)Mathf.Round(CommonDefine.villageUpgradeBaseCost[(int)facilityType] * Mathf.Pow(CommonDefine.villageUpgradeCostMultiplier, currentLevel));
    }

    // 1. 일일 골드 수입 (Mine 레벨 비례)
    public static int GetGoldIncomePerDay()
    {
        int level = GetUpgradeLevel(VillageUpgradeIndex.Mine);
        return CommonDefine.defaultVillageGold + (level * CommonDefine.villageIncomePerLevel);
    }

    // 2. 최대 에너지 (House 레벨 비례)
    public static float GetMaxEnergy()
    {
        int level = GetUpgradeLevel(VillageUpgradeIndex.House);
        return CommonDefine.defaultPlayerMaxEnergy + (level * CommonDefine.villageMaxEnergyPerLevel);
    }

    // 3. 공격력 (Forge 레벨 비례)
    public static float GetPlayerAtkPow()
    {
        int level = GetUpgradeLevel(VillageUpgradeIndex.Forge);
        //범위가 늘어나는 방식으로 수정 필요
        return CommonDefine.defaultPlayerMaxAtkPow + (level * CommonDefine.villageAtkPowerPerLevel);
    }

    // 4. 일일 에너지 회복량 (Farm 레벨 비례)
    public static float GetEnergyRegenPerDay()
    {
        int level = GetUpgradeLevel(VillageUpgradeIndex.Farm);
        return CommonDefine.defaultPlayerRegenEnergyPerDay + (level * CommonDefine.villageEnergyRegenPerDay);
    }
}