using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TimeManager : MonoBehaviourPunCallbacks
{
    public static TimeManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    //MasterClient가 관리
    [HideInInspector]
    //Master가 대표로 관리하며, 해당 플래그를 브로드캐스트 하여 클라 전체에게 공유한다.
    public bool TurnTimerActivated { get; private set; } = false;
    public bool ForceToHit { get; private set; } = false;
    //각 플레이어에게 주어지는 시간(RoomProp에서 받아옴)
    private float PlayerTurnLimitedTime;
    //시작, 끝 시간
    private double _startTime;
    private double _endTime;
    //타이머 요청자
    private Player Requester;


    private void Start()
    {
        _startTime = -1f;
        _endTime = -1f;
        Requester = null;
        PlayerTurnLimitedTime = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TurnTime);
    }

    //Start에서도 초기화 하지만, 혹시 모를 오류를 대비하여 이를 통해서도 값을 받아온다.
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue(RoomPropKeys.TurnTime, out var time))
        {
            PlayerTurnLimitedTime = (float)time;
        }
    }

    //시간 계산 로직
    public void Update()
    {
        //턴 타이머가 시작되고, MasterClient인 경우에만 시간을 계산한다.
        if (!TurnTimerActivated) return;

        // 게임이 종료(END)되었다면 타이머를 비활성화합니다.
        if (PhotonNetwork.InRoom)
        {
            GamePhaseValue phase = PhotonPropertyHelper.GetRoomProp<GamePhaseValue>(RoomPropKeys.GamePhase);
            if (phase == GamePhaseValue.END)
            {
                TurnTimerActivated = false;
                return;
            }
        }

        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.Time >= _endTime)
            {
                EndTurnLogic();
            }
        }
    }

    //타이머 시작 함수
    public void StartTurnTimer()
    {
        //MasterClient에게 타이머 시작 로직을 요청한다.
        photonView.RPC(nameof(RPC_StartTurnTimer), RpcTarget.MasterClient);
    }

    //타이머 중단 함수(플레이어가 임의로 턴을 넘긴 경우 호출 됨)
    public void AbortTurnTimer()
    {
        photonView.RPC(nameof(RPC_AbortTurnTimer), RpcTarget.MasterClient);
    }

    //타이머가 끝나서 턴을 넘기는 함수(MasterClient가 호출함)
    private void EndTurnLogic()
    {
        //타이머 비활성화 설정
        TurnTimerActivated = false;
        int curTurnNum = GameHelper.getCurrentTurnActorNum();

        //요청자 정보가 있고, 해당 요청자가 현재 턴인 경우
        if (Requester != null && curTurnNum == Requester.ActorNumber)
        {
            //해당 요청자에게 턴 종료 처리 진행하도록 설정
            photonView.RPC(nameof(RPC_RequesterTurnEnded), Requester);
        }
        //요청자가 현재 턴이 아니고, 싱글 플레이 모드인 경우
        else if (GameManager.Instance.isSoloPlay)
        {
            //현재 턴에 해당되는 ai 오브젝트를 가져오고
            if (PlayerManager.Instance.AIPlayerObj.TryGetValue(curTurnNum, out GameObject AI))
            {
                //관련 함수 실행
                AIController AC = AI.GetComponent<AIController>();
                AC.ForceStopAndTeleportToHit();
                AC.TryHit(true);
            }
        }

        _startTime = -1f;
        _endTime = -1f;
        Requester = null;
    }

    //MasterClient가 실행하는 턴 시작 설정
    [PunRPC]
    public void RPC_StartTurnTimer(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        //요청자의 ActNum을 받아서 저장한다.
        Requester = PhotonNetwork.CurrentRoom.GetPlayer(info.Sender.ActorNumber);

        //타이머 활성화
        TurnTimerActivated = true;
        //해당 플래그를 브로드캐스트 한다.
        photonView.RPC(nameof(Broadcast_SetTurnTimeFlag), RpcTarget.Others, TurnTimerActivated);

        //시작, 끝 시간 설정 및 프로퍼티 업데이트
        _startTime = PhotonNetwork.Time;
        _endTime = _startTime + PlayerTurnLimitedTime;
        Vector2 timeValue = new Vector2((float)_startTime, (float)_endTime);
        //시간 프로퍼티 삽입
        PhotonPropertyHelper.SetRoomProp(RoomPropKeys.PlayerTurnStartEndTime, timeValue);
    }

    //MasterClient가 실행하는 턴 타이머 중단 함수
    [PunRPC]
    public void RPC_AbortTurnTimer()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        //타이머 중단
        TurnTimerActivated = false;
        //브로드캐스트로 플래그 전파
        photonView.RPC(nameof(Broadcast_SetTurnTimeFlag), RpcTarget.Others, TurnTimerActivated);
        _startTime = -1f;
        _endTime = -1f;
        Requester = null;
    }

    [PunRPC]
    public void Broadcast_SetTurnTimeFlag(bool flag)
    {
        TurnTimerActivated = flag;
    }

    //타이머 요청자가 실행하는 타이머 종료 함수
    [PunRPC]
    public void RPC_RequesterTurnEnded(PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient) return;
        //PlayerManager에서 요청자의 플레이어 오브젝트를 받아온다.
        if (PlayerManager.Instance.LocalPlayerObj == null)
        {
            Debug.LogError("No PlayerController Component!!");
            return;
        }

        //UniTask로 이동 및 턴 넘기기 로직을 실행한다.
        ProcessPlayerTurnEndByTimer_Async().Forget();
    }

    //턴 종료 및 턴 넘기기 로직
    public async UniTask ProcessPlayerTurnEndByTimer_Async()
    {
        ForceToHit = true;
        //TODO : 플레이어 이동
        //플레이어 번호
        int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
        //플레이어 오브젝트
        GameObject PlayerObj = PlayerManager.Instance.LocalPlayerObj;
        //플레이어 이동 위치 및 회전 정보
        Vector3 des = PlayerManager.Instance.hitPos[actNum - 1];
        Quaternion rot = PlayerManager.Instance.spawnRot[actNum - 1];

        InitGameUIs();
        //플레이어의 PlayerController 컴포넌트
        PlayerController pc = PlayerObj.GetComponent<PlayerController>();

        //FadeIn
        await FadeCanvas.Instance.FadeInAsync(1f);

        //플레이어 순간이동
        if (PlayerObj != null)
            TeleportPlayer(PlayerObj, des, rot);

        //FadeOut
        await FadeCanvas.Instance.FadeOutAsync(1f);

        //턴 넘기기 시도
        if (pc != null)
            pc.TryHit(true);

        ForceToHit = false;
    }

    private void InitGameUIs()
    {
        InventoryLockCanvasController.Instance.UnSetLockpickUI();
        ItemSelectionController.instance.CloseItemSelection();
        LockpickController.instance.HandleLocalFail();
        ItemOfferCanvasController.instance.Close();
    }

    //플레이어를 특정 위치로 이동시키는 함수
    private void TeleportPlayer(GameObject player, Vector3 destination, Quaternion rotation)
    {
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = destination;
            player.transform.rotation = rotation;
            cc.enabled = true;
        }
    }
}
