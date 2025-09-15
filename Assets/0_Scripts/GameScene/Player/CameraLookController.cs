using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// �� ��ũ��Ʈ�� Cinemachine Pan Tilt ������Ʈ�� �ִ� ī�޶� �־�� �մϴ�.
[RequireComponent(typeof(CinemachinePanTilt))]
public class CameraLookController : MonoBehaviour
{
	[Header("Input Settings")]
	[Tooltip("ī�޶� ���ۿ� ����� Input Action (Vector2 Ÿ��)")]
	[SerializeField] private InputActionReference lookActionReference;

	[Header("Camera Control Settings")]
	[Tooltip("ī�޶� �¿� ȸ�� ����")]
	[SerializeField] private float horizontalSensitivity = 20f;
	[Tooltip("ī�޶� ���� ȸ�� ����")]
	[SerializeField] private float verticalSensitivity = 20f;

	[Header("Camera Tilt Limits")]
	[Tooltip("ī�޶� �����ٺ� �� �ִ� �ִ� ����")]
	[SerializeField] private float maxTilt = 80.0f;
	[Tooltip("ī�޶� �÷��ٺ� �� �ִ� �ִ� ����")]
	[SerializeField] private float minTilt = -70.0f;

	// ������ �ó׸ӽ� ������Ʈ
	private CinemachinePanTilt panTilt;
	// �Է� ���� ������ ����
	private Vector2 lookInput;

	void Awake()
	{
		// ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ���� CinemachinePanTilt ������Ʈ�� ã�ƿ�
		panTilt = GetComponent<CinemachinePanTilt>();
	}

	void OnEnable()
	{
		// Input Action Ȱ��ȭ �� �̺�Ʈ ����
		lookActionReference.action.Enable();
		lookActionReference.action.performed += OnLookPerformed;
		lookActionReference.action.canceled += OnLookCanceled;
	}

	void OnDisable()
	{
		// Input Action ��Ȱ��ȭ �� �̺�Ʈ ���� ����
		lookActionReference.action.performed -= OnLookPerformed;
		lookActionReference.action.canceled -= OnLookCanceled;
		lookActionReference.action.Disable();
	}

	// �Է��� �߻����� �� ȣ��Ǵ� �Լ�
	private void OnLookPerformed(InputAction.CallbackContext context)
	{
		lookInput = context.ReadValue<Vector2>();
	}

	// �Է��� ������ �� ȣ��Ǵ� �Լ�
	private void OnLookCanceled(InputAction.CallbackContext context)
	{
		lookInput = Vector2.zero;
	}

	void Update()
	{
		if (panTilt == null) return;

		// ���� Pan/Tilt ������ ������
		float currentPan = panTilt.PanAxis.Value;
		float currentTilt = panTilt.TiltAxis.Value;

		// ���콺 �Է°� ������ ���Ͽ� �̹� �����ӿ� ������ ���� ����ϰ�, ���� ������ ������
		currentPan += lookInput.x * horizontalSensitivity * Time.deltaTime;
		currentTilt += lookInput.y * verticalSensitivity * Time.deltaTime;

		// Tilt(����) ������ ������ �ִ�/�ּ� ������ ����� �ʵ��� ����
		currentTilt = Mathf.Clamp(currentTilt, minTilt, maxTilt);

		// ���� ���� ������ PanTilt ������Ʈ�� �ٽ� ����
		panTilt.PanAxis.Value = currentPan;
		panTilt.TiltAxis.Value = currentTilt;
	}
}