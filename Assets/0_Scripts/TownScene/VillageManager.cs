using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Events;
using System;

public class VillageManager : MonoBehaviourPunCallbacks
{
    public static VillageManager Instance { get; private set; }

    // 골드 및 업그레이드 관리를 위한 캐시
    private readonly Hashtable _propCache = new Hashtable();

    public UnityEvent<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 현재 내 골드 가져오기
    public int GetMyGold()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKeys.Gold, out object gold))
        {
            return (int)gold;
        }
        return 0;
    }

    // 골드 추가하기
    public void AddGold(int amount)
    {
        int currentGold = GetMyGold();

        _propCache.Clear();
        _propCache[PlayerPropKeys.Gold] = currentGold + amount;
        PhotonNetwork.LocalPlayer.SetCustomProperties(_propCache);
        OnGoldChanged?.Invoke(currentGold + amount);
    }

    // 업그레이드 시도 (UI 버튼 등에서 호출)
    public void TryUpgradeLevel(VillageUpgradeIndex villageType)
    {
        int currentLevel = VillageStat.GetUpgradeLevel(villageType);
        int currentGold = GetMyGold();

        int upgradeCost = VillageStat.GetLevelUpgradedCost(villageType);

        if (currentGold >= upgradeCost)
        {
            // 업그레이드 진행
            ProcessUpgrade(villageType, currentLevel, currentGold, upgradeCost);
            Debug.Log($"{villageType} 업그레이드 성공! Lv.{currentLevel} -> Lv.{currentLevel + 1}");
        }
        else
        {
            Debug.Log("골드가 부족합니다.");
        }
    }

    // 서버에 데이터 업데이트 요청
    private void ProcessUpgrade(VillageUpgradeIndex facilityType, int currentLevel, int currentGold, int cost)
    {
        Player myPlayer = PhotonNetwork.LocalPlayer;

        _propCache.Clear();

        // 1. 골드 차감
        _propCache[PlayerPropKeys.Gold] = currentGold - cost;

        // 2. 업그레이드 배열 갱신
        int[] currentUpgrades = (int[])myPlayer.CustomProperties[PlayerPropKeys.VillageUpgrades];
        if (currentUpgrades == null || currentUpgrades.Length == 0)
        {
            currentUpgrades = new int[Enum.GetValues(typeof(VillageUpgradeIndex)).Length];
        }
        currentUpgrades[(int)facilityType] = currentLevel + 1;
        OnGoldChanged?.Invoke(currentGold - cost);

        _propCache[PlayerPropKeys.VillageUpgrades] = currentUpgrades;

        // 3. 서버 전송
        myPlayer.SetCustomProperties(_propCache);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}