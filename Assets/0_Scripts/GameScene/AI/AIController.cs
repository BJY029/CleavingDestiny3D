using Cysharp.Threading.Tasks;
using System.Threading;
using Photon.Pun;
using UnityEngine;
using System;
using UnityEngine.AI;
using Photon.Realtime;
using Potan.CoreUtils;

[RequireComponent(typeof(PhotonView))]
public class AIController : MonoBehaviour, IPlayerAction, IAnimNotify, IPunInstantiateMagicCallback
{
    //AI 플레이어 고유 번호
    public int PlayerActNum { get; set; }

    [HideInInspector] public AIBrain aiBrain;
    public PlayerEffectPoints EffectPoints { get; private set; }

    //움직임 관련 파라미터
    [Header("Move")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.0f;
    public float rotationSmoothTime = 0.08f;
    public float accelation = 12f;
    public float deceleration = 12f;

    //애니메이션 관련 파라미터
    private PlayerAnimationController animationController;

    //마을 업그레이드 중인지 여부를 저장할 플래그
    private bool UpgradePhase;
    private bool WhileHittingMotion;
    private bool attackRequestSent;
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

    //시간 제한과 같은 모종의 이유로 AI 로직 취소할 때 사용하는 토큰
    private CancellationTokenSource turnCts;


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
        EffectPoints = GetComponent<PlayerEffectPoints>();
        //애니메이션 컨트롤러 할당
        if (TryGetComponent(out PlayerAnimationController pac))
        {
            animationController = pac;
        }
        else
        {
            DevLog.LogError($"AIController: PlayerAnimationController not found", this);
        }
    }

    private void Start()
    {
        PlayerObjectRegistry.Register(this);
    }

    private void OnDestroy()
    {
        PlayerObjectRegistry.Unregister(this);
    }

    private void Update()
    {
        if (TurnManager.Instance.isUpgradePhase)
        {
            //아직 관련 처리를 진행하지 않은 경우
            if (!UpgradePhase)
            {
                if (turnCts != null)
                {
                    turnCts.Cancel();
                    turnCts.Dispose();
                    turnCts = null;
                }

                VillageUpgradePhase();
                //마을 업그레이드 페이즈에 돌입했음을 명시
                UpgradePhase = true;
            }
            return;
        }
        else //마을 업그레이드 페이즈가 아닌 경우
        {
            //그런데 아직 마을 업그레이드 페이즈로 설정되어 있는 경우
            if (UpgradePhase)
            {
                //관련 처리 진행 후
                VillageUpgradePhaseOut();
                //마을 업그레이드 페이즈에서 빠져나왔음을 명시
                UpgradePhase = false;
            }
        }

        UpdateAnimation();
    }

    // 움직임에 따른 애니메이션 업데이트
    private void UpdateAnimation()
    {
        if (animationController == null) return;

        //NevMeshAgent의 속도 값 받아오기
        Vector3 velocity = aiBrain.aINevMeshController.agent.velocity;
        //현재 속도 값 구하기(벡터 크기)
        float currentSpeed = velocity.magnitude;

        //속도 값 Clamp
        float maxSpeed = aiBrain.aINevMeshController.agent.speed;
        float speed01 = (maxSpeed > 0f) ? Mathf.Clamp01(currentSpeed / maxSpeed) : 0f;

        //거의 움직이지 않는 경우
        if (currentSpeed < 0.01f || speed01 < 0.001f)
        {
            animationController.UpdateMoveVisuals(0f, 0f, Time.deltaTime);
        }
        else
        {
            //방향 구하기(벡터 방향)
            Vector3 dir = velocity.normalized;

            //현재 전방 및 우측 방향을 수평면에 정사영
            Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

            //내적으로 캐릭터 이동 방향 구하기
            float moveX = Vector3.Dot(dir, right) * speed01;
            float moveZ = Vector3.Dot(dir, fwd) * speed01;

            //애니메이션 반영
            animationController.UpdateMoveVisuals(moveX, moveZ, Time.deltaTime);
        }
    }

    private void ResetAnimation()
    {
        animationController?.UpdateMoveVisuals(0f, 0f, 0.1f);
    }

    //플레이어 턴 처리 비동기 함수(임시)
    public async UniTask PlayTurnAsync()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        //기존에 실행하고 있는 UniTask 가 있으면 즉시 중단
        if (turnCts != null)
        {
            turnCts.Cancel();
            turnCts.Dispose();
        }
        //토큰 새로 발행
        turnCts = new CancellationTokenSource();
        CancellationToken token = turnCts.Token;

