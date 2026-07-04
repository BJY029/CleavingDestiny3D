using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class LobbyConnectController : MonoBehaviourPunCallbacks
{
	private string gameVersion = "1";

	private void Awake()
	{
		//  MasterClient  ȯ, ڵ  ȯ Ȱȭ
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	private void Start()
	{
		// 오프라인 모드 상태로 로비에 진입할 경우 해제 처리
		if (PhotonNetwork.OfflineMode)
		{
			PhotonNetwork.OfflineMode = false;
		}
		
		if (GameManager.Instance != null)
		{
			GameManager.Instance.isSoloPlay = false;
		}
		
		Connect();
		Application.runInBackground = true;
	}

	private void Connect()
	{
		// 1. 서버에 완전히 연결되어 있고, 로비 참가 명령을 즉시 보낼 수 있는 상태인 경우
		if (PhotonNetwork.IsConnectedAndReady)
		{
			LobbyUIManager.instance.setConnectedText("Connected. Joining lobby...");

			if (!PhotonNetwork.InLobby)
				PhotonNetwork.JoinLobby();
			else
				LobbyUIManager.instance.setConnectedText("Connected to lobby.");
			return;
		}

		// 2. 소켓은 연결되어 있으나, 마스터 서버 복귀 중인 과도기 상태인 경우 (IsConnectedAndReady가 false인 상황)
		if (PhotonNetwork.IsConnected)
		{
			LobbyUIManager.instance.setConnectedText("Connecting to master server...");
			// 이 상태에서는 아무것도 하지 않고 대기하면, 포톤이 준비를 마친 후 
			// 자동으로 OnConnectedToMaster() 콜백을 실행하여 JoinLobby()를 처리합니다.
			return;
		}

		// 3. 아예 연결이 완전히 끊긴 상태인 경우 (Disconnected) -> 재접속 프로세스 작동
		LobbyUIManager.instance.setConnectedText("Disconnected. Reconnecting...");
		
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
		PhotonNetwork.GameVersion = gameVersion;
		PhotonNetwork.ConnectUsingSettings();
	}

	//Master server  , Lobby  õ
	public override void OnConnectedToMaster()
	{
		if (PhotonNetwork.OfflineMode) return;

		LobbyUIManager.instance.setConnectedText("Connected. Joining lobby...");

		PhotonNetwork.JoinLobby();
	}

	//κ  Ϸ 
	public override void OnJoinedLobby()
	{
		LobbyUIManager.instance.setConnectedText("Connected to lobby.");

		if (ButtonController.Instance != null)
		{
			ButtonController.Instance.SetButtonsInteractable(true);
		}

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
		//�÷��̾��� ������Ƽ�� �޾ƿ��� ���� �ؽ� ����
		ExitGames.Client.Photon.Hashtable allClear = new ExitGames.Client.Photon.Hashtable();

		//���� �÷��̾�(ȣ����)�� ������Ƽ�� ���ƺ��鼭
		foreach (var key in PhotonNetwork.LocalPlayer.CustomProperties.Keys)
		{
			//������Ƽ �ʱ�ȭ ����
			allClear[key.ToString()] = null;
		}
		PhotonNetwork.LocalPlayer.SetCustomProperties(allClear);
		Debug.Log("Player Props Init Completed");
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		// 솔로 모드 시작 중이거나, 오프라인 모드 상태이거나, 이미 솔로 플레이 중이라면 온라인 재접속 하지 않음
		if (MatchController.IsStartingSoloGlobal || 
			PhotonNetwork.OfflineMode || 
			(GameManager.Instance != null && GameManager.Instance.isSoloPlay))
		{
			Debug.Log("[LobbyConnectController] 솔로 모드(오프라인) 상태이므로 온라인 재접속을 수행하지 않습니다.");
			return;
		}

		Debug.LogWarning($"[LobbyConnectController] Disconnected from Photon. Cause: {cause}. Reconnecting...");
		LobbyUIManager.instance.setConnectedText("Disconnected. Reconnecting...");

		if (ButtonController.Instance != null)
		{
			ButtonController.Instance.SetButtonsInteractable(false);
		}
		
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
		PhotonNetwork.GameVersion = gameVersion;
		PhotonNetwork.ConnectUsingSettings();
	}
}
