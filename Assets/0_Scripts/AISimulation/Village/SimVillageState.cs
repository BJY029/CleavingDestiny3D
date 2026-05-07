using System;
using System.Collections.Generic;
using UnityEngine;

public enum VStateType
{
    MaxEnergy, DayEnergy, MaxHitDamage, MinHitDamage, VBarrier, VGold, VIncomeGold
}

public class VDesc
{
    public string MineDesc = "일일 골드 수입 : {0}";
    public string FarmDesc = "일일 기력 회복량 : {0}, 최대 기력 소유 가능량 : {1}";
    public string BarrierDesc = "일일 기본 방어력 : {0}";
    public string ForgeDesc = "최대 데미지 : {0}, 최소 데미지 : {1}";
}
public class VillageObjInfo
{
    public VillageLevelData _levelData;
    private VillageBalanceData _balanceData;
    public string curLevelDesc { get; private set; }
    public string nextLevelDesc { get; private set; }
    public int upgradeGold;

    private int _currentLevel;
    public int currentLevel
    {
        get => _currentLevel;
        set
        {
            // 값이 같으면 무시
            if (_currentLevel == value) return;

            _currentLevel = value;

            // 레벨이 변경될 때마다 텍스트를 자동으로 갱신합니다.
            curLevelDesc = GetVillDesc(_currentLevel, _balanceData);
            nextLevelDesc = GetVillDesc(_currentLevel + 1, _balanceData);
            if (_levelData.TryGetUpgradeCost(currentLevel - 1, out int goldV)) upgradeGold = goldV;
        }
    }

    public VillageObjInfo(VillageLevelData data, int level, VillageBalanceData balanceData)
    {
        this._levelData = data;
        this._balanceData = balanceData;
        this.currentLevel = level;
        if (data.TryGetUpgradeCost(level - 0, out int goldV)) this.upgradeGold = goldV;
        this.curLevelDesc = GetVillDesc(level, balanceData);
        this.nextLevelDesc = GetVillDesc(level + 1, balanceData);
    }

    public string GetVillDesc(int level, VillageBalanceData balanceData)
    {
        VDesc vDesc = new VDesc();

        if (level > 5) return "Max Level";
        if (_levelData.VillageType == VillageType.Mine)
        {
            int goldIncome = _levelData.EffectValues[level - 1];
            return string.Format(vDesc.MineDesc, goldIncome);
        }
        else if (_levelData.VillageType == VillageType.Farm)
        {
            int MaxEng = Mathf.RoundToInt(balanceData.EnergyMaxBase + (level * balanceData.EnergyMaxMultiplier));
            int DayEng = Mathf.RoundToInt(balanceData.EnergyIncomeBase + (level * balanceData.EnergyIncomeMultiplier));
            return string.Format(vDesc.FarmDesc, DayEng, MaxEng);
        }
        else if (_levelData.VillageType == VillageType.Barrier)
        {
            int barrierIncome = _levelData.EffectValues[level - 1];
            return string.Format(vDesc.BarrierDesc, barrierIncome);
        }
        else if (_levelData.VillageType == VillageType.Forge)
        {
            int MaxDmg = Mathf.RoundToInt(balanceData.AxeDamageBase - ((level + 1) * balanceData.AxeDamageLevelMultiplier * balanceData.AxeDamageMinMultiplier));
            int MinDmg = Mathf.RoundToInt(balanceData.AxeDamageBase + ((level + 1) * balanceData.AxeDamageLevelMultiplier * balanceData.AxeDamageMaxMultiplier));
            return string.Format(vDesc.ForgeDesc, MaxDmg, MinDmg);
        }
        return "error";
    }
}

public class SimVillageState
{
    public List<VillageObjInfo> P1VillageObjInfos;
    public List<VillageObjInfo> P2VillageObjInfos;
    private VillageBalanceData _villageBalanceData;

    public static event Action<int, VStateType, int> OnVStatChange;
    public static event Action<int, VillageObjInfo> OnVillageObjChanged;

    private int _p1MaxEnergy;
    public int p1MaxEnergy
    {
        get => _p1MaxEnergy;
        set
        {
            if (value == _p1MaxEnergy) return;
            _p1MaxEnergy = value;
            OnVStatChange?.Invoke(1, VStateType.MaxEnergy, _p1MaxEnergy);
        }
    }

    private int _p1DayEnergy;
    public int p1DayEnergy
    {
        get => _p1DayEnergy;
        set
        {
            if (value == _p1DayEnergy) return;
            _p1DayEnergy = value;
            OnVStatChange?.Invoke(1, VStateType.DayEnergy, _p1DayEnergy);
        }
    }

    private int _p2MaxEnergy;
    public int p2MaxEnergy
    {
        get => _p2MaxEnergy;
        set
        {
            if (value == _p2MaxEnergy) return;
            _p2MaxEnergy = value;
            OnVStatChange?.Invoke(2, VStateType.MaxEnergy, _p2MaxEnergy);
        }
    }

