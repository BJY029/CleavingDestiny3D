using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;
using ExitGames.Client.Photon;
using System;
using UnityEngine.InputSystem;

public class TurnManager : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private float VillageUpgradeLimitedTime;

	//현재 마을 업그레이드 중인지
	public bool isUpgradePhase;
	//마을 업그레이드 제한 시간
	private float startTime;
	private float endTime;

	public static TurnManager Instance;

	//F키가 눌리면 발생될 이벤트
	public event Action OnInteractFKeyDown;
	//턴 변경 플래그
	private bool TurnHasChanged = false;
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
		startTime = -1.0f;
		endTime = -1.0f;
	}

	private void Update()
	{
		if (Keyboard.current == null) return;

		//만약 'F'키가 눌린 경우
		if (Keyboard.current.fKey.wasPressedThisFrame)
		{
			//'F'키 이벤트 실행
			OnInteractFKeyDown?.Invoke();
		}

		//권한자이면서 동시에 마을 페이즈에 돌입한 경우
		if (IsInitializer() && isUpgradePhase)
		{
			//마을 페이즈 종료 조건을 지속적으로 확인한다.
			CheckVillageUpgradePhase();
		}
	}

	//플레이어 중 가장 작은 actor number를 가진 플레이어가 각종 권한을 갖는다.
	private bool IsInitializer()
	{
		var players = PhotonNetwork.PlayerList;
		int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
		int minActor = players.Min(p => p.ActorNumber);
		return myActor == minActor;
	}

	//턴 변경 요청이 발생한 경우
	public void RequestChangeTurn()
	{
		//만약 현재 마을 페이즈가 실행중인 경우, 턴 변경 요청은 무시한다.
		bool isUpgradePhase = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
		if (isUpgradePhase) return;

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
		if(currentTurnActor != requesterActorNumber)
		{
			Debug.LogError("Turn request ERROR!!\ncurrentTurnActor : " + currentTurnActor + " requestActor : " + requesterActorNumber);
			return;
		}

		//턴 변경 수행
		ChangeToNextTurn();
	}


	//턴 전환 함수
	private void ChangeToNextTurn()
	{
		//턴 정보를 담은 리스트를 불러온다.
		int[] TurnOrder = PhotonPropertyHelper.GetRoomProp<int[]>(RoomPropKeys.TurnOrder);
		if(TurnOrder == null || TurnOrder.Length == 0)
		{
			Debug.LogError("TurnInfo Property Setting ERROR");
			return;
		}

		//현재 턴 순서 값을 가져온다.
		int currentIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurn);
		Debug.Log("Turn " + currentIndex + " End");
		//다음 턴 순서를 계산한다.
		int nextIndex = (currentIndex + 1) % TurnOrder.Length;

		//만약 모든 턴을 돌고 한 웨이브가 끝난 경우
		if(nextIndex == 0)
		{
			//현재 웨이브 값을 불러온다.
			int currentWaveCnt = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
			Debug.Log("Wave " +  currentWaveCnt + " End");
			//웨이브 값을 하나 증가시킨다.
			currentWaveCnt += 1;

			//만약 최대 웨이브에 도달한 경우
			int MaxWaveCnt = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.MaxWaveCnt);
			if (currentWaveCnt >= MaxWaveCnt)
			{
				//날짜 변경인 경우
				//마을 페이즈를 시작한다.
				StartVillageUpgradePhase();
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
		photonView.RPC(nameof(TurnChanedInvoked), RpcTarget.All);

		//다음 턴 정보를 갱신한다.
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentTurn, nextIndex);
		Debug.Log("Turn " + nextIndex + " Start");
	}

	[PunRPC]
	public void TurnChanedInvoked()
	{
		TurnHasChanged = true;
	}

	//마을 업그레이드 페이즈를 설정한다.
	private void StartVillageUpgradePhase()
	{
		//제일 낮은 순서번호의 플레이어만 초기화 수행
		if (!IsInitializer()) return;

		//만약 이미 실행중이라면 실행하지 않음
		bool isAlreadyUpgraded = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
		if (isAlreadyUpgraded) return;

		//현재 시간
		startTime = (float)PhotonNetwork.Time;
		//종료 시간
		endTime = (float)PhotonNetwork.Time + VillageUpgradeLimitedTime;
		//프로퍼티 삽입을 위해 배열 형태로 저장
		float[] timeValue = new float[] { startTime, endTime };
		//시간 프로퍼티 삽입
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.VillageUpgradeStartEndTime, timeValue);
		//마을 페이즈 진입 프로퍼티 초기화
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.IsVillageUpgradePhase, true);

		//관련 UI 처리를 진행한다.
		GameCanvasController.Instance.SetActiveCanvas(false);
		PlayerCanvasController.Instance.SetActiveCanvas(false);
		VillageUIManager.Instance.SetActiveCanvas(true);

	}

	//현재 마을 변경 페이즈가 진행중인지 체크한다.
	private void CheckVillageUpgradePhase()
	{
		//시간값을 지속적으로 계산한다.
		float now = (float)PhotonNetwork.Time;

		if (now >= endTime)
		{
			//시간이 모두 지난 경우
			//마을 페이즈 종료
			CompleteDayChangeAfterUpgrade();
			endTime = -1.0f;
		}
	}

	//마을 페이즈 종료
	private void CompleteDayChangeAfterUpgrade()
	{
		//권한자만 실행
		if (!IsInitializer()) return;

		//마을 페이즈 종료 프로퍼티 초기화
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.IsVillageUpgradePhase, false);

		//관련 UI를 처리한다.
		VillageUIManager.Instance.SetActiveCanvas(false);
		GameCanvasController.Instance.SetActiveCanvas(true);
		PlayerCanvasController.Instance.SetActiveCanvas(true);

		//현재 날짝 값을 불러온다.
		int currentDayCnt = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentDay);
		Debug.Log("Day " + currentDayCnt + " End");
		//날짜 값을 하나 증가시킨다.
		currentDayCnt += 1;

		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentDay, currentDayCnt);
		//웨이브 값은 0으로 초기화시킨다.
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentWave, CommonDefine.defaultWave);
		Debug.Log("Day " + currentDayCnt + " Start");

		photonView.RPC(nameof(TurnChanedInvoked), RpcTarget.All);
		//다음 턴 정보를 갱신한다.
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentTurn, CommonDefine.defaultTurn);
		Debug.Log("Turn " + 0 + " Start");

		startTime = endTime = -1.0f;
	}

	//프로퍼티 변경 감지
	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		//턴 관련 프로퍼티가 변경되었고, 아직 처리되지 않은 경우
		if(propertiesThatChanged.ContainsKey(RoomPropKeys.CurrentTurn) && TurnHasChanged)
		{
			//관련 UI 처리를 진행해주고
			PlayerCanvasController.Instance.UpdateGameHitText();
			GameCanvasController.Instance.UpdateDayText();
			GameCanvasController.Instance.UpdateWaveText();
			//중복 처리 방지위해 플래그를 설정한다.
			TurnHasChanged=false;
		}

		//마을 페이즈가 진행되는 여부를 저장한다.
		if (propertiesThatChanged.ContainsKey(RoomPropKeys.IsVillageUpgradePhase)){
			isUpgradePhase = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
		}
	}
}
