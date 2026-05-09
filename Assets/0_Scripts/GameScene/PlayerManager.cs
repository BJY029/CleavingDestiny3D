using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
	//전역 접근
	public static PlayerManager Instance { get; private set; }

	//플레이어 정보 관리용 딕셔너리(각 클라이언트마다 관리한다.)
	private Dictionary<int, RuntimePlayerInfo> players = new();
	//읽기 전용 딕셔너리
	public IReadOnlyDictionary<int, RuntimePlayerInfo> Players => players;
	//플레이어 인벤토리 관리용 딕셔너리(각 클라가 자신의 인벤토리만 관리한다. 즉 딕셔너리당 1개만 저장됨)
	//만약 싱글 플레이 모드인 경우, MasterClient가 자신의 인벤과 AI의 인벤 두 개를 저장한다.
	private Dictionary<int, WorldInventory> playersInv = new Dictionary<int, WorldInventory>();
	//읽기 전용 딕셔너리
	public IReadOnlyDictionary<int, WorldInventory> PlayersInv => playersInv;

	public Dictionary<int, GameObject> AIPlayerObj = new Dictionary<int, GameObject>();

	//AI 모드에서 사용되는 AI Actnum 저장용(만약에만약에 AI가 여러명이 된다면 폐기해야 함)
	public int AIActNum { get; private set; }

	//AI 에서 사용되는 플래그, 준비 완료 여부를 나타낸다.
	public bool succeedToPreapreGame = false;

	//중복 초기화 방지 플래그
	private bool isAlreadyInitialized;
	//MAsterClietn 에서만 사용
	private bool AllReadyFlag = false;

	public Transform CenterObject;
	private float hit_radius = 3.0f;
	private float radius = 4.5f;
	private float InvRadius = 16f;
	private Vector3[] spawnPos;
	public Vector3[] hitPos { get; private set; }
	private Vector3[] spawnInvPos;
	public Quaternion[] spawnRot { get; private set; }
	private Quaternion[] spawnInvRot;
	public GameObject LocalPlayerObj { get; private set; }

	public int TotalPlayerCount => players.Count;


	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
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

	private bool IsInitializer() => PhotonNetwork.IsMasterClient;
	//private bool IsInitializer()
	//{
	//	var players = PhotonNetwork.PlayerList;
	//	int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
	//	int minActor = players.Min(p => p.ActorNumber);
	//	return myActor == minActor;
	//}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
	{
		if (changedProps.ContainsKey("TurnInfo") && isAlreadyInitialized == false)
		{
			isAlreadyInitialized = true;
			UploadTurnInfoPropertyToRoom();
			StartCoroutine(PrepareStartGame());
		}
	}

	//플레이어 프로퍼티 감지
	//모든 플레이어들이 준비가 되었는지 확인하기 위한 함수
	public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
	{
		if (!IsInitializer()) return;     // 마스터만
		if (AllReadyFlag) return;   //이미 모두 준비되었다고 감지 된 경우 아래 처리 안함

		//플레이어들의 IsReady 프로퍼티가 업데이트 된 경우
		if (changedProps.ContainsKey(PlayerPropKeys.IsReady))
		{
			//모든 플레이어들이 준비 되었는지 확인
			if (AllPlayersReady())
			{
				//준비 된 경우, 플래그를 설정하고
				AllReadyFlag = true;

				//첫 턴/오퍼를 "원자적으로" 세팅
				int[] turnOrder = PhotonPropertyHelper.GetRoomProp<int[]>(RoomPropKeys.TurnOrder);
				int firstActor = turnOrder[0];
				int turnIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
				string offers = OfferAuthority.Instance.MakeOfferForTurn(firstActor, turnIndex);
				//프로퍼티 한 번에 업데이트
				var ht = new ExitGames.Client.Photon.Hashtable
			{
				{ RoomPropKeys.CurrentTurn, 0 },
				{ RoomPropKeys.CurrentTurnActor, firstActor },
				{ ItemPropKeys.OFFER(firstActor), offers ?? "" },
			};
				PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
				//UI 처리를 위한 함수 호출
				TurnManager.Instance.setOfferGeneratedFromOutsied(true);
			}
		}
	}

	//AI 플레이어가 프로퍼티 초기화 후 호출하는 확인 함수
	private void InitPlayerPropsInStartGame()
	{
		if (!AllPlayersReady() || AllReadyFlag) return;

		//준비 된 경우, 플래그를 설정하고
		AllReadyFlag = true;

		//첫 턴/오퍼를 "원자적으로" 세팅
		int[] turnOrder = PhotonPropertyHelper.GetRoomProp<int[]>(RoomPropKeys.TurnOrder);
		int firstActor = turnOrder[0];
		int turnIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
		string offers = OfferAuthority.Instance.MakeOfferForTurn(firstActor, turnIndex);
		Debug.Log($"First Offer Gened : {offers}");
		//프로퍼티 한 번에 업데이트
		var ht = new ExitGames.Client.Photon.Hashtable
			{
				{ RoomPropKeys.CurrentTurn, 0 },
				{ RoomPropKeys.CurrentTurnActor, firstActor },
				{ ItemPropKeys.OFFER(firstActor), offers ?? "" },
			};
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
		//UI 처리를 위한 함수 호출
		TurnManager.Instance.setOfferGeneratedFromOutsied(true);
	}

	private void UploadTurnInfoPropertyToRoom()
	{
		if (!IsInitializer())
			return;


	}

	private IEnumerator PrepareStartGame()
	{
		IEnumerator handelUI = BranchUIController.Instance.FadeoutCurtain_GameStart();
		StartCoroutine(handelUI);

		InitPlayersInfo();
		SpawnPlayersOnCircle();

		InitPlayerProps();
		if (IsInitializer()) InitAIProps();

		yield return handelUI;

		CameraSwitchManager.Instance.Branch_to_Game();

		int myActNum = PhotonNetwork.LocalPlayer.ActorNumber;

		GameObject spawnPlayer = PhotonNetwork.Instantiate($"Player/Player{myActNum}", spawnPos[myActNum - 1], spawnRot[myActNum - 1]);
		PlayerController pc = spawnPlayer.GetComponent<PlayerController>();
		pc.PlayerActNum = myActNum;
		LocalPlayerObj = spawnPlayer;

		GameObject PlayersInv = PhotonNetwork.Instantiate("Inventory/InventoryTent", spawnInvPos[myActNum - 1], spawnInvRot[myActNum - 1]);
		PlayerStatus.Instance.SetPlayerInventory(PlayersInv);
		InventoryBarrier ib = PlayersInv.GetComponentInChildren<InventoryBarrier>();
		playersInv.Add(myActNum, PlayersInv.GetComponent<WorldInventory>());
		ib.SetPermission(spawnPlayer);

		TrySpawnAI();


		CameraSwitchManager.Instance.Off_ExceptPlayerCam();

		GameCanvasController.Instance.gameObject.SetActive(true);
		GameCanvasController.Instance.UpdateDayText();
		GameCanvasController.Instance.UpdateWaveText();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		//AI에서 사용될 플래그, AI 모드에선 MasterClient만 해당 코드를 수행하므로, 별도의 처리 없이 그냥 플래그 초기화
		succeedToPreapreGame = true;
	}

	//Masterclient가(즉 싱글 모드의 로컬 플레이어) AI 스폰 수행
	private void TrySpawnAI()
	{
		if (!IsInitializer()) return;

		foreach (var kvp in players)
		{
			RuntimePlayerInfo rp = kvp.Value;
			Player p = PhotonNetwork.CurrentRoom.GetPlayer(rp.actorNumber);

			if (p == null)
			{
				object[] initData = new object[1] { rp.actorNumber };
				//int aiActNum = rp.actorNumber;
				int myActNum = PhotonNetwork.LocalPlayer.ActorNumber;
				int opposite_num = myActNum == 1 ? 2 : 1;

				GameObject spawnAI = PhotonNetwork.InstantiateRoomObject(
					$"Player/Player{opposite_num}_AI",
					spawnPos[opposite_num - 1],
					spawnRot[opposite_num - 1],
					0,
					initData);
				AIPlayerObj.Add(rp.actorNumber, spawnAI);
				AIActNum = rp.actorNumber;
				//PlayerController AIController = spawnAI.GetComponent<PlayerController>();
				//AIController.PlayerActNum = rp.actorNumber;

				GameObject spawnAIInv = PhotonNetwork.InstantiateRoomObject(
					"Inventory/InventoryTent",
					spawnInvPos[opposite_num - 1],
					spawnInvRot[opposite_num - 1],
					0,
					initData
				);
				//GameObject spawnAIInv = PhotonNetwork.Instantiate("Inventory/InventoryTent", spawnInvPos[opposite_num - 1], spawnInvRot[opposite_num - 1]);
				PlayerStatus.Instance.SetAIInventory(spawnAIInv);
				InventoryBarrier ib = spawnAIInv.GetComponentInChildren<InventoryBarrier>();
				//ib.SetPermission(spawnAIInv);

				//ai 플레이어의 인벤토리 등록
				AIController ai = spawnAI.GetComponent<AIController>();
				playersInv.Add(rp.actorNumber, spawnAIInv.GetComponent<WorldInventory>());
				ai.aiBrain.InventoryManager.AIInv = spawnAIInv.GetComponent<WorldInventory>();
			}
		}
	}

	//미니게임으로부터 turn 순서가 정해지면, 해당 정보 기반으로 플레이어 정보 채워넣을 예정
	private void InitPlayersInfo()
	{

		players.Clear();
		var room = PhotonNetwork.CurrentRoom;
		var props = room.CustomProperties;
		int[] TurnList = (int[])props["TurnInfo"];
		if (TurnList == null) Debug.LogError("TurnList is null");

		// 각 플레이어 정보 삽입
		for (int i = 0; i < TurnList.Length; i++)
		{
			int turnId = TurnList[i];
			Player p = PhotonNetwork.CurrentRoom.GetPlayer(turnId);
			if (p != null)
			{
				var rp = new RuntimePlayerInfo(p, i);
				players.Add(rp.actorNumber, rp);

				if (p == PhotonNetwork.LocalPlayer)
				{
					PhotonPropertyHelper.SetPlayerProp(p.ActorNumber, PlayerPropKeys.MyTurn, i);
				}
			}
			else
			{
				//AI 플레이어인 경우 AI 정보 삽입
				var rp = new RuntimePlayerInfo(turnId, i);
				players.Add(rp.actorNumber, rp);
			}

			if (IsInitializer() && i == 0)
			{
				PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentTurnActor, turnId);
			}
		}
		// Debug.Log("PlayerManager: InitPlayersInfo completed. players: " + players.Count);

		//각 플레이어에 대해서
		// foreach (Player p in PhotonNetwork.PlayerList)
		// {
		// 	//정보 삽입
		// 	var rp = new RuntimePlayerInfo(p);

		// 	// //플레이어 프로퍼티 불러오기
		// 	// var ht = p.CustomProperties;
		// 	// //플레이어 이름, 내 턴 정보 초기화
		// 	// rp.playerName = ht.TryGetValue("playerName", out var name) ? (string)name : "player" + rp.actorNumber;
		// 	//해당 플레이어의 턴 정보 불러오기
		// 	int playerTurn = getIndex(TurnList, p.ActorNumber);
		// 	if (playerTurn == -1)
		// 	{
		// 		Debug.LogError("No Player Turn");
		// 		rp.turnIdx = -1;
		// 		return;
		// 	}
		// 	rp.turnIdx = playerTurn;

		// 	if (p == PhotonNetwork.LocalPlayer)
		// 	{
		// 		PhotonPropertyHelper.SetPlayerProp(p, PlayerPropKeys.MyTurn, playerTurn);
		// 	}
		// 	if (IsInitializer() && playerTurn == 0)
		// 	{
		// 		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentTurnActor, p.ActorNumber);
		// 	}
		// 	Debug.Log($"playerActorNum : {rp.actorNumber}, turn:{rp.turnIdx}");
		// 	players.Add(rp.actorNumber, rp);
		// }
	}

	private void SpawnPlayersOnCircle()
	{
		spawnPos = new Vector3[players.Count];
		hitPos = new Vector3[players.Count];
		spawnInvPos = new Vector3[players.Count];
		spawnRot = new Quaternion[players.Count];
		spawnInvRot = new Quaternion[players.Count];

		float ang = 360 / (float)players.Count;

		for (int i = 0; i < players.Count; i++)
		{
			float angle = 2f * Mathf.PI * i / players.Count;

			//플레이어 스폰 위치 계산
			Vector3 offset = new Vector3(
				Mathf.Cos(angle) * radius,
				0f,
				Mathf.Sin(angle) * radius);

			Vector3 hit_offset = new Vector3(
				Mathf.Cos(angle) * hit_radius,
				0f,
				Mathf.Sin(angle) * hit_radius);


			//플레이어 인벤토리 스폰 위치 계산
			Vector3 InvOffset = new Vector3(
				Mathf.Cos(angle) * InvRadius,
				0f,
				Mathf.Sin(angle) * InvRadius);

			spawnPos[i] = offset + CenterObject.position;
			hitPos[i] = hit_offset + CenterObject.position;
			spawnInvPos[i] = InvOffset + CenterObject.position;
			spawnRot[i] = Quaternion.LookRotation(CenterObject.position - spawnPos[i]);
			spawnInvRot[i] = Quaternion.Euler(0f, (180f + (ang * i)) % 360f, 0f);
		}
	}


	private void InitPlayerProps()
	{
		Player player = PhotonNetwork.LocalPlayer;
		var playerSetting = GameManager.Instance.playerDefaultSetting;
		var roomSetting = GameManager.Instance.roomDefaultSetting;

		var ht = new ExitGames.Client.Photon.Hashtable
		{
			{ PlayerPropKeys.VillageHP, playerSetting.villageHP},
			{ PlayerPropKeys.MaxVillageHP, playerSetting.villageHP},
			{ PlayerPropKeys.VillageBarrier, playerSetting.villageBarrier},
			{PlayerPropKeys.TreeAtkMulti, playerSetting.VillageDmgMulti},
			{ PlayerPropKeys.BarrierArmor, playerSetting.initialBarrierArmor},
			// 참조 복사를 방지하기 위해 Clone() 또는 ToArray() 사용
			{ PlayerPropKeys.VillageUpgrades, (int[])playerSetting.initialVillageUpgrades.Clone()},
			{ PlayerPropKeys.Gold, playerSetting.initialGold },
			{ PlayerPropKeys.DayGoldIncome, playerSetting.initialDayGoldIncome},
			{ PlayerPropKeys.MaxAtkPow, playerSetting.maxAtkPow },
			{ PlayerPropKeys.MinAtkPow, playerSetting.minAtkPow},
			{ PlayerPropKeys.Energy, playerSetting.initialEnergy},
			{ PlayerPropKeys.MaxEnergy, playerSetting.maxEnergy },
			{ PlayerPropKeys.EnergyIncome, playerSetting.energyIncomePerDay},
			{ PlayerPropKeys.CarryOverEnergy, playerSetting.carryOverEnergy},
			{ PlayerPropKeys.DayTimeDamage, playerSetting.dayTimeDamage },
			{ PlayerPropKeys.TotalDamage, playerSetting.initialTotalDamage },
			{ PlayerPropKeys.BarrierConversionRate, playerSetting.barrierConversionRate },
			{ PlayerPropKeys.Item_CommonWeight, playerSetting.commonWeight },
			{ PlayerPropKeys.Item_HeroWeight, playerSetting.heroWeight },
			{ PlayerPropKeys.Item_RareWeight, playerSetting.rareWeight },
			{ PlayerPropKeys.Item_LegendaryWeight, playerSetting.legendaryWeight},
			{ PlayerPropKeys.VDamageProcessCompleted, false},
			{ PlayerPropKeys.PDamageProcessCompleted, false},
		};
		PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

		var rt = new ExitGames.Client.Photon.Hashtable
		{
			{ItemPropKeys.INV(player.ActorNumber), ItemInfoSerializer.MakeEmptyInv(playerSetting.inventoryCapacity) },
			{ItemPropKeys.INV_CAPACITY(player.ActorNumber), playerSetting.inventoryCapacity },
			{ItemPropKeys.OFFER(player.ActorNumber),"" },
			{ItemPropKeys.LOCKPICK(player.ActorNumber), roomSetting.lockpickCount },
			{ItemPropKeys.LOCKCNT(player.ActorNumber), roomSetting.lockCount},
			{ItemPropKeys.COMMON_RATE(player.ActorNumber), roomSetting.common_reduction_rate},
			{ItemPropKeys.HERO_RATE(player.ActorNumber), roomSetting.hero_reduction_rate},
			{ItemPropKeys.RARE_RATE(player.ActorNumber), roomSetting.rare_reduction_rate},
			{ItemPropKeys.LEGENDARY_RATE(player.ActorNumber), roomSetting.legendary_reduction_rate},
		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(rt);

		//모든 프로퍼티가 준비 완료된 경우
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.IsReady, true);

		Debug.Log("Init Player Props Success");
	}

	//기존 멀티 환경에서의 PlayerProperty를
	//AI 환경에선 프로퍼티를 RoomProperty로 설정
	//TODO : 각 플레이어 프로퍼티 접근 로직을 변경해야 함
	private void InitAIProps()
	{
		var playerSetting = GameManager.Instance.playerDefaultSetting;
		var roomSetting = GameManager.Instance.roomDefaultSetting;

		foreach (var kvp in players)
		{
			RuntimePlayerInfo rp = kvp.Value;

			Player p = PhotonNetwork.CurrentRoom.GetPlayer(rp.actorNumber);
			if (p == null)
			{
				var ht = new ExitGames.Client.Photon.Hashtable
		{
			{ $"{PlayerPropKeys.VillageHP}_{rp.actorNumber}", playerSetting.villageHP},
			{ $"{PlayerPropKeys.MaxVillageHP}_{rp.actorNumber}", playerSetting.villageHP},
			{ $"{PlayerPropKeys.VillageBarrier}_{rp.actorNumber}", playerSetting.villageBarrier},
			{ $"{PlayerPropKeys.TreeAtkMulti}_{rp.actorNumber}", playerSetting.VillageDmgMulti},
			{ $"{PlayerPropKeys.BarrierArmor}_{rp.actorNumber}", playerSetting.initialBarrierArmor},
			// 참조 복사를 방지하기 위해 Clone() 사용 (스크립터블 값이 바뀌는 문제가 있었음)
			{ $"{PlayerPropKeys.VillageUpgrades}_{rp.actorNumber}", (int[])playerSetting.initialVillageUpgrades.Clone()},
			{ $"{PlayerPropKeys.Gold}_{rp.actorNumber}", playerSetting.initialGold },
			{ $"{PlayerPropKeys.DayGoldIncome}_{rp.actorNumber}", playerSetting.initialDayGoldIncome},
			{ $"{PlayerPropKeys.MaxAtkPow}_{rp.actorNumber}", playerSetting.maxAtkPow },
			{ $"{PlayerPropKeys.MinAtkPow}_{rp.actorNumber}", playerSetting.minAtkPow},
			{ $"{PlayerPropKeys.Energy}_{rp.actorNumber}", playerSetting.initialEnergy},
			{ $"{PlayerPropKeys.MaxEnergy}_{rp.actorNumber}", playerSetting.maxEnergy },
			{ $"{PlayerPropKeys.EnergyIncome}_{rp.actorNumber}", playerSetting.energyIncomePerDay},
			{ $"{PlayerPropKeys.CarryOverEnergy}_{rp.actorNumber}", playerSetting.carryOverEnergy},
			{ $"{PlayerPropKeys.DayTimeDamage}_{rp.actorNumber}", playerSetting.dayTimeDamage },
			{ $"{PlayerPropKeys.TotalDamage}_{rp.actorNumber}", playerSetting.initialTotalDamage },
			{ $"{PlayerPropKeys.BarrierConversionRate}_{rp.actorNumber}", playerSetting.barrierConversionRate },
			{ $"{PlayerPropKeys.Item_CommonWeight}_{rp.actorNumber}", playerSetting.commonWeight },
			{ $"{PlayerPropKeys.Item_HeroWeight}_{rp.actorNumber}", playerSetting.heroWeight },
			{ $"{PlayerPropKeys.Item_RareWeight}_{rp.actorNumber}", playerSetting.rareWeight },
			{ $"{PlayerPropKeys.Item_LegendaryWeight}_{rp.actorNumber}", playerSetting.legendaryWeight},
			{ $"{PlayerPropKeys.VDamageProcessCompleted}_{rp.actorNumber}", false},
			{ $"{PlayerPropKeys.PDamageProcessCompleted}_{rp.actorNumber}", false},
			{ItemPropKeys.INV(rp.actorNumber), ItemInfoSerializer.MakeEmptyInv(playerSetting.inventoryCapacity) },
			{ItemPropKeys.INV_CAPACITY(rp.actorNumber), playerSetting.inventoryCapacity },
			{ItemPropKeys.OFFER(rp.actorNumber),"" },
			{ItemPropKeys.LOCKPICK(rp.actorNumber), roomSetting.lockpickCount },
			{ItemPropKeys.LOCKCNT(rp.actorNumber), roomSetting.lockCount},
			{ItemPropKeys.COMMON_RATE(rp.actorNumber), roomSetting.common_reduction_rate},
			{ItemPropKeys.HERO_RATE(rp.actorNumber), roomSetting.hero_reduction_rate},
			{ItemPropKeys.RARE_RATE(rp.actorNumber), roomSetting.rare_reduction_rate},
			{ItemPropKeys.LEGENDARY_RATE(rp.actorNumber), roomSetting.legendary_reduction_rate},
		};

				PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
			}
			//AI 플레이어도 Ready 됨을 명시하고, 해당 프로퍼티는 RoomProp에 저장되므로 따로 확인 함수를 호출해줘야 한다.
			PhotonPropertyHelper.SetRoomProp($"{PlayerPropKeys.IsReady}_{rp.actorNumber}", true);
			InitPlayerPropsInStartGame();
		}

	}

	//모든 플레이어의 프로퍼티가 초기화 되었는지 확인
	//싱글 모드일 경우 AI 프로퍼티 초기화도 확인(RoomProp 확인)
	private bool AllPlayersReady()
	{
		bool usersReady = PhotonNetwork.PlayerList.All(p => p.CustomProperties.TryGetValue(PlayerPropKeys.IsReady, out var v) && (bool)v);

		if (!usersReady) return false;

		//싱글 플레이 모드인 경우
		if (GameManager.Instance.isSoloPlay)
		{
			//room 프로퍼티 가져와서
			var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
			foreach (var kvp in PlayerManager.Instance.Players)
			{
				//ai 플레이어 찾고
				int actNum = kvp.Value.actorNumber;
				Player p = PhotonNetwork.CurrentRoom.GetPlayer(actNum);

				if (p == null)  //p == ai 플레이어
				{
					string aiKey = $"{PlayerPropKeys.IsReady}_{actNum}";
					//프로퍼티 값 확인
					if (!roomProps.TryGetValue(aiKey, out var v) || !(bool)v)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

}
