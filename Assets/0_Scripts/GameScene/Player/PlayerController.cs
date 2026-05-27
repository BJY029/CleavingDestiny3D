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

    //input system 관련 파라미터
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintPressed;

    [SerializeField]
    private PlayerInput playerInput;
    private InputAction moveAction, lookAction, sprintAction;

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

        //playerInput 가져오기
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();

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
            if (playerInput != null)
            {
                //잘못된 페어링을 해제하고
                playerInput.enabled = false;
            }
            //해당 플레이어의 카메라를 끈다.
            if (_mainCamera != null) _mainCamera.SetActive(false); // 원격 카메라 끄기
            enabled = true; // Update에서 조기 return 할 거라 스크립트는 켬

            if (firstPersonObjects != null)
            {
                firstPersonObjects.SetActive(false); //1인칭 오브젝트 끄기
            }
            return;
        }

        // 이후부터는 로컬 플레이어만 실행하는 코드

        // 본체만 숨기고 그림자는 남기도록 설정
        SetShadowsOnlyRecursively(gameObject, firstPersonObjects);
        firstPersonObjects.SetActive(true); //1인칭 오브젝트 켜기

        cam = _mainCamera.GetComponent<Camera>();
        //로컬 인스턴스, 즉 자기 자신이라면 장치 탈취 방지
        if (playerInput != null)
        {
            playerInput.neverAutoSwitchControlSchemes = true;
        }

        //로컬에서만 액션 바인딩
        if (playerInput != null && playerInput.actions != null)
        {
            var actions = playerInput.actions;
            moveAction = actions.FindAction("Move");
            lookAction = actions.FindAction("Look");
            sprintAction = actions.FindAction("Sprint");
        }

        isLookingAtTree = false;
        UpgradePhase = false;
        WhileAnimation = false;
        RayMultiplyer = 1;
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
        moveAction?.Enable();
        lookAction?.Enable();
        sprintAction?.Enable();

        //F 키가 눌리는 이벤트에 해당 함수 삽입
        KeyInteractManager.instance.OnInteractFKeyDown += HandleInteractFKey;
        KeyInteractManager.instance.OnInteractSpaceKeyDown += HandleInteractSpaceKey;
    }

    private void OnDisable()
    {
        if (!photonView.IsMine) return;
        moveAction?.Disable();
        lookAction?.Disable();
        sprintAction?.Disable();

        KeyInteractManager.instance.OnInteractFKeyDown -= HandleInteractFKey;
        KeyInteractManager.instance.OnInteractSpaceKeyDown -= HandleInteractSpaceKey;
    }

    private bool inputLocked;

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
        inputLocked = locked;

        if (locked)
        {
            //입력 액션 꺼서 “누르고 있던 키” 이벤트가 더 이상 들어오지 않게
            moveAction?.Disable();
            lookAction?.Disable();
            sprintAction?.Disable();

            ResetInputState();
            ResetMotionState();

            // 애니메이션 즉시 정지
            animationController?.UpdateMoveVisuals(0f, 0f, 0.1f);
        }
        else
        {
            moveAction?.Enable();
            lookAction?.Enable();
            sprintAction?.Enable();

            ResetInputState(); // 재개 시에도 0으로 시작(키 눌림은 다음 프레임 ReadInput으로 다시 잡힘)
        }
    }

    //마을 업그레이드에 돌입시 실행될 함수
    public void VillageUpgradePhase()
    {
        //플레이어 카메라를 끄고
        cam.enabled = false;
        //메인 카메라 켜기
        CameraSwitchManager.Instance.GameCameraToggle(true);
    }

    //마을 업그레이드가 끝나면 실행될 함수
    public void VillageUpgradePhaseOut()
    {
        //메인 카메라 끄고
        CameraSwitchManager.Instance.GameCameraToggle(false);
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
        || SettingCanvasController.instance.IsSettingPanelOpened)
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

        if (photonView.IsMine)
        {
            animationController?.UpdateCamera(lookInput);
        }

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

    //F키가 눌렸을 때 실행될 함수
    private void HandleInteractFKey()
    {
        //내 객체가 아니면 return
        if (!photonView.IsMine) return;
        //내 턴이 아니면 return
        if (!GameHelper.IsMyTurn()) return;

        currentInteractable?.OnInteract(this);
    }

    //스페이스 바가 눌렸을 때 실행될 함수
    private void HandleInteractSpaceKey()
    {
        //내 객체가 아니면 return
        if (!photonView.IsMine) return;
        //내 턴이 아니면 return
        if (!GameHelper.IsMyTurn()) return;

        //미니 게임 관련 인터페이스 상호작용 수행
        currentMinigame?.OnInteract(this);
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

            characterController.enabled = false;
            // 적절한 거리로 나무와의 위치 조정
            Vector3 dirToTree = (TreeStatus.Instance.transform.position - transform.position).normalized;
            Vector3 properPos = TreeStatus.Instance.transform.position - dirToTree * properDistanceToTree;
            transform.position = properPos;
            characterController.enabled = true;
        }
        //Offer 패널 접근 막기
        ItemOfferCanvasController.instance.Close();
        int currentMaxAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MaxAtkPow);
        int currentMinAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MinAtkPow);
        damage = currentMinAtkDamage + Mathf.RoundToInt((currentMaxAtkDamage - currentMinAtkDamage) * (damageRatio / 100));
        //Hit 애니메이션 재생 
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

    //HIT 애니메이션이 종료된 후 behaviour에 등록된 NotifyOnAnimExit로 호출되는 함수
    //IAnimNotify 인터페이스 상속으로 인해 다음 함수 구현
    public void OnAnimStateExit(int stateKey)
    {
        //stateKey가 1이면, 즉 Hit 관련 모션이면
        if (stateKey == 1)
        {
            if (!photonView.IsMine) return;

            if (damage < 0)
            {
                Debug.LogError("damage error");
                return;
            }
            //Hit 한 순간의 데미지 값을 인자로 해서 턴 전환 함수 호출
            TurnManager.Instance.RequestChangeTurn(damage, this);
            damage = -1;
            WhileAnimation = false;
        }
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

    //사용자 입력을 input system에서 받아온다.
    private void ReadInput()
    {
        moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        lookInput = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        sprintPressed = sprintAction != null && sprintAction.IsPressed();
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
        animationController.UseItemAnimation(itemSlotTransform, currentItemClass, itemTexture, () =>
         {
             WhileAnimation = false;
         });
    }
}
