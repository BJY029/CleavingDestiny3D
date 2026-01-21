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

public class TurnManager : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private float VillageUpgradeLimitedTime;

	public VillageSceneManager villageSceneManager;

	private int _villageActionId = 0;
	private int _villageShieldProcessDone = -1;
	private int _villageDamageProcessDone = -1;

	public bool isUpgradePhase;

	public static TurnManager Instance;

	//F키가 눌리면 발생될 이벤트
	public event Action OnInteractFKeyDown;
	//턴 변경 플래그
	private bool TurnHasChanged = false;
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
		if (Keyboard.current == null) return;

		//만약 'F'키가 눌린 경우
		if (Keyboard.current.fKey.wasPressedThisFrame)
		{
			//'F'키 이벤트 실행
			//관련 이벤트는 PlayerController.cs에서 처리(HandleInteractFKey())
			OnInteractFKeyDown?.Invoke();
		}

		// K키가 눌리고 마스터 클라이언트이며 아직 마을 페이즈가 아닌 경우 강제 시작
		if (Keyboard.current.kKey.wasPressedThisFrame && IsInitializer() && !isUpgradePhase)
		{
			StartVillageUpgradePhase();
		}
	}

	public void SetVillageSceneManager(VillageSceneManager vsm)
	{
		villageSceneManager = vsm;
		vsm.OnVillagePhaseEnded += CompleteDayChangeAfterUpgrade;
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

		ItemHandlingSystem.instance.OnTurnStart();
	}

	public void WaveEnd()
	{
		if (!PhotonNetwork.IsMasterClient) return;
		ItemHandlingSystem.instance.OnWaveEnd();
	}

	//턴 변경 요청이 발생한 경우
	public void RequestChangeTurn(int damage)
	{
		//만약 현재 마을 페이즈가 실행중인 경우, 턴 변경 요청은 무시한다.
		bool isUpgradePhase = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
		if (isUpgradePhase) return;

		//데미지 계산 및 반영(아이템 효과 반영)
		ItemHandlingSystem.instance.RequestHit(damage, true);

		//RPC로 모든 플레이어에게 턴 변경 요청을 보낸다.
		photonView.RPC(nameof(RPC_RequestChangeTurn), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);

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

		//턴 변경을 요청한 플레이어와, 현재 턴에 해당하는 플레이어가 일치하지 않는 경우
		if (currentTurnActor != requesterActorNumber)
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
				float treeDmg = TreeStatus.Instance.getTreeAtkPow();
				photonView.RPC(nameof(TreeActionProcess), RpcTarget.All, treeDmg);
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

	[PunRPC]
	public void TurnChanedInvoked()
	{
		TurnHasChanged = true;
	}

	[PunRPC]
	public void TreeActionProcess(float dmg)
	{
		StartCoroutine(TreeAction(dmg));
	}

	IEnumerator TreeAction(float dmg)
	{
		_villageActionId = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

		//VilageStart�� �ߵ��Ǵ� ������ ó��
		photonView.RPC(nameof(RPC_RequestVillageShileldProcess), RpcTarget.MasterClient, _villageActionId);
		while (_villageActionId != _villageShieldProcessDone)
			yield return null;

		//photonView.RPC(nameof(RPC_RequestVillageDamageProcess), RpcTarget.MasterClient, dmg, _villageActionId);
		//while (_villageActionId != _villageDamageProcessDone)
		//	yield return null;


		//���� ������ ó�� ����
		PlayerStatus.Instance.DamagedVillage(dmg);
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

	[PunRPC]
	private void RPC_RequestVillageDamageProcess(float dmg, int actionId, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		PlayerStatus.Instance.DamagedVillage(dmg);

		photonView.RPC(nameof(RPC_OnVillageDamageProcessDone), RpcTarget.All, actionId);
	}

	[PunRPC]
	private void RPC_OnVillageDamageProcessDone(int actionId, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient) return;
		_villageDamageProcessDone = actionId;
	}

	//���� ���׷��̵� ����� �����Ѵ�.
	private void StartVillageUpgradePhase()
	{
		//제일 낮은 순서번호의 플레이어만 초기화 수행
		if (!IsInitializer()) return;

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

			{RoomPropKeys.CurrentTurn, nextIndex },
			{RoomPropKeys.CurrentTurnActor, nextActor },
			{RoomPropKeys.TurnIndex, turnIndex },

			{ItemPropKeys.OFFER(nextActor), offerStr ?? "" },
		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

		photonView.RPC(nameof(setOfferGenerated), RpcTarget.All, true);
		photonView.RPC(nameof(TurnChanedInvoked), RpcTarget.All);
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
}
