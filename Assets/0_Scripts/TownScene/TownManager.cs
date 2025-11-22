using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Events;
using System;

public class TownManager : MonoBehaviourPunCallbacks
{
    public static TownManager Instance { get; private set; }

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

    // 특정 시설의 현재 레벨 가져오기
    public int GetFacilityLevel(VillageUpgradeIndex facilityType)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerPropKeys.VillageUpgrades, out object upgrades))
        {
            int[] upgradeList = (int[])upgrades;
            return upgradeList[(int)facilityType];
        }
        return 0;
    }

    // 업그레이드 시도 (UI 버튼 등에서 호출)
    public void TryUpgradeFacility(VillageUpgradeIndex facilityType)
    {
        int currentLevel = GetFacilityLevel(facilityType);
        int currentGold = GetMyGold();

        // TODO: 밸런스에 맞는 가격 공식 적용 필요 (예: 레벨 * 100 + 100)
        int upgradeCost = (currentLevel + 1) * 100;

        if (currentGold >= upgradeCost)
        {
            // 업그레이드 진행
            ProcessUpgrade(facilityType, currentLevel, currentGold, upgradeCost);
            Debug.Log($"{facilityType} 업그레이드 성공! Lv.{currentLevel} -> Lv.{currentLevel + 1}");
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

    // 게임 씬으로 돌아가기
    public void ReturnToGameScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(CommonDefine.GAMESCENE);
        }
    }
}