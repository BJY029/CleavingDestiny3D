using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	[Header("Move")]
	public float walkSpeed = 3.5f;
	public float sprintSpeed = 6.0f;
	public float rotationSmoothTime = 0.08f;
	public float accelation = 12f;
	public float deceleration = 12f;

	[Header("Jump/Gravity")]
	public float jumpHeight = 1.2f;
	public float gravity = -20f;
	public float groundCheckRadius = 0.25f;
	public Transform groundCheck;      // 발 아래 빈 오브젝트 추천
	public LayerMask groundLayers = ~0;

	[Header("Camera")]
	public Transform cameraTransform;  // Main Camera의 transform
	public GameObject _mainCamera;

	[Header("Animator")]
	public Animator animator;          // Speed, IsGrounded, Yvel 파라미터 사용 예시

	[SerializeField]
	private float mouseSensitivity = 1.0f;

	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 90.0f; //위쪽 카메라 회전 제한

	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -90.0f;//아래쪽 카메라 회전 제한


	private Vector2 moveInput;
	private Vector2 lookInput;
	private bool sprintPressed;
	private bool jumpPressed;

	private CharacterController characterController;
	private float currentSpeed;
	private float targetRotation;
	private float rotationVelocity;
	private Vector3 velocity;
	[SerializeField]
	private bool isGrounded;
	private float yaw;
	private float pitch;

	private const float _threshold = 0.01f; //미세 입력 무시 기준값
// cinemachine
	private float _cinemachineTargetYaw;
	private float _cinemachineTargetPitch;



	private PlayerInput playerInput;
	private InputAction moveAction, lookAction, sprintAction, jumpAction;

	private void Awake()
	{
		characterController = GetComponent<CharacterController>();

		var pi = GetComponent<PlayerInput>();
		if (pi != null)
		{
			moveAction = pi.actions["Move"];
			lookAction = pi.actions["Look"];
			sprintAction = pi.actions["Sprint"];
			jumpAction = pi.actions["Jump"];
		}
	}

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
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		//ApplyAnimation();
		ReadInput();
		//GroundCheck();
		HandleMovement(Time.deltaTime);
		//HandleJumpAndGravity(Time.deltaTime);

		//OnDrawGizmosSelected();
	}

	private void FixedUpdate()
	{
	}

	private void LateUpdate()
	{
		//HandleRotation(Time.deltaTime);
		CameraRotation();
		//AlignSmart();
		//AlignToCameraYaw_Fast();
	}

	private void ReadInput()
	{
		moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
		lookInput = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
		sprintPressed = sprintAction != null && sprintAction.IsPressed();
		jumpPressed = jumpAction != null && jumpAction.WasPressedThisFrame();
	}


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
	}

	private void HandleMovement(float dt)
	{
		// 카메라 기준 이동 벡터
		Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
		Vector3 camRight = cameraTransform != null ? cameraTransform.right : Vector3.right;
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

		Vector3 horizontalVelocity = moveDir.normalized * currentSpeed;

		// 최종 이동 (y는 HandleJumpAndGravity에서 넣음)
		Vector3 final = horizontalVelocity;
		final.y = velocity.y;

		characterController.Move(final * dt);
	}

	private void HandleRotation(float dt)
	{
		Vector2 look = lookInput;

		yaw += look.x * mouseSensitivity * dt;
		pitch -= look.y * mouseSensitivity * dt;	

		transform.rotation = Quaternion.Euler(0f, yaw, 0f);

		if(cameraTransform != null)
		{
			cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
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

		_cinemachineTargetYaw += lookInput.x * mouseSensitivity * Time.deltaTime;
		_cinemachineTargetPitch -= lookInput.y * mouseSensitivity * Time.deltaTime;


		//상하 회전 각도를 위아래로 제한
		_cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, BottomClamp, TopClamp);

		//플레이어 오브젝트 자체를 Y축 회전시켜서 방향을 전환
		transform.rotation = Quaternion.Euler(0.0f, _cinemachineTargetYaw, 0.0f);
		//카메라 pivot(CameraRoot)의 X축 회전을 적용하여 상하 시야 회전
		cameraTransform.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
	}

	public float fastTurnRate = 1000f;
	public float nearAngleDeg = 10f;
	public float nearSmoothTime = 0.04f;
	private float rotVel;

	private void AlignSmart()
	{
		Vector3 camF = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);
		if (camF.sqrMagnitude < 1e-4f) return;

		float targetYaw = Mathf.Atan2(camF.x, camF.z) * Mathf.Rad2Deg;
		float delta = Mathf.DeltaAngle(transform.eulerAngles.y, targetYaw);

		if (Mathf.Abs(delta) > nearAngleDeg)
		{
			// 멀면 빠르게
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, Quaternion.Euler(0, targetYaw, 0),
				fastTurnRate * Time.deltaTime);
		}
		else
		{
			// 거의 맞으면 살짝 부드럽게
			float yaw = Mathf.SmoothDampAngle(
				transform.eulerAngles.y, targetYaw, ref rotVel, nearSmoothTime, Mathf.Infinity, Time.deltaTime);
			transform.rotation = Quaternion.Euler(0, yaw, 0);
		}
	}

	// 변수
	public float turnRateDegPerSec = 540f; // 360~720 권장


	private void AlignToCameraYaw_Fast()
	{
		if (!cameraTransform) return;

		Vector3 camF = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);
		if (camF.sqrMagnitude < _threshold) return;

		Quaternion target = Quaternion.LookRotation(camF, Vector3.up);
		transform.rotation = Quaternion.RotateTowards(
			transform.rotation, target, turnRateDegPerSec * Time.deltaTime);
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnRateDegPerSec * Time.deltaTime);
	}



	private void HandleJumpAndGravity(float dt)
	{
		if(isGrounded && jumpPressed)
		{
			//v = sqrt(2gh)
			velocity.y = Mathf.Sqrt(jumpHeight * gravity * -2f);
		}

		//중력 상시 적용
		velocity.y += gravity * dt;
	}

	private void ApplyAnimation()
	{
		if(!animator) return;

		Vector3 v = characterController.velocity;
		v.y = 0f;
		float speed01 = Mathf.InverseLerp(0f, sprintSpeed, v.magnitude);

		animator.SetFloat("Speed", speed01);
		animator.SetBool("IsGrounded", isGrounded);
		animator.SetFloat("Yvel", velocity.y);
	}

	private void OnDrawGizmosSelected()
	{
		if (groundCheck != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
		}
	}
}
