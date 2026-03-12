using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

namespace Village
{
    public class VillageManager : IVillageManager
    {
        private IVillageStatProvider _statProvider;

        // static Hashtable로 캐싱하여 매번 재사용
        private static readonly Hashtable _propCache = new Hashtable();
        // Enum 길이를 static으로 캐싱하여 재사용
        private static readonly int _villageTypeCount = Enum.GetValues(typeof(VillageType)).Length;

        private int _cachedGold = 0;
        private bool _goldChangedBySelf = false;

        public event Action<int> OnGoldChanged;

        public void Initialize(IVillageStatProvider statProvider)
        {
            _statProvider = statProvider;

            // 정적 인스턴스 재사용 시, 이전 씬에서 연결된 이벤트 리스너들을 정리해야 함 (Memory Leak 방지)
            OnGoldChanged = null;

            // 씬이 새로 로드될 때 Photon에 저장된 기존 골드 정보를 가져옴
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKeys.Gold, out object gold))
            {
                _cachedGold = Convert.ToInt32(gold);
            }
            else
            {
                _cachedGold = 0; // 데이터가 없으면 0으로 초기화
            }
        }

        public int GetMyGold() => _cachedGold;

        public void AddGold(int amount)
        {
            _goldChangedBySelf = true;
            _cachedGold += amount;

            _propCache.Clear();
            _propCache[PlayerPropKeys.Gold] = _cachedGold;
            PhotonNetwork.LocalPlayer.SetCustomProperties(_propCache);

            OnGoldChanged?.Invoke(_cachedGold);
        }

        public void SyncFromPhoton(Player targetPlayer, Hashtable changedProps)
        {
            if (!_goldChangedBySelf && targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey(PlayerPropKeys.Gold))
            {
                if (changedProps.TryGetValue(PlayerPropKeys.Gold, out object gold))
                {
                    _cachedGold = Convert.ToInt32(gold);
                    OnGoldChanged?.Invoke(_cachedGold);
                }
            }
            _goldChangedBySelf = false;
        }

        public bool TryUpgradeLevel(VillageType facilityType)
        {
            int currentGold = GetMyGold();
            int currentLevel = _statProvider.GetVillageLevel(facilityType);
            int cost = _statProvider.GetLevelUpgradedCost(facilityType, currentLevel);

            if (cost <= 0)
            {
                return false;
            }

            if (currentGold >= cost)
            {
                ProcessUpgrade(facilityType, currentLevel, currentGold, cost);
                return true;
            }
            return false;
        }

        private void ProcessUpgrade(VillageType facilityType, int currentLevel, int currentGold, int cost)
        {
            Player myPlayer = PhotonNetwork.LocalPlayer;
            _cachedGold = currentGold - cost;

            _propCache.Clear();
            _propCache[PlayerPropKeys.Gold] = _cachedGold;

            // Photon 역직렬화 타입(int[]/object[]) 차이를 흡수하고, 원본 배열 오염을 막기 위해 복사본을 사용
            int[] currentUpgrades = GetUpgradeLevelsSnapshot(myPlayer);
            int nextLevel = currentLevel + 1;
            currentUpgrades[(int)facilityType] = nextLevel;
            _propCache[PlayerPropKeys.VillageUpgrades] = currentUpgrades;

            // 건물 타입에 따른 플레이어 스탯 업데이트
            UpdatePlayerStatsByLevel(facilityType, nextLevel);

            _goldChangedBySelf = true;
            myPlayer.SetCustomProperties(_propCache);
            OnGoldChanged?.Invoke(_cachedGold);
        }

        private void UpdatePlayerStatsByLevel(VillageType facilityType, int nextLevel)
        {
            // _statProvider를 통해 계산된 최신 스탯을 _propCache에 추가
            switch (facilityType)
            {
                case VillageType.Mine:
                    _propCache[PlayerPropKeys.DayGoldIncome] = _statProvider.GetGoldIncomePerDay(nextLevel);
                    break;
                case VillageType.Farm:
                    _propCache[PlayerPropKeys.MaxEnergy] = _statProvider.GetMaxEnergy(nextLevel);
                    _propCache[PlayerPropKeys.EnergyIncome] = _statProvider.GetEnergyIncomePerDay(nextLevel);
                    break;
                case VillageType.Barrier:
                    _propCache[PlayerPropKeys.BarrierArmor] = _statProvider.GetBarrierArmor(nextLevel);
                    break;
                case VillageType.Forge:
                    var (min, max) = _statProvider.GetAxeRangeDamage(nextLevel);
                    _propCache[PlayerPropKeys.MinAtkPow] = min;
                    _propCache[PlayerPropKeys.MaxAtkPow] = max;
                    break;
                    // TODO: 대장간 등 추가 예정인 건물들에 대한 로직
            }
        }

        private static int[] GetUpgradeLevelsSnapshot(Player player)
        {
            int[] levels = new int[_villageTypeCount];

            if (player == null
                || player.CustomProperties == null
                || !player.CustomProperties.TryGetValue(PlayerPropKeys.VillageUpgrades, out object village)
                || village == null)
            {
                return levels;
            }

            if (village is int[] intList)
            {
                Array.Copy(intList, levels, Mathf.Min(intList.Length, _villageTypeCount));
                return levels;
            }

            if (village is object[] objList)
            {
                int count = Mathf.Min(objList.Length, _villageTypeCount);
                for (int i = 0; i < count; i++)
                {
                    levels[i] = objList[i] == null ? 0 : Convert.ToInt32(objList[i]);
                }
            }

            return levels;
        }
    }
}