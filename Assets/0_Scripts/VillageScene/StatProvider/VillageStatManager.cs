using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Potan.CoreUtils;
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
                        DevLog.LogError("VillageStatManager 인스턴스를 찾을 수 없습니다!");
                    }
                }
                return _instance;
            }
        }

        [SerializeField]
        private VillageLevelData[] _villageLevelDatas;

        [SerializeField]
        private VillageBalanceData _villageBalanceData;
        public VillageBalanceData VillageBalance => _villageBalanceData;

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

        public int GetVillageLevel(VillageType type, int actorNumber = -1)
        {
            if (actorNumber == -1) actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            var village = PhotonPropertyHelper.GetPlayerProp<object>(actorNumber, PlayerPropKeys.VillageUpgrades);
            if (village != null)
            {
                return ExtractLevelFromObject(type, village);
            }
            return 0;
        }

        private int ExtractLevelFromObject(VillageType type, object village)
        {
            int index = (int)type;

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
                    return System.Convert.ToInt32(objList[index]);
                }
            }
            return 0;
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
}