        try
        {
            string myOfferKey = ItemPropKeys.OFFER(PlayerActNum);
            string offerStr = PhotonPropertyHelper.GetRoomProp<string>(myOfferKey);

            //일정 시간 딜레이 준 후
            await UniTask.Delay(500, cancellationToken: token);

            //아이템 선택 로직 수행
            await aiBrain.ItemSelector.ChooseItemAsync(offerStr, token);

            //임시 코드(Lockpick 연산 수행)
            await aiBrain.ItemActionManager.ProcessLockpickItem(token);

            //일정 시간 딜레이 준 후
            await UniTask.Delay(500, cancellationToken: token);

            //인벤토리 연산 수행
            await aiBrain.InventoryManager.ProcessInventoryAsync(token);

            await UniTask.Delay(500, cancellationToken: token);

            //임시 코드(Lockpick 연산 수행)
            if (await aiBrain.ItemActionManager.ProcessLockpickItem(token))
            {
                await aiBrain.InventoryManager.ProcessInventoryAsync(token);
            }

            //Hit 위치로 이동
            await aiBrain.aINevMeshController.MoveToLocationAsync(LocationCommand.MY_HIT, token);

            //턴 변경 시도
            TryHit();
        }
        //중간에 토큰이 취소될 경우 호출됨
        catch (OperationCanceledException)
        {
            Debug.LogWarning($"[AI {PlayerActNum}] Turn time ended...");
        }
    }

    //제한 시간이 끝났을 때 AI 플레이어를 강제로 텔레포트 시키는 함수
    public void ForceStopAndTeleportToHit()
    {
        //턴 관련 토큰을 취소시켜 AI 로직을 중단시킨다.
        if (turnCts != null)
        {
            turnCts.Cancel();
        }

        //NevMeshAgent의 계산을 중지시킨다.
        NavMeshAgent agent = aiBrain.aINevMeshController.agent;
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
        //순간 이동 및 회전을 수행한다.
        agent.Warp(aiBrain.aINevMeshController.myHitPos);
        aiBrain.aINevMeshController.SnapToTarget(LocationCommand.MY_HIT);
        //애니메이션 초기화
        ResetAnimation();
    }

    // 마을 페이즈 진입
    public void VillageUpgradePhase()
    {
        aiBrain.VillageUpgrader.EnterVillage().Forget();
    }

    // 마을 페이즈 종료
    public void VillageUpgradePhaseOut()
    {
        aiBrain.VillageUpgrader.ExitVillage();
    }

    //턴 변경(나무 때리기) 처리 함수
    public void TryHit(bool IsItRandom = false)
    {
        //현재 임시로 랜덤 데미지 부여하도록 설정
        if (IsItRandom)
            damageRatio = UnityEngine.Random.Range(0f, 100f);
        else
            //타이머 중지
            TimeManager.instance.AbortTurnTimer();

        int currentMaxAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PlayerActNum, PlayerPropKeys.MaxAtkPow);
        int currentMinAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PlayerActNum, PlayerPropKeys.MinAtkPow);
        //최적의 데미지 계산 및 반영
        damage = (IsItRandom) ?
            currentMinAtkDamage + Mathf.RoundToInt((currentMaxAtkDamage - currentMinAtkDamage) * (damageRatio / 100))
            : Mathf.RoundToInt(aiBrain.TreeAttacker.SelectDamage());
        //Hit 애니메이션 재생 

        attackRequestSent = false;
        PlayHit();
    }

    //Hit 애니메이션을 재생하는 함수
    public void PlayHit()
    {
        //모션 재생 플래그 활성화
        WhileHittingMotion = true;
        //Hit 관련 UI 비활성화
        PlayerCanvasController.Instance.SetHitTextUnActive();

        // Hit 모션 재생
        animationController?.PlayHitAnimation();
    }

    public void RequestAttackAtImpact()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (attackRequestSent) return;

        if (damage < 0)
        {
            Debug.LogError("damage error");
            return;
        }

        attackRequestSent = true;

        ItemVFXController.Instance.Any_StopItemVFX(VFXType.PowerUP, PlayerActNum);
        TurnManager.Instance.RequestChangeTurn(damage, this);
    }

    public void OnAnimStateExit(int stateKey)
    {
        //stateKey가 1이면, 즉 Hit 관련 모션이면
        if (stateKey == 1)
        {
            if (!attackRequestSent && damage >= 0)
            {
                Debug.LogError("damage error");
                RequestAttackAtImpact();
            }
            //Hit 한 순간의 데미지 값을 인자로 해서 턴 전환 함수 호출
            //TurnManager.Instance.RequestChangeTurn(damage, this);
            damage = -1;
            WhileHittingMotion = false;
        }
    }
}
