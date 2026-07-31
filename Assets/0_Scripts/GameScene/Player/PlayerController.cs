using NUnit.Framework;
using Photon.Pun;
using Potan.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public interface ILookInteractable
{
    void OnLookEnter(IPlayerAction pc);
    void OnLookExit(IPlayerAction pc);
    void OnInteract(IPlayerAction pc);
}

//미니게임 상호작용 인터페이스
public interface IMinigameInteractable
{
    void OnInteract(PlayerController pc);
}

//CharacterController 컴포넌트 강제 할당
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPun, IPlayerAction, IAnimNotify
{
    public int PlayerActNum { get; set; }
    //움직임 관련 파라미터
    [Header("Move")]
    public float walkSpeed = 3.5f;
    public float sprintSpeed = 6.0f;
    public float rotationSmoothTime = 0.08f;
    public float accelation = 12f;
    public float deceleration = 12f;

    //중력 관련 파라미터
    [Header("Gravity")]
    public float gravity = -20f;
    public Transform groundCheck;      // 발 아래 빈 오브젝트 추천
    public LayerMask groundLayers = ~0;

    //카메라 관련 파라미터
    [Header("Camera")]
    public Transform pivotTransform;  // Main Camera의 pivot
    public GameObject _mainCamera;  //실제 카메라

    //애니메이션 관련 파라미터
    private PlayerAnimationController animationController;

    [Header("First Person")]
    public GameObject firstPersonObjects; //1인칭 시점 전용 오브젝트

    //감도
    [SerializeField]
    private float mouseSensitivity = 0.1f;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f; //위쪽 카메라 회전 제한

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;//아래쪽 카메라 회전 제한

    [Header("Terrain Position")]
    [SerializeField] private Terrain targetTerrain;
    [SerializeField] private float terrainHeightOffset = 0.05f;

    //input system 관련 파라미터
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintPressed;

    private bool isPreparingTreeCut = false; // 나무 베기 준비 상태
    private bool attackRequestSent;

    //움직임 관련 코드 내부에서 사용하는 파라미터
    private CharacterController characterController;
    private float currentSpeed;
    private Vector3 velocity;
    //중력 관련 코드 내부에서 사용하는  파라미터
    public bool isGrounded;
    private float groundCheckRadius;

    private const float _threshold = 0.01f; //미세 입력 무시 기준값	

    //회전 관련 파라미터
    private float Yaw;
    private float Pitch;

    private Camera cam;
    [Header("Raycast")]
    [SerializeField] private float RayDistance = 5f;
    //Ray를 비활성화 하기 위한 레이 거리 조절기(비활성화시 0으로 설정되어 곱해짐)
    public int RayMultiplyer = 1;
    [SerializeField] private LayerMask targetLayer = ~0;
    private ILookInteractable currentInteractable;

    private IMinigameInteractable currentMinigame;

    public bool isLookingAtTree { get; set; }

    // 도끼로 타격하기 좋은 위치
    public float properDistanceToTree = 2.5f;


    //마을 업그레이드 중인지 여부를 저장할 플래그
    private bool UpgradePhase;
    private bool WhileAnimation;
    private float damageRatio;
    private int damage;

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

    [Header("First Person Visual Settings")]
    [SerializeField] private GameObject[] characterModelObjects; // 캐릭터 몸체 모델 오브젝트 (1인칭 시 그림자 전용 처리)

    private void Awake()
    {
        //애니메이션 컨트롤러 할당
        if (!TryGetComponent(out animationController))
        {
            DevLog.LogError($"PlayerController: PlayerAnimationController not found", this);
        }

        //컨트롤러 받아오기
        if (!TryGetComponent(out characterController))
        {
            DevLog.LogError($"PlayerController: CharacterController not found", this);
        }

        //groundCheckRadius 안전 처리
        groundCheckRadius = 0.2f;
        if (groundCheck != null)
        {
            var sc = groundCheck.GetComponent<SphereCollider>();
            if (sc) groundCheckRadius = sc.radius;
        }

        // 만약 내 플레이어가 아니라면
        if (!photonView.IsMine)
        {
            //해당 플레이어의 카메라를 끈다.
            if (_mainCamera != null) _mainCamera.SetActive(false); // 원격 카메라 끄기

            if (firstPersonObjects != null)
            {
                firstPersonObjects.SetActive(false); //1인칭 오브젝트 끄기
            }
            return;
        }

        // 이후부터는 로컬 플레이어만 실행하는 코드

        // 캐릭터 본체 모델만 숨기고 그림자는 남김 (휴머노이드 손 축에 별도로 붙은 도끼는 렌더링 유지)
        HideCharacterModelBody();

        if (firstPersonObjects != null)
        {
            DisableFirstPersonRenderersOnly(firstPersonObjects); // 1인칭 카메라 등 제외 1인칭 레거시 렌더러 비활성화
        }

        cam = _mainCamera.GetComponent<Camera>();
        CameraSwitchManager.Instance.RegisterPlayerCamera(cam);
        DamageTextManager.instance.SetTargetCamera(cam);

        isLookingAtTree = false;
        UpgradePhase = false;
        WhileAnimation = false;
        RayMultiplyer = 1;
    }

    private void HideCharacterModelBody()
    {
        if (characterModelObjects != null)
        {
            // 지정된 캐릭터 본체 모델의 렌더러만 ShadowsOnly로 처리
            foreach (var model in characterModelObjects)
            {
                SetShadowsOnlyRecursively(model);
            }
        }
        else
        {
            // 미지정 시 캐릭터 SkinnedMeshRenderer (몸체) 메쉬만 ShadowsOnly 처리
            var renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var r in renderers)
            {
                if (r != null && (firstPersonObjects == null || !r.transform.IsChildOf(firstPersonObjects.transform)))
                {
                    // 도끼(MeshRenderer나 Axe이름)가 아닌 스킨드 메쉬 본체만 그림자 전용 설정
                    if (!r.name.ToLower().Contains("axe") && !r.name.ToLower().Contains("weapon"))
                    {
                        r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    }
                }
            }
        }
    }

    private void DisableFirstPersonRenderersOnly(GameObject root)
    {
        if (root == null) return;
        root.SetActive(true); // 카메라는 작동해야 하므로 오브젝트는 활성화 유지

        // 자식 오브젝트들의 Renderer만 비활성화하여 1인칭 전용 도끼/손 메쉬만 숨김
        var renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = false;
        }
    }



    // 해당 오브젝트의 모든 자식 오브젝트들의 렌더러를 ShadowsOnly 모드로 변경하는 함수 (본인 포함)
    private void SetShadowsOnlyRecursively(GameObject obj, GameObject dismissObj = null)
    {
        if (obj == null) return;

        if (obj.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            if (child.gameObject != dismissObj)
            {
                SetShadowsOnlyRecursively(child.gameObject, dismissObj);
            }
        }
    }

    //해당 캐릭터가 활성화 혹은 비활성화 되면 움직임을 제한
    private void OnEnable()
    {
        if (!photonView.IsMine) return;
        TrySubscribeEvents();
    }

    private void Start()
    {
        if (!photonView.IsMine) return;
        TrySubscribeEvents();
    }

    private void TrySubscribeEvents()
    {
        if (isSubscribed) return;
        if (KeyInteractManager.Instance == null) return;

        KeyInteractManager.Instance.OnInteractKeyDown += HandleInteractFKeyDown;
        KeyInteractManager.Instance.OnInteractKeyUp += HandleInteractFKeyUp;
        KeyInteractManager.Instance.OnInteractSpaceKeyDown += HandleInteractSpaceKey;

        KeyInteractManager.Instance.OnMoveInput += HandleMoveInputEvent;
        KeyInteractManager.Instance.OnMousePositionInput += HandleLookInputEvent;
        KeyInteractManager.Instance.OnRunInput += HandleRunInputEvent;

        isSubscribed = true;
    }

    private void OnDisable()
    {
        if (!photonView.IsMine) return;

        if (isSubscribed && KeyInteractManager.Instance != null)
        {
            KeyInteractManager.Instance.OnInteractKeyDown -= HandleInteractFKeyDown;
            KeyInteractManager.Instance.OnInteractKeyUp -= HandleInteractFKeyUp;
            KeyInteractManager.Instance.OnInteractSpaceKeyDown -= HandleInteractSpaceKey;

            KeyInteractManager.Instance.OnMoveInput -= HandleMoveInputEvent;
            KeyInteractManager.Instance.OnMousePositionInput -= HandleLookInputEvent;
            KeyInteractManager.Instance.OnRunInput -= HandleRunInputEvent;

            isSubscribed = false;
        }
    }

    private bool inputLocked = true;
    private bool isSubscribed = false;

    private void ResetInputState()
    {
        moveInput = Vector2.zero;
        lookInput = Vector2.zero;
        sprintPressed = false;
    }

    private void ResetMotionState()
    {
        currentSpeed = 0f;

        // 수평 이동 끊기
        velocity.x = 0f;
        velocity.z = 0f;

        // 바닥에 붙어있으면 아래로 살짝 눌러주는 값 유지(원하면 0으로 해도 됨)
        if (isGrounded) velocity.y = -2f;
    }

    public void SetInputLocked(bool locked)
    {
        if (inputLocked == locked) return; // 상태 변화가 없다면 바이패스 (비동기 입력 씹힘 방지)

        inputLocked = locked;

        if (locked)
        {
            // 중앙 매니저를 통해 조작 입력 잠금
            KeyInteractManager.Instance?.SetPlayerActionsEnabled(false);

            ResetInputState();
            ResetMotionState();

            // 애니메이션 즉시 정지
            animationController?.UpdateMoveVisuals(0f, 0f, 0.1f);
        }
        else
        {
            // 중앙 매니저의 인풋 입력 재활성화
            KeyInteractManager.Instance?.SetPlayerActionsEnabled(true);

            ResetInputState(); // 조개 풀릴 때 0으로 시작
        }
    }

    //마을 업그레이드에 돌입시 실행될 함수
    public void VillageUpgradePhase()
    {
        //플레이어 카메라를 끄고
        cam.enabled = false;
        //메인 카메라 켜기
        //CameraSwitchManager.Instance.GameCameraToggle(true);
    }

    //마을 업그레이드가 끝나면 실행될 함수
    public void VillageUpgradePhaseOut()
    {
        //메인 카메라 끄고
        //CameraSwitchManager.Instance.GameCameraToggle(false);
        //플레이어 카메라 켜기
        cam.enabled = true;
    }

    //움직임 및 중력 관련 코드 실행
    private void Update()
    {
        //내 photonView가 아니면 실행하지 않는다.
        if (!photonView.IsMine)
        {
            return;
        }

        // 나무 베기 준비 상태 중 F 이외의 키 입력 시 취소 처리
        if (isPreparingTreeCut)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame && !Keyboard.current.fKey.wasPressedThisFrame)
            {
                CancelTreeCut();
            }
        }

        if (TimeManager.instance.ForceToHit)
        {
            SetInputLocked(true);
            return;
        }

        if (WhileAnimation)
        {
            SetInputLocked(true);
            return;
        }
        if (ItemOfferCanvasController.instance.isOfferPanelOpened
        || ItemSelectionController.instance.IsItemSelectionActivated
        || SettingCanvasController.instance.IsSettingPanelOpened
        || BettingSystemController.instance.BettingSystemActivated)
        {
            SetInputLocked(true);
            return;
        }
        //만약 특정 미니게임이 실행되었다면
        if (LockpickController.instance.IsGameActive())
        {
            //만약 현재 실행중인 미니게임이 없다면, 해당 인터페이스로 설정
            if (currentMinigame == null) currentMinigame = LockpickController.instance;
            //움직임 막기
            SetInputLocked(true);
            return;
        }
        if (WoodChopController.instance.isPlaying)
        {
            if (currentMinigame == null) currentMinigame = WoodChopController.instance;
            //움직임 막기
            SetInputLocked(true);
            return;
        }

        //별다른 미니게임이 실행중이지 않으면, 인터페이스를 초기화한다.
        if (currentMinigame != null) currentMinigame = null;
        //만약 마을 업그레이드 페이즈에 돌입한 경우
        if (TurnManager.Instance.isUpgradePhase)
        {
            //아직 관련 처리를 진행하지 않은 경우
            if (!UpgradePhase)
            {
                //처리 진행후
                VillageUpgradePhase();
                //마을 업그레이드 페이즈에 돌입했음을 명시
                UpgradePhase = true;
            }
            SetInputLocked(true);
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

        SetInputLocked(false);
        ReadInput();
        GroundCheck();
        HandleMovement(Time.deltaTime);
        HandleGravity(Time.deltaTime);

        animationController?.UpdateCamera(lookInput);

        //카메라 정면 방향으로 Ray 발사
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        //디버그용 Ray 그리기
        Debug.DrawRay(ray.origin, ray.direction * RayDistance, Color.green);
        //QueryTriggerInteraction.Collide : Trigger로 설정된 콜라이더와도 충돌을 감지
        if (Physics.Raycast(ray, out var hit, RayDistance, targetLayer, QueryTriggerInteraction.Collide))
        {
            //충돌한 물체 혹은 그 부모에게서 ILookInteractable 인터페이스 찾기
            var next = hit.transform.GetComponentInParent<ILookInteractable>();

            //보고있던 대상이 변경 되었는지 확인
            if (!ReferenceEquals(next, currentInteractable))
            {
                //기존에 보고 있던 것이 있으면, 시선 해제
                currentInteractable?.OnLookExit(this);
                //현재 대상을 새로운 것으로 교체
                currentInteractable = next;
                //새로운 대상에게 시선 진입 처리
                currentInteractable?.OnLookEnter(this);
            }
        }
        else //Raycast 실패 시
        {
            //보고 있던 것이 있었다면 해제
            if (currentInteractable != null)
            {
                currentInteractable.OnLookExit(this);
                currentInteractable = null;
            }
        }
    }

    //처음 ray로 감지했을 때 실행될 함수
    void OnRayEnter(Transform transform)
    {
        //현재 감지중인 오브젝트의 레이어 가져오기
        LayerMask detectedLayer = transform.gameObject.layer;

        //감지한 오브젝트의 레이어가 Tree인 경우
        if (detectedLayer == LayerMask.NameToLayer(CommonDefine.TREELAYER))
        {
            //Hit 관련 텍스트 표출
            PlayerCanvasController.Instance.SetHitTextActive();
            //현재 나무를 보고있다고 플래그 설정
            isLookingAtTree = true;
        }
    }

    //방금 ray에서 벗어난 경우
    void OnRayExit(Transform transform)
    {
        //기존에 감지중이였던 오브젝트의 레이어 가져오기
        LayerMask outLayer = transform.gameObject.layer;

        //방금까지 감지중이였던 레이어가 나무였으면
        if (outLayer == LayerMask.NameToLayer(CommonDefine.TREELAYER))
        {
            //Hit 관련 텍스트를 끈다.
            PlayerCanvasController.Instance.SetHitTextUnActive();
            //나무를 보고 있지 않다고 플래그 설정
            isLookingAtTree = false;
        }
    }

    // F 키를 눌렀을 때 (Press Down) - 나무 베기 준비 상태 전환 / 이미 준비 상태면 타격 실행
    private void HandleInteractFKeyDown()
    {
        if (!photonView.IsMine) return;
        if (!GameHelper.IsMyTurn()) return;

        if (isPreparingTreeCut)
        {
            TriggerTreeStrike();
        }
        else if (isLookingAtTree && !WhileAnimation)
        {
            isPreparingTreeCut = true;
            WhileAnimation = true; // 차징 중 움직임 잠금
            SetInputLocked(true); // 상호작용 즉시 입력/속도 잠금 (슬라이딩 방지)

            MoveToTreeAttackPosition();

            PlayerCanvasController.Instance.SetHitTextUnActive();
            PlayerCanvasController.Instance.OpenGauge();

            animationController?.PlayReadyAnimation();
        }
        else
        {
            currentInteractable?.OnInteract(this);
        }
    }

    // F 키를 뗐을 때 (Release Up) - 토글형으로 변경으로 인해 아무 작업도 수행하지 않음
    private void HandleInteractFKeyUp()
    {
        // 토글형 시스템(KeyDown으로 준비, 다시 KeyDown으로 타격)이므로 KeyUp 이벤트에서는 처리하지 않습니다.
    }

    // 타격 실행 처리 메서드 (2번째 F KeyDown 입력 시 호출)
    private void TriggerTreeStrike()
    {
        isPreparingTreeCut = false;

        damageRatio = PlayerCanvasController.Instance.SelectNow();
        TimeManager.instance.AbortTurnTimer();
        ItemOfferCanvasController.instance.Close();

        int currentMaxAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MaxAtkPow);
        int currentMinAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MinAtkPow);
        damage = currentMinAtkDamage + Mathf.RoundToInt((currentMaxAtkDamage - currentMinAtkDamage) * (damageRatio / 100));

        DevLog.Log($"<color=green>[HIT - DoublePressStrike]</color> Player {PlayerActNum} KeyDown Strike with damageRatio: {damageRatio}, calculated damage: {damage}", this);

        attackRequestSent = false;
        animationController?.PlayStrikeAnimation();
    }

    // 나무 베기 준비 상태 취소 메서드
    private void CancelTreeCut()
    {
        isPreparingTreeCut = false;
        WhileAnimation = false;

        PlayerCanvasController.Instance.CloseGauge();
        PlayerCanvasController.Instance.SetHitTextActive(); // 나무 근처에 있으므로 텍스트 복구

        animationController?.CancelReadyAnimation();
    }

    //스페이스 바가 눌렸을 때 실행될 함수
    private void HandleInteractSpaceKey()
    {
        //내 객체가 아니면 return
        if (!photonView.IsMine) return;
        if (WoodChopController.instance.isPlaying)
        {
            //미니 게임 관련 인터페이스 상호작용 수행
            currentMinigame?.OnInteract(this);
        }

        //내 턴이 아니면 return
        if (!GameHelper.IsMyTurn()) return;

        //미니 게임 관련 인터페이스 상호작용 수행
        currentMinigame?.OnInteract(this);
    }

    private void HandleMoveInputEvent(Vector2 input)
    {
        if (inputLocked) return;
        moveInput = input;
    }

    private void HandleLookInputEvent(Vector2 input)
    {
        if (inputLocked) return;
        lookInput = input;
    }

    private void HandleRunInputEvent(bool isRunning)
    {
        if (inputLocked) return;
        sprintPressed = isRunning;
    }

    private bool checkTreeInteractable()
    {
        if (!isLookingAtTree) return false;
        if (WhileAnimation) return false;
        return true;
    }

    public void TryHit(bool IsItRandom = false)
    {
        //만약, 사용자 선택이 아닌 임의의 데미지인 경우(턴 시간 초과)
        if (IsItRandom)
        {
            damageRatio = Random.Range(0f, 100f);
        }
        else
        {
            if (!checkTreeInteractable()) return;
            //Hit 순간의 게이지 데미지 값 받기
            damageRatio = PlayerCanvasController.Instance.SelectNow();
            //타이머 중지
            TimeManager.instance.AbortTurnTimer();

            MoveToTreeAttackPosition();
        }
        //Offer 패널 접근 막기
        ItemOfferCanvasController.instance.Close();
        int currentMaxAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MaxAtkPow);
        int currentMinAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MinAtkPow);
        damage = currentMinAtkDamage + Mathf.RoundToInt((currentMaxAtkDamage - currentMinAtkDamage) * (damageRatio / 100));
        //Hit 애니메이션 재생 
        attackRequestSent = false;
        PlayHit();
    }

    //Hit 애니메이션을 재생하는 함수
    public void PlayHit()
    {
        DevLog.Log($"<color=green>[HIT]</color> Player {PlayerActNum} TryHit with damageRatio: {damageRatio}, calculated damage: {damage}", this);
        //모션 재생 플래그 활성화
        WhileAnimation = true;
        //Hit 관련 UI 비활성화
        PlayerCanvasController.Instance.SetHitTextUnActive();

        // 애니메이션 컨트롤러에 타격 요청 (내부에서 RPC 처리됨)
        animationController?.PlayHitAnimation();
    }

    public void RequestAttackAtImpact()
    {
        if (!photonView.IsMine) return;

        if (attackRequestSent) return;

        if (damage < 0)
        {
            Debug.LogError("damage error");
            return;
        }

        attackRequestSent = true;

        TurnManager.Instance.RequestChangeTurn(damage, this);
    }

    //HIT 애니메이션이 종료된 후 behaviour에 등록된 NotifyOnAnimExit로 호출되는 함수
    //IAnimNotify 인터페이스 상속으로 인해 다음 함수 구현
    public void OnAnimStateExit(int stateKey)
    {
        //stateKey가 1이면, 즉 Hit 관련 모션이면
        if (stateKey == 1)
        {
            if (!photonView.IsMine) return;

            if (!attackRequestSent && damage >= 0)
            {
                Debug.LogError(
                    "[PlayerController] Impact callback was not invoked. " +
                    "Requesting attack result on animation exit.");
                RequestAttackAtImpact();
            }
            //Hit 한 순간의 데미지 값을 인자로 해서 턴 전환 함수 호출
            //TurnManager.Instance.RequestChangeTurn(damage, this);
            damage = -1;
            WhileAnimation = false;

            PlayerCanvasController.Instance.CloseGauge();
        }
    }

    private bool TryGetTreeAttackPosition(out Vector3 targetPosition, out Quaternion targetRotation)
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;

        if (TreeStatus.Instance == null)
        {
            Debug.LogError("[PlayerController] TreeStatus.Instance가 없습니다.");
            return false;
        }

        if (targetTerrain == null)
        {
            targetTerrain = Terrain.activeTerrain;
        }

        if (targetTerrain == null)
        {
            Debug.LogError("[PlayerController] Target Terrain이 지정되지 않았습니다.");
            return false;
        }

        Vector3 treePosition = TreeStatus.Instance.transform.position;
        Vector3 directionFromTree = transform.position - treePosition;

        directionFromTree.y = 0f;

        if (directionFromTree.sqrMagnitude < 0.001f)
        {
            directionFromTree = -transform.forward;
            directionFromTree.y = 0f;
        }

        directionFromTree.Normalize();

        targetPosition = treePosition + directionFromTree * properDistanceToTree;

        float terrainHeight = targetTerrain.SampleHeight(targetPosition) + targetTerrain.transform.position.y;
        targetPosition.y = terrainHeight + terrainHeightOffset;

        Vector3 lookDirection = treePosition - targetPosition;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            targetRotation = Quaternion.LookRotation(lookDirection.normalized);
        }
        return true;
    }

    private void MoveToTreeAttackPosition()
    {
        if (!TryGetTreeAttackPosition(out Vector3 targetPosition, out Quaternion targetRotation)) return;

        bool wasControllerEnabled = characterController != null && characterController.enabled;

        if (wasControllerEnabled) characterController.enabled = false;

        transform.SetPositionAndRotation(targetPosition, targetRotation);

        if (wasControllerEnabled) characterController.enabled = true;

        velocity.y = -2f;
    }

    //회전 관련 코드 실행
    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (inputLocked)
        {
            lookInput = Vector2.zero; // 안전빵
            return;
        }


        CameraRotation();
    }

    //사용자의 입력은 KeyInteractManager의 이벤트를 통해 비동기로 처리되므로 폴링 루틴은 공백으로 둡니다.
    private void ReadInput()
    {
    }


    //바닥 체크 함수
    private void GroundCheck()
    {
        Vector3 checkPos = groundCheck != null ? groundCheck.position :
            (transform.position + Vector3.down * (characterController.height * 0.5f - characterController.skinWidth + 0.05f));

        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);

        //땅에 붙어있는 경우, 약간 아래로 누르는 힘 적용
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;
    }

    //이동 관련 함수 처리
    private void HandleMovement(float dt)
    {
        // 카메라 기준 이동 벡터
        Vector3 camForward = pivotTransform.forward;
        Vector3 camRight = pivotTransform.right;
        camForward.y = 0f; camRight.y = 0f;
        camForward.Normalize(); camRight.Normalize();

        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 moveDir = (camRight * inputDir.x + camForward * inputDir.z);
        float targetSpeed = (sprintPressed ? sprintSpeed : walkSpeed) * Mathf.Clamp01(inputDir.magnitude);

        // 가감속
        if (targetSpeed > currentSpeed)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelation * dt);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * dt);

        Vector3 final = moveDir.normalized * currentSpeed;
        final.y = velocity.y;
        characterController.Move(final * dt);

        // 시각적 애니메이션 처리
        UpdateAnimation(moveDir, dt);
    }

    private void UpdateAnimation(Vector3 moveDir, float dt)
    {
        if (animationController == null) return;

        float mag = moveDir.magnitude;
        float maxMoveSpeed = sprintSpeed;
        float speed01 = (maxMoveSpeed > 0f) ? Mathf.Clamp01(currentSpeed / maxMoveSpeed) : 0f;

        if (mag < 0.001f || speed01 < 0.001f)
        {
            animationController.UpdateMoveVisuals(0f, 0f, dt);
        }
        else
        {
            Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
            Vector3 dir = moveDir / mag;

            float moveX = Vector3.Dot(dir, right) * speed01;
            float moveZ = Vector3.Dot(dir, fwd) * speed01;

            animationController.UpdateMoveVisuals(moveX, moveZ, dt);
        }
    }


    //카메라 회전을 담당하는 함수
    //마우스 혹은 조이스틱의 입력을 받아 카메라의 pitch와 캐릭터의 Yaw 방향 조정
    //CameraRoot를 로컬 회전 시키고, 플레이어의 전체 회전은 Y축 기준으로 조정
    private void CameraRotation()
    {
        //만약 입력벡터의 크기가 threshold 보다 작으면(즉 카메라를 거의 움직이지 않았을 경우)
        //혹은 LockCameraPosition이 true인 경우
        //카메라 회전을 하지 않는다.
        if (lookInput.sqrMagnitude < _threshold)
            return;

        //좌우 회전 값(감도 적용)
        Yaw += lookInput.x * mouseSensitivity;
        //상하 회전 값(감도 적용)
        Pitch -= lookInput.y * mouseSensitivity;

        //좌우 회전 값을 0~360 범위만 갖도로 제한(즉, 361->1도)
        Yaw = Mathf.Repeat(Yaw, 360f);
        //상하 회전 각도를 위아래로 제한
        Pitch = Mathf.Clamp(Pitch, BottomClamp, TopClamp);

        //플레이어 오브젝트 자체를 Y축 회전시켜서 방향을 전환
        transform.rotation = Quaternion.Euler(0.0f, Yaw, 0.0f);
        //카메라 pivot(CameraRoot)의 X축 회전을 적용하여 상하 시야 회전
        pivotTransform.transform.localRotation = Quaternion.Euler(Pitch, 0.0f, 0.0f);
    }

    //중력 계산 함수
    private void HandleGravity(float dt)
    {
        //중력 상시 적용
        velocity.y += gravity * dt;
    }

    //땅에 닿았는지여부를 체크하는 콜라이더 체크용 함수
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void PlayUseItemAnimation(Transform itemSlotTransform, ItemClass currentItemClass, Texture itemTexture)
    {
        WhileAnimation = true;
        SetInputLocked(true); // 상호작용 즉시 입력/속도 잠금 (슬라이딩 방지)
        animationController.UseItemAnimation(itemSlotTransform, currentItemClass, itemTexture, () =>
         {
             WhileAnimation = false;
         });
    }
}
