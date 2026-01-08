using Photon.Realtime;

namespace Village
{
    public interface IVillageStatProvider
    {
        int GetVillageLevel(VillageType type, Player targetPlayer = null);
        int GetLevelUpgradedCost(VillageType facilityType, Player targetPlayer = null);
        int GetLevelUpgradedCost(VillageType facilityType, int currentLevel, Player targetPlayer = null);

        int GetGoldIncomePerDay(Player targetPlayer = null);
        float GetMaxEnergy(Player targetPlayer = null);
        float GetEnergyIncomePerDay(Player targetPlayer = null);
        float GetBarrierArmor(Player targetPlayer = null);
    }
}
