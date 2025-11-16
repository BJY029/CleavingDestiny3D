using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using JetBrains.Annotations;
public class PlayerManager : MonoBehaviourPunCallbacks
{
	//전역 접근
    public static PlayerManager Instance { get; private set; }

	//플레이어 정보 관리용 딕셔너리(각 클라이언트마다 관리한다.)
    private Dictionary<int, RuntimePlayer> players = new();
	//읽기 전용 딕셔너리
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
		//게임 시작 시, 미니게임 시작
		StickGameController.Instance.InitSticks();
	}

	//미니게임으로부터 turn 순서가 정해지면, 해당 정보 기반으로 플레이어 정보 채워넣을 예정
	private void InitPlayersFromPhoton()
	{
		players.Clear();

		//각 플레이어에 대해서
		foreach(Player p in PhotonNetwork.PlayerList)
		{
			//정보 삽입
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
