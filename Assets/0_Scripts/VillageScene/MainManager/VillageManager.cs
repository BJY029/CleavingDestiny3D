using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Events;
using System;

namespace Village
{
    public class VillageManager : IVillageManager
    {
        private readonly IVillageStatProvider _statProvider;
        private readonly Hashtable _propCache = new Hashtable();
        private int _cachedGold = 0;
        private bool _goldChangedBySelf = false;

        public event Action<int> OnGoldChanged;

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
                    _cachedGold = (int)gold;
                    OnGoldChanged?.Invoke(_cachedGold);
                }
            }
            _goldChangedBySelf = false;
        }

        public VillageManager(IVillageStatProvider statProvider)
        {
            _statProvider = statProvider;

            // 씬이 새로 로드될 때 Photon에 저장된 기존 골드 정보를 가져옴
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKeys.Gold, out object gold))
            {
                _cachedGold = (int)gold;
            }
            else
            {
                _cachedGold = 0; // 데이터가 없으면 0으로 초기화
            }
        }

        public bool TryUpgradeLevel(VillageType facilityType)
        {
            int currentGold = GetMyGold();
            int currentLevel = _statProvider.GetVillageLevel(facilityType); // Instance 대신 필드 사용
            int cost = _statProvider.GetLevelUpgradedCost(facilityType, currentLevel);

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

            int[] currentUpgrades = (int[])myPlayer.CustomProperties[PlayerPropKeys.VillageUpgrades] ?? new int[Enum.GetValues(typeof(VillageType)).Length];
            currentUpgrades[(int)facilityType] = currentLevel + 1;
            _propCache[PlayerPropKeys.VillageUpgrades] = currentUpgrades;

            _goldChangedBySelf = true;
            myPlayer.SetCustomProperties(_propCache);
            OnGoldChanged?.Invoke(_cachedGold);
        }
    }
}