using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using JetBrains.Annotations;
public class PlayerManager : MonoBehaviourPunCallbacks
{
    public static PlayerManager Instance { get; private set; }

    private Dictionary<int, RuntimePlayer> players = new();
    public IReadOnlyDictionary<int, RuntimePlayer> Players => players;

	private void Awake()
	{
		if(Instance != null)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
	}

	private void Start()
	{
		StickGameController.Instance.InitSticks();
	}

	private void InitPlayersFromPhoton()
	{
		players.Clear();

		foreach(Player p in PhotonNetwork.PlayerList)
		{
			var rp = new RuntimePlayer();
			rp.actorNumber = p.ActorNumber;

			var ht = p.CustomProperties;

			rp.playerName = ht.TryGetValue("playerName", out var name) ? (string)name : "player"+rp.actorNumber;
			rp.isMyTurn = false;
			rp.turnIdx = -1;

			players.Add(rp.actorNumber, rp);
		}
	}
}
