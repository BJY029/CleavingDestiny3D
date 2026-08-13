using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;
using Photon.Realtime;

public enum NewDrugMissionState
{
    None, PendingNextDay, Active, Complete, Failed,
}

public class NewDrugMissionRuntime
{
    public int OwnerActorNumber;

    public NewDrugMissionState State = NewDrugMissionState.None;

    public INewDrugMission CurrentMission;
    public NewDrugMissionContext Context;

    public bool DevelopmentItemUsed;
    public bool RewardGranted;
    public bool NewDrugUsed;
}

public class NewDrugMissionManager : MonoBehaviourPun
{
    public static NewDrugMissionManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private Dictionary<int, NewDrugMissionRuntime> missionByActor = new Dictionary<int, NewDrugMissionRuntime>();
    private NewDrugMissionRuntime GetOrCreateRuntime(int actorNum)
    {
        if (!missionByActor.TryGetValue(actorNum, out NewDrugMissionRuntime runtime))
        {
            runtime = new NewDrugMissionRuntime
            {
                OwnerActorNumber = actorNum,
                State = NewDrugMissionState.None,
            };

            missionByActor.Add(actorNum, runtime);
        }

        return runtime;
    }

    public bool CanUseNewDrugDevelopmentItem(int actorNum, out string reason)
    {
        reason = string.Empty;

        NewDrugMissionRuntime runtime = GetOrCreateRuntime(actorNum);

        if (runtime.DevelopmentItemUsed)
        {
            reason = "신약 개발 아이템은 게임 중 한 번만 사용할 수 있습니다.";
            return false;
        }

        if (runtime.State == NewDrugMissionState.PendingNextDay)
        {
            reason = "신약 개발 미션이 다음 날 시작 대기 중입니다.";
            return false;
        }

        if (runtime.State == NewDrugMissionState.Active)
        {
            reason = "이미 신약개발 미션이 진행 중입니다.";
            return false;
        }

        if (runtime.RewardGranted)
        {
            reason = "이미 완성된 신약을 획득했습니다.";
            return false;
        }

        if (runtime.NewDrugUsed)
        {
            reason = "이미 완성된 신약을 사용했습니다.";
            return false;
        }

        return true;
    }

    public bool ReserveMissionForNextDay(int actorNum)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return false;

        if (!CanUseNewDrugDevelopmentItem(actorNum, out string reason))
        {
            Debug.Log($"[신약 개발 예약 실패] {reason}");
            return false;
        }

        NewDrugMissionRuntime runtime = GetOrCreateRuntime(actorNum);

        runtime.DevelopmentItemUsed = true;
        runtime.State = NewDrugMissionState.PendingNextDay;

