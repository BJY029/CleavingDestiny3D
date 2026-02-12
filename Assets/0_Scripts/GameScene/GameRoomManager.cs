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
		var roomSet = GameManager.Instance.roomDefaultSetting;

		GenerateRoomSeed();
		var ht = new ExitGames.Client.Photon.Hashtable
			{
				{ RoomPropKeys.AllReady,false},
				{ RoomPropKeys.TreeHP,      roomSet.treeHP },
				{ RoomPropKeys.TreeMaxHP,   roomSet.treeHP },
				{ RoomPropKeys.TreeAtkPow,  roomSet.treeAtkPow},
				{ RoomPropKeys.CurrentDay,  roomSet.startDay},
				{ RoomPropKeys.CurrentTurn, roomSet.initialTurn},
				{ RoomPropKeys.TurnIndex,   roomSet.initialTurnIndex },
				{ RoomPropKeys.GamePhase,   roomSet.initialPhase },
				{ RoomPropKeys.MaxWaveCnt,  roomSet.maxWave },
				{ RoomPropKeys.CurrentWave, roomSet.initialWave},
				{ RoomPropKeys.IsVillageUpgradePhase, false },
				{ RoomPropKeys.IsTreeBulkDamage, false},
				{ ItemPropKeys.NEXT_UID,    roomSet.initialUID },
				{ RoomPropKeys.VillagePhaseTime, roomSet.villagePhaseTime },
				{ RoomPropKeys.MatchLoserActor, roomSet.LoserActNum},
				{ RoomPropKeys.MatchResultReason, roomSet.MatchEndReason},
				{ RoomPropKeys.MatchResolveTurnIndex, roomSet.ResolvedTurnIdx},
			};
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

		TreeStatus.Instance.SetTreeStatusUI();

		Debug.Log("Init Room Props Success");
	}
}