    private int _p2DayEnergy;
    public int p2DayEnergy
    {
        get => _p2DayEnergy;
        set
        {
            if (value == _p2DayEnergy) return;
            _p2DayEnergy = value;
            OnVStatChange?.Invoke(2, VStateType.DayEnergy, _p2DayEnergy);
        }
    }

    private int _p1MaxHitDmg;
    public int p1MaxHitDmg
    {
        get => _p1MaxHitDmg;
        set
        {
            if (value == _p1MaxHitDmg) return;
            _p1MaxHitDmg = value;
            OnVStatChange?.Invoke(1, VStateType.MaxHitDamage, _p1MaxHitDmg);
        }
    }

    private int _p2MaxHitDmg;
    public int p2MaxHitDmg
    {
        get => _p2MaxHitDmg;
        set
        {
            if (value == _p2MaxHitDmg) return;
            _p2MaxHitDmg = value;
            OnVStatChange?.Invoke(2, VStateType.MaxHitDamage, _p2MaxHitDmg);
        }
    }

    private int _p1MinHitDmg;
    public int p1MinHitDmg
    {
        get => _p1MinHitDmg;
        set
        {
            if (value == _p1MinHitDmg) return;
            _p1MinHitDmg = value;
            OnVStatChange?.Invoke(1, VStateType.MinHitDamage, _p1MinHitDmg);
        }
    }

    private int _p2MinHitDmg;
    public int p2MinHitDmg
    {
        get => _p2MinHitDmg;
        set
        {
            if (value == _p2MinHitDmg) return;
            _p2MinHitDmg = value;
            OnVStatChange?.Invoke(2, VStateType.MinHitDamage, _p2MinHitDmg);
        }
    }

    private int _p1BasicVillageBarrier;
    public int p1BasicVillageBarrier
    {
        get => _p1BasicVillageBarrier;
        set
        {
            if (value == _p1BasicVillageBarrier) return;
            _p1BasicVillageBarrier = value;
            OnVStatChange?.Invoke(1, VStateType.VBarrier, _p1BasicVillageBarrier);
        }
    }

    private int _p2BasicVillageBarrier;
    public int p2BasicVillageBarrier
    {
        get => _p2BasicVillageBarrier;
        set
        {
            if (value == _p2BasicVillageBarrier) return;
            _p2BasicVillageBarrier = value;
            OnVStatChange?.Invoke(2, VStateType.VBarrier, _p2BasicVillageBarrier);
        }
    }

    private int _p1VillGold;
    public int p1VillGold
    {
        get => _p1VillGold;
        set
        {
            if (value == _p1VillGold) return;
            _p1VillGold = value;
            OnVStatChange?.Invoke(1, VStateType.VGold, _p1VillGold);
        }
    }

    private int _p2VillGold;
    public int p2VillGold
    {
        get => _p2VillGold;
        set
        {
            if (value == _p2VillGold) return;
            _p2VillGold = value;
            OnVStatChange?.Invoke(2, VStateType.VGold, _p2VillGold);
        }
    }

    private int _p1IncomeVillGold;
    public int p1IncomeVillGold
    {
        get => _p1IncomeVillGold;
        set
        {
            if (value == _p1IncomeVillGold) return;
            _p1IncomeVillGold = value;
            OnVStatChange?.Invoke(1, VStateType.VIncomeGold, _p1IncomeVillGold);
        }
    }

    private int _p2IncomeVillGold;
    public int p2IncomeVillGold
    {
        get => _p2IncomeVillGold;
        set
        {
            if (value == _p2IncomeVillGold) return;
            _p2IncomeVillGold = value;
            OnVStatChange?.Invoke(2, VStateType.VIncomeGold, _p2IncomeVillGold);
        }
    }

    public SimVillageState(VillageBalanceData balanceData, VillageLevelData[] levelDatas)
    {
        _villageBalanceData = balanceData;
        P1VillageObjInfos = new List<VillageObjInfo>();
        P2VillageObjInfos = new List<VillageObjInfo>();

        p1MaxEnergy = p2MaxEnergy = p1DayEnergy = p2DayEnergy = (int)balanceData.EnergyIncomeBase;

        p1MaxHitDmg = p2MaxHitDmg = (int)(balanceData.AxeDamageBase - balanceData.AxeDamageLevelMultiplier);
        p1MinHitDmg = p2MinHitDmg = (int)(balanceData.AxeDamageBase + balanceData.AxeDamageLevelMultiplier);

        p1BasicVillageBarrier = p2BasicVillageBarrier = (int)balanceData.BarrierArmorBase;
        p1VillGold = p2VillGold = (int)balanceData.GoldIncomeBase * 2;

        for (int i = 0; i < levelDatas.Length; i++)
        {
            VillageObjInfo p1Info = new VillageObjInfo(levelDatas[i], 1, balanceData);
            VillageObjInfo p2Info = new VillageObjInfo(levelDatas[i], 1, balanceData);
            P1VillageObjInfos.Add(p1Info);
            P2VillageObjInfos.Add(p2Info);
            OnVillageObjChanged?.Invoke(1, p1Info);
            OnVillageObjChanged?.Invoke(2, p2Info);
        }
    }

