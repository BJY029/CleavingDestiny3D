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
	private Dictionary<int, RuntimePlayer> players = new();
	//읽기 전용 딕셔너리
	public IReadOnlyDictionary<int, RuntimePlayer> Players => players;

	//중복 초기화 방지 플래그
	private bool isAlreadyInitialized;
	//MAsterClietn 에서만 사용
	private bool AllReadyFlag = false;

	public Transform CenterObject;
	private float radius = 4.5f;
	private float InvRadius = 16f;
	private Vector3[] spawnPos;
	private Vector3[] spawnInvPos;
	private Quaternion[] spawnRot;
	private Quaternion[] spawnInvRot;


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
		if (AllReadyFlag) return;	//이미 모두 준비되었다고 감지 된 경우 아래 처리 안함

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

		yield return handelUI;

		CameraSwitchManager.Instance.Branch_to_Game();

		int myActNum = PhotonNetwork.LocalPlayer.ActorNumber;
		PhotonNetwork.Instantiate($"Player/Player{myActNum}", spawnPos[myActNum - 1], spawnRot[myActNum - 1]);
		PhotonNetwork.Instantiate("Inventory/InventoryTent", spawnInvPos[myActNum - 1], spawnInvRot[myActNum - 1]);
		CameraSwitchManager.Instance.Off_ExceptPlayerCam();

		GameCanvasController.Instance.gameObject.SetActive(true);
		GameCanvasController.Instance.UpdateDayText();
		GameCanvasController.Instance.UpdateWaveText();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	//미니게임으로부터 turn 순서가 정해지면, 해당 정보 기반으로 플레이어 정보 채워넣을 예정
	private void InitPlayersInfo()
	{
		
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

			//플레이어 프로퍼티 불러오기
			var ht = p.CustomProperties;
			//플레이어 이름, 내 턴 정보 초기화
			rp.playerName = ht.TryGetValue("playerName", out var name) ? (string)name : "player" + rp.actorNumber;
			rp.isMyTurn = false;
			//해당 플레이어의 턴 정보 불러오기
			int playerTurn = getIndex(TurnList, p.ActorNumber);
			if (playerTurn == -1)
			{
				Debug.LogError("No Player Turn");
				rp.turnIdx = -1;
				return;
			}
			rp.turnIdx = playerTurn;

			if (p == PhotonNetwork.LocalPlayer)
			{
				PhotonPropertyHelper.SetPlayerProp(p, PlayerPropKeys.MyTurn, playerTurn);
			}
			if (IsInitializer() && playerTurn == 0)
			{
				PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentTurnActor, p.ActorNumber);
			}
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

			//플레이어 인벤토리 스폰 위치 계산
			Vector3 InvOffset = new Vector3(
				Mathf.Cos(angle) * InvRadius,
				0f,
				Mathf.Sin(angle) * InvRadius);

			spawnPos[i] = offset + CenterObject.position;
			spawnInvPos[i] = InvOffset + CenterObject.position;
			spawnRot[i] = Quaternion.LookRotation(CenterObject.position - spawnPos[i]);
			spawnInvRot[i] = Quaternion.Euler(0f, (180f + (ang * i)) % 360f, 0f);
		}
	}


	private void InitPlayerProps()
	{
		Player player = PhotonNetwork.LocalPlayer;

		var ht = new ExitGames.Client.Photon.Hashtable
		{
			{ PlayerPropKeys.VillageHP, CommonDefine.defaultTreeHP},
			{ PlayerPropKeys.VillageBarrier, CommonDefine.defaultVillageBarrier},
			{  PlayerPropKeys.VillageUpgrades, CommonDefine.defaultVillageUpgrades},
			{ PlayerPropKeys.Gold, CommonDefine.defaultGold },
			{ PlayerPropKeys.MaxAtkPow, CommonDefine.defaultPlayerMaxAtkPow },
			{  PlayerPropKeys.MinAtkPow, CommonDefine.defaultPlayerMinAtkPow},
			{  PlayerPropKeys.Energy, CommonDefine.defaultPlayerEnergy},
			{PlayerPropKeys.MaxEnergy, CommonDefine.defaultPlayerMaxEnergy },
			{ PlayerPropKeys.CarryOverEnergy, CommonDefine.defaultCarryOverEnergy},
			{PlayerPropKeys.DayTimeDamage, CommonDefine.defaultDayTimeDamage },
			{PlayerPropKeys.TotalDamage, CommonDefine.defaultTotalDamage },
			{PlayerPropKeys.BarrierConversionRate, CommonDefine.defaultBarrierConversionRate },
			{PlayerPropKeys.Item_CommonWeight, CommonDefine.defaultCommonItemWeight },
			{ PlayerPropKeys.Item_HeroWeight, CommonDefine.defaultHeroItemWeight },
			{PlayerPropKeys.Item_RareWeight, CommonDefine.defaultRareItemWeight },
			{ PlayerPropKeys.Item_LegendaryWeight, CommonDefine.defaultLegendaryItemWeight},
		};
		PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

		var rt = new ExitGames.Client.Photon.Hashtable
		{
			{ItemPropKeys.INV(player.ActorNumber), ItemInfoSerializer.MakeEmptyInv(CommonDefine.defaultInventoryCapacity) },
			{ItemPropKeys.INV_CAPACITY(player.ActorNumber), CommonDefine.defaultInventoryCapacity },
			{ItemPropKeys.OFFER(player.ActorNumber),"" },
		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(rt);

		//모든 프로퍼티가 준비 완료된 경우
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.IsReady, true);

		Debug.Log("Init Player Props Success");
	}

	bool AllPlayersReady()
	{
		return PhotonNetwork.PlayerList.All(p => p.CustomProperties.TryGetValue(PlayerPropKeys.IsReady, out var v) && (bool)v);
	}

}
