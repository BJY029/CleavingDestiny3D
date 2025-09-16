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
	public Transform groundCheck;      // �� �Ʒ� �� ������Ʈ ��õ
	public LayerMask groundLayers = ~0;

	[Header("Camera")]
	public Transform cameraTransform;  // Main Camera�� transform
	public GameObject _mainCamera;

	[Header("Animator")]
	public Animator animator;          // Speed, IsGrounded, Yvel �Ķ���� ��� ����

	[SerializeField]
	private float mouseSensitivity = 1.0f;

	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 90.0f; //���� ī�޶� ȸ�� ����

	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -90.0f;//�Ʒ��� ī�޶� ȸ�� ����


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

	private const float _threshold = 0.01f; //�̼� �Է� ���� ���ذ�
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
		//�⺻�� ��� :
		//controller.height * 0.5f (contorller ���� ����(��, ĸ�� �߽ɿ��� �ٴڱ��� �Ÿ�)
		//controller.skinWidth (ChararcterController�� ���� �浹 ����������, ������ �ּ� ��ġ�� �ʰ� �ϴ� ��)
		//�� ���� ���� �̴���, skinWidth ��ŭ �ٴ��� �����ľ� ���� ���� ��Ҵٰ� �Ǵ��ϱ� ����,
		//�߰��� +0.05f�� ��¦ �� ������ �ٴڿ� ���������� ���̴� ����
		Vector3 checkPos = groundCheck != null ? groundCheck.position : 
			(transform.position + Vector3.down * (characterController.height * 0.5f - characterController.skinWidth + 0.05f));

		isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);

		//���� �پ��ִ� ���, �ణ �Ʒ��� ������ �� ����
		if (isGrounded && velocity.y < 0f)
			velocity.y = -2f;
	}

	private void HandleMovement(float dt)
	{
		// ī�޶� ���� �̵� ����
		Vector3 camForward = cameraTransform != null ? cameraTransform.forward : Vector3.forward;
		Vector3 camRight = cameraTransform != null ? cameraTransform.right : Vector3.right;
		camForward.y = 0f; camRight.y = 0f;
		camForward.Normalize(); camRight.Normalize();

		Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
		Vector3 moveDir = (camRight * inputDir.x + camForward * inputDir.z);
		float targetSpeed = (sprintPressed ? sprintSpeed : walkSpeed) * Mathf.Clamp01(inputDir.magnitude);

		// ������
		if (targetSpeed > currentSpeed)
			currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelation * dt);
		else
			currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * dt);

		Vector3 horizontalVelocity = moveDir.normalized * currentSpeed;

		// ���� �̵� (y�� HandleJumpAndGravity���� ����)
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

	//ī�޶� ȸ���� ����ϴ� �Լ�
	//���콺 Ȥ�� ���̽�ƽ�� �Է��� �޾� ī�޶��� pitch�� ĳ������ Yaw ���� ����
	//CameraRoot�� ���� ȸ�� ��Ű��, �÷��̾��� ��ü ȸ���� Y�� �������� ����
	private void CameraRotation()
	{
		//���� �Էº����� ũ�Ⱑ threshold ���� ������(�� ī�޶� ���� �������� �ʾ��� ���)
		//Ȥ�� LockCameraPosition�� true�� ���
		//ī�޶� ȸ���� ���� �ʴ´�.
		if (lookInput.sqrMagnitude < _threshold)
			return;

		_cinemachineTargetYaw += lookInput.x * mouseSensitivity * Time.deltaTime;
		_cinemachineTargetPitch -= lookInput.y * mouseSensitivity * Time.deltaTime;


		//���� ȸ�� ������ ���Ʒ��� ����
		_cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, BottomClamp, TopClamp);

		//�÷��̾� ������Ʈ ��ü�� Y�� ȸ�����Ѽ� ������ ��ȯ
		transform.rotation = Quaternion.Euler(0.0f, _cinemachineTargetYaw, 0.0f);
		//ī�޶� pivot(CameraRoot)�� X�� ȸ���� �����Ͽ� ���� �þ� ȸ��
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
			// �ָ� ������
			transform.rotation = Quaternion.RotateTowards(
				transform.rotation, Quaternion.Euler(0, targetYaw, 0),
				fastTurnRate * Time.deltaTime);
		}
		else
		{
			// ���� ������ ��¦ �ε巴��
			float yaw = Mathf.SmoothDampAngle(
				transform.eulerAngles.y, targetYaw, ref rotVel, nearSmoothTime, Mathf.Infinity, Time.deltaTime);
			transform.rotation = Quaternion.Euler(0, yaw, 0);
		}
	}

	// ����
	public float turnRateDegPerSec = 540f; // 360~720 ����


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

		//�߷� ��� ����
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