        Debug.Log($"[신약 개발 예약] Actor {actorNum}의 신약 개발 미션이 다음 날 시작됩니다.");

        Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);
        photonView.RPC(nameof(RPC_ShowMissionReservedUI), targetPlayer, "", "");
        return true;
    }

    public void TryStartPendingMissionOnNextDay()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;

        foreach (var pair in missionByActor)
        {
            NewDrugMissionRuntime runtime = pair.Value;

            if (runtime.State != NewDrugMissionState.PendingNextDay) continue;

            StartMissionNow(runtime);
        }
    }

    private void StartMissionNow(NewDrugMissionRuntime runtime)
    {
        runtime.CurrentMission = CreateRandomMission();

        runtime.Context = new NewDrugMissionContext
        {
            OwnerActorNumber = runtime.OwnerActorNumber,
            StartTurnIndex = GetCurrentTurnIndex(),
            StartWaveIndex = GetCurrentWaveIndex(),
            CurrentTurnIndex = GetCurrentTurnIndex(),
            CurrentWaveIndex = GetCurrentWaveIndex()
        };

        runtime.CurrentMission.Init(runtime.Context);

        runtime.State = NewDrugMissionState.Active;

        Debug.Log($"[신약 개발 미션 시작] Actor {runtime.OwnerActorNumber} / " +
            $"{runtime.CurrentMission.MissionName} / {runtime.CurrentMission.MissionDesc}"
        );

        Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(runtime.OwnerActorNumber);
        string MissionName = runtime.CurrentMission.MissionName;
        string MissionContext = runtime.CurrentMission.MissionDesc;
        photonView.RPC(nameof(RPC_ShowMissionUI), targetPlayer, MissionName, MissionContext);
    }

    private INewDrugMission CreateRandomMission()
    {
        List<INewDrugMission> missions = new List<INewDrugMission>
        {
            new NoItemOnlyBasicAttackMission(),
            new PrecisionDamageMission(100),
            new SpendStaminaInOneTurnMission(9),
            new ReachDefenseInOneTurnMission(500),
        };

        int randomIndex = Random.Range(0, missions.Count);
        return missions[randomIndex];
    }

    public void ReceiveGameEvent(NewDrugGameEvent gameEvent)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            return;

        foreach (var pair in missionByActor)
        {
            NewDrugMissionRuntime runtime = pair.Value;

            if (runtime.State != NewDrugMissionState.Active)
                continue;

            if (runtime.CurrentMission == null)
                continue;

            if (runtime.RewardGranted)
                continue;

            if (!IsEventRelevantToRuntime(runtime, gameEvent))
                continue;

            if (runtime.Context != null)
            {
                runtime.Context.CurrentTurnIndex = gameEvent.TurnIndex;
                runtime.Context.CurrentWaveIndex = gameEvent.WaveIndex;
            }

            runtime.CurrentMission.OnGameEvent(gameEvent);

            if (runtime.CurrentMission.IsSuccess)
            {
                GrantNewDrugReward(runtime);
            }
            else if (runtime.CurrentMission.IsFailed)
            {
                FailMission(runtime);
            }
        }
    }

    private bool IsEventRelevantToRuntime(NewDrugMissionRuntime runtime, NewDrugGameEvent gameEvent)
    {
        switch (gameEvent.Type)
        {
            case NewDrugGameEventType.ItemUsed:
            case NewDrugGameEventType.BasicAttackUsed:
            case NewDrugGameEventType.SkillUsed:
            case NewDrugGameEventType.TreeDamaged:
            case NewDrugGameEventType.StaminaSpent:
            case NewDrugGameEventType.DefenseChanged:
            case NewDrugGameEventType.TurnStarted:
            case NewDrugGameEventType.TurnEnded:
                return gameEvent.ActorNumber == runtime.OwnerActorNumber;

            case NewDrugGameEventType.WaveStarted:
            case NewDrugGameEventType.WaveEnded:
            case NewDrugGameEventType.GameEnded:
                return true;

            default:
                return false;
        }
    }

    private void GrantNewDrugReward(NewDrugMissionRuntime runtime)
    {
        if (runtime.RewardGranted) return;

        runtime.RewardGranted = true;
        runtime.State = NewDrugMissionState.Complete;

        Debug.Log("[신약 개발 미션 성공] 신약 아이템 지급");

        //TODO: 보상 아이템 인벤토리에 지급
        //TODO : UI 처리
        Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(runtime.OwnerActorNumber);
        photonView.RPC(nameof(RPC_PlayMissionSuccesseSFX), targetPlayer);
        string MissionName = runtime.CurrentMission.MissionName;
        string MissionContext = runtime.CurrentMission.MissionDesc;
        photonView.RPC(nameof(RPC_ShowMissionSuccessUI), targetPlayer, MissionName, MissionContext);
        photonView.RPC(nameof(RPC_ActiveNewDrug), targetPlayer, runtime.OwnerActorNumber);
        PhotonPropertyHelper.SetRoomProp(ItemPropKeys.INV_NEWDRUG(runtime.OwnerActorNumber), true);
    }

    private void FailMission(NewDrugMissionRuntime runtime)
    {
        runtime.State = NewDrugMissionState.Failed;

        Debug.Log("[신약 개발 미션 실패]");

        //TODO : ui 처리
        Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(runtime.OwnerActorNumber);
        photonView.RPC(nameof(RPC_PlayMissionFailedSFX), targetPlayer);
        string MissionName = runtime.CurrentMission.MissionName;
        string MissionContext = runtime.CurrentMission.MissionDesc;
        photonView.RPC(nameof(RPC_ShowMissionFailUI), targetPlayer, MissionName, MissionContext);
    }

    [PunRPC]
    private void RPC_PlayMissionSuccesseSFX()
    {
        AudioManager.Instance.PlaySfx2D("MissionSuccess");
    }

    [PunRPC]
    private void RPC_PlayMissionFailedSFX()
    {
        AudioManager.Instance.PlaySfx2D("MissionFailed");
    }

    public void MaskNewDrugUsed(int actorNum)
    {
        NewDrugMissionRuntime runtime = GetOrCreateRuntime(actorNum);

        if (runtime.NewDrugUsed)
            return;

        runtime.NewDrugUsed = true;
    }

    private int GetCurrentTurnIndex()
    {
        return PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
    }

    private int GetCurrentWaveIndex()
    {
        return PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
    }

    [PunRPC]
    private void RPC_ShowMissionReservedUI(string missionName, string missionContext)
    {
        PlayerCanvasController.Instance.ToggleMissionUI(true);
        PlayerCanvasController.Instance.SetMissionUI(missionName, missionContext, NewDrugMissionState.PendingNextDay);
    }

    [PunRPC]
    private void RPC_ShowMissionUI(string missionName, string missionContext)
    {
        PlayerCanvasController.Instance.SetMissionUI(missionName, missionContext, NewDrugMissionState.Active);
    }

    [PunRPC]
    private void RPC_ShowMissionSuccessUI(string missionName, string missionContext)
    {
        PlayerCanvasController.Instance.SetMissionUI(missionName, missionContext, NewDrugMissionState.Complete);
        StartCoroutine(OffMissionUI());
    }

    [PunRPC]
    private void RPC_ActiveNewDrug(int actorNum)
    {
        if (PlayerManager.Instance.PlayersInv.TryGetValue((actorNum), out WorldInventory MyInv))
        {
            MyInv.ToggleNewDrugPosition(true);
        }
        else
        {
            Debug.LogError($"Failed to access {actorNum}'s Inv");
            return;
        }
    }

    [PunRPC]
    private void RPC_ShowMissionFailUI(string missionName, string missionContext)
    {
        PlayerCanvasController.Instance.SetMissionUI(missionName, missionContext, NewDrugMissionState.Failed);
        StartCoroutine(OffMissionUI());
    }

    private IEnumerator OffMissionUI()
    {
        yield return new WaitForSeconds(3f);
        PlayerCanvasController.Instance.ToggleMissionUI(false);
    }
}
