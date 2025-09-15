using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

// 이 스크립트는 Cinemachine Pan Tilt 컴포넌트가 있는 카메라에 있어야 합니다.
[RequireComponent(typeof(CinemachinePanTilt))]
public class CameraLookController : MonoBehaviour
{
	[Header("Input Settings")]
	[Tooltip("카메라 조작에 사용할 Input Action (Vector2 타입)")]
	[SerializeField] private InputActionReference lookActionReference;

	[Header("Camera Control Settings")]
	[Tooltip("카메라 좌우 회전 감도")]
	[SerializeField] private float horizontalSensitivity = 20f;
	[Tooltip("카메라 상하 회전 감도")]
	[SerializeField] private float verticalSensitivity = 20f;

	[Header("Camera Tilt Limits")]
	[Tooltip("카메라가 내려다볼 수 있는 최대 각도")]
	[SerializeField] private float maxTilt = 80.0f;
	[Tooltip("카메라가 올려다볼 수 있는 최대 각도")]
	[SerializeField] private float minTilt = -70.0f;

	// 제어할 시네머신 컴포넌트
	private CinemachinePanTilt panTilt;
	// 입력 값을 저장할 변수
	private Vector2 lookInput;

	void Awake()
	{
		// 스크립트가 붙어있는 게임 오브젝트에서 CinemachinePanTilt 컴포넌트를 찾아옴
		panTilt = GetComponent<CinemachinePanTilt>();
	}

	void OnEnable()
	{
		// Input Action 활성화 및 이벤트 구독
		lookActionReference.action.Enable();
		lookActionReference.action.performed += OnLookPerformed;
		lookActionReference.action.canceled += OnLookCanceled;
	}

	void OnDisable()
	{
		// Input Action 비활성화 및 이벤트 구독 해제
		lookActionReference.action.performed -= OnLookPerformed;
		lookActionReference.action.canceled -= OnLookCanceled;
		lookActionReference.action.Disable();
	}

	// 입력이 발생했을 때 호출되는 함수
	private void OnLookPerformed(InputAction.CallbackContext context)
	{
		lookInput = context.ReadValue<Vector2>();
	}

	// 입력이 멈췄을 때 호출되는 함수
	private void OnLookCanceled(InputAction.CallbackContext context)
	{
		lookInput = Vector2.zero;
	}

	void Update()
	{
		if (panTilt == null) return;

		// 현재 Pan/Tilt 각도를 가져옴
		float currentPan = panTilt.PanAxis.Value;
		float currentTilt = panTilt.TiltAxis.Value;

		// 마우스 입력과 감도를 곱하여 이번 프레임에 움직일 양을 계산하고, 현재 각도에 더해줌
		currentPan += lookInput.x * horizontalSensitivity * Time.deltaTime;
		currentTilt += lookInput.y * verticalSensitivity * Time.deltaTime;

		// Tilt(상하) 각도가 설정된 최대/최소 범위를 벗어나지 않도록 제한
		currentTilt = Mathf.Clamp(currentTilt, minTilt, maxTilt);

		// 계산된 최종 각도를 PanTilt 컴포넌트에 다시 적용
		panTilt.PanAxis.Value = currentPan;
		panTilt.TiltAxis.Value = currentTilt;
	}
}