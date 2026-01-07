using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;

/// <summary>
/// 상태 이상/전투 계산 중 공유되는 컨텍스트
/// - 상태 이상이 TreeHP/RoomProps 등을 변경하고 싶을 때 접근
/// </summary>
public class EffectContext
{
    public DeterministicRng Rng { get; private set; }

    public Action<string> Log {  get; private set; }

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
}
