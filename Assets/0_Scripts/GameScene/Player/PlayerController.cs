using UnityEngine;
using UnityEngine.InputSystem;

//CharacterController 컴포넌트 강제 할당
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
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


	private void Awake()
	{
		//컨트롤러 받아오기
		characterController = GetComponent<CharacterController>();
		//바닥 감지용 구 반지름 가져오기
		groundCheckRadius = groundCheck.GetComponent<SphereCollider>().radius;
		//input system 살당
		var pi = GetComponent<PlayerInput>();
		if (pi != null)
		{
			moveAction = pi.actions["Move"];
			lookAction = pi.actions["Look"];
			sprintAction = pi.actions["Sprint"];
			jumpAction = pi.actions["Jump"];
		}
	}

	//해당 캐릭터가 활성화 혹은 비활성화 되면 움직임을 제한
	private void OnEnable()
	{
		moveAction?.Enable();
		lookAction?.Enable();
		sprintAction?.Enable();
		jumpAction?.Enable();
	}

	private void OnDisable()
	{
		moveAction?.Disable();
		lookAction?.Disable();
		sprintAction?.Disable();
		jumpAction?.Disable();
	}

	private void Start()
	{
		//애니메이터 할당(할당 실패시 false로 설정)
		hasAnimator = TryGetComponent(out animator);
		//마우스 고정 및 숨기기
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
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

	//움직임 및 점프 관련 코드 실행
	private void Update()
	{
		//ApplyAnimation();
		ReadInput();
		GroundCheck();
		HandleMovement(Time.deltaTime);
		HandleJumpAndGravity(Time.deltaTime);
	}
	
	//회전 관련 코드 실행
	private void LateUpdate()
	{
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
