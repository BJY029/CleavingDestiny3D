using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockpickController : MonoBehaviourPunCallbacks, IMinigameInteractable
{
    public static LockpickController instance;

    //관련 UI
    [Header("UI Components")]
    public GameObject gameObj;
    public RectTransform needleRect;
    public RectTransform targetZoneRect;

    [Header("Settings")]
    public float rotationSpeed = 200f;
    //성공 감지 범위
    [Range(5f, 30f)]
    public float SuccessAngleRange = 15f;
    //회전 방향(-1 | 1)
    private int SpinDirection = 1;
    //게임 진행 여부
    private bool isGameActive = false;
    //남은 Lock 수
    private int currentRemainingLocks = 0;

    //잠금 해제 성공 시, 베리어를 해제하기 위한 인자들
    private int TargetBarrierOwnerActNum = -1;
    private InventoryBarrier IB;
    private IPlayerAction Requester;

    private TextMeshProUGUI DescText;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DescText = gameObj.GetComponentInChildren<TextMeshProUGUI>();
        DescText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_PlayerSpace);
        gameObj.SetActive(false);
    }

    void Update()
    {
        if (!isGameActive) return;

        //게임 진행 시, 바늘을 지속적으로 회전시킨다.
        needleRect.Rotate(0, 0, -rotationSpeed * SpinDirection * Time.deltaTime);
    }

    //게임 시작시 함수
    public void SetGameActive(int ActorNumber, InventoryBarrier ib, IPlayerAction PC)
    {
        TargetBarrierOwnerActNum = ActorNumber;
        //특정 플레이어의 Lock 수를 가져온다.
        int LockCnt = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.LOCKCNT(ActorNumber));
        Debug.Log(LockCnt);
        //UI 활성화
        gameObj.SetActive(true);
        //게임 시작 플래그 설정
        isGameActive = true;
        //Mini 게임 시작
        StartMinigame(LockCnt);

        //잠금 해제를 당하는 인벤토리 베리어
        IB = ib;
        //잠금 해제를 시도하는 플레이어
        Requester = PC;
    }


    public void SetGameActiveForAI(int ActorNumber, InventoryBarrier ib, IPlayerAction PC)
    {
        //잠금 해제하고자 하는 베리어 주인 Actor 번호
        TargetBarrierOwnerActNum = ActorNumber;
        //잠금 해제를 당하는 인벤토리 베리어
        IB = ib;
        //잠금 해제를 시도하는 플레이어
        Requester = PC;
    }

    //성공하면 성공한 플레이어에게 권한 부여
    public void IsAISuccess(bool b)
    {
        if (b)
        {
            GameObject player;
            if (Requester is AIController childObjAI)
            {
                player = childObjAI.gameObject;
                int ActNum = player.GetComponent<AIController>().PlayerActNum;
                //잠금 해제를 당한 베리어에 입장할 수 있는 권한 부여
                IB.GrantPermission(ActNum, player);
                //잠금 해제를 성공한 플레이어에게 해당 인벤토리에 상호작용이 가능하도록 키를 부여
                Requester.SetInvAdmissionTicket(TargetBarrierOwnerActNum);
            }
        }
        InitGameInfo();
    }

    //게임 그만하기
    public void StopGameActive()
    {
        isGameActive = false;
        gameObj.SetActive(false);
    }

    //미니게임 시작 함수
    public void StartMinigame(int requiredUnlockCount)
    {
        //현재 남은 Lock 수 가져오기
        currentRemainingLocks = requiredUnlockCount;
        //게임 플래그 설정
        isGameActive = true;
        gameObj.SetActive(true);

        SpinDirection = 1; // 방향 초기화
        SetRandomTarget(); // 첫 타겟 설정

        AudioManager.Instance.PlayTemporaryBgm("Gear");
    }

    //체크 포인트를 랜덤 위치에 생성하는 함수
    private void SetRandomTarget()
    {
        float randomAngle = Random.Range(0f, 360f);
        targetZoneRect.localEulerAngles = new Vector3(0, 0, randomAngle);
    }

    //상호작용 시(Spacebar) 호출될 함수
    private void TryUnlock()
    {
        //바늘과 체크포인트의 위치를 받아온다.
        Quaternion needleRot = needleRect.rotation;
        Quaternion targetRot = targetZoneRect.rotation;

        //두 각도 차이 계산
        float angleDiff = Quaternion.Angle(needleRot, targetRot);

        //클라이언트에서 즉시 성공/실패 판정
        if (angleDiff <= SuccessAngleRange)
        {
            Debug.Log("Hit! Next Stage.");
            AudioManager.Instance.PlaySfx2D("Lock_Click");
            HandleLocalSuccess();
        }
        else
        {
            Debug.Log("Miss! Failed.");
            HandleLocalFail();
        }
    }

    //성공으로 판정시
    private void HandleLocalSuccess()
    {
        currentRemainingLocks--;

        // 모든 자물쇠를 풀었는지 확인
        if (currentRemainingLocks <= 0)
        {
            // 최종 성공!
            isGameActive = false;
            gameObj.SetActive(false);
            //잠금 해제를 성공한 플레이어
            int ActNum = PhotonNetwork.LocalPlayer.ActorNumber;
            GameObject player;
            if (Requester is PlayerController childObj)
            {
                player = childObj.gameObject;
            }
            else if (Requester is AIController childObjAI)
            {
                player = childObjAI.gameObject;
            }
            else
            {
                Debug.LogError("Requester has non PlayerController or AIController");
                return;
            }
            //잠금 해제를 성공한 플레이어 오브젝트
            //GameObject player = Requester.gameObject;
            //잠금 해제를 당한 베리어에 입장할 수 있는 권한 부여
            IB.GrantPermission(ActNum, player);
            //잠금 해제를 성공한 플레이어에게 해당 인벤토리에 상호작용이 가능하도록 키를 부여
            Requester.SetInvAdmissionTicket(TargetBarrierOwnerActNum);
            //알림 전송
            photonView.RPC(nameof(RPC_NotifyUnlockSuccess), RpcTarget.MasterClient, ActNum);

            InitGameInfo();
            AudioManager.Instance.PlaySfx2D("Unlock");
        }
        else
        {
            // 아직 단계가 남았으면 다음 단계 진행 (방향 반대로, 타겟 재설정)
            SpinDirection *= -1;
            SetRandomTarget();
        }
    }

    //실패로 판정시
    public void HandleLocalFail()
    {
        // 실패 처리
        isGameActive = false;
        gameObj.SetActive(false);

        InitGameInfo();

        // 필요하다면 실패 사실을 서버에 알림
        // photonView.RPC(nameof(RPC_NotifyUnlockFail), RpcTarget.MasterClient);
    }

    private void InitGameInfo()
    {
        IB = null;
        Requester = null;
        TargetBarrierOwnerActNum = -1;
        AudioManager.Instance.RestorePreviousBgm();
    }
    //MasterClient의 최종 처리(문 열기 처리)
    [PunRPC]
    private void RPC_NotifyUnlockSuccess(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"Player {actorNumber} has unlocked the door!");

        // 여기서 실제 문을 여는 로직이나 룸 프로퍼티 업데이트 수행
        // 예: PhotonPropertyHelper.SetRoomProp(ItemPropKeys.IS_DOOR_OPEN, true);

        // 필요하다면 모든 클라이언트에게 문이 열렸음을 알리는 RPC 호출
        // photonView.RPC("RPC_OpenDoorVisuals", RpcTarget.All);
    }

    //현재 게임이 활성화 되었는지 체크
    public bool IsGameActive()
    {
        return isGameActive;
    }

    //인터페이스 함수
    //상호작용 함수
    public void OnInteract(PlayerController pc)
    {
        if (!isGameActive) return;
        TryUnlock();
    }
}
