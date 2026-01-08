using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable] //Client -> MasterClient
public struct AttackCommand
{
	public int attackerNum;		//요청자 Actor 번호
	public int baseDamage;		//게이지로 계산한 원본 데미지
	public bool isBasicAttack;	//평타인지 아닌지 여부
	public int clientNonce;		//중복 요청 방지
}

[Serializable] //MasterClient -> client
public struct AttackResult
{
	public int attackerNum;		//요청을 보낸 Actor 번호
	public int finalDamage;		//최종 확정 데미지(숨김 상태인 경우 다른 플레이어에겐 -1)
	public bool hidden;			//데미지 숨김 상태인지 여부
	public float treeHpAfter;	//UI 반영용 treeHP 결과
}

public class ItemHandlingSystem : MonoBehaviourPunCallbacks
{
	public static ItemHandlingSystem instance;

	private StatusSystem _statusSystem;
	private GameEventBus _gameEventBus;
	private DamageResolver _damageResolver;
	private DeterministicRng _rng;

	private void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(gameObject);

		_statusSystem = new StatusSystem();
		_gameEventBus = new GameEventBus(_statusSystem);
		_damageResolver = new DamageResolver(_gameEventBus, _statusSystem);

		//Wave 시드 값
		//현재는 임시로 설정
		//해당 값은 wave 변경 될 때마다 Masterclient가 변경 및 처리 수행
		_rng = new DeterministicRng(12345);
	}

	//플레이어가 사용한 아이템을 StatusInstance 객체로 객체화 후, 해당 플레이어의 _statuisSystem 리스트에 삽입한다.
	//후에 턴 변화가 발생할 때, 플레이어의 stasusSystem 내의 아이템들이 적절한 타이밍에 실행된다.
	public void AddItemStatusInstance(int actorNum, ItemSO item)
	{
		//MasterClient만 처리한다.
		if (!PhotonNetwork.IsMasterClient) return;

		//해당 아이템에 부착된 효과들을 돌면서
		foreach (EffectSpec es in item.effects)
		{
			//AddStatus에 정의된 해당 아이템 정보를 가져온다.
			//이는 추후에 다른 아이템을 처리하기 위해 별개의 코드를 넣어야 할 듯 하다.
			StatusSpec ss = es.statusSpce;

			//남은 턴 수를 기본값으로 초기화하고
			int remainTurns = 9999;
			//각 타입에 따라서 남은 턴 수를 계산한다.
			switch (ss.durationType)
			{
				//이번 턴에만 사용되는 아이템인 경우
				case DurationType.ThisTurn:
					remainTurns = 1;
					break;
				//다음 턴까지 사용되는 아이템일 경우
				case DurationType.NextTurn:
					remainTurns = 2;
					break;
				//N번의 Turn 동안 활성화되는 아이템일 경우
				case DurationType.Turns:
					remainTurns = ss.durationTurns;
					break;
				//이번 일자 동안 활성화 되는 아이템일 경우
				case DurationType.UnitlWaveEnd:
					//현재 wave 값
					int currentWave = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
					//최대 wave 값
					int MaxWaveCnt = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.MaxWaveCnt);
					//플레이어 수(Turn 수)
					int PlayerCnt = PhotonNetwork.PlayerList.Length;
					//현재 턴 인덱스
					int currentTurnIdx = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurn);
					//남은 턴 계산
						//ex) 2번째 wave의 첫번째 턴인 경우 
						//remainTurns = (3 - 1 - 1) * 2 + (2 - 0) = 4
					remainTurns = (MaxWaveCnt - currentWave - 1) * PlayerCnt + (PlayerCnt - currentTurnIdx);
					break;
			}

			//상태 객체 선언
			var st = new StatusInstance
			{
				spec = ss,
				ownerActorNum = actorNum,
				sourceActorNum = actorNum,
				remainingTurns = remainTurns
			};

			//플레이어의 상태 관리 시스템 리스트에 삽입
			_statusSystem.Add(st);

			//디버깅
			Debug.Log($"[Item] AddStatus {ss.statusId} to {actorNum}");
		}
	}

	//턴 전환 시 호출될 함수
	public void RequestHit(int baseDamage, bool isBasicAttack)
	{
		//json 형식으로 공격 커맨드 객체 생성
		var cmd = new AttackCommand
		{
			attackerNum = PhotonNetwork.LocalPlayer.ActorNumber,
			baseDamage = baseDamage,
			isBasicAttack = isBasicAttack,
			clientNonce = UnityEngine.Random.Range(int.MinValue, int.MaxValue),
		};

		//Json 형태로 직렬화 해서 MasterClient에게 요청 전송
		photonView.RPC(nameof(RPC_RequestAttack), RpcTarget.MasterClient, JsonUtility.ToJson(cmd));
	}
	
	//MasterClient에서 처리 수행
	[PunRPC]
	private void RPC_RequestAttack(string json, PhotonMessageInfo info)
	{
		//검증
		if (!PhotonNetwork.IsMasterClient) return;

		//역직렬화
		var cmd = JsonUtility.FromJson<AttackCommand>(json);

		//요청자와 객체 정보가 같은지 확인(핵 방지)
		if (info.Sender.ActorNumber != cmd.attackerNum)
		{
			Debug.LogError("[ERROR]It is not real Requester");
			return;
		}
		//턴 검증 2
		if (!IsMyTurnCheckInMaster(cmd.attackerNum))
		{
			Debug.LogError("[ERROR]It is not Requester Turn");
			return;
		}

		//컨텍스트 생성
		var ctx = new EffectContext(_rng, Debug.Log);

		//데미지 객체 생성
		var dmg = new DamagePacket
		{
			attackerNum = cmd.attackerNum,
			isBasicAttack = cmd.isBasicAttack,
			baseDamage = cmd.baseDamage
		};

		//최종 데미지 계산(아이템도 함께 반영하여 계산)
		_damageResolver.Resolve(dmg, ctx);

		//각 아이템의 남은 턴 수 계산, 남은 턴수가 모두 지나면 해당 아이템을 _statusSystem 리스트에서 삭제한다.
		_statusSystem.TickTurnEnd(cmd.attackerNum);

		//나무 데미지 업데이트
		float hp = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeHP);
		hp -= dmg.finalDamage;
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, hp);

		//계산된 결과를 각 클라이언트에게 브로드캐스트하는 함수 호출
		BroadcastHitResult(cmd.attackerNum, dmg.finalDamage, dmg.hidden, hp);
	}

	//마스터 클라이언트가 계산 결과를 각 클라이언트에게 브로드캐스트 하는 함수
	private void BroadcastHitResult(int attackNum, int finalDmg, bool hidden, float treeHPAfter)
	{
		Debug.Log("Final Damage : " + finalDmg);

		//플레이어 배열 가져오기
		Player[] playerNums = PhotonNetwork.PlayerList;

		//공격자와 그 외 플레이어 구분
		Player attacker = null;
		List<Player> opposites = new List<Player>();

		foreach(Player player in playerNums )
		{
			if(player.ActorNumber == attackNum)
			{
				attacker = player;
			}
			else
			{
				opposites.Add(player);
			}
		}

		//예외 처리
		if(attacker == null)
		{
			Debug.LogError("No Attacker Player Info");
			return;
		}

		//공격자(요청자)에게 전달할 json 객체
		var fullInfo = new AttackResult
		{
			attackerNum = attackNum,
			finalDamage = finalDmg,
			hidden = hidden,
			treeHpAfter = treeHPAfter,
		};

		//그외 플레이어들에게 전달할 json 객체
		var maskedInfo = new AttackResult
		{
			attackerNum = attackNum,
			//hidden 여부에 따라서 데미지 정보 공개 혹은 비공개
			finalDamage = hidden ? -1 : finalDmg,
			hidden = hidden,
			treeHpAfter = treeHPAfter,
		};

		//직렬화
		string fullInfoJson = JsonUtility.ToJson(fullInfo);
		string maskedInfoJson = JsonUtility.ToJson(maskedInfo);
		//각 요청자와 그외 플레이어들에게 RPC로 결과 전송
		photonView.RPC(nameof(RPC_OnAttackResult), attacker, fullInfoJson);
		foreach(Player player in opposites)
		{
			photonView.RPC(nameof(RPC_OnAttackResult), player, maskedInfoJson);
		}
	}

	[PunRPC]
	private void RPC_OnAttackResult(string json)
	{
		var res = JsonUtility.FromJson<AttackResult>(json);


		//UI 처리
		if(res.hidden && res.finalDamage < 0)
		{
			//데미지를 가리는 UI 처리 수행
		}
		else
		{
			//기본 처리(데미지 공개)
		}

		if(res.attackerNum == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			float currentTotalDamage = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage);
			float currentConBarrier = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.BarrierConversionRate);
			float currentBarrier;

			//데미지 합계 계산
			currentTotalDamage += res.finalDamage;
			//Barrier 값 계산
			currentBarrier = currentTotalDamage * (1 + currentConBarrier);
			//변경된 스탯 값 프로퍼티에 업데이트
			PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage, currentTotalDamage);
			PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier, currentBarrier);
		}
	}

	private bool IsMyTurnCheckInMaster(int attackerNum)
	{
		int cur = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
		return cur == attackerNum;
	}
}
