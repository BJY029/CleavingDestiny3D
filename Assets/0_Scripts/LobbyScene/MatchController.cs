using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MatchController : MonoBehaviourPunCallbacks
{
	/*  FLAGS  */
	//현재 매치매이킹 중인지 확인
	private bool isFindingMatch = false;
	//텍스트 연출에 사용될 플래그
	private bool spining;
	//씬 전환 가능 여부 설정
	private bool allowSceneChange;
	//역할 분배 관련 플래그
	private bool roleDistribution;


	//매치매이킹 관련 타이머 설정 코루틴
	private Coroutine timerCoroutine;
	//로딩 텍스트 관련 코루틴
	private Coroutine spiningCoroutine;
	//텍스트 연출에 사용될 시간
	private float duration = 0.4f;


	

	//매치메이킹 관련 UI 요소
	[Header("Loading Panel")]
	public GameObject LoadingPanel;
	public Text LoadingText;
	public Text SceneLoadingText;
	public Text Timer;
	public Button StopMatching;

	private void Awake()
	{
		allowSceneChange = false;
		roleDistribution = false;
		spining = false;
		SceneLoadingText.text = "";
	}

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

			//...이 지속적으로 출력되로록 한다.
			LoadingText.text = "Looking for an opponent";
			if (spiningCoroutine != null)
			{
				spining = false;
				StopCoroutine(spiningCoroutine);
			}
			spiningCoroutine = StartCoroutine(SpiningDots(LoadingText));
			

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
			StopCoroutine(spiningCoroutine);
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
			GameManager.instance.nextScene = CommonDefine.GAMESCENE;

			//역할 분배
			RoleDistribution();
			//씬 로딩
			StartCoroutine(LoadScene());
			//씬 전환 코루틴 실행
			StartCoroutine(StopTimerAndFinalizeMatch());
		}
		else
		{
			//...이 지속적으로 출력되도록 한다.
			LoadingText.text = "Waiting for opponent";
			if (spiningCoroutine != null)
			{
				spining = false;
				StopCoroutine(spiningCoroutine);
			}
			spiningCoroutine = StartCoroutine(SpiningDots(LoadingText));
		}
	}

	//특정 텍스트에 ...이 반복적으로 출력되는 코루틴
	IEnumerator SpiningDots(Text texts)
	{
		yield return null;
		spining = true;

		string originText = texts.text;
		int curDot = 0;
		string Dot = "";
		while(spining)
		{
			Dot = "";
			for (int i = 0; i < curDot; i++)
			{
				Dot += ".";
			}
			texts.text = originText + Dot;
			curDot = (curDot + 1) % 4;

			yield return new WaitForSeconds(duration);
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

		//씬이 비동기로 로드가 완료되고, 역한 분배가 끝나면 씬을 전환한다.
		yield return new WaitUntil(() => allowSceneChange == true && roleDistribution == true);

		//해당 방의 방장이면, 방을 안보이게 설정하고 씬 이동
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.CurrentRoom.IsOpen = false;
			//다른 플레이어들도 같이 이동된다.(AutomaticallySyncScene = true 설정 해야 함)
			PhotonNetwork.LoadLevel("GameScene");
		}
	}

	//플레이어에게 역할을 분배하는 함수
	private void RoleDistribution()
	{
		//만약 MasterClient가 아니라면 해당 함수는 실행하지 않는다.
		if (!PhotonNetwork.IsMasterClient) return;

		//현재 방의 플레이어들을 리스트로 불러온다.
		var players = PhotonNetwork.PlayerList.ToList();
		//랜덤 객체 정의 후, (0 ~ players.count - 1) 즉, 0 혹은 1의 숫자를 반환받는다.
		System.Random rand = new System.Random();
		int r = rand.Next(players.Count);

		Player p1;
		//만약 0이면 그대로이고 0번째에 위치한 플레이어가 p1
		if (r == 0)
		{
			p1 = players[0];
		}
		else//1이면 1번째에 위치한 플레이어를 p1로 설정한다.
		{
			p1 = players[1];
		}

		//플레이어들을 돌면서
		foreach(var player in players)
		{
			//해당 플레이어가 p1 플레이어와 같은 경우, P1 할당, 아니면 P2 할당
			string role = (player == p1) ? "P1" : "P2";

			//플레이어 역할 정볼르 HashTable에 저장하고
			var props = new ExitGames.Client.Photon.Hashtable
			{
				{"Role", role },
			};

			//해당 플레이어의 CustomProperties를 서버에 업데이트 한다.
			player.SetCustomProperties(props);
		}

		//모든 역할 분배가 끝나면 플래그를 true로 변경한다.
		roleDistribution = true;
	}

	//비동기로 씬을 로드하는 함수
	IEnumerator LoadScene()
	{
		yield return null;
		//비동기로 씬 로딩
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameManager.instance.nextScene);
		//씬 전환 막기
		asyncOperation.allowSceneActivation = false;

		//텍스트 관련 설정
		string originText = "Loading Scene";
		int curDot = 0;
		string Dot = "";

		//비동기 씬 로딩이 끝날 때까지
		while(!asyncOperation.isDone)
		{
			//아직 로딩 중이라면
			if(asyncOperation.progress < 0.9f)
			{
				//지속적으로 텍스트 출력
				Dot = "";
				for (int i = 0; i < curDot; i++)
				{
					Dot += ".";
				}
				SceneLoadingText.text = originText + Dot;
				curDot = (curDot + 1) % 4;

				yield return new WaitForSeconds(duration);
			}
			else //로딩이 완료되면
			{
				//씬 전환 허용 후
				asyncOperation.allowSceneActivation = true;
				//씬 전환이 가능하도록 설정한다.
				allowSceneChange = true;
				yield break;
			}
		}
	}
}
