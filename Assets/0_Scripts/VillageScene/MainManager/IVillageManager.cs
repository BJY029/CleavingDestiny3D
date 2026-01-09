using UnityEngine.Events;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

namespace Village
{
    public interface IVillageManager
    {
        event Action<int> OnGoldChanged;
        int GetMyGold();
        void AddGold(int amount);
        bool TryUpgradeLevel(VillageType facilityType);
        void SyncFromPhoton(Player targetPlayer, Hashtable changedProps);
    }
}