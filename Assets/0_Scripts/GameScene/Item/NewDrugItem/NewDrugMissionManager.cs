using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class NewDrugMissionManager : MonoBehaviourPun
{
    public static NewDrugMissionManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private INewDrugMission currentMission;
    private NewDrugMissionContext context;

    private PlayerController missionOwner;

    private bool missionStarted;
    private bool rewardGranted;
    private bool newDrugUsed;

    public bool IsMissionStarted => missionStarted;
    public bool IsRewardGranted => rewardGranted;
    public bool IsNewDrugUsed => newDrugUsed;



    public void StartRandomMission(PlayerController player, int CurrentTurnIndex, int CurrentWaveIndex)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;

        if (missionStarted) return;
        if (rewardGranted) return;
        if (newDrugUsed) return;

        missionOwner = player;

        context = new NewDrugMissionContext
        {
            Owner = player,
            StartTurnIndex = CurrentTurnIndex,
            StartWaveIndex = CurrentWaveIndex,
            CurrentTurnIndex = CurrentTurnIndex,
            CurrentWaveIndex = CurrentWaveIndex,
        };

        currentMission = CreateRandomMission();
        currentMission.Init(context);

        missionStarted = true;

        Debug.Log($"[신약 개발 미션 시작] {currentMission.MissionName} / {currentMission.MissionDesc}");

        //TODO : UI 처리
    }

    private INewDrugMission CreateRandomMission()
    {
        List<INewDrugMission> missions = new List<INewDrugMission>
        {
            new NoItemOnlyBasicAttackMission(),
            new PrecisionDamageMission(100),
            new SpendStaminaInOneTurnMission(12),
            new ReachDefenseInOneTurnMission(500),
        };

        int randomIndex = Random.Range(0, missions.Count);
        return missions[randomIndex];
    }

    public void ReceiveGameEvent(NewDrugGameEvent gameEvent)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;

        if (!missionStarted) return;
        if (currentMission == null) return;
        if (rewardGranted) return;

        context.CurrentTurnIndex = gameEvent.TurnIndex;
        context.CurrentWaveIndex = gameEvent.WaveIndex;

        currentMission.OnGameEvent(gameEvent);

        if (currentMission.IsSuccess) GrantNewDrugReward();
        else if (currentMission.IsFailed) FailMission();
    }

    private void GrantNewDrugReward()
    {
        if (rewardGranted) return;

        rewardGranted = true;
        missionStarted = false;

        Debug.Log("[신약 개발 미션 성공] 신약 아이템 지급");

        //TODO: 보상 아이템 인벤토리에 지급
        //TODO : UI 처리
    }

    private void FailMission()
    {
        missionStarted = false;

        Debug.Log("[신약 개발 미션 실패]");

        //TODO : ui 처리
    }

    public void MaskNewDrugUsed()
    {
        if (newDrugUsed) return;
        newDrugUsed = true;
    }

    [PunRPC]
    private void RPC_ShowMissionUI(string missionName, string description)
    {

    }

    [PunRPC]
    private void RPC_ShowMissionSuccessUI()
    {

    }

    [PunRPC]
    private void RPC_ShowMissionFailUI()
    {

    }
}
