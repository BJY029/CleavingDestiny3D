using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Village
{
    public class VillageStatManager : MonoBehaviour, IVillageStatProvider
    {
        private static VillageStatManager _instance;
        public static VillageStatManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<VillageStatManager>();
                    if (_instance == null)
                    {
                        Debug.LogError("VillageStatManager 인스턴스를 찾을 수 없습니다!");
                    }
                }
                return _instance;
            }
        }

        [SerializeField]
        private VillageLevelData[] _villageLevelDatas;

        [SerializeField]
        private VillageBalanceData _villageBalanceData;

        private readonly Dictionary<VillageType, VillageLevelData> _villageDataDict = new Dictionary<VillageType, VillageLevelData>();

        private void Awake()
        {
            if (_instance == null || _instance == this)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitVillageData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitVillageData()
        {
            _villageDataDict.Clear();
            if (_villageLevelDatas == null) return;

            foreach (var data in _villageLevelDatas)
            {
                if (data == null) continue;
                _villageDataDict[data.VillageType] = data;
            }
        }

        // 특정 플레이어의 건물의 현재 레벨을 안전하게 가져오기
        public int GetVillageLevel(VillageType type, Player targetPlayer = null)
        {
            // 매개변수가 없으면 로컬 플레이어를 기본값으로 사용
            var player = targetPlayer ?? PhotonNetwork.LocalPlayer;

            if (player == null) return 0;

            if (player.CustomProperties.TryGetValue(PlayerPropKeys.VillageUpgrades, out object village))
            {
                int index = (int)type;

                // Photon은 때때로 int[]를 object[]로 역직렬화할 때가 있음
                if (village is int[] intList)
                {
                    if (index >= 0 && index < intList.Length)
                    {
                        return intList[index];
                    }
                }
                else if (village is object[] objList)
                {
                    if (index >= 0 && index < objList.Length)
                    {
                        // Photon에서 숫자는 종종 long으로 오거나 박싱될 수 있음
                        return System.Convert.ToInt32(objList[index]);
                    }
                }
            }
            return 0; // 오류시 0레벨 반환
        }

        // 특정 건물의 다음 레벨 업그레이드 비용 계산
        public int GetLevelUpgradedCost(VillageType facilityType, Player targetPlayer = null)
        {
            int currentLevel = GetVillageLevel(facilityType, targetPlayer);
            return GetLevelUpgradedCost(facilityType, currentLevel, targetPlayer);
        }

        public int GetLevelUpgradedCost(VillageType facilityType, int currentLevel, Player targetPlayer = null)
        {
            // 모든 건물 공통 비용 공식 사용
            // 최대 레벨(Lv5, index 4) 도달 시 업그레이드 불가
            if (currentLevel >= _villageBalanceData.MaxLevel - 1) return 0;

            // Cost = Base * Multiplier^(currentLevel)
            return _villageBalanceData.UpgradeCostBase * (int)Mathf.Pow(_villageBalanceData.UpgradeCostMultiplier, currentLevel);
        }

        // 1. 일일 골드 수입 (Mine 레벨 비례)
        // Gold(L) = Base * (L^2 + L), where L = level + 1
        public int GetGoldIncomePerDay(int level)
        {
            int L = level + 1;
            return _villageBalanceData.GoldIncomeBase * (L * L + L);
        }

        // maxEnergy = base + (level * 5)
        public int GetMaxEnergy(int level)
        {
            return Mathf.RoundToInt(_villageBalanceData.EnergyMaxBase + (level * _villageBalanceData.EnergyMaxMultiplier));
        }

        // Energy =  base + (level * 3.2)
        public int GetEnergyIncomePerDay(int level)
        {
            return Mathf.RoundToInt(_villageBalanceData.EnergyIncomeBase + (level * _villageBalanceData.EnergyIncomeMultiplier));
        }

        // barrier(L) = 0 if L=1, else Base * Multiplier^(L-1)
        // input level is 0-indexed (L = level + 1)
        public int GetBarrierArmor(int level)
        {
            if (level <= 0) return 0;
            return Mathf.RoundToInt(_villageBalanceData.BarrierArmorBase * Mathf.Pow(_villageBalanceData.BarrierArmorMultiplier, level - 1));
        }

        // minPow = 1000 - (Level * 100 * 0.5)
        // maxPow = 1000 + (Level * 100 * 1.5)
        public (int min, int max) GetAxeRangeDamage(int level)
        {
            return (
                 Mathf.RoundToInt(_villageBalanceData.AxeDamageBase - ((level + 1) * _villageBalanceData.AxeDamageLevelMultiplier * _villageBalanceData.AxeDamageMinMultiplier)),
                Mathf.RoundToInt(_villageBalanceData.AxeDamageBase + ((level + 1) * _villageBalanceData.AxeDamageLevelMultiplier * _villageBalanceData.AxeDamageMaxMultiplier))
            );
        }

        // TODO: 상점 강화 함수 필요
    }
}
/*
 * [Village Stat Formulas]
 * 
 * L = Level Index (0, 1, 2, 3, 4) -> In-game (Lv.1, Lv.2, Lv.3, Lv.4, Lv.5)
 * 
 * 1. Upgrade Cost (Level L -> L+1)
 *    Cost = UpgradeCostBase * UpgradeCostMultiplier^L
 *    (Base: 100, Multiplier: 2) -> 100, 200, 400, 800
 * 
 * 2. Mine (Gold Income)
 *    Let N = L + 1 (Actual Level 1~5)
 *    Gold = GoldIncomeBase * (N^2 + N)
 *    (Base: 50) -> 100, 300, 600, 1000, 1500
 * 
 * 3. Farm (Energy)
 *    MaxEnergy    = EnergyMaxBase + (L * EnergyMaxMultiplier)
 *    EnergyIncome = EnergyIncomeBase + (L * EnergyIncomeMultiplier)
 * 
 * 4. Shop (Rare Item Chance) -> TODO
 * 
 * 5. Forge (Axe Damage)
 *    N = L + 1
 *    MinDamage = AxeDamageBase - (N * AxeDamageLevelMultiplier * AxeDamageMinMultiplier)
 *    MaxDamage = AxeDamageBase + (N * AxeDamageLevelMultiplier * AxeDamageMaxMultiplier)
 * 
 * 6. Barrier (Defense)
 *    If L == 0 (Lv.1): 0
 *    Else: BarrierArmorBase * BarrierArmorMultiplier^(L-1)
 *    (Base: 100, Multiplier: 2) -> 0, 100, 200, 400, 800
 */