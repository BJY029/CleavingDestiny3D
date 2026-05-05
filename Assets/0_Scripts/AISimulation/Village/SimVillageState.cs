using System.Collections.Generic;

public class VillageObjInfo
{
    public VillageLevelData levelData;
    public int currentLevel;

    public VillageObjInfo(VillageLevelData data, int level)
    {
        this.levelData = data;
        this.currentLevel = level;
    }

}

public class SimVillageState
{
    public List<VillageObjInfo> villageObjInfos;

    public int p1MaxEnergy, p2MaxEnergy;
    public int p1MaxHitDmg, p2MaxHitDmg;
    public int p1MinHitDmg, p2MinHitDmg;
    public int p1BasicVillageBarrier, p2BasicVillageBarrier;
    public int p1VillGold, p2VillGold;

    public SimVillageState(VillageBalanceData balanceData, VillageLevelData[] levelDatas)
    {
        p1MaxEnergy = p2MaxEnergy = (int)balanceData.EnergyIncomeBase;
        p1MaxHitDmg = p2MaxEnergy = (int)(balanceData.AxeDamageBase * balanceData.AxeDamageMaxMultiplier);
        p1MinHitDmg = p2MinHitDmg = (int)(balanceData.AxeDamageBase * balanceData.AxeDamageMinMultiplier);
        p1BasicVillageBarrier = p2BasicVillageBarrier = (int)balanceData.BarrierArmorBase;
        p1VillGold = p2VillGold = (int)balanceData.GoldIncomeBase * 2;

        for (int i = 0; i < levelDatas.Length; i++)
        {
            VillageObjInfo info = new VillageObjInfo(levelDatas[i], 1);
            villageObjInfos.Add(info);
        }
    }
}