    public void UpgradeVillageObject(int playerNum, VillageType facilityType)
    {
        // 대상 플레이어의 리스트 선택
        List<VillageObjInfo> targetList = playerNum == 1 ? P1VillageObjInfos : P2VillageObjInfos;

        // 리스트에서 해당 타입의 건물을 찾음
        VillageObjInfo targetObj = targetList.Find(info => info._levelData.VillageType == facilityType);

        if (targetObj != null)
        {
            if (targetObj._levelData.TryGetUpgradeCost(targetObj.currentLevel, out int upgradeGold))
            {
                int VillGold = playerNum == 1 ? p1VillGold : p2VillGold;
                if (VillGold >= upgradeGold)
                {
                    if (playerNum == 1) p1VillGold = Mathf.Max(0, p1VillGold - upgradeGold);
                    else p2VillGold = Mathf.Max(0, p2VillGold - upgradeGold);
                }
                else
                {
                    Debug.LogError($"Not Enough Money to Upgrade, VillGold : {VillGold}/UpgradeGold : {upgradeGold}");
                    return;
                }
            }
            else
            {
                Debug.LogError("Can't get Upgrade Gold Value");
                return;
            }
            // 1. 레벨 증가 (이때 VillageObjInfo 내부에서 설명 텍스트가 자동 갱신됨)
            targetObj.currentLevel += 1;

            // 2. 업그레이드 반영
            UpgradeValueByType(playerNum, targetObj);

            // 3. UI 갱신을 위해 콜백 이벤트 발생
            OnVillageObjChanged?.Invoke(playerNum, targetObj);
        }
    }

    public void UpdateGoldWhenStartVillPhase()
    {
        p1VillGold += p1IncomeVillGold;
        p2VillGold += p2IncomeVillGold;
    }

    private void UpgradeValueByType(int playerNum, VillageObjInfo target)
    {
        switch (target._levelData.VillageType)
        {
            case (VillageType.Mine):
                int VillGold = GetGoldIncomePerDay(target.currentLevel);
                if (playerNum == 1) p1IncomeVillGold = VillGold;
                else p2IncomeVillGold = VillGold;
                break;
            case (VillageType.Forge):
                var AtkDmgs = GetAxeRangeDamage(target.currentLevel);
                if (playerNum == 1)
                {
                    p1MinHitDmg = AtkDmgs.min;
                    p1MaxHitDmg = AtkDmgs.max;
                }
                else
                {
                    p2MinHitDmg = AtkDmgs.min;
                    p2MaxHitDmg = AtkDmgs.max;
                }
                break;
            case (VillageType.Farm):
                int MaxEng = GetMaxEnergy(target.currentLevel);
                int IncomeEng = GetEnergyIncomePerDay(target.currentLevel);
                if (playerNum == 1)
                {
                    p1MaxEnergy = MaxEng;
                    p1DayEnergy = IncomeEng;
                }
                else
                {
                    p2MaxEnergy = MaxEng;
                    p2DayEnergy = IncomeEng;
                }
                break;
            case (VillageType.Barrier):
                int Bar = GetBarrierArmor(target.currentLevel);
                if (playerNum == 1) p1BasicVillageBarrier = Bar;
                else p2BasicVillageBarrier = Bar;
                break;
        }
    }

    public int GetLevelUpgradedCost(VillageType facilityType, int currentLevel)
    {
        if (currentLevel >= _villageBalanceData.MaxLevel - 1) return 0;
        return _villageBalanceData.UpgradeCostBase * (int)Mathf.Pow(_villageBalanceData.UpgradeCostMultiplier, currentLevel);
    }

    public int GetGoldIncomePerDay(int level)
    {
        int L = level + 1;
        return _villageBalanceData.GoldIncomeBase * (L * L + L);
    }

    public int GetMaxEnergy(int level)
    {
        return Mathf.RoundToInt(_villageBalanceData.EnergyMaxBase + (level * _villageBalanceData.EnergyMaxMultiplier));
    }

    public int GetEnergyIncomePerDay(int level)
    {
        return Mathf.RoundToInt(_villageBalanceData.EnergyIncomeBase + (level * _villageBalanceData.EnergyIncomeMultiplier));
    }

    public int GetBarrierArmor(int level)
    {
        if (level <= 0) return 0;
        return Mathf.RoundToInt(_villageBalanceData.BarrierArmorBase * Mathf.Pow(_villageBalanceData.BarrierArmorMultiplier, level - 1));
    }

    public (int min, int max) GetAxeRangeDamage(int level)
    {
        return (
             Mathf.RoundToInt(_villageBalanceData.AxeDamageBase - ((level + 1) * _villageBalanceData.AxeDamageLevelMultiplier * _villageBalanceData.AxeDamageMinMultiplier)),
            Mathf.RoundToInt(_villageBalanceData.AxeDamageBase + ((level + 1) * _villageBalanceData.AxeDamageLevelMultiplier * _villageBalanceData.AxeDamageMaxMultiplier))
        );
    }
}