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

	public float GetTreeHP(SimGameState state)
	{
		return state.treeHP;
	}

	public void SetTreeHP(float newHP)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] SetTreeHP_MasterOnly called on non-master. Ignored.");
			return;
		}

		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, newHP);
	}

	public void SetTreeHP(float newHP, SimGameState state)
	{
		state.treeHP = newHP;
	}


	public float GetPlayerVillageHP(int actorNum)
	{
		return PhotonPropertyHelper.GetPlayerProp<float>(actorNum, PlayerPropKeys.VillageHP);
	}

	public float GetPlayerVillageHP(int playerNum, SimGameState state)
	{
		return playerNum == 1 ? state.p1VillHP : state.p2VillHP;
	}


	public void SetPlayerVIllageHP(int actorNum, float newHP)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] SetTreeHP_MasterOnly called on non-master. Ignored.");
			return;
		}

		PhotonPropertyHelper.SetPlayerProp(actorNum, PlayerPropKeys.VillageHP, newHP);
	}
	public void SetPlayerVIllageHP(int playerNum, float newHP, SimGameState state)
	{
		if (playerNum == 1) state.p1VillHP = newHP;
		else state.p2VillHP = newHP;
	}



	public float GetPlayerVillageShield(int actorNum)
	{
		return PhotonPropertyHelper.GetPlayerProp<float>(actorNum, PlayerPropKeys.VillageBarrier);
	}
	public float GetPlayerVillageShield(int playerNum, SimGameState state)
	{
		return playerNum == 1 ? state.p1VillBarrier : state.p2VillBarrier;
	}



	public void SetPlayerVIllageShield(int actorNum, float newValue)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] SetTreeHP_MasterOnly called on non-master. Ignored.");
			return;
		}

		PhotonPropertyHelper.SetPlayerProp(actorNum, PlayerPropKeys.VillageBarrier, newValue);
	}
	public void SetPlayerVIllageShield(int playerNum, float newValue, SimGameState state)
	{
		if (playerNum == 1) state.p1VillBarrier = newValue;
		else state.p2VillBarrier = newValue;
	}



	public int GetPlayerEng(int actorNum)
	{
		return PhotonPropertyHelper.GetPlayerProp<int>(actorNum, PlayerPropKeys.Energy);
	}
	public int GetPlayerEng(int playerNum, SimGameState state)
	{
		return playerNum == 1 ? state.p1Energy : state.p2Energy;
	}


	public void SetPlayerEng(int actorNum, int newValue)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Log?.Invoke("[Warning] SetTreeHP_MasterOnly called on non-master. Ignored.");
			return;
		}

		PhotonPropertyHelper.SetPlayerProp(actorNum, PlayerPropKeys.Energy, newValue);
	}
	public void SetPlayerEng(int playerNum, int newValue, SimGameState state)
	{
		if (playerNum == 1) state.p1Energy = newValue;
		else state.p2Energy = newValue;
	}


	public float GetBarrierConversionRate(int actorNum)
	{
		return PhotonPropertyHelper.GetPlayerProp<float>(actorNum, PlayerPropKeys.BarrierConversionRate);
	}
	public float GetBarrierConversionRate(int playerNum, SimGameState state)
	{
		return playerNum == 1 ? state.p1VillBarConRate : state.p2VillBarConRate;
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
	public void AddPlayerLockPickCount(int playerNum, SimGameState state)
	{
		if (playerNum == 1) state.p1LockpickCnt++;
		else state.p2LockpickCnt++;
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
	public void AddPlayerLockCount(int playerNum, SimGameState state)
	{
		if (playerNum == 1) state.p1LockCnt++;
		else state.p2LockCnt++;
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
	public void RemovePlayerLockPickCount(int playerNum, SimGameState state)
	{
		if (playerNum == 1) state.p1LockpickCnt = Mathf.Max(state.p1LockpickCnt - 1, 0);
		else state.p2LockpickCnt = Mathf.Max(state.p2LockpickCnt - 1, 0);
	}


	public int GetPlayerLockPickCount(int actorNum)
	{
		return PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.LOCKPICK(actorNum));
	}
	public int GetPlayerLockpickCount(int playerNum, SimGameState state)
	{
		return playerNum == 1 ? state.p1LockpickCnt : state.p2LockpickCnt;
	}

	public void SetHideDmgTrigger(int actorNum)
	{
		PhotonPropertyHelper.SetRoomProp(ItemPropKeys.HIDEDMG(actorNum), true);
	}

}
