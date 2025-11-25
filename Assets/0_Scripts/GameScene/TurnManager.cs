using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Linq;
using ExitGames.Client.Photon;

public class TurnManager : MonoBehaviourPunCallbacks
{
	public static TurnManager Instance;
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
	}

	private bool IsInitializer()
	{
		var players = PhotonNetwork.PlayerList;
		int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
		int minActor = players.Min(p => p.ActorNumber);
		return myActor == minActor;
	}

	public void RequestChangeTurn()
	{
		photonView.RPC(nameof(RPC_RequestChangeTurn), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
	}

	private void Update()
	{
		//나무를 보고 있으면서 ,내 턴이면서 , f키를 누른 경우 아래 rpc 호출
	}

	[PunRPC]
	private void RPC_RequestChangeTurn(int requesterActorNumber, PhotonMessageInfo info)
	{
		if (!IsInitializer()) return;

		int currentTurnActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurn);
		if(currentTurnActor != requesterActorNumber)
		{
			Debug.LogError("Turn request ERROR!!");
			return;
		}

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
				//현재 날짝 값을 불러온다.
				int currentDayCnt = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentDay);
				Debug.Log("Day " + currentDayCnt + " End");
				//날짜 값을 하나 증가시킨다.
				currentDayCnt += 1;

				PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentDay, currentDayCnt);
				//웨이브 값은 0으로 초기화시킨다.
				PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentWave, CommonDefine.defaultWave);
				Debug.Log("Day " + currentDayCnt + " Start");


				//END DAY 로직 추가하기
			}
			else//최대 웨이브 값이 아닌 경우
			{
				//웨이브 값을 갱신한다.
				PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentWave, currentWaveCnt);
				Debug.Log("Wave " + currentWaveCnt + " Start");
			}
		}
		//다음 턴 정보를 갱신한다.
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentTurn, nextIndex);
		Debug.Log("Turn " + nextIndex + " Start");
	}

	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		if(propertiesThatChanged.ContainsKey(RoomPropKeys.CurrentTurn))
		{
			//내 턴인지 확인 후 관련 이벤트 처리 진행
		}
	}
}
