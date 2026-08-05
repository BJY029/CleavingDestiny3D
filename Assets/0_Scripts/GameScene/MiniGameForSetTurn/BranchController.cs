using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;


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

	private void Awake()
	{
		isSelected = false;
		rend = GetComponent<Renderer>();
		if (normalMat == null && rend != null)
		{
			normalMat = rend.material;   // 초기 머티리얼 저장
		}
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

		transform.localScale = new Vector3(5f, 5f * myLength, 5f);
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - myLength);
	}

	//순서 설정
	private void setIndex(int index)
	{ myIndex = index; }


	public float getLength() { return myLength; }
	public int getIdx() { return myIndex; }

	//선택되는 경우 실행될 함수
	public void SetSelected()
	{
		isSelected = true;
		if (rend != null && selectedMat != null)
		{
			rend.material = selectedMat;
		}
	}

	//마우스가 해당 브랜치에 올라가는 경우
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isSelected) return;

		AudioManager.Instance.PlaySfx2D("Branch_Hover");

		//Debug.LogError("mouse in");
		if (rend != null && highlightMat != null)
		{
			rend.material = highlightMat;
		}
	}

	//마우스가 해당 브랜치로부터 떠나는 경우
	public void OnPointerExit(PointerEventData eventData)
	{
		if (isSelected) return;

		//Debug.LogError("mouse out");
		if (rend != null && normalMat != null)
		{
			rend.material = normalMat;
		}
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
