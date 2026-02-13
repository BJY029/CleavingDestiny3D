using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class LobbyConnectController : MonoBehaviourPunCallbacks
{
	private string gameVersion = "1";

	private void Awake()
	{
		//속한 방의 MasterClient의 씬 변환에, 자동 씬 변환 활성화
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	private void Start()
	{
		//연결 시도
		Connect();
		Application.runInBackground = true;
	}

	private void Connect()
	{
		if (PhotonNetwork.IsConnected)
		{
			LobbyUIManager.instance.setConnectedText("Connected. Joining lobby...");

			if (!PhotonNetwork.InLobby)
				PhotonNetwork.JoinLobby();
			else
				LobbyUIManager.instance.setConnectedText("Connected to lobby.");
			return;
		}
		//게임 서버 지역 고정(한국)
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
		//게임 배포 버전 설정
		PhotonNetwork.GameVersion = gameVersion;
		//서버 접속 시도
		PhotonNetwork.ConnectUsingSettings();
	}

	//Master server 연결 후, Lobby에 연결 시도
	public override void OnConnectedToMaster()
	{
		LobbyUIManager.instance.setConnectedText("Connected. Joining lobby...");

		PhotonNetwork.JoinLobby();
	}

	//로비 연결 완료 출력
	public override void OnJoinedLobby()
	{
		LobbyUIManager.instance.setConnectedText("Connected to lobby.");

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		Debug.Log("Connected Region: " + PhotonNetwork.CloudRegion);
		Debug.Log("AppVersion: " + PhotonNetwork.AppVersion);
		Debug.Log("Connected: " + PhotonNetwork.IsConnected);
		Debug.Log("In Lobby: " + PhotonNetwork.InLobby);
		LobbyUIManager.instance.setNickname("Player" + PhotonNetwork.LocalPlayer.ActorNumber.ToString());

		InitPlayerProps();
	}

	private void InitPlayerProps()
	{
		if (PhotonNetwork.LocalPlayer.CustomProperties.Count == 0) return;
		//플레이어의 프로퍼티를 받아오기 위한 해시 선언
		ExitGames.Client.Photon.Hashtable allClear = new ExitGames.Client.Photon.Hashtable();

		//로컬 플레이어(호출자)의 프로퍼티를 돌아보면서
		foreach (var key in PhotonNetwork.LocalPlayer.CustomProperties.Keys)
		{
			//프로퍼티 초기화 수행
			allClear[key.ToString()] = null;
		}
		PhotonNetwork.LocalPlayer.SetCustomProperties(allClear);
		Debug.Log("Player Props Init Completed");
	}
}
