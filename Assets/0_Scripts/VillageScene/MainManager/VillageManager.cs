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

        private static readonly Hashtable _propCache = new Hashtable();
        private static readonly int _villageTypeCount = Enum.GetValues(typeof(VillageType)).Length;

        private int _cachedGold = 0;
        private bool _goldChangedBySelf = false;

        public event Action<int> OnGoldChanged;

        public void Initialize(IVillageStatProvider statProvider)
        {
            _statProvider = statProvider;
            OnGoldChanged = null;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKeys.Gold, out object gold))
            {
                _cachedGold = Convert.ToInt32(gold);
            }
            else
            {
                _cachedGold = 0;
            }
        }

        public int GetMyGold(int actorNumber = -1)
        {
            if (actorNumber == -1) actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return _cachedGold;
            }
            else
            {
                return PhotonPropertyHelper.GetPlayerProp(actorNumber, PlayerPropKeys.Gold, 0);
            }
        }

        public void AddGold(int amount)
        {
            _goldChangedBySelf = true;
            _cachedGold += amount;

            PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.Gold, _cachedGold);

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

        public bool TryUpgradeLevel(VillageType facilityType, int actorNumber = -1)
        {
            // 업그레이드 시도: 골드 충분 여부 확인 후, 충분하면 골드 차감 및 레벨 업
            // 보안을 위해 클라이언트는 자기 정보만 업그레이드 가능
            // 호스트만 다른 플레이어의 업그레이드 시도 가능 (AI 제어용)

            if (actorNumber == -1) actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

            if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber && !PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("다른 플레이어의 건물 업그레이드는 호스트만 시도할 수 있습니다.");
                return false;
            }

            int currentGold = GetMyGold(actorNumber);
            int currentLevel = _statProvider.GetVillageLevel(facilityType, actorNumber);
            int cost = _statProvider.GetLevelUpgradedCost(facilityType, currentLevel);

            if (cost <= 0) return false;

            if (currentGold >= cost)
            {
                ProcessUpgrade(facilityType, currentLevel, currentGold, cost, actorNumber);
                return true;
            }
            return false;
        }

        private void ProcessUpgrade(VillageType facilityType, int currentLevel, int currentGold, int cost, int actorNumber)
        {
            int newGold = currentGold - cost;
            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                _cachedGold = newGold;
                _goldChangedBySelf = true;
            }

            _propCache.Clear();
            _propCache[PlayerPropKeys.Gold] = newGold;

            int[] currentUpgrades = GetUpgradeLevelsSnapshot(actorNumber);
            int nextLevel = currentLevel + 1;
            currentUpgrades[(int)facilityType] = nextLevel;
            _propCache[PlayerPropKeys.VillageUpgrades] = currentUpgrades;

            UpdatePlayerStatsByLevel(facilityType, nextLevel);

            foreach (var key in _propCache.Keys)
            {
                PhotonPropertyHelper.SetPlayerProp(actorNumber, key.ToString(), _propCache[key]);
            }

            if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                OnGoldChanged?.Invoke(_cachedGold);
            }
        }

        private void UpdatePlayerStatsByLevel(VillageType facilityType, int nextLevel)
        {
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
            }
        }

        private static int[] GetUpgradeLevelsSnapshot(int actorNumber)
        {
            int[] levels = new int[_villageTypeCount];

            object village = PhotonPropertyHelper.GetPlayerProp<object>(actorNumber, PlayerPropKeys.VillageUpgrades);
            if (village == null) return levels;

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
