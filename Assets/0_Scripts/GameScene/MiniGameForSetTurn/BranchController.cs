using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using System;


public class BranchController : MonoBehaviourPunCallbacks, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	//자신의 길이 정보 및 순서 정보 저장
	private float myLength;
	private int myIndex = -1;

	//선택 플래그
	public bool isSelected;
	//각 상황에 변경될 머테리얼
	public Material normalMat;
	public Material highlightMat;
	public Material selectedMat;

	private Renderer rend;

	[Header("Select Obj")]
	[SerializeField] private GameObject myMarkerPrefab;
	[SerializeField] private GameObject oppMarkerPrefab;
	[SerializeField] private bool topIsMaxZ = true;
	// 월드 좌표 기준 위치 보정
	[SerializeField] private Vector3 markerOffset = new Vector3(0f, 0.5f, 0f);

	// 프리팹의 기본 회전에서 추가로 적용할 회전
	[SerializeField] private Vector3 markerRotationOffset;

	// 0이면 회전하지 않음
	[SerializeField] private float markerRotateSpeed = 90f;

	[Header("마커 화면 위치")]
	[SerializeField] private Camera markerCamera;
	[SerializeField] private Vector2 markerScreenOffset = new Vector2(0f, 40f);

	private GameObject markerInstance;
	private GameObject currentMarkerPrefab;

	private bool isLocalHovered;
	private bool isOppHovered;


	private void Awake()
	{
		isSelected = false;
		rend = GetComponent<Renderer>();
		if (normalMat == null && rend != null)
		{
			normalMat = rend.material;   // 초기 머티리얼 저장
		}
	}

	public void SetMarkerCam(Camera cam)
	{
		markerCamera = cam;
	}

	private void LateUpdate()
	{
		if (markerInstance == null) return;

		UpdateMarkerPosition();

		markerInstance.transform.Rotate(Vector3.forward, markerRotateSpeed * Time.deltaTime, Space.World);
	}

	private void OnDestroy()
	{
		HideMarker();
	}

	public void InitBranch(int idx, int length)
	{
		setIndex(idx);
		setLength(length);
	}

	//길이 및 위치 설정
	private void setLength(float length)
	{
		myLength = length;

		transform.localScale = new Vector3(16f, 5f * myLength, 8f);
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - myLength);
	}

	//순서 설정
	private void setIndex(int index)
	{ myIndex = index; }


	public float getLength() { return myLength; }
	public int getIdx() { return myIndex; }

	//선택되는 경우 실행될 함수
	public void SetSelected(int selectingActorNumber)
	{
		isSelected = true;
		if (rend != null && selectedMat != null)
		{
			rend.material = selectedMat;
		}

		bool isMySelection = PhotonNetwork.LocalPlayer.ActorNumber == selectingActorNumber;

		ShowMarker(isMySelection ? myMarkerPrefab : oppMarkerPrefab);
	}

	//마우스가 해당 브랜치에 올라가는 경우
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isSelected) return;
		if (isLocalHovered) return;

		isLocalHovered = true;

		AudioManager.Instance.PlaySfx2D("Branch_Hover");

		//Debug.LogError("mouse in");
		if (rend != null && highlightMat != null)
		{
			rend.material = highlightMat;
		}

		RefreshHoverMarker();

		if (myIndex >= 0 && PhotonNetwork.InRoom && BranchSpawner.Instance != null)
		{
			BranchSpawner.Instance.NotifyBranchHover(myIndex, true);
		}
	}

	//마우스가 해당 브랜치로부터 떠나는 경우
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isLocalHovered) return;

		isLocalHovered = false;

		if (myIndex >= 0 && PhotonNetwork.InRoom && BranchSpawner.Instance != null)
		{
			BranchSpawner.Instance.NotifyBranchHover(myIndex, false);
		}

		if (isSelected) return;

		//Debug.LogError("mouse out");
		if (rend != null && normalMat != null)
		{
			rend.material = normalMat;
		}

		RefreshHoverMarker();
	}

	//해당 브랜치가 클릭되는 경우
	public void OnPointerClick(PointerEventData eventData)
	{
		//Debug.LogError("mouse click");
		//예외 체크 후
		if (myIndex == -1) return;
		if (!PhotonNetwork.InRoom) return;

		//나뭇가지 선택 함수 호출
		StickGameController.Instance.OnClickStick(myIndex);
		AudioManager.Instance.PlaySfx2D("Branch_Selected");
	}

	private void ShowMarker(GameObject prefab)
	{
		if (prefab == null)
		{
			HideMarker();
			return;
		}

		if (markerInstance != null && currentMarkerPrefab == prefab)
		{
			UpdateMarkerPosition();
			return;
		}

		HideMarker();

		Quaternion rotation = prefab.transform.rotation * Quaternion.Euler(markerRotationOffset);

		markerInstance = Instantiate(prefab, GetMarkerPosition(), rotation);
		currentMarkerPrefab = prefab;
	}

	private void HideMarker()
	{
		if (markerInstance != null)
		{
			markerInstance.SetActive(false);
			Destroy(markerInstance);
		}

		markerInstance = null;
		currentMarkerPrefab = null;
	}

	private Vector3 GetMarkerPosition()
	{
		Vector3 branchTip = transform.position;

		if (rend != null)
		{
			Bounds bounds = rend.bounds;
			float topZ = topIsMaxZ ? bounds.max.z : bounds.min.z;

			branchTip = new Vector3(bounds.center.x, bounds.center.y, topZ);
		}

		Camera cam = markerCamera != null ? markerCamera : Camera.main;

		if (cam == null)
		{
			return branchTip + markerOffset;
		}

		Vector3 screenPosition = cam.WorldToScreenPoint(branchTip);

		screenPosition.x += markerScreenOffset.x;
		screenPosition.y += markerScreenOffset.y;

		return cam.ScreenToWorldPoint(screenPosition);
	}

	private void UpdateMarkerPosition()
	{
		if (markerInstance == null) return;

		markerInstance.transform.position = GetMarkerPosition();
	}

	private void RefreshHoverMarker()
	{
		if (isSelected) return;

		if (isLocalHovered) ShowMarker(myMarkerPrefab);
		else if (isOppHovered) ShowMarker(oppMarkerPrefab);
		else HideMarker();
	}

	public void SetOppHovered(bool hovered)
	{
		isOppHovered = hovered;
		RefreshHoverMarker();
	}

	////해당 함수는 추후에 위치가 UI 관련 스크립트로 변경 될 가능성 있음
	//public void OnClickMyStick()
	//{
	//	//예외 체크 후
	//	if (myIndex == -1) return;
	//	if (!PhotonNetwork.InRoom) return;

	//	//나뭇가지 선택 함수 호출
	//	StickGameController.Instance.OnClickStick(myIndex);
	//}
}
