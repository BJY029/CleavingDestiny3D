using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using ExitGames.Client.Photon;

//CharacterController 컴포넌트 강제 할당
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IAnimNotify
{
	//움직임 관련 파라미터
	[Header("Move")]
	public float walkSpeed = 3.5f;
	public float sprintSpeed = 6.0f;
	public float rotationSmoothTime = 0.08f;
	public float accelation = 12f;
	public float deceleration = 12f;

	//점프와 중력 관련 파라미터
	[Header("Jump/Gravity")]
	public float jumpHeight = 1.2f;
	public float gravity = -20f;
	public Transform groundCheck;      // 발 아래 빈 오브젝트 추천
	public LayerMask groundLayers = ~0;

	//카메라 관련 파라미터
	[Header("Camera")]
	public Transform pivotTransform;  // Main Camera의 pivot
	public GameObject _mainCamera;	//실제 카메라
	public float cameraDistance = 3.0f;

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
	private bool jumpPressed;

	[SerializeField]
	private PlayerInput playerInput;
	private InputAction moveAction, lookAction, sprintAction, jumpAction;

	//움직임 관련 코드 내부에서 사용하는 파라미터
	private CharacterController characterController;
	private float currentSpeed;
	private Vector3 velocity;
	//점프 관련 코드 내부에서 사용하는  파라미터
	public bool isGrounded;
	private float groundCheckRadius;

	private const float _threshold = 0.01f; //미세 입력 무시 기준값	

	//회전 관련 파라미터
	private float Yaw;
	private float Pitch;

	private Camera cam;
	[Header("Raycast")]
	[SerializeField] private float RayDistance;
	[SerializeField] private LayerMask targetLayer;
	private Transform lastHitTarget = null;
	private bool isLookingAtTree;

	private PhotonView photonView;
	//마을 업그레이드 중인지 여부를 저장할 플래그
	private bool UpgradePhase;
	private bool WhileHittingMotion;
	//private float damageRatio;


	private void Awake()
	{
		photonView = GetComponent<PhotonView>();

		//playerInput 가져오기
		if (playerInput == null) playerInput = GetComponent<PlayerInput>();

		//컨트롤러 받아오기
		characterController = GetComponent<CharacterController>();

		//groundCheckRadius 안전 처리
		groundCheckRadius = 0.2f;
		if (groundCheck != null)
		{
			var sc = groundCheck.GetComponent<SphereCollider>();
			if (sc) groundCheckRadius = sc.radius;
		}

		//만약 내 플레이어가 아니라면
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
			return;
		}

		cam = _mainCamera.GetComponent<Camera>();
		//로컬 인스턴스, 즉 자기 자신이라면 장치 탈취 방지
		if (playerInput != null)
		{
			playerInput.neverAutoSwitchControlSchemes = true;
		}

		//커서 설정
		

		//애니메이터 할당(할당 실패시 false로 설정)
		hasAnimator = TryGetComponent(out animator);

		//애니메이션 접근 파라미터를 해시코드로 관리
		AssignAnimationIDs();

		//로컬에서만 액션 바인딩
		if (playerInput != null && playerInput.actions != null)
		{
			var actions = playerInput.actions;
			moveAction = actions.FindAction("Move");
			lookAction = actions.FindAction("Look");
			sprintAction = actions.FindAction("Sprint");
			jumpAction = actions.FindAction("Jump");
		}

		isLookingAtTree = false;
		UpgradePhase = false;
		WhileHittingMotion = false;
	}

	//해당 캐릭터가 활성화 혹은 비활성화 되면 움직임을 제한
	private void OnEnable()
	{
		if (!photonView.IsMine) return;
		moveAction?.Enable();
		lookAction?.Enable();
		sprintAction?.Enable();
		jumpAction?.Enable();

		//F 키가 눌리는 이벤트에 해당 함수 삽입
		TurnManager.Instance.OnInteractFKeyDown += HandleInteractFKey;
	}

	private void OnDisable()
	{
		if (!photonView.IsMine) return;
		moveAction?.Disable();
		lookAction?.Disable();
		sprintAction?.Disable();
		jumpAction?.Disable();

		TurnManager.Instance.OnInteractFKeyDown -= HandleInteractFKey;
	}

	private bool inputLocked;

	private void ResetInputState()
	{
		moveInput = Vector2.zero;
		lookInput = Vector2.zero;
		sprintPressed = false;
		jumpPressed = false;
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
			jumpAction?.Disable();

			ResetInputState();
			ResetMotionState();

			// 애니메이션도 즉시 정지 느낌 주고 싶으면(선택)
			if (hasAnimator)
			{
				animator.SetFloat(_animIDSpeedX, 0f);
				animator.SetFloat(_animIDSpeedZ, 0f);
			}
		}
		else
		{
			moveAction?.Enable();
			lookAction?.Enable();
			sprintAction?.Enable();
			jumpAction?.Enable();

			ResetInputState(); // 재개 시에도 0으로 시작(키 눌림은 다음 프레임 ReadInput으로 다시 잡힘)
		}
	}


	private void Start()
	{
		if (!photonView.IsMine)
		{
			return;
		}
		//애니메이터 할당(할당 실패시 false로 설정)
		hasAnimator = TryGetComponent(out animator);
		//마우스 고정 및 숨기기
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
		//애니메이션 접근 파라미터를 해시코드로 관리
		AssignAnimationIDs();
	}
	//애니메이터 파라미터를 해시코드로 저장해서 관리한다.
	private void AssignAnimationIDs()
	{
		_animIDSpeedX = Animator.StringToHash("Speed_X");
		_animIDSpeedZ = Animator.StringToHash("Speed_Z");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	}

	//마을 업그레이드에 돌입시 실행될 함수
	private void VillageUpgradePahse()
	{
		//플레이어 카메라를 끄고
		cam.enabled = false;
		//메인 카메라 켜기
		CameraSwitchManager.Instance.GameCameraToggle(true);
	}

	//마을 업그레이드가 끝나면 실행될 함수
	private void VillageUpgradePahseOut()
	{
		//메인 카메라 끄고
		CameraSwitchManager.Instance.GameCameraToggle(false);
		//플레이어 카메라 켜기
		cam.enabled = true;
	}

	//움직임 및 점프 관련 코드 실행
	private void Update()
	{
		//내 photonView가 아니면 실행하지 않는다.
		if (!photonView.IsMine)
		{
			return;
		}

		if (WhileHittingMotion)
		{
			SetInputLocked(true);
			return;
		}
		if (ItemOfferCanvasController.instance.isOfferPanelOpened)
		{
			SetInputLocked(true);
			return;
		}

		//만약 마을 업그레이드 페이즈에 돌입한 경우
		if (TurnManager.Instance.isUpgradePhase)
		{
			//아직 관련 처리를 진행하지 않은 경우
			if(!UpgradePhase)
			{
				//처리 진행후
				VillageUpgradePahse();
				//마을 업그레이드 페이즈에 돌입했음을 명시
				UpgradePhase = true;
			}
			SetInputLocked(true);
			return;
		}
		else //마을 업그레이드 페이즈가 아닌 경우
		{
			//그런데 아직 마을 업그레이드 페이즈로 설정되어 있는 경우
			if(UpgradePhase)
			{
				//관련 처리 진행 후
				VillageUpgradePahseOut();
				//마을 업그레이드 페이즈에서 빠져나왔음을 명시
				UpgradePhase = false;
			}
		}

		SetInputLocked(false);
		//ApplyAnimation();
		ReadInput();
		GroundCheck();
		HandleMovement(Time.deltaTime);
		HandleJumpAndGravity(Time.deltaTime);

		//카메라 정면 방향으로 Ray 발사
		Ray ray = new Ray(cam.transform.position, cam.transform.forward);
		//디버그용 Ray 그리기
		Debug.DrawRay(ray.origin, ray.direction * RayDistance, Color.green);
		//ray가 target layer를 감지하면
		if (Physics.Raycast(ray, out RaycastHit hitInfo, RayDistance, targetLayer))
		{
			//해당 레이어에 해당되는 transform 객체 가져오기
			Transform currentHit = hitInfo.transform;

			//처음 감지하는 객체인 경우
			if(lastHitTarget != currentHit)
			{
				//처리 진행
				OnRayEnter(currentHit);
			}
			//현재 감지중인 객체로 설정
			lastHitTarget = currentHit;
			
		}
		else//target layer를 감지하지 못한 경우
		{
			//만약 감지중인 객체가 있었던 경우(즉, 방금 시선에서 해당 layer가 벗어난 경우)
			if(lastHitTarget != null)
			{
				//관련 처리 진행
				OnRayExit(lastHitTarget);
				//감지 중인 객체가 없다고 설정
				lastHitTarget = null;
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
		//현재 나무를 보고 있지 않은 경우 return
		if (!isLookingAtTree) return;
		if (WhileHittingMotion) return;
		
		//Hit 순간의 게이지 데미지 값 받기
		float damageRatio = PlayerCanvasController.Instance.SelectNow();
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.DamageRatio, damageRatio);
		//Hit 애니메이션 재생 
		PlayHit();
		//턴 전환 함수 호출
		//TurnManager.Instance.RequestChangeTurn(PlayerCanvasController.Instance.SelectNow());
	}

	//Hit 애니메이션을 재생하는 함수
	private void PlayHit()
	{
		//모션 재생 플래그 활성화
		WhileHittingMotion = true;
		//Hit 관련 UI 비활성화
		PlayerCanvasController.Instance.SetHitTextUnActive();
		//임의의 Hit 모션 재생 후 해당 모션 index 받아오기
		int idx = gameObject.GetComponent<PlayerAnimationController>().PlayHit();
		//받아온 Hit 모션 index로 전체 플레이어에게 RPC로 해당 애니메이션 재생(동기화)
		photonView.RPC(nameof(RPC_PlayHit), RpcTarget.All, idx);
	}

	//HIT 애니메이션 동기화용 RPC 함수
	[PunRPC]
	private void RPC_PlayHit(int idx)
	{
		gameObject.GetComponent<PlayerAnimationController>().PlayHit(idx);
	}

	//HIT 애니메이션이 종료된 후 behaviour에 등록된 NotifyOnAnimExit로 호출되는 함수
	//IAnimNotify 인터페이스 상속으로 인해 다음 함수 구현
	public void OnAnimStateExit(int stateKey)
	{
		//stateKey가 1이면, 즉 Hit 관련 모션이면
		if (stateKey == 1)
		{
			float damageRatio = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.DamageRatio);
			if(damageRatio == -1)
			{
				Debug.LogError("damageRatio init error");
				return;
			}
			//Hit 한 순간의 데미지 값을 인자로 해서 턴 전환 함수 호출
			TurnManager.Instance.RequestChangeTurn(damageRatio);
			damageRatio = -1f;
			WhileHittingMotion = false;
			//PlayerCanvasController.Instance.SetHitTextActive();
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
		jumpPressed = jumpAction != null && jumpAction.WasPressedThisFrame();
	}


	//바닥 체크 함수
	private void GroundCheck()
	{
		//기본값 계산 :
		//controller.height * 0.5f (contorller 높이 절반(즉, 캡슐 중심에서 바닥까지 거리)
		//controller.skinWidth (ChararcterController의 물리 충돌 보정값으로, 여유를 둬서 겹치지 않게 하는 값)
		//두 값을 빼는 이뉴는, skinWidth 만큼 바닥을 지나쳐야 실제 땅에 닿았다고 판단하기 때문,
		//추가로 +0.05f로 살짝 더 내려서 바닥에 안정적으로 붙이는 연산
		Vector3 checkPos = groundCheck != null ? groundCheck.position : 
			(transform.position + Vector3.down * (characterController.height * 0.5f - characterController.skinWidth + 0.05f));

		isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);

		//땅에 붙어있는 경우, 약간 아래로 누르는 힘 적용
		if (isGrounded && velocity.y < 0f)
			velocity.y = -2f;

		//점프 상태여부에 따라 관련 애니메이션을 실행한다.
		if(hasAnimator)
		{
			animator.SetBool(_animIDGrounded, isGrounded);
		}
	}

	//이동 관련 함수 처리
	private void HandleMovement(float dt)
	{
		// 카메라 기준 이동 벡터
		Vector3 camForward = pivotTransform.forward;
		Vector3 camRight = pivotTransform.right;
		//y축 값을 0으로 하여 수직 이동 삭제
		camForward.y = 0f; camRight.y = 0f;
		//각 벡터 크기 정규화
		camForward.Normalize(); camRight.Normalize();

		//입력된 방향 구하기
		Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
		//입력된 방향을 토대로 실제 이동 방향 구하기
		Vector3 moveDir = (camRight * inputDir.x + camForward * inputDir.z);
		//목표 속도 계산
		float targetSpeed = (sprintPressed ? sprintSpeed : walkSpeed) * Mathf.Clamp01(inputDir.magnitude);

		// 가감속
		if (targetSpeed > currentSpeed)
			currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelation * dt);
		else
			currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * dt);

		// 최종 이동 (y는 HandleJumpAndGravity에서 넣음)
		Vector3 final = moveDir.normalized * currentSpeed;
		final.y = velocity.y;
		//움직임 적용
		characterController.Move(final * dt);

		//애니메이터가 있다면
		if(hasAnimator)
		{
			//움직임 방향 계산
			float mag = moveDir.magnitude;
			//움직임 최대 속도 정의
			float maxMoveSpeed = sprintSpeed;
			//현재 움직임 속도를 0~1 범위로 변환(달리기시 1)
			float speed01 = (maxMoveSpeed > 0f) ? Mathf.Clamp01(currentSpeed / maxMoveSpeed) : 0f;
			//만약 현재 정지 상태라면
			if(mag < 0.001f || speed01 < 0.001f)
			{
				animator.SetFloat(_animIDSpeedX, 0f, 0.1f, dt);
				animator.SetFloat(_animIDSpeedZ, 0f, 0.1f, dt);
			}
			else//움직이는 상태이면
			{
				//ProjectOnPlane(n, v)는 법선이 n인 평면에 정사영하는 함수
				//따라서 아래 함수는 법선이 Vector3.up(즉, y축)인 수평면 XZ 평면에 정사영 하게 된다.
				//이를 통해 수직 성분을 제거하여 수평면에 투영한 벡터를 얻을 수 있게 된다.
				Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
				Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
				//방향 벡터
				Vector3 dir = moveDir / mag;

				//Vector3.Dot(x, y)는, x와 y 사이 각도의 코사인 값을 반환한다. 따라서 범위는 -1~1이 된다.
				//현재 방향과 좌우 방향의 각도를 코사인 값으로 받아서 속도 값을 곱해 최종 움직임 값을 계산한다.
				//1 : 우측, 0 : 정면, -1 : 좌측
				float moveX = Vector3.Dot(dir, right) * speed01;
				//현재 방향과 앞뒤 방향의 각도를 코사인 값으로 받아서 속도 값을 곱패 최종 움직임 값을 계산한다.
				//1 : 완전 정면. 0 : 직각, -1 : 뒤
				float moveZ = Vector3.Dot(dir, fwd) * speed01;

				//각 움직임 값을 애니메이터에 적용한다.
				animator.SetFloat(_animIDSpeedX, moveX, 0.1f, dt);
				animator.SetFloat(_animIDSpeedZ, moveZ, 0.1f, dt);
			}
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
		//메인 카메라는 부모 오브젝트인 pivot 오브젝트를 기준으로 항상 cameraDistanc만큼 떨어진 거리 유지
		_mainCamera.transform.localPosition = new Vector3(0, 0, -cameraDistance);
	}

	//점프와 중력 계산 함수
	private void HandleJumpAndGravity(float dt)
	{
		//땅에 있는 상태에서 점프키가 눌린 경우
		if (isGrounded && jumpPressed)
		{
			//수직 속도 계산
			velocity.y = Mathf.Sqrt(jumpHeight * gravity * -2f);
		}

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
}
