using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PlayerManager : MonoBehaviourPunCallbacks
{
	//전역 접근
    public static PlayerManager Instance { get; private set; }

	//플레이어 정보 관리용 딕셔너리(각 클라이언트마다 관리한다.)
    private Dictionary<int, RuntimePlayer> players = new();
	//읽기 전용 딕셔너리
    public IReadOnlyDictionary<int, RuntimePlayer> Players => players;

	private bool isAlreadyInitialized;

	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		isAlreadyInitialized = false;
		if (IsInitializer())
		{
			//게임 시작 시, 미니게임 시작
			StickGameController.Instance.InitSticks();
		}

		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
	}

	private bool IsInitializer()
	{
		var players = PhotonNetwork.PlayerList;
		int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
		int minActor = players.Min(p => p.ActorNumber);
		return myActor == minActor;
	}

	public override void OnRoomPropertiesUpdate(Hashtable changedProps)
	{
		if (changedProps.ContainsKey("TurnInfo") && isAlreadyInitialized == false)
			InitPlayersInfo();
	}

	//미니게임으로부터 turn 순서가 정해지면, 해당 정보 기반으로 플레이어 정보 채워넣을 예정
	private void InitPlayersInfo()
	{
		isAlreadyInitialized = true;
		players.Clear();
		var room = PhotonNetwork.CurrentRoom;
		var props = room.CustomProperties;
		int[] TurnList =  (int[])props["TurnInfo"];
		if (TurnList == null) Debug.LogError("TurnListt is null");

		//각 플레이어에 대해서
		foreach (Player p in PhotonNetwork.PlayerList)
		{
			//정보 삽입
			var rp = new RuntimePlayer();
			rp.actorNumber = p.ActorNumber;

			var ht = p.CustomProperties;

			rp.playerName = ht.TryGetValue("playerName", out var name) ? (string)name : "player"+rp.actorNumber;
			rp.isMyTurn = false;

			int playerTurn = getIndex(TurnList, p.ActorNumber);
			if(playerTurn == -1)
			{
				Debug.LogError("No Player Turn");
				rp.turnIdx = -1;
			}
			rp.turnIdx = playerTurn;
			Debug.LogError($"playerActorNum : {rp.actorNumber}, turn:{rp.turnIdx}");

			players.Add(rp.actorNumber, rp);
		}
	}

	private int getIndex(int[] list, int value)
	{
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i] == value)
				return i;
		}
		return -1;
	}
}
