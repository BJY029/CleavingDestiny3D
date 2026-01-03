using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class GameRoomManager : MonoBehaviourPunCallbacks
{
	private void Start()
	{
		if (IsInitializer())
		{
			InitRoomProps();
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

	private void GenerateRoomSeed()
	{
			if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(RoomPropKeys.RoomSeed))
			{
				int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
				PhotonPropertyHelper.SetRoomProp(RoomPropKeys.RoomSeed, seed);

			}
	}

	private void InitRoomProps()
	{
		GenerateRoomSeed();
		var ht = new ExitGames.Client.Photon.Hashtable
			{
				{ RoomPropKeys.AllReady,false},
				{ RoomPropKeys.TreeHP,CommonDefine.defaultTreeHP },
				{ RoomPropKeys.TreeAtkPow, CommonDefine.defaultTreeAtkPow},
				{ RoomPropKeys.CurrentDay, CommonDefine.defaultStartDay},
				{ RoomPropKeys.CurrentTurn, CommonDefine.defaultTurn},
				{ RoomPropKeys.TurnIndex, CommonDefine.defaultTurnIndex },
				{ RoomPropKeys.GamePhase, CommonDefine.defaultPhaseValue },
				{ RoomPropKeys.MaxWaveCnt, CommonDefine.defaultMaxWave },
				{ RoomPropKeys.CurrentWave, CommonDefine.defaultWave},
				{ RoomPropKeys.IsVillageUpgradePhase, false },
				{ItemPropKeys.NEXT_UID, CommonDefine.defaultUID },
			};
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

		TreeStatus.Instance.SetTreeStatusUI();

		Debug.Log("Init Room Props Success");
	}
}
