using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Linq;
using Unity.InferenceEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Village;
using UnityEngine.SceneManagement;
using System.Threading;
using Cysharp.Threading.Tasks;

public class TurnManager : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private float VillageUpgradeLimitedTime;

	public VillageSceneManager villageSceneManager;

	private int _villageActionId = 0;
	private int _villageShieldProcessDone = -1;
	//private int _villageDamageProcessDone = -1;

	public bool isUpgradePhase;

	public static TurnManager Instance;

	//F키가 눌리면 발생될 이벤트
	//public event Action OnInteractFKeyDown;
	//턴 변경 플래그
	//private bool TurnHasChanged = false;
	private int _lastProcessedTrun = -1;
	//한 번만 Offer 정보 관련 UI를 처리하기 위한 장치
	private bool offerGenerated = false;
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
		TryOpenOfferFromRoomState();
	}

	private void Update()
	{
		// if (Keyboard.current == null) return;

		// //만약 'F'키가 눌린 경우
		// if (Keyboard.current.fKey.wasPressedThisFrame)
		// {
		// 	//'F'키 이벤트 실행
		// 	//관련 이벤트는 PlayerController.cs에서 처리(HandleInteractFKey())
		// 	OnInteractFKeyDown?.Invoke();
		// }

		// // K키가 눌리고 마스터 클라이언트이며 아직 마을 페이즈가 아닌 경우 강제 시작
		// if (Keyboard.current.kKey.wasPressedThisFrame && IsInitializer() && !isUpgradePhase)
		// {
		// 	StartVillageUpgradePhase();
		// }
	}

	public void SetVillageSceneManager(VillageSceneManager vsm)
	{
		villageSceneManager = vsm;
		//vsm.OnVillagePhaseEnded += CompleteDayChangeAfterUpgrade;
		vsm.OnVillagePhaseEnded += ActivateTreeAttack;
	}

	//플레이어 중 가장 작은 actor number를 가진 플레이어가 각종 권한을 갖는다.
	//private bool IsInitializer()
	//{
	//	var players = PhotonNetwork.PlayerList;
	//	int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
	//	int minActor = players.Min(p => p.ActorNumber);
	//	return myActor == minActor;
	//}

	private bool IsInitializer() => PhotonNetwork.IsMasterClient;

	public void NewTurnStart()
	{
		if (!PhotonNetwork.IsMasterClient) return;
		ItemHandlingSystem.instance.InitRandomSystem();
		ItemHandlingSystem.instance.OnTurnStart();
	}

	public void WaveEnd()
	{
		if (!PhotonNetwork.IsMasterClient) return;
		ItemHandlingSystem.instance.OnWaveEnd();
		//PlayerStatus.Instance.InitTreeAtkMultRate();
	}

	private void RemoveUsedItem()
	{
		if (!PhotonNetwork.IsMasterClient) return;
		ItemHandlingSystem.instance.InitDay();
	}

	//턴 변경 요청이 발생한 경우
	public void RequestChangeTurn(int damage, IPlayerAction requester)
	{
		//만약 현재 마을 페이즈가 실행중인 경우, 턴 변경 요청은 무시한다.
		bool isUpgradePhase = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
		if (isUpgradePhase) return;

		//데미지 계산 및 반영(아이템 효과 반영)
		Debug.Log("RequestHit");
		ItemHandlingSystem.instance.RequestHit(damage, true, requester);

		//RPC로 모든 플레이어에게 턴 변경 요청을 보낸다.
		//photonView.RPC(nameof(RPC_RequestChangeTurn), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);

	}

	//모든 클라이언트의 마을 데미지 로직이 완료되었는지 확인하는 함수
	public void PlayerDamageChecker(int attackerNum)
	{
		photonView.RPC(nameof(RPC_PlayerDamageChecker), RpcTarget.MasterClient, attackerNum);
	}

	[PunRPC]
	public void RPC_PlayerDamageChecker(int attackerNum)
	{
		//MasterClient만 수행
		if (!IsInitializer()) return;
		//모든 플레이어의 데미지 처리가 완료되었는지 확인한다.
		if (PlayerHitCheck())
		{
			Debug.Log("All Player Hit Succeed");
			//RPC로 모든 플레이어에게 턴 변경 요청을 보낸다.
			photonView.RPC(nameof(RPC_RequestChangeTurn), RpcTarget.All, attackerNum);
			//마을 데미지 플래그 초기화
			ResetAllPlayersHitState();
		}
	}

	//모든 플레이어의 VDamageProcessCompleted 프로퍼티를 검사하여 반환하는 함수
	private bool PlayerHitCheck()
	{
		bool isPlayerAllHit = PhotonNetwork.PlayerList.All(p => p.CustomProperties.TryGetValue(PlayerPropKeys.PDamageProcessCompleted, out var v) && (bool)v);
		if (!isPlayerAllHit) return false;

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
					string aiKey = $"{PlayerPropKeys.PDamageProcessCompleted}_{actNum}";
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

	//모든 플레이어의 VDamageProcessCompleted 프로퍼티를 초기화 하는 함수
	public void ResetAllPlayersHitState()
	{
		// 권한 체크 (방장만 실행)
		if (!PhotonNetwork.IsMasterClient) return;

		//변경할 속성을 담은 해시테이블 생성 (메모리 절약을 위해 루프 밖에서 생성)
		var props = new ExitGames.Client.Photon.Hashtable()
	{
		{ PlayerPropKeys.PDamageProcessCompleted, false }
	};

		//방에 있는 모든 플레이어 순회하며 적용
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			player.SetCustomProperties(props);
		}
	}

	[PunRPC]
	private void RPC_RequestChangeTurn(int requesterActorNumber, PhotonMessageInfo info)
	{
		//그러나 실제로 턴 변경 관련 처리를 하는 플레이어는 한명이며, 이는 가장 낮은 Actor Number를 가진 플레이어다.
		if (!IsInitializer()) return;

		//턴 정보를 받아온다.
		int[] TurnOrder = PhotonPropertyHelper.GetRoomProp<int[]>(RoomPropKeys.TurnOrder);
		int currentTurnIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurn);
		int currentTurnActor = TurnOrder[currentTurnIndex];

		//만약 턴 정보가 일치하거나, AI 모드인 경우 true
		bool isRequestVaild = (currentTurnActor == requesterActorNumber) ||
								(info.Sender.IsMasterClient && GameHelper.IsCurrentTurnAI());

		//턴 변경을 요청한 플레이어와, 현재 턴에 해당하는 플레이어가 일치하지 않는 경우
		if (!isRequestVaild)
		{
			Debug.LogError("Turn request ERROR!!\ncurrentTurnActor : " + currentTurnActor + " requestActor : " + requesterActorNumber);
			return;
		}

		//나무 때리기 액션 수행, Hit 요청자가 해당 함수 실행
		//photonView.RPC(nameof(RPC_DoHitOnRequester), info.Sender, damageRatio);

		//턴 변경 수행
		ChangeToNextTurn();
	}

	//Hit 요청자 클라에서 실행될 Hit 처리
	//[PunRPC]
	//private void RPC_DoHitOnRequester(float damageRatio)
	//{
	//	int dmg = PlayerStatus.Instance.HitAction(damageRatio);
	//	photonView.RPC(nameof(RPC_ApplyTreeDamage), RpcTarget.All, dmg);
	//}

	[PunRPC]
	private void RPC_ApplyTreeDamage(int dmg, PhotonMessageInfo info)
	{
		if (!IsInitializer()) return;

		//	TreeStatus.Instance.getHitByPlayer(dmg);
	}

	//처음 PlayerManager에서 초기화 된 후 Offer UI를 처리하기 위한 함수
	public void setOfferGeneratedFromOutsied(bool flag)
	{
		photonView.RPC(nameof(setOfferGenerated), RpcTarget.All, true);
	}

	[PunRPC]
	public void setOfferGenerated(bool flag)
	{
		offerGenerated = flag;
	}

	//턴 전환 함수
	private void ChangeToNextTurn()
	{
		if (!IsInitializer()) return;
		//턴 정보를 담은 리스트를 불러온다.
		int[] TurnOrder = PhotonPropertyHelper.GetRoomProp<int[]>(RoomPropKeys.TurnOrder);
		if (TurnOrder == null || TurnOrder.Length == 0)
		{
			Debug.LogError("TurnInfo Property Setting ERROR");
			return;
		}

		//현재 턴 순서 값을 가져온다.
		int currentIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurn);
		Debug.Log("Turn " + currentIndex + " End");
		//다음 턴 순서, 다음 턴의 Actor 번호, 턴 카운트, 해당 턴에 제공할 Offer를 계산한다.
		int nextIndex = (currentIndex + 1) % TurnOrder.Length;
		int nextActor = TurnOrder[nextIndex];
		int turnIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex) + 1;
		string offerStr = OfferAuthority.Instance.MakeOfferForTurn(nextActor, turnIndex);


		//만약 모든 턴을 돌고 한 웨이브가 끝난 경우
		if (nextIndex == 0)
		{
			//현재 웨이브 값을 불러온다.
			int currentWaveCnt = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
			Debug.Log("Wave " + currentWaveCnt + " End");
			//웨이브 값을 하나 증가시킨다.
			currentWaveCnt += 1;

			//만약 최대 웨이브에 도달한 경우
			int MaxWaveCnt = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.MaxWaveCnt);
			if (currentWaveCnt >= MaxWaveCnt)
			{
				//날짜 변경인 경우
				//마을 공격 액션 수행
				photonView.RPC(nameof(TreeActionProcess), RpcTarget.All);
				//StartVillageUpgradePhase();
				return;
			}
			else//최대 웨이브 값이 아닌 경우
			{
				//웨이브 값을 갱신한다.
				PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentWave, currentWaveCnt);
				Debug.Log("Wave " + currentWaveCnt + " Start");
			}
		}
		//턴 변경 관련 처리를 한 번만 수행하기 위해 다음과 같은 RPC로 변수 초기화
		//photonView.RPC(nameof(TurnChanedInvoked), RpcTarget.All);

		//프로퍼티를 한번에 업데이트 한다.
		var ht = new ExitGames.Client.Photon.Hashtable
		{
			{RoomPropKeys.CurrentTurn, nextIndex},
			{RoomPropKeys.CurrentTurnActor, nextActor},
			{RoomPropKeys.TurnIndex, turnIndex },
			{ItemPropKeys.OFFER(nextActor), offerStr ?? "" }
		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
		//마을 페이즈에 돌입하게 되면, 중간에 return이 되서 offer가 반영이 안된다.
		//따라서 꼭 실제로 프로퍼티가 업데이트 된 후에 해당 플레그를 true로 설정해야 한다.
		photonView.RPC(nameof(setOfferGenerated), RpcTarget.All, true);
	}

	// [PunRPC]
	// public void TurnChanedInvoked()
	// {
	// 	TurnHasChanged = true;
	// }

	[PunRPC]
	public void TreeActionProcess()
	{
		StartCoroutine(TreeAction());
	}

	IEnumerator TreeAction()
	{
		_villageActionId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

		//VilageStart�� �ߵ��Ǵ� ������ ó��
		if (PhotonNetwork.IsMasterClient)
			photonView.RPC(nameof(RPC_RequestVillageShileldProcess), RpcTarget.MasterClient, _villageActionId);
		while (_villageActionId != _villageShieldProcessDone)
			yield return null;

		// photonView.RPC(nameof(RPC_RequestVillageDamageProcess), RpcTarget.MasterClient, dmg, _villageActionId);
		// while (_villageActionId != _villageDamageProcessDone)
		// 	yield return null;


		//���� ������ ó�� ����
		//PlayerStatus.Instance.DamagedVillage(dmg);
		//���� ������ �߰�
		//���� ����� �����Ѵ�.
		StartVillageUpgradePhase();
	}

	[PunRPC]
	private void RPC_RequestVillageShileldProcess(int actionId, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		ItemHandlingSystem.instance.OnVillageStart();

		photonView.RPC(nameof(RPC_OnVillageShieldProcessDone), RpcTarget.All, actionId);
	}

	[PunRPC]
	private void RPC_OnVillageShieldProcessDone(int actionId, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient) return;
		_villageShieldProcessDone = actionId;
	}

	// [PunRPC]
	// private void RPC_RequestVillageDamageProcess(float dmg, int actionId, PhotonMessageInfo info)
	// {
	// 	if (!PhotonNetwork.IsMasterClient) return;

	// 	PlayerStatus.Instance.DamagedVillage(dmg);

	// 	photonView.RPC(nameof(RPC_OnVillageDamageProcessDone), RpcTarget.All, actionId);
	// }

	// [PunRPC]
	// private void RPC_OnVillageDamageProcessDone(int actionId, PhotonMessageInfo info)
	// {
	// 	if (!info.Sender.IsMasterClient) return;
	// 	_villageDamageProcessDone = actionId;
	// }

	//���� ���׷��̵� ����� �����Ѵ�.
	public void StartVillageUpgradePhase()
	{
		//제일 낮은 순서번호의 플레이어만 초기화 수행
		if (!IsInitializer()) return;

		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.GamePhase, GamePhaseValue.NIGHT_VILLAGE);
		//'하루'의 길이를 가진 아이템을 삭제한다.
		WaveEnd();

		//만약 이미 실행중이라면 실행하지 않음
		bool isAlreadyUpgraded = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
		if (isAlreadyUpgraded) return;

		//현재 시간
		float startTime = (float)PhotonNetwork.Time;
		//종료 시간
		float endTime = (float)PhotonNetwork.Time + VillageUpgradeLimitedTime;
		//프로퍼티 삽입을 위해 배열 형태로 저장
		Vector2 timeValue = new Vector2(startTime, endTime);
		//시간 프로퍼티 삽입
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.VillageUpgradeStartEndTime, timeValue);
		//마을 업그레이드 페이즈 플래그 설정 (OnRoomPropertiesUpdate에서 마을 씬 로드)
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.IsVillageUpgradePhase, true);

	}

	//나무가 마을들을 공격하는 로직 실행
	public void ActivateTreeAttack()
	{
		if (!IsInitializer()) return;
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.GamePhase, GamePhaseValue.NIGHT_TREEATK);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.IsTreeBulkDamage, true);
		//MasterClient가 대표로 기본 나무 공격력 가져오고
		float treeDmg = TreeStatus.Instance.getTreeAtkPow();
		//각 클라에게 데미지 계산 요청
		photonView.RPC(nameof(StartTreeAtackCoroutine), RpcTarget.All, treeDmg);
		//CompleteDayChangeAfterUpgrade();
	}

	//코루틴 호출
	[PunRPC]
	public void StartTreeAtackCoroutine(float treeDmg)
	{
		StartCoroutine(ProcessTreeAtack(treeDmg));
	}

	//연출 및 나무 데미지 계산
	IEnumerator ProcessTreeAtack(float treeDmg)
	{
		//관련 애니메이션 처리

		//각 클라이에서 자신의 Multi 적용하여 데미지 계산 및 적용 수행
		PlayerStatus.Instance.DamagedVillage(treeDmg);
		//임시 코드
		//yield return new WaitForSeconds(2f);
		yield return null;
	}

	//모든 클라이언트의 마을 데미지 로직이 완료되었는지 확인하는 함수
	public void TreeDamageChecker()
	{
		photonView.RPC(nameof(RPC_TreeDamageChecker), RpcTarget.MasterClient);
	}

	[PunRPC]
	public void RPC_TreeDamageChecker()
	{
		//MasterClient만 수행
		if (!IsInitializer()) return;
		//모든 플레이어의 데미지 처리가 완료되었는지 확인한다.
		if (AllPlayersReady())
		{
			//패배 감지 수행
			if (MatchResultManager.Instance.TryResolveResultByVillageHP())
			{
				Debug.Log("Game End By VillageDestroyed");
				return;
			}
			//마을 데미지 플래그 초기화
			ResetAllPlayersReadyState();
			//완료된 경우, 마을 종료 수행
			CompleteDayChangeAfterUpgrade();
			PhotonPropertyHelper.SetRoomProp(RoomPropKeys.IsTreeBulkDamage, false);
		}
	}

	//모든 플레이어의 VDamageProcessCompleted 프로퍼티를 검사하여 반환하는 함수
	private bool AllPlayersReady()
	{
		bool usersReady = PhotonNetwork.PlayerList.All(p => p.CustomProperties.TryGetValue(PlayerPropKeys.VDamageProcessCompleted, out var v) && (bool)v);

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
					string aiKey = $"{PlayerPropKeys.VDamageProcessCompleted}_{actNum}";
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

	//모든 플레이어의 VDamageProcessCompleted 프로퍼티를 초기화 하는 함수
	public void ResetAllPlayersReadyState()
	{
		// 권한 체크 (방장만 실행)
		if (!PhotonNetwork.IsMasterClient) return;

		//변경할 속성을 담은 해시테이블 생성 (메모리 절약을 위해 루프 밖에서 생성)
		var props = new ExitGames.Client.Photon.Hashtable()
	{
		{ PlayerPropKeys.VDamageProcessCompleted, false }
	};

		//방에 있는 모든 플레이어 순회하며 적용
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			player.SetCustomProperties(props);
		}
	}

	//마을 페이즈 종료
	private void CompleteDayChangeAfterUpgrade()
	{
		//권한자만 실행
		if (!IsInitializer()) return;

		// 1. 프로퍼티 설정 해제
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.IsVillageUpgradePhase, false);

		//플레이어 상태 정보 초기화
		PlayerStatus.Instance.initPlayerStatus();

		//관련 UI를 처리한다.
		GameCanvasController.Instance.SetActiveCanvas(true);
		PlayerCanvasController.Instance.SetActiveCanvas(true);

		// 기본 세팅값 참조
		var roomSet = GameManager.Instance.roomDefaultSetting;

		//다음 날짜 값을 계산.
		int day = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentDay) + 1;

		//다음 턴 정보 계산
		// ** 만약 방 설정을 추가할거라면 SO가 아닌 방 설정값을 참조하도록 변경 필요 **
		int[] TurnOrder = PhotonPropertyHelper.GetRoomProp<int[]>(RoomPropKeys.TurnOrder);
		int nextIndex = roomSet.initialTurn;
		int nextActor = TurnOrder[nextIndex];
		int turnIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex) + 1;
		string offerStr = OfferAuthority.Instance.MakeOfferForTurn(nextActor, turnIndex);

		//해당 프로퍼티를 한번에 업데이트 한다.
		var ht = new ExitGames.Client.Photon.Hashtable
		{
			{RoomPropKeys.CurrentDay, day },
			{RoomPropKeys.CurrentWave, roomSet.initialWave },
			{RoomPropKeys.GamePhase, GamePhaseValue.DAY},

			{RoomPropKeys.CurrentTurn, nextIndex },
			{RoomPropKeys.CurrentTurnActor, nextActor },
			{RoomPropKeys.TurnIndex, turnIndex },

			{ItemPropKeys.OFFER(nextActor), offerStr ?? "" },
		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

		photonView.RPC(nameof(setOfferGenerated), RpcTarget.All, true);
		//photonView.RPC(nameof(TurnChanedInvoked), RpcTarget.All);
		RemoveUsedItem();
	}

	//프로퍼티 변경 감지
	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		//나의 Actor 번호
		int me = PhotonNetwork.LocalPlayer.ActorNumber;
		//현재 턴에 해당되는 Actor 번호(최신 값을 기준으로 한다.)
		int turnActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
		if (propertiesThatChanged.TryGetValue(RoomPropKeys.CurrentTurnActor, out var taObj))
		{
			turnActor = Convert.ToInt32(taObj);
			TryOpenOfferFromRoomState();
			StatusUIModel.instance.StatusOnChanged?.Invoke();
			//자신의 턴이라면
			if (me == turnActor)
			{
				//턴 타이머 시작
				TimeManager.instance.StartTurnTimer();
			}
			//MasterClient이면서, 현재 턴이 AI 턴인 경우
			else if (PhotonNetwork.IsMasterClient && GameHelper.IsCurrentTurnAI())
			{
				//AI 턴 비동기 함수 호출
				Debug.Log($"AI Turn(AI num : {turnActor}).. Processing by MasterClient");
				AI_PlayTurnAsync(turnActor).Forget();
			}
			else//오류 상황
			{
				Debug.LogError($"ERROR! Turn Actor : {turnActor}");
			}
		}

		//턴 관련 프로퍼티가 변경되었고, 아직 처리되지 않은 경우
		if (propertiesThatChanged.TryGetValue(RoomPropKeys.CurrentTurn, out var turnObj))
		{
			int newTurn = Convert.ToInt32(turnObj);
			if (newTurn != _lastProcessedTrun)
			{
				_lastProcessedTrun = newTurn;

				//관련 UI 처리를 진행해주고
				PlayerCanvasController.Instance.UpdateGameHitText();
				GameCanvasController.Instance.UpdateDayText();
				GameCanvasController.Instance.UpdateWaveText();

				//턴 변경 시 발동될 아이템 실행 하는 함수 호출
				NewTurnStart();
			}
			//중복 처리 방지위해 플래그를 설정한다.
			//TurnHasChanged=false;
		}

		//내 Offer RoomProperty Key 가져오기
		string myOfferKey = ItemPropKeys.OFFER(me);
		//변경된 Offer 프로퍼티가 내 것에 해당되는 경우
		if (propertiesThatChanged.TryGetValue(myOfferKey, out var offerObj))
		{
			//그리고 offer가 처음 제공되는 경우
			if (offerGenerated)
			{
				//해당 프로퍼티로부터 offer string을 받아온다.
				string offers = offerObj as string ?? "";

				//내 턴이고, offer가 유효하면
				if (turnActor == me && !string.IsNullOrEmpty(offers))
				{
					//만약 offer 메시지가 Error 메시지로 이루어진 경우
					if (string.Equals(offers, ERROR.FULL_INV.ToString()))
					{
						//Warning으로 경고문을 발생시킨다.
						PlayerCanvasController.Instance.SetWarningTextActive(UI_CSV.UI_Warning_FullInv);
						ItemOfferCanvasController.instance.Close();
						return;
					}
					//관련 UI를 처리한다.
					ItemOfferCanvasController.instance.initItemOfferPanel(offers, me);
				}
				else
				{
					//내 턴이 아니면 UI를 닫는다.
					ItemOfferCanvasController.instance.Close();
				}
				//플래그 처리
				offerGenerated = false;
			}
		}
	}

	//처음 게임이 시작 될 때, 네트워크 이슈로 프로퍼티가 아직 초기화 되지 않았는데 
	//호출 되는 경우, CurrentTurnActor 설정 이슈 등등의 문제로 버그가 발생함
	//이를 막기 위해서 CurrentTurnActor 프로퍼티가 변경될 시 별도의 체크를 수행하는 함수를 호출한다.
	public void TryOpenOfferFromRoomState()
	{
		int me = PhotonNetwork.LocalPlayer.ActorNumber;
		int turnActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
		string offer = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.OFFER(me));

		if (turnActor == me && !string.IsNullOrEmpty(offer))
		{
			if (ItemOfferCanvasController.instance != null)
			{
				//만약 offer 메시지가 Error 메시지로 이루어진 경우
				if (string.Equals(offer, ERROR.FULL_INV.ToString()))
				{
					//Warning으로 경고문을 발생시킨다.
					PlayerCanvasController.Instance.SetWarningTextActive(UI_CSV.UI_Warning_FullInv);
					ItemOfferCanvasController.instance.Close();
					return;
				}
				ItemOfferCanvasController.instance.initItemOfferPanel(offer, me);
			}
		}

		VillageUpgradeLimitedTime = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.VillagePhaseTime);
	}

	//비동기로 플레이어 턴 처리 수행
	private async UniTaskVoid AI_PlayTurnAsync(int aiActorNum)
	{
		//객체 파괴시 토큰 파괴
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		//게임 시작 준비가 완료된 후에 아래 함수를 수행하도록 설정
		//게임 시작시 ai 플레이어가 생성되기 전에 딕셔너리에서 찾는 것을 방지하기 위함
		await UniTask.WaitUntil(() => PlayerManager.Instance.succeedToPreapreGame, cancellationToken: token);
		TimeManager.instance.StartTurnTimer();
		//AI 딕셔너리에서 고유 번호에 해당되는 AI 오브젝트 불러오기
		if (PlayerManager.Instance.AIPlayerObj.TryGetValue(aiActorNum, out var p))
		{
			//해당 AI 컨트롤러의 턴 수행
			AIController ac = p.GetComponent<AIController>();
			await ac.PlayTurnAsync();
		}
		else
		{
			Debug.LogError($"[TurnManager] Can't find {aiActorNum} AI!");
		}
	}
}
