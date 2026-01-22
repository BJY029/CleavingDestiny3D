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
            // TryGet을 체이닝하듯 검사
            if (_villageDataDict.TryGetValue(facilityType, out var levelData))
            {
                if (levelData.TryGetUpgradeCost(currentLevel, out int cost))
                {
                    return cost;
                }
                else
                {
                    Debug.LogWarning($"업그레이드 가격: {facilityType}는 존재하나, {currentLevel}레벨에 대한 비용 데이터를 찾을 수 없습니다. (MaxLevel: {levelData.MaxLevel})");
                }
            }
            else
            {
                Debug.LogError($"업그레이드 가격: VillageStatManager의 _villageLevelDatas에 {facilityType} 데이터가 등록되지 않았습니다!");
            }

            return 0; // 오류 시 0 반환
        }

        // 1. 일일 골드 수입 (Mine 레벨 비례) (미리 계산된 값 사용)
        public int GetGoldIncomePerDay(Player targetPlayer = null)
        {
            int level = GetVillageLevel(VillageType.Mine, targetPlayer);
            if (_villageDataDict.TryGetValue(VillageType.Mine, out VillageLevelData levelData) &&
                levelData.TryGetEffectValue(level, out int incomeGold))
            {
                return incomeGold;
            }
            return 0; // 오류 시 0 반환
        }

        // maxEnergy = base + (level * 5)
        public float GetMaxEnergy(Player targetPlayer = null)
        {
            int level = GetVillageLevel(VillageType.Farm, targetPlayer);
            return _villageBalanceData.EnemyMaxBase + (level * _villageBalanceData.EnemyMaxMultiplier);
        }

        // Energy =  base + (level * 3.2)<반올림>
        public float GetEnergyIncomePerDay(Player targetPlayer = null)
        {
            int level = GetVillageLevel(VillageType.Farm, targetPlayer);
            return _villageBalanceData.EnergyIncomeBase + (level * _villageBalanceData.EnergyIncomeMultiplier);
        }

        // barrier(w) = barrier(w-1) * (1.5)^(w-1) (미리 계산된 값 사용)
        public float GetBarrierArmor(Player targetPlayer = null)
        {
            int level = GetVillageLevel(VillageType.Shop, targetPlayer);
            if (_villageDataDict.TryGetValue(VillageType.Shop, out VillageLevelData levelData) &&
                levelData.TryGetEffectValue(level, out int barrierArmor))
            {
                return barrierArmor;
            }
            return 0; // 오류 시 0 반환
        }

        // TODO: 상점 강화 함수 필요
        // TODO: 대장간 강화 함수 필요

        public string GetLevelDescriptionID(VillageType facilityType, int level)
        {
            if (_villageDataDict.TryGetValue(facilityType, out var levelData))
            {
                if (levelData.TryGetEffectValue(level, out int _))
                {
                    var descriptions = levelData.LevelDescriptionID;
                    if (level >= 0 && level < descriptions.Length)
                    {
                        return descriptions[level];
                    }
                }
            }
            return string.Empty; // 오류 시 빈 문자열 반환
        }
    }
}