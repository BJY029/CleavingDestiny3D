using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class MatchController : MonoBehaviourPunCallbacks
{
	/* FLAGS  */
	// 현재 매치메이킹 중인지 확인
	private bool isFindingMatch = false;
	// 텍스트 애니메이션(점 세개) 루프 플래그
	private bool spining;
	// 씬 전환 허용 여부 플래그
	private bool allowSceneChange;
	// 역할 분배 완료 여부 플래그
	private bool roleDistribution;


	// 매치메이킹 경과 시간 업데이트 코루틴
	private Coroutine timerCoroutine;
	// 로딩 텍스트 애니메이션 코루틴
	private Coroutine spiningCoroutine;
	// 애니메이션 속도(점 찍히는 시간)
	private float duration = 0.4f;


	// 매치메이킹 관련 UI 요소들
	[Header("Loading Panel")]
	public GameObject LoadingPanel;
	public TextMeshProUGUI LoadingText;
	public TextMeshProUGUI SceneLoadingText;
	public TextMeshProUGUI Timer;
	public Button StopMatching;

	private void Awake()
	{
		allowSceneChange = false;
		roleDistribution = false;
		spining = false;
		SceneLoadingText.text = "";
	}

	// 매치메이킹을 시작하는 함수
	protected void FindMatch()
	{
		// 서버에 접속되어 있는 상태라면
		if (PhotonNetwork.IsConnectedAndReady)
		{
			if (LoadingPanel == null) return;

			// 매치메이킹 플래그 설정
			isFindingMatch = true;

			// 로딩 패널 활성화
			LoadingPanel.transform.localScale = Vector3.one;

			// 매칭 중 메시지 설정 및 애니메이션 시작
			LoadingText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_Finding);
			if (spiningCoroutine != null)
			{
				spining = false;
				StopCoroutine(spiningCoroutine);
			}
			spiningCoroutine = StartCoroutine(SpiningDots(LoadingText));


			// 룸 생성 및 접속 옵션 설정
			RoomOptions roomOptions = new RoomOptions();
			// 최대 인원을 2명으로 설정
			roomOptions.MaxPlayers = 2;
			roomOptions.IsVisible = true;
			roomOptions.IsOpen = true;

			// 기존 타이머 코루틴이 있다면 중지
			if (timerCoroutine != null)
			{
				StopCoroutine(timerCoroutine);
			}
			// 경과 시간 타이머 시작
			timerCoroutine = StartCoroutine(UpdateTimer());

			// 랜덤 룸에 입장 시도하고, 없으면 생성 (FillRoom 모드)
			PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, null, null, null, roomOptions);
		}
		else
		{
			Debug.LogError("서버에 연결되지 않았습니다.");
		}
	}

	// 매치메이킹 취소 버튼 클릭 시 호출
	protected void CancelMatch()
	{
		// 매칭 중이 아니라면 실행 안 함
		if (!isFindingMatch) return;
		isFindingMatch = false;

		// 현재 룸에 입장해 있다면 나감
		if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();

		// 모든 코루틴 및 타이머 초기화
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
			StopCoroutine(spiningCoroutine);
			Timer.text = "00:00";
		}

		// 로딩 패널 비활성화
		if (LoadingPanel != null) LoadingPanel.transform.localScale = Vector3.zero;
	}

	// 경과 시간을 00:00 포맷으로 업데이트하는 코루틴
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

	// 룸에 입장이 완료되었을 때 실행되는 콜백
	public override void OnJoinedRoom()
	{
		CheckPlayersInRoom();
	}

	// 방을 나갔을 때 매칭 중이었다면 매칭 취소 처리
	public override void OnLeftRoom()
	{
		if (isFindingMatch)
			CancelMatch();
	}

	// 다른 플레이어가 방에 들어왔을 때 실행되는 콜백
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		CheckPlayersInRoom();
	}

	// 방 안의 인원을 체크하여 게임 시작 여부 판단
	private void CheckPlayersInRoom()
	{
		// 인원이 2명이 되면
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
		{
			GameManager.Instance.nextScene = CommonDefine.GAMESCENE;

			// 역할 분배 및 씬 로드 시작
			// RoleDistribution(); // 주석 해제하여 사용 가능
			StartCoroutine(LoadScene());
			StartCoroutine(StopTimerAndFinalizeMatch());
		}
		else
		{
			// 아직 한 명뿐이라면 대기 메시지 출력
			LoadingText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_Waiting);
			if (spiningCoroutine != null)
			{
				spining = false;
				StopCoroutine(spiningCoroutine);
			}
			spiningCoroutine = StartCoroutine(SpiningDots(LoadingText));
		}
	}

	// 텍스트 뒤에 "..." 애니메이션을 보여주는 코루틴
	IEnumerator SpiningDots(TextMeshProUGUI texts)
	{
		yield return null;
		spining = true;

		string originText = texts.text;
		int curDot = 0;
		string Dot = "";
		while (spining)
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

	// 매치 성공 후 최종 처리를 담당하는 코루틴
	IEnumerator StopTimerAndFinalizeMatch()
	{
		isFindingMatch = false;

		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
		}
		LoadingText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_MatchSuccess);

		// 매칭 중단 버튼 비활성화
		StopMatching.gameObject.SetActive(false);

		// 비동기 씬 로딩과 역할 분배가 모두 끝날 때까지 대기
		yield return new WaitUntil(() => allowSceneChange == true && roleDistribution == true);

		// 방장(MasterClient)만 씬을 로드하도록 제어
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.CurrentRoom.IsOpen = false;
			// 모든 클라이언트가 동시에 씬을 이동하도록 함
			PhotonNetwork.LoadLevel("GameScene");
		}
	}

	// 플레이어들에게 역할을 랜덤하게 배정하는 함수
	private void RoleDistribution()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		var players = PhotonNetwork.PlayerList.ToList();
		System.Random rand = new System.Random();
		int r = rand.Next(players.Count);

		Player p1;
		if (r == 0) p1 = players[0];
		else p1 = players[1];

		foreach (var player in players)
		{
			string role = (player == p1) ? "P1" : "P2";

			var props = new ExitGames.Client.Photon.Hashtable
			{
				{ "Role", role },
			};

			player.SetCustomProperties(props);
		}

		roleDistribution = true;
	}

	// 비동기 방식으로 다음 씬을 미리 로드하는 함수
	IEnumerator LoadScene()
	{
		yield return null;
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameManager.Instance.nextScene);
		// 즉시 전환되지 않도록 설정
		asyncOperation.allowSceneActivation = false;

		string originText = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_Loading);
		int curDot = 0;
		string Dot = "";

		while (!asyncOperation.isDone)
		{
			// 90% 미만 로드 시 로딩 텍스트 애니메이션
			if (asyncOperation.progress < 0.9f)
			{
				Dot = "";
				for (int i = 0; i < curDot; i++)
				{
					Dot += ".";
				}
				SceneLoadingText.text = originText + Dot;
				curDot = (curDot + 1) % 4;

				yield return new WaitForSeconds(duration);
			}
			else // 로딩이 완료되면
			{
				// 씬 활성화 및 플래그 설정
				asyncOperation.allowSceneActivation = true;
				allowSceneChange = true;
				yield break;
			}
		}
	}
}