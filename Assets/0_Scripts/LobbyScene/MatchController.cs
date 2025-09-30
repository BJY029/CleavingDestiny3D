using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;

public class MatchController : MonoBehaviourPunCallbacks
{
	//현재 매치매이킹 중인지 확인
	private bool isFindingMatch = false;
	//매치매이킹 관련 타이머 설정 코루틴
	private Coroutine timerCoroutine;

	//매치메이킹 관련 UI 요소
	[Header("Loading Panel")]
	public GameObject LoadingPanel;
	public Text LoadingText;
	public Text Timer;
	public Button StopMatching;

	//매치메이킹을 시도하는 함수
	protected void FindMatch()
	{
		//연결 가능한 상태에서
		if (PhotonNetwork.IsConnectedAndReady)
		{
			if (LoadingPanel == null) return;

			//매치메이킹 플래그 설정
			isFindingMatch = true;

			//로딩 패널 띄우기
			LoadingPanel.transform.localScale = Vector3.one;
			LoadingText.text = "Looking for an opponent";

			//생성할 방 옵션 설정
			RoomOptions roomOptions = new RoomOptions();
			//최대 방 인원 수 및 방 참여 가능하게 설정
			roomOptions.MaxPlayers = 2;
			roomOptions.IsVisible = true;
			roomOptions.IsOpen = true;

			//타이머 관련 코루틴이 실행중이면 멈추고
			if (timerCoroutine != null)
			{
				StopCoroutine(timerCoroutine);
			}
			//타이머를 시작한다.
			timerCoroutine = StartCoroutine(UpdateTimer());

			//JoinRandomroom 먼저 시도 : 조건에 맞는 임의의 방에 참가를 시도한다.
			//실패 시 CreateRoom 실행 : 만약 참가할 수 있는 방을 못찾으면, 방을 생성하고, 해당 방의 첫번째 플레이어로 입장한다.
			//주요 파라미터
			//expectedCustomRoomProperties : 특정 커스텀 속성을 가진 방만 필터링하여 들어가고 싶을 때 사용(ex : map : "desert")
			//roomOption : 생성될 방의 조건
			//typedLobby : 특정 로비를 지정하여, 그 안에서만 방을 찾거나 생성하고 싶을 때 사용한다.(보통은 null로 해서 기본로비로 설정)
			PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, null, null, null, roomOptions);
		}
		else
		{
			Debug.LogError("Not connected to server.");
		}
	}

	//매치 메이킹 취소 버튼을 누르면 실행될 함수
	protected void CancelMatch()
	{
		//애초에 매치메이킹 중이 아니였다면 실행 x
		if (!isFindingMatch) return;
		//매치메이킹 플래그 설정
		isFindingMatch = false;

		//현재 방에 들어와있는 경우 방에서 나간다.
		if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();

		//타이머 초기화
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
			Timer.text = "00:00";
		}

		//로딩 패널 비활성화(크기 0으로 설정)
		if (LoadingPanel != null) LoadingPanel.transform.localScale = Vector3.zero;
	}

	//타이머를 설정하는 코루틴
	IEnumerator UpdateTimer()
	{
		float elapsedTime = 0f;
		while (isFindingMatch)
		{
			elapsedTime += Time.deltaTime;

			float min = Mathf.FloorToInt(elapsedTime / 60);
			float sec = Mathf.FloorToInt(elapsedTime % 60);

			Timer.text = string.Format("{0:00}:{1:00}", min, sec);

			yield return null;
		}
	}

	//방에 성공적으로 참가했을 때 실행될 함수
	//기존에 존재하는 방에 해당되는 플레이어가 방에 참가하게 되면 실행할 함수
	public override void OnJoinedRoom()
	{
		CheckPlayersInRoom();
	}

	//의도치 않게 방을 떠난 경우, 매치메이킹을 취소한다.
	public override void OnLeftRoom()
	{
		if (isFindingMatch)
			CancelMatch();
	}

	//특정 플레이어가 방에 참가하면 실행할 함수
	//해당 함수는 방을 만들고 대기하고 있던 플레이어가, 특정 플레이어가 방에 들어오면 실행하게 된다.
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		CheckPlayersInRoom();
	}

	//게임 시작 조건 확인 및 게임 시작
	private void CheckPlayersInRoom()
	{
		//인원 조건이 충족되면
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
		{
			//게임 시작
			StartCoroutine(StopTimerAndFinalizeMatch());
		}
		else
		{
			LoadingText.text = "Waiting for opponent...";
		}
	}

	//게임을 시작하기 위한 코루틴
	IEnumerator StopTimerAndFinalizeMatch()
	{
		//매치메이킹 플래그 설정
		isFindingMatch = false;

		//타이머 정지
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
		}
		LoadingText.text = "Matching success! Moving to gamescene";

		//매치메이킹 취소 못하게 버튼 비활성화
		StopMatching.gameObject.SetActive(false);

		//2초정도 대기
		yield return new WaitForSeconds(2f);

		//해당 방의 방장이면, 방을 안보이게 설정하고 씬 이동
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.CurrentRoom.IsOpen = false;
			//다른 플레이어들도 같이 이동된다.(AutomaticallySyncScene = true 설정 해야 함)
			PhotonNetwork.LoadLevel("GameScene");
		}
	}
}
