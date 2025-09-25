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
	}

	private void Connect()
    {
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
        LobbyUIManager.instance.setNickname("Player" + PhotonNetwork.CountOfPlayers.ToString());

        PhotonNetwork.JoinLobby();
    }

	//로비 연결 완료 출력
	public override void OnJoinedLobby()
	{
		LobbyUIManager.instance.setConnectedText("Connected to lobby.");

		Debug.Log("Connected Region: " + PhotonNetwork.CloudRegion);
		Debug.Log("AppVersion: " + PhotonNetwork.AppVersion);
		Debug.Log("Connected: " + PhotonNetwork.IsConnected);
		Debug.Log("In Lobby: " + PhotonNetwork.InLobby);
	}
}
