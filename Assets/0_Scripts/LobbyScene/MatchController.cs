using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class MatchController : MonoBehaviourPunCallbacks
{
	private readonly string[] dots = { "", ".", "..", "..." };

	/*  FLAGS  */
	//현재 매칭메이킹 중인지 확인
	private bool isFindingMatch = false;
	//텍스트 애니메이션 재생 플래그
	private bool spining = false;
	//씬 전환 허용 여부 플래그
	private bool allowSceneChange = false;
	//역할 분배 완료 플래그
	private bool roleDistribution = false;
	//솔로 모드 진입 플래그 (정적 변수에 상태 유지)
	public static bool IsStartingSoloGlobal = false;
	protected bool isStartingSolo
	{
		get => IsStartingSoloGlobal;
		set => IsStartingSoloGlobal = value;
	}
	public bool IsStartingSolo => isStartingSolo;


	//매치메이킹 경과 타이머 갱신 코루틴
	private Coroutine timerCoroutine;
	//로딩 텍스트 애니메이션 코루틴
	private CancellationTokenSource spiningCancelToken;
	//텍스트 애니메이션 갱신 시간
	private float duration = 0.4f;


	// 씬로더의 공용 UI 요소를 실시간 바인딩하여 씬 복귀 시 레퍼런스 유실 방지
	protected GameObject LoadingPanel => SceneLoader.Instance != null ? SceneLoader.Instance.loadingPanel : null;
	protected TextMeshProUGUI LoadingText => SceneLoader.Instance != null ? SceneLoader.Instance.mainLoadingText : null;
	protected TextMeshProUGUI SceneLoadingText => SceneLoader.Instance != null ? SceneLoader.Instance.sceneLoadingText : null;
	protected TextMeshProUGUI Timer => SceneLoader.Instance != null ? SceneLoader.Instance.timer : null;
	protected Button StopMatching => SceneLoader.Instance != null ? SceneLoader.Instance.stopMatching : null;

	protected virtual void Start()
	{
		SceneLoadingText.text = "";
	}

	//매치메이킹을 시도하는 함수
	protected void FindMatch()
	{
		Debug.Log($"[MatchController] FindMatch() 호출됨. IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}, InRoom: {PhotonNetwork.InRoom}");
		// 서버 연결 완료 및 방에 있지 않은 상태 확인 (GameServer에서 매칭 시도 방지)
		if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)
		{
			if (LoadingPanel == null) return;

			//매치메이킹 플래그 설정
			isFindingMatch = true;

			//로딩 패널 활성화
			LoadingPanel.SetActive(true);
			LoadingPanel.transform.localScale = Vector3.one;

			//...이 반복되어 표시되도록 한다.
			LoadingText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_Finding);
			if (spiningCancelToken != null)
			{
				spiningCancelToken.Cancel();
				spiningCancelToken.Dispose();
			}
			spiningCancelToken = new CancellationTokenSource();
			SpiningDots(LoadingText, spiningCancelToken.Token).Forget();


			//방 생성 옵션 설정
			RoomOptions roomOptions = new RoomOptions();
			//최대 룸 인원 수 2명 설정
			roomOptions.MaxPlayers = 2;
			roomOptions.IsVisible = true;
			roomOptions.IsOpen = true;

			//타이머 갱신 코루틴이 동작중이면 멈춤
			if (timerCoroutine != null)
			{
				StopCoroutine(timerCoroutine);
			}
			//타이머를 시작한다.
			timerCoroutine = StartCoroutine(UpdateTimer());

			//JoinRandomroom 매칭 시도 : 조건에 맞는 방이 있으면 입장을 시도한다.
			//실패 시 CreateRoom 실행 : 들어갈 방이 없으면 방을 생성하고, 해당 방의 첫번째 플레이어가 된다.
			//주요 파라미터
			//expectedCustomRoomProperties : 특정 커스텀 속성을 가진 방을 필터링하여 들어가고 싶을 때 사용(ex : map : "desert")
			//roomOption : 룸의 생성 옵션
			//typedLobby : 특정 로비에 진입하여, 그 안에서만 방을 찾거나 생성하고 싶을 때 사용한다.(지금은 null로 해서 기본로비 사용)
			PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, null, null, null, roomOptions);
		}
		else
		{
			Debug.LogError($"Not connected to server or in room. IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}, InRoom: {PhotonNetwork.InRoom}");
		}
	}

	//매치 메이킹 취소 버튼을 눌렀을 때 호출될 함수
	protected void CancelMatch()
	{
		//애초에 매치메이킹 중이 아니라면 실행 x
		if (!isFindingMatch) return;
		//매치메이킹 플래그 해제
		isFindingMatch = false;

		//솔로 모드 진입 시도 중이었다면 취소
		if (isStartingSolo)
		{
			isStartingSolo = false;
			GameManager.Instance.isSoloPlay = false;
		}

		//현재 방에 들어와있다면 방에서 나간다.
		if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();

		//타이머 초기화
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
			Timer.text = "00:00";

			if (spiningCancelToken != null)
			{
				spiningCancelToken.Cancel();
				spiningCancelToken.Dispose();
				spiningCancelToken = null;
			}
		}

		//로딩 패널 비활성화(크기 0으로 설정)
		if (LoadingPanel != null)
		{
			LoadingPanel.transform.localScale = Vector3.zero;
			LoadingPanel.SetActive(false);
		}
	}

	//타이머를 갱신하는 코루틴
	IEnumerator UpdateTimer()
	{
		float elapsedTime = 0f;
		while (isFindingMatch)
		{
			elapsedTime += Time.deltaTime;

			float min = Mathf.FloorToInt(elapsedTime / 60);
			float sec = Mathf.FloorToInt(elapsedTime % 60);

			Timer.SetText("{0:00}:{1:00}", min, sec);

			yield return null;
		}
	}

	//방에 성공적으로 들어갔을 때 호출되는 함수
	//방을 생성하는 방장이나 해당 방에 들어가는 플레이어 모두 들어오면 호출되는 함수
	public override void OnJoinedRoom()
	{
		CheckPlayersInRoom();
	}

	//의도치 않게 방을 나간 경우, 매치메이킹을 취소한다.
	public override void OnLeftRoom()
	{
		if (isFindingMatch)
			CancelMatch();
	}

	//특정 플레이어가 방에 접속하면 호출되는 함수
	//해당 함수는 이미 방에 들어와 있는 플레이어가, 다른 플레이어가 방에 들어옴을 감지하게 된다.
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		CheckPlayersInRoom();
	}

	// 서버 연결이 끊어졌을 때 호출되는 콜백
	public override void OnDisconnected(DisconnectCause cause)
	{
		// 솔로 모드 진입을 위해 의도적으로 연결을 끊은 경우
		if (isStartingSolo && isFindingMatch)
		{
			StartOfflineRoom();
		}
	}

	//현재 방의 인원 확인 및 로직 처리
	private void CheckPlayersInRoom()
	{
		//인원 구성이 완료되면 (2명이거나, 솔로모드라 오프라인 모드일 경우)
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2 || PhotonNetwork.OfflineMode)
		{
			isStartingSolo = false;
			GameManager.Instance.nextScene = CommonDefine.GAMESCENE;

			//역할 분배
			RoleDistribution();

			// 솔로 플레이(오프라인 모드)일 경우, 자동으로 P1 역할 할당 및 분배 완료 처리
			if (PhotonNetwork.OfflineMode)
			{
				var props = new ExitGames.Client.Photon.Hashtable { { "Role", "P1" } };
				PhotonNetwork.LocalPlayer.SetCustomProperties(props);
				roleDistribution = true;
			}

			// 로딩 UI만 표시 (포톤 LoadLevel로 씬 전환이 전파되며 씬 로드 완료 시 자동 해제됨)
			SceneLoader.Instance.ShowLoadingUI();
			AudioManager.Instance.PlaySfx2D("Match_Con");
			//씬 전환 코루틴 시작
			StartCoroutine(StopTimerAndFinalizeMatch());
		}
		else
		{
			//...이 반복되어 표시되도록 한다.
			LoadingText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_Waiting);
			if (spiningCancelToken != null)
			{
				spiningCancelToken.Cancel();
				spiningCancelToken.Dispose();
			}
			spiningCancelToken = new CancellationTokenSource();
			SpiningDots(LoadingText, spiningCancelToken.Token).Forget();
		}
	}

	//특정 텍스트 뒤에 ...이 반복적으로 표시되는 코루틴
	async UniTask SpiningDots(TextMeshProUGUI texts, CancellationToken token)
	{
		spining = true;

		string originText = texts.text;
		int curDot = 0;

		try
		{
			while (!token.IsCancellationRequested && spining && !destroyCancellationToken.IsCancellationRequested)
			{
				texts.text = originText + dots[curDot];
				curDot = (curDot + 1) % 4;

				await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
			}
		}
		finally
		{
			// 종료시 스피닝 플래그 해제 및 원래 텍스트 복원
			spining = false;
			texts.text = originText;
		}

	}

	//매칭을 종료하기 위한 코루틴
	IEnumerator StopTimerAndFinalizeMatch()
	{
		//매치메이킹 플래그 해제
		isFindingMatch = false;

		//타이머 중지
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
		}
		LoadingText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_MatchSuccess);

		//매치메이킹 취소 불가능하게 버튼 비활성화
		StopMatching.gameObject.SetActive(false);

		// 마스터 클라이언트는 역할 분배 완료를 대기하고, 일반 클라이언트는 즉시 통과
		yield return new WaitUntil(() => !PhotonNetwork.IsMasterClient || roleDistribution == true);

		//해당 방의 방장이며, 방이 안보이게 설정하고 씬 이동
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.CurrentRoom.IsOpen = false;
			//다른 플레이어들도 같이 이동시킨다.(AutomaticallySyncScene = true 되어 있어야 함)
			PhotonNetwork.LoadLevel("GameScene");
		}
	}

	//플레이어에게 역할을 분배하는 함수
	//현재 미사용-------------------------
	private void RoleDistribution()
	{
		//만약 MasterClient가 아니라면 해당 함수를 실행하지 않는다.
		if (!PhotonNetwork.IsMasterClient) return;

		//현재 방의 플레이어들을 리스트로 불러온다.
		var players = PhotonNetwork.PlayerList.ToList();
		//랜덤 객체 생성 후, (0 ~ players.Count) 중, 0 혹은 1의 숫자를 반환받는다.
		System.Random rand = new System.Random();
		int r = rand.Next(players.Count);

		Player p1;
		//만약 0이면 그대로 0번째 위치한 플레이어가 p1
		if (r == 0)
		{
			p1 = players[0];
		}
		else//1이면 1번째 위치한 플레이어를 p1으로 지정한다.
		{
			p1 = players[1];
		}

		//플레이어들을 돌면서
		foreach (var player in players)
		{
			//해당 플레이어가 p1 플레이어와 같을 경우, P1 할당, 아니면 P2 할당
			string role = (player == p1) ? "P1" : "P2";

			//플레이어 커스텀 속성을 HashTable로 생성하고
			var props = new ExitGames.Client.Photon.Hashtable
			{
				{"Role", role },
			};

			//해당 플레이어의 CustomProperties에 속성을 업데이트 한다.
			player.SetCustomProperties(props);
		}

		//모든 역할 분배가 끝난 뒤 플래그를 true로 설정한다.
		roleDistribution = true;
	}

	protected void StartSoloplay()
	{
		Debug.Log($"[MatchController] StartSoloplay() 호출됨. LoadingPanel: {LoadingPanel != null}");
		if (LoadingPanel == null) return;

		// 매치메이킹 플래그 설정 (로딩 UI 및 취소 로직용)
		isFindingMatch = true;

		// 로딩 패널 활성화
		LoadingPanel.SetActive(true);
		LoadingPanel.transform.localScale = Vector3.one;

		// 로딩 텍스트 설정
		LoadingText.text = "Loading Solo Mode";
		if (spiningCancelToken != null)
		{
			spiningCancelToken.Cancel();
			spiningCancelToken.Dispose();
		}
		spiningCancelToken = new CancellationTokenSource();
		SpiningDots(LoadingText, spiningCancelToken.Token).Forget();

		// 타이머 갱신 시작 (UI 일관성 유지)
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
		}
		timerCoroutine = StartCoroutine(UpdateTimer());

		// 오프라인 모드를 키기 위해서는 서버 연결을 끊어야 한다.
		isStartingSolo = true;

		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.Disconnect();
		}
		else
		{
			StartOfflineRoom();
		}
	}

	// 오프라인 룸 생성 및 진입
	private void StartOfflineRoom()
	{
		GameManager.Instance.isSoloPlay = true;
		PhotonNetwork.OfflineMode = true;
		PhotonNetwork.CreateRoom("SoloRoom"); // 오프라인 룸 생성 -> OnJoinedRoom 호출됨
	}

}
