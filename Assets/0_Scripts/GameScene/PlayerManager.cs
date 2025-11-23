using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
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

	//중복 초기화 방지 플래그
	private bool isAlreadyInitialized;

	public Transform CenterObject;
	private float radius = 4.5f;
	private Vector3[] spawnPos;
	private Quaternion[] spawnRot;


	private void Awake()
	{
		if (Instance != null)
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
	}

	private bool IsInitializer()
	{
		var players = PhotonNetwork.PlayerList;
		int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
		int minActor = players.Min(p => p.ActorNumber);
		return myActor == minActor;
	}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
	{
		if (changedProps.ContainsKey("TurnInfo") && isAlreadyInitialized == false)
			StartCoroutine(PrepareStartGame());
	}

	private IEnumerator PrepareStartGame()
	{
		IEnumerator handelUI = BranchUIController.Instance.FadeoutCurtain_GameStart();
		StartCoroutine(handelUI);

		InitPlayersInfo();
		SpawnPlayersOnCircle();
		InitPlayerProps();

		yield return handelUI;
		CameraSwitchManager.Instance.Branch_to_Game();

		int myActNum = PhotonNetwork.LocalPlayer.ActorNumber;
		PhotonNetwork.Instantiate($"Player/Player{myActNum}", spawnPos[myActNum - 1], spawnRot[myActNum - 1]);
		CameraSwitchManager.Instance.Off_ExceptPlayerCam();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	//미니게임으로부터 turn 순서가 정해지면, 해당 정보 기반으로 플레이어 정보 채워넣을 예정
	private void InitPlayersInfo()
	{
		isAlreadyInitialized = true;
		players.Clear();
		var room = PhotonNetwork.CurrentRoom;
		var props = room.CustomProperties;
		int[] TurnList = (int[])props["TurnInfo"];
		if (TurnList == null) Debug.LogError("TurnListt is null");

		//각 플레이어에 대해서
		foreach (Player p in PhotonNetwork.PlayerList)
		{
			//정보 삽입
			var rp = new RuntimePlayer();
			rp.actorNumber = p.ActorNumber;

			var ht = p.CustomProperties;

			rp.playerName = ht.TryGetValue("playerName", out var name) ? (string)name : "player" + rp.actorNumber;
			rp.isMyTurn = false;

			int playerTurn = getIndex(TurnList, p.ActorNumber);
			if (playerTurn == -1)
			{
				Debug.LogError("No Player Turn");
				rp.turnIdx = -1;
			}
			rp.turnIdx = playerTurn;
			Debug.Log($"playerActorNum : {rp.actorNumber}, turn:{rp.turnIdx}");

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

	private void SpawnPlayersOnCircle()
	{
		spawnPos = new Vector3[players.Count];
		spawnRot = new Quaternion[players.Count];

		for (int i = 0; i < players.Count; i++)
		{
			float angle = 2f * Mathf.PI * i / players.Count;

			Vector3 offset = new Vector3(
				Mathf.Cos(angle) * radius,
				0f,
				Mathf.Sin(angle) * radius);

			spawnPos[i] = offset + CenterObject.position;

			spawnRot[i] = Quaternion.LookRotation(CenterObject.position - spawnPos[i]);
		}
	}


	private void InitPlayerProps()
	{
		Player player = PhotonNetwork.LocalPlayer;
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.VillageHP, CommonDefine.defaultTreeHP);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.VillageBarrier, CommonDefine.defaultVillageBarrier);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.VillageUpgrades, CommonDefine.defaultVillageUpgrades);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.Gold, CommonDefine.defaultVillageGold);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.AtkPow, CommonDefine.defaultPlayerAtkPow);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.Energy, CommonDefine.defaultPlayerEnergy);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.MaxEnergy, CommonDefine.defaultPlayerMaxEnergy);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.DayTimeDamage, CommonDefine.defaultDayTimeDamage);

		Debug.Log("Init Player Props Success");
	}
}
