using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Events;
using System;

public class VillageManager : MonoBehaviourPunCallbacks
{
    public static VillageManager Instance { get; private set; }

    private readonly Hashtable _propCache = new Hashtable();
    private int _cachedGold = 0; // 캐시된 골드 값
    private bool _goldChangedBySelf = false;

    public UnityEvent<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
    }

    private void Start()
    {
        // 초기화
        _cachedGold = GetMyGold();
    }

    // 캐시된 골드 값 반환 (Update에서 사용 가능)
    public int GetMyGold()
    {
        return _cachedGold;
    }

    // Photon 속성에서 골드 값 동기화
    private int FetchGoldFromPhoton()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKeys.Gold, out object gold))
        {
            return (int)gold;
        }
        return 0;
    }

    public void AddGold(int amount)
    {
        int currentGold = _cachedGold;
        _goldChangedBySelf = true;

        _propCache.Clear();
        _propCache[PlayerPropKeys.Gold] = currentGold + amount;
        PhotonNetwork.LocalPlayer.SetCustomProperties(_propCache);
        
        _cachedGold = currentGold + amount; // 캐시 업데이트
        OnGoldChanged?.Invoke(_cachedGold);
    }

    // CustomProperties 변경 시 자동 동기화
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!_goldChangedBySelf && targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey(PlayerPropKeys.Gold))
        {
            _cachedGold = FetchGoldFromPhoton();
            OnGoldChanged?.Invoke(_cachedGold);
        }
        _goldChangedBySelf = false;
        
    }

    public bool TryUpgradeLevel(VillageUpgradeIndex facilityType)
    {
        Player myPlayer = PhotonNetwork.LocalPlayer;
        int currentGold = GetMyGold();
        int currentLevel = VillageStat.GetUpgradeLevel(facilityType);
        int cost = VillageStat.GetLevelUpgradedCost(facilityType);
        if (currentGold >= cost)
        {
            ProcessUpgrade(facilityType, currentLevel, currentGold, cost);
            return true;
        }
        else
        {
            Debug.Log("Not enough gold to upgrade.");
            return false;
        }
    }

    private void ProcessUpgrade(VillageUpgradeIndex facilityType, int currentLevel, int currentGold, int cost)
    {
        Player myPlayer = PhotonNetwork.LocalPlayer;

        _propCache.Clear();
        _propCache[PlayerPropKeys.Gold] = currentGold - cost;

        int[] currentUpgrades = (int[])myPlayer.CustomProperties[PlayerPropKeys.VillageUpgrades];
        if (currentUpgrades == null || currentUpgrades.Length == 0)
        {
            currentUpgrades = new int[Enum.GetValues(typeof(VillageUpgradeIndex)).Length];
        }
        currentUpgrades[(int)facilityType] = currentLevel + 1;
        
        _cachedGold = currentGold - cost; // 캐시 업데이트
        OnGoldChanged?.Invoke(_cachedGold);

        _goldChangedBySelf = true;

        _propCache[PlayerPropKeys.VillageUpgrades] = currentUpgrades;
        myPlayer.SetCustomProperties(_propCache);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}