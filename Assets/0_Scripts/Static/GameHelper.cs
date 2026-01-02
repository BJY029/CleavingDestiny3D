using Photon.Pun;
using UnityEngine;

public static class GameHelper
{
	//현재 내 턴인지 확인하는 함수
    public static bool IsMyTurn()
    {
		var room = PhotonNetwork.CurrentRoom;
		if (room == null) return false;

		int CurrentTurn = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurn);
		int myTurn = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MyTurn);

		return CurrentTurn == myTurn;
	}

	public static int getCurrentTurnActorNum()
	{
		int currentTurnIdx = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurn);
		int[] TurnInfos = PhotonPropertyHelper.GetRoomProp<int[]>(RoomPropKeys.TurnOrder);
		return TurnInfos[currentTurnIdx];
	}
}
