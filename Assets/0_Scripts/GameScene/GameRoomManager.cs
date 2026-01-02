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
	private bool IsInitializer()
	{
		var players = PhotonNetwork.PlayerList;
		int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
		int minActor = players.Min(p => p.ActorNumber);
		return myActor == minActor;
	}

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
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, CommonDefine.defaultTreeHP);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeAtkPow, CommonDefine.defaultTreeAtkPow);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentDay, CommonDefine.defaultStartDay);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentTurn, CommonDefine.defaultTurn);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TurnIndex, CommonDefine.defaultTurnIndex);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.GamePhase, CommonDefine.defaultPhaseValue);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.MaxWaveCnt, CommonDefine.defaultMaxWave);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.CurrentWave, CommonDefine.defaultWave);
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.IsVillageUpgradePhase, false);

		PhotonPropertyHelper.SetRoomProp(ItemPropKeys.NEXT_UID, CommonDefine.defaultUID);

		TreeStatus.Instance.SetTreeStatusUI();

		Debug.Log("Init Room Props Success");
	}
}
