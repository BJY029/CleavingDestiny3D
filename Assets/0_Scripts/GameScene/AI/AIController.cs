using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PhotonView))]
public class AIController : MonoBehaviour, IPlayerAction, IAnimNotify, IPunInstantiateMagicCallback
{
    //AI 플레이어 고유 번호(현재는 1000 으로 고정 번호 부여)
    public int PlayerActNum { get; set; }
    AIBrain aiBrain;

    //AI 움직임 처리용(아직 구현 안함)
    private NavMeshAgent agent;

    //움직임 관련 파라미터
    [Header("Move")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.0f;
    public float rotationSmoothTime = 0.08f;
    public float accelation = 12f;
    public float deceleration = 12f;

    //애니메이션 관련 파라미터
    [Header("Animator")]
    private Animator animator;
    private bool hasAnimator;
    private float _animationBlend;
    private int _animIDSpeedX;
    private int _animIDSpeedZ;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;


    //마을 업그레이드 중인지 여부를 저장할 플래그
    private bool UpgradePhase;
    private bool WhileHittingMotion;
    //데미지 관련 값
    private float damageRatio;
    private int damage;

    public bool isLookingAtTree { get; set; }

    //특정 인벤토리에 들어가기 위한 키(값 = 인벤토리 주인 ActorNum)
    private int InvAdmissionTicket = -1;
    public void SetInvAdmissionTicket(int Num)
    {
        InvAdmissionTicket = Num;
    }
    public int GetInvAdmissionticket()
    {
        return InvAdmissionTicket;
    }


    //프로퍼티 생성시 호출되는 함수
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //플레이어 고유 번호를 초기화 한다.
        if (info.photonView.InstantiationData != null)
        {
            PlayerActNum = (int)info.photonView.InstantiationData[0];
        }
        else
        {
            PlayerActNum = info.photonView.OwnerActorNr;
        }

        //AI 로직을 담당하는 AIBrain 초기화
        aiBrain = GetComponent<AIBrain>();
        aiBrain.InitializeBrain(PlayerActNum);


        //임시 플래그 설정, 움직임 구현 시 설정해야 함
        isLookingAtTree = true;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    //플레이어 턴 처리 비동기 함수(임시)
    public async UniTask PlayTurnAsync()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        CancellationToken token = this.GetCancellationTokenOnDestroy();

        string myOfferKey = ItemPropKeys.OFFER(PlayerActNum);
        string offerStr = PhotonPropertyHelper.GetRoomProp<string>(myOfferKey);

        //일정 시간 딜레이 준 후
        await UniTask.Delay(1000, cancellationToken: token);

        await aiBrain.ItemSelector.ChooseItemAsync(offerStr);

        //일정 시간 딜레이 준 후
        await UniTask.Delay(1500, cancellationToken: token);

        //턴 변경 시도
        TryHit();
    }

    //구현 예정
    public void VillageUpgradePhase()
    {
        throw new System.NotImplementedException();
    }
    //구현 예정
    public void VillageUpgradePhaseOut()
    {
        throw new System.NotImplementedException();
    }

    //턴 변경(나무 때리기) 처리 함수
    public void TryHit(bool IsItRandom = true)
    {
        //현재 임시로 랜덤 데미지 부여하도록 설정
        if (IsItRandom)
        {
            damageRatio = Random.Range(0f, 100f);
        }
        else
        {
            //if (!checkTreeInteractable()) return;
            //Hit 순간의 게이지 데미지 값 받기
            //damageRatio = PlayerCanvasController.Instance.SelectNow();
            //타이머 중지
            //TimeManager.instance.AbortTurnTimer();
        }
        //타이머 중지
        TimeManager.instance.AbortTurnTimer();

        int currentMaxAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PlayerActNum, PlayerPropKeys.MaxAtkPow);
        int currentMinAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PlayerActNum, PlayerPropKeys.MinAtkPow);
        //최적의 데미지 계산 및 반영
        damage = Mathf.RoundToInt(aiBrain.TreeAttacker.SelectDamage());
        //Hit 애니메이션 재생 
        PlayHit();
    }

    //Hit 애니메이션을 재생하는 함수
    public void PlayHit()
    {
        //모션 재생 플래그 활성화
        WhileHittingMotion = true;
        //Hit 관련 UI 비활성화
        PlayerCanvasController.Instance.SetHitTextUnActive();
        //임의의 Hit 모션 재생 후 해당 모션 index 받아오기
        int idx = gameObject.GetComponent<PlayerAnimationController>().PlayHit();
    }

    public void OnAnimStateExit(int stateKey)
    {
        //stateKey가 1이면, 즉 Hit 관련 모션이면
        if (stateKey == 1)
        {
            if (damage < 0)
            {
                Debug.LogError("damage error");
                return;
            }
            //Hit 한 순간의 데미지 값을 인자로 해서 턴 전환 함수 호출
            TurnManager.Instance.RequestChangeTurn(damage, this);
            damage = -1;
            WhileHittingMotion = false;
        }
    }
}
