using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class BranchController : MonoBehaviourPunCallbacks
{
	//자신의 길이 정보 및 순서 정보 저장
	private float myLength;
	private int myIndex = -1;


	public void InitBranch(int idx, int length)
	{
		setIndex(idx);
		setLength(length);
	}

	//길이 및 위치 설정
	private void setLength(float length)
	{
		myLength = length;

		transform.localScale = new Vector3(1, myLength, 1);
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - myLength);
	}

	//순서 설정
	private void setIndex(int index)
	{ myIndex = index; }


	public float getLength() {  return myLength; }
	public int getIdx() { return myIndex; }

	//해당 함수는 추후에 위치가 UI 관련 스크립트로 변경 될 가능성 있음
	public void OnClickMyStick()
	{
		//예외 체크 후
		if (myIndex == -1) return;
		if (!PhotonNetwork.InRoom) return;

		//나뭇가지 선택 함수 호출
		StickGameController.Instance.OnClickStick(myIndex);
	}
}
