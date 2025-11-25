using Photon.Pun;
using UnityEngine;

public static class GameHelper
{
    public static bool IsMyTurn()
    {
		var room = PhotonNetwork.CurrentRoom;
		if (room == null) return false;

		int CurrentTurn = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurn);
		int myTurn = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MyTurn);

		return CurrentTurn == myTurn;
	}
}
