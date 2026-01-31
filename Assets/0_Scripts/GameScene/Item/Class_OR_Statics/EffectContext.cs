using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

/// <summary>
/// 상태 이상/전투 계산 중 공유되는 컨텍스트
/// - 상태 이상이 TreeHP/RoomProps 등을 변경하고 싶을 때 접근
/// </summary>
public class EffectContext
{
	public DeterministicRng Rng { get; private set; }

	public Action<string> Log { get; private set; }

	public EffectContext(DeterministicRng rng, Action<string> log)
	{
		Rng = rng;
		this.Log = log;
	}

	public float GetTreeHP()
	{
		return PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeHP);
	}

	public void SetTreeHP_MasterOnly(float newHP)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] SetTreeHP_MasterOnly called on non-master. Ignored.");
			return;
		}

		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, newHP);
	}

	public float GetPlayerVillageHP(int actorNum)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);
		return PhotonPropertyHelper.GetPlayerProp<float>(player, PlayerPropKeys.VillageHP);
	}

	public void SetPlayerVIllageHP(int actorNum, float newHP)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] SetTreeHP_MasterOnly called on non-master. Ignored.");
			return;
		}

		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.VillageHP, newHP);
	}

	public float GetPlayerVillageShield(int actorNum)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);
		return PhotonPropertyHelper.GetPlayerProp<float>(player, PlayerPropKeys.VillageBarrier);
	}

	public void SetPlayerVIllageShield(int actorNum, float newValue)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] SetTreeHP_MasterOnly called on non-master. Ignored.");
			return;
		}

		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.VillageBarrier, newValue);
	}

	public int GetPlayerEng(int actorNum)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);
		return PhotonPropertyHelper.GetPlayerProp<int>(player, PlayerPropKeys.Energy);
	}

	public void SetPlayerEng(int actorNum, int newValue)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] SetTreeHP_MasterOnly called on non-master. Ignored.");
			return;
		}

		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.Energy, newValue);
	}

	public float GetBarrierConversionRate(int actorNum)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNum);
		return PhotonPropertyHelper.GetPlayerProp<float>(player, PlayerPropKeys.BarrierConversionRate);
	}

	public void AddPlayerLockPickCount(int actorNum)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] LockPickSetting's called on non-master. Ignored.");
			return;
		}

		int currentLockPickCnt = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.LOCKPICK(actorNum));
		PhotonPropertyHelper.SetRoomProp(ItemPropKeys.LOCKPICK(actorNum), currentLockPickCnt + 1);
	}

	public void AddPlayerLockCount(int actorNum)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] LockPickSetting's called on non-master. Ignored.");
			return;
		}

		int currentLockCnt = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.LOCKCNT(actorNum));
		PhotonPropertyHelper.SetRoomProp(ItemPropKeys.LOCKCNT(actorNum), currentLockCnt + 1);
	}

	public void RemovePlayerLockPickCount(int actorNum)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] LockPickSetting's called on non-master. Ignored.");
			return;
		}

		int currentLockPickCnt = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.LOCKPICK(actorNum));
		PhotonPropertyHelper.SetRoomProp(ItemPropKeys.LOCKPICK(actorNum), Mathf.Max(currentLockPickCnt - 1, 0));
		Log?.Invoke($"[Lockpick] Player{actorNum}'s Lockpick changed... {currentLockPickCnt} -> {currentLockPickCnt - 1}");
	}

	public int GetPlayerLockPickCount(int actorNum)
	{
		return PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.LOCKPICK(actorNum));
	}
}
