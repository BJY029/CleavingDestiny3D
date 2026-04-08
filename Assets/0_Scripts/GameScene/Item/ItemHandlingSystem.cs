using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

[Serializable] //Client -> MasterClient
public struct AttackCommand
{
	public int attackerNum;     //요청자 Actor 번호
	public int baseDamage;      //게이지로 계산한 원본 데미지
	public bool isBasicAttack;  //평타인지 아닌지 여부
	public int clientNonce;     //중복 요청 방지
}

[Serializable] //MasterClient -> client
public struct AttackResult
{
	public int attackerNum;     //요청을 보낸 Actor 번호
	public int finalDamage;     //최종 확정 데미지(숨김 상태인 경우 다른 플레이어에겐 -1)
	public float convertedBarrier; //최종 방어력 Rate
	public bool hidden;         //데미지 숨김 상태인지 여부
	public float treeHpAfter;   //UI 반영용 treeHP 결과
}


public class ItemHandlingSystem : MonoBehaviourPunCallbacks
{
	public static ItemHandlingSystem instance;

	private StatusSystem _statusSystem;
	private GameEventBus _gameEventBus;
	public DamageResolver _damageResolver { get; private set; }
	public DeterministicRng _rng { get; private set; }
	//Inventory usage limits
	private Dictionary<int, List<string>> UsedTurnItem;
	private Dictionary<int, List<string>> UsedDayItem;
	private Dictionary<int, List<string>> UsedGameItem;

	private void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(gameObject);

		_statusSystem = new StatusSystem();
		_gameEventBus = new GameEventBus(_statusSystem);
		_damageResolver = new DamageResolver(_gameEventBus, _statusSystem);

		UsedTurnItem = new Dictionary<int, List<string>>();
		UsedDayItem = new Dictionary<int, List<string>>();
		UsedGameItem = new Dictionary<int, List<string>>();

		//Turn 시드 값 설정 및 적용
		//Turn이 변경될 때마다 재설정 된다.
		//재현 가능한 랜덤 시스템,
		InitRandomSystem();
	}

	public void InitRandomSystem()
	{
		int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		Debug.Log($"[InitRandomSystem] Generated Turn Seed: {seed}");
		_rng = new DeterministicRng(seed);
	}

	/// <summary>
	/// Manage and control items that can only be used once in a day in a list
	/// </summary>
	/// <param name="itemId"></param>
	public void AddUsedDayItem(int actorNum, string itemId)
	{
		if (!UsedDayItem.ContainsKey(actorNum))
			UsedDayItem.Add(actorNum, new List<string>());
		UsedDayItem[actorNum].Add(itemId);
	}

	/// <summary>
	/// When the day is reset, it is reset.
	/// </summary>
	public void ClearUsedDayItem()
	{
		foreach (var I in UsedDayItem)
		{
			I.Value.Clear();
		}
	}

	/// <summary>
	/// Manage and control items that can only be used once in a turn in a list
	/// </summary>
	/// <param name="itemId"></param>
	public void AddUsedTurnItem(int actorNum, string itemId)
	{
		if (!UsedTurnItem.ContainsKey(actorNum))
			UsedTurnItem.Add(actorNum, new List<string>());

		UsedTurnItem[actorNum].Add(itemId);
	}

	/// <summary>
	/// When the turn is reset, it is reset.
	/// </summary>
	public void ClearUsedTurnItem()
	{
		foreach (var I in UsedTurnItem)
		{
			I.Value.Clear();
		}
	}

	public void AddUsedGameItem(int actorNum, string itemId)
	{
		if (!UsedGameItem.ContainsKey(actorNum))
			UsedGameItem.Add(actorNum, new List<string>());
		UsedGameItem[actorNum].Add(itemId);
	}

	/// <summary>
	/// Check if the item is available (check turn, wave).
	/// </summary>
	/// <param name="itemId"></param>
	/// <returns></returns>
	public bool CheckItemAvaiable(int actorNum, string itemId)
	{
		// 1. UsedGameItem에 키가 있는지 확인 후 검사
		if (UsedGameItem.ContainsKey(actorNum) && UsedGameItem[actorNum].Contains(itemId))
			return false;

		// 2. UsedDayItem에 키가 있는지 확인 후 검사
		if (UsedDayItem.ContainsKey(actorNum) && UsedDayItem[actorNum].Contains(itemId))
			return false;

		// 3. UsedTurnItem에 키가 있는지 확인 후 검사
		if (UsedTurnItem.ContainsKey(actorNum) && UsedTurnItem[actorNum].Contains(itemId))
			return false;

		return true;
	}

	public int HasLockPick(int actorNum)
	{
		var ctx = new EffectContext(_rng, Debug.Log);

		int lockPickCnt = ctx.GetPlayerLockPickCount(actorNum);
		return lockPickCnt;
	}

	public void UseLockPick(int actorNum)
	{
		photonView.RPC(nameof(RPC_UseLockPick), RpcTarget.MasterClient, actorNum);
	}

	public bool HasDebuff(int actNum)
	{
		foreach (var st in _statusSystem.ALL)
		{
			if (st.spec.tags == TagMask.Negative && st.ownerActorNum == actNum)
			{
				return true;
			}
		}
		return false;
	}

	[PunRPC]
	public void RPC_UseLockPick(int actorNum)
	{
		if (!PhotonNetwork.IsMasterClient) return;
		var ctx = new EffectContext(_rng, Debug.Log);

		ctx.RemovePlayerLockPickCount(actorNum);
	}


	//플레이어가 사용한 아이템을 StatusInstance 객체로 객체화 후, 해당 플레이어의 _statuisSystem 리스트에 삽입한다.
	//후에 턴 변화가 발생할 때, 플레이어의 stasusSystem 내의 아이템들이 적절한 타이밍에 실행된다.
	public void AddItemStatusInstance(int actorNum, ItemSO item, int itemUID)
	{
		//MasterClient만 처리한다.
		if (!PhotonNetwork.IsMasterClient) return;

		if (item.oncePerTurn) AddUsedTurnItem(actorNum, item.itemId);
		if (item.oncePerDay) AddUsedDayItem(actorNum, item.itemId);
		if (item.oncePerGame) AddUsedGameItem(actorNum, item.itemId);

		//If Sacrifice Item
		if (item.itemId == "2002")
		{
			if (!GameHelper.IsCurrentTurnAI())
			{
				ItemSelectionController.instance.SetItemSelectionActive(actorNum, itemUID);
			}
		}
		//If Lockpick Item
		if (item.itemId == "4000")
		{
			var ctx = new EffectContext(_rng, Debug.Log);
			ctx.AddPlayerLockPickCount(actorNum);
			ctx.Log?.Invoke($"[ItemLockPick] Player{actorNum}'s LockPick added");
			return;
		}
		if (item.itemId == "4001")
		{
			var ctx = new EffectContext(_rng, Debug.Log);
			ctx.AddPlayerLockCount(actorNum);
			ctx.Log?.Invoke($"[ItemLockPick] Player{actorNum}'s Lock added");
			return;
		}

		//아이템 적용 대상을 기준으로 분기하여 처리한다.
		switch (item.target)
		{
			//아이템 적용 대상이 자기 자신인 경우
			case ItemTarget.Self:
			case ItemTarget.SelfVillage:
			case ItemTarget.Tree:
				//해당 아이템에 부착된 효과들을 돌면서
				foreach (EffectSpec es in item.effects)
				{
					//AddStatus 외의 다른 아이템 효과로 정의된 아이템일 경우
					if (es.effectType != ItemEffect.AddStatus)
					{
						ItemProcessImm(actorNum, es);

						continue;
					}

					//AddStatus에 정의된 해당 아이템 정보를 가져온다.
					//이는 추후에 다른 아이템을 처리하기 위해 별개의 코드를 넣어야 할 듯 하다.
					StatusSpec ss = es.statusSpce;

					//남은 턴 수를 durationType을 기반으로 초기화하고
					int remainTurns = getRemainTurns(ss);

					//StatusIntance를 생성한 다음
					var st = setAndGetStatusInstance(ss, actorNum, actorNum, remainTurns);

					//플레이어의 상태 관리 시스템 리스트에 삽입
					_statusSystem.Add(st);

					Master_UpdateItemStatusUI(item, st);

					//디버깅
					Debug.Log($"[Item] AddStatus {ss.statusId} to {actorNum}");
				}
				break;

			//아이템 적용 대상이 다른 플레이어인 경우
			case ItemTarget.Opponent:
			case ItemTarget.OpponentVillage:
			case ItemTarget.OpponentTree:
				foreach (EffectSpec es in item.effects)
				{
					Player[] playerNums = PhotonNetwork.PlayerList;
					//AddStatus 외의 다른 아이템 효과로 정의된 아이템일 경우
					if (es.effectType != ItemEffect.AddStatus)
					{
						if (GameManager.Instance.isSoloPlay)
						{
							foreach (int num in PlayerManager.Instance.AIPlayerObj.Keys)
							{
								ItemProcessImm(num, es);
							}
						}
						else
						{
							//나를 제외한 다른 모든 플레이어에게 해당 즉시 적용 아이템 효과를 적용한다.
							foreach (Player player in playerNums)
							{
								if (player.ActorNumber != actorNum)
									ItemProcessImm(player.ActorNumber, es);
							}
						}
						continue;
					}

					//AddStatus에 정의된 해당 아이템 정보를 가져온다.
					StatusSpec ss = es.statusSpce;

					//남은 턴 수를 durationType을 기반으로 초기화하고
					int remainTurns = getRemainTurns(ss);

					//다른 플레이어어정보로 초기화하여 해당 아이템 효과를 삽입한다.
					foreach (Player player in playerNums)
					{
						if (player.ActorNumber != actorNum)
						{
							var opst = setAndGetStatusInstance(ss, player.ActorNumber, actorNum, remainTurns);

							_statusSystem.Add(opst);

							Master_UpdateItemStatusUI(item, opst);
							//디버깅
							Debug.Log($"[Item] AddStatus {ss.statusId} to {player.ActorNumber}");
						}
					}
				}
				break;


			//아이템 적용 대상이 전체인 경우
			case ItemTarget.Global:
				foreach (EffectSpec es in item.effects)
				{
					Player[] playerNums = PhotonNetwork.PlayerList;
					//AddStatus 외의 다른 아이템 효과로 정의된 아이템일 경우
					if (es.effectType != ItemEffect.AddStatus)
					{
						if (GameManager.Instance.isSoloPlay)
						{
							foreach (int num in PlayerManager.Instance.AIPlayerObj.Keys)
							{
								ItemProcessImm(num, es);
							}
						}
						else
						{
							//모두에게 즉시 적용
							foreach (Player player in playerNums)
							{
								ItemProcessImm(player.ActorNumber, es);
							}
						}
						continue;
					}

					//AddStatus에 정의된 해당 아이템 정보를 가져온다.
					StatusSpec ss = es.statusSpce;

					//남은 턴 수를 durationType을 기반으로 초기화하고
					int remainTurns = getRemainTurns(ss);

					//각 플레이어 정보로 초기화하여 아이템효과를 삽입한다.
					foreach (Player player in playerNums)
					{
						var gbst = setAndGetStatusInstance(ss, player.ActorNumber, actorNum, remainTurns);

						_statusSystem.Add(gbst);

						Master_UpdateItemStatusUI(item, gbst);
						//디버깅
						Debug.Log($"[Item] AddStatus {ss.statusId} to {player.ActorNumber}");
					}
				}
				break;
		}
	}

	private void Master_UpdateItemStatusUI(ItemSO item, StatusInstance st)
	{
		StatusSyncHub.instance.Master_BroadcastAdd(new ItemStatusInfo
		{
			itemId = item.itemId,
			statusId = st.spec.statusId,
			ownerActNum = st.ownerActorNum,
			sourceActNum = st.sourceActorNum,
			remainingTurns = (st.spec.durationType == DurationType.Turns) ? -1 : st.remainingTurns,
			type = item.type,
			activateTrigger = st.spec.triggers,
			stackCount = 1
		});
	}

	//희생 아이템 최종 처리 프로세스
	public void ProcessSacrificeItem(int ActNum, int UID)
	{
		photonView.RPC(nameof(RPC_ProcessSacrificeItem), RpcTarget.MasterClient, ActNum, UID);
	}

	[PunRPC]
	public void RPC_ProcessSacrificeItem(int ActNum, int UID)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		Debug.Log("Item Sacrifice Process Activated");
		//플레이어 번호 기준으로 인벤토리 정보 가져오기
		int capacity = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(ActNum));
		string invStr = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(ActNum));

		//인벤토리 슬롯 가져오기
		var invSlots = ItemInfoSerializer.Decode(invStr, capacity);
		if (invSlots == null) return;

		//UID를 기반으로 아이템 정보 가져오기
		ItemSO selectedItem = null;
		for (int i = 0; i < invSlots.Length; i++)
		{
			if (invSlots[i].itemID == null) continue;
			if (UID == invSlots[i].uniqueId)
			{
				selectedItem = ItemDB.Instance.Get(invSlots[i].itemID);
				break;
			}
		}

		if (selectedItem == null) return;

		Debug.Log($"Selected Item ID = {selectedItem.itemId}");

		//Rate 값 가져오기
		float reduceRate = 1f;
		switch (selectedItem.itemClass)
		{
			case ItemClass.Common:
				reduceRate -= PhotonPropertyHelper.GetRoomProp<float>(ItemPropKeys.COMMON_RATE(ActNum));
				break;
			case ItemClass.Hero:
				reduceRate -= PhotonPropertyHelper.GetRoomProp<float>(ItemPropKeys.HERO_RATE(ActNum));
				break;
			case ItemClass.Rare:
				reduceRate -= PhotonPropertyHelper.GetRoomProp<float>(ItemPropKeys.RARE_RATE(ActNum));
				break;
			case ItemClass.Legendary:
				reduceRate -= PhotonPropertyHelper.GetRoomProp<float>(ItemPropKeys.LEGENDARY_RATE(ActNum));
				break;
		}

		//Rate 값 계산 및 반영
		float currentRate = PhotonPropertyHelper.GetPlayerProp<float>(ActNum, PlayerPropKeys.TreeAtkMulti);
		PhotonPropertyHelper.SetPlayerProp(ActNum, PlayerPropKeys.TreeAtkMulti, currentRate * reduceRate);
		Debug.Log($"Player{ActNum}'s Village Attack Mult Rate : {currentRate * reduceRate}");

		//해당 아이템 삭제
		InventoryAuthority.Instance.DeleteItemByUID(ActNum, UID);
	}

	private int getRemainTurns(StatusSpec ss)
	{
		int remainTurns = 9999;
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
			case DurationType.UntilWaveEnd:
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
		return remainTurns;
	}


	//StatusInstance를 생성하고 반환하는 함수
	private StatusInstance setAndGetStatusInstance(StatusSpec ss, int ownerAct, int sourceAct, int remainTurns)
	{
		var st = new StatusInstance
		{
			spec = ss,
			ownerActorNum = ownerAct,
			sourceActorNum = sourceAct,
			remainingTurns = remainTurns
		};
		return st;
	}

	public void OnTurnStart()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		var ctx = new EffectContext(_rng, Debug.Log);
		int turnActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
		_damageResolver.ResolveWhenStartTurn(ctx, turnActor);
		ClearUsedTurnItem();
	}

	public void OnWaveEnd()
	{
		if (!PhotonNetwork.IsMasterClient) return;
		//아이템 사용 길이가 '하루'에 속하는 아이템을 statusSystem에서 삭제하는 함수 실행
		_statusSystem.RemoveAllByDuration(DurationType.UntilWaveEnd);
		ClearUsedDayItem();
	}

	public void OnVillageStart()
	{
		if (!PhotonNetwork.IsMasterClient) return;
		var ctx = new EffectContext(_rng, Debug.Log);
		_damageResolver.ResolveWhenVillageStart(ctx);
	}

	public void InitDay()
	{
		if (!PhotonNetwork.IsMasterClient) return;
		_statusSystem.RemoveRemainingTurns_Zero();
		ClearUsedDayItem();
		ClearUsedTurnItem();
	}
	// public void OnTreeDamage()
	// {
	// 	if (!PhotonNetwork.IsMasterClient) return;
	// 	var ctx = new EffectContext(_rng, Debug.Log);
	// 	_damageResolver.ResolveWhenVillageStart(ctx);
	// }

	//사용 즉시 적용 될 아이템들을 확인하고 해당 아이템을 적용하는 함수
	public void CheckAndActivateImmItem()
	{
		if (!PhotonNetwork.IsMasterClient) return;

	}

	//턴 전환 시 호출될 함수
	public void RequestHit(int baseDamage, bool isBasicAttack, IPlayerAction requester)
	{
		//json 형식으로 공격 커맨드 객체 생성
		var cmd = new AttackCommand
		{
			attackerNum = requester.PlayerActNum,//인터페이스를 통해서 플레이어의 고유 번호 받아온다.
			baseDamage = baseDamage,
			isBasicAttack = isBasicAttack,
			clientNonce = UnityEngine.Random.Range(int.MinValue, int.MaxValue),
		};
		//Debug.Log("커맨드 생성, RPC 전송");
		//Json 형태로 직렬화 해서 MasterClient에게 요청 전송
		photonView.RPC(nameof(RPC_RequestAttack), RpcTarget.MasterClient, JsonUtility.ToJson(cmd));
	}

	//MasterClient에서 처리 수행
	[PunRPC]
	private void RPC_RequestAttack(string json, PhotonMessageInfo info)
	{
		//검증
		if (!PhotonNetwork.IsMasterClient) return;
		//Debug.Log("역직렬화 수행");
		//역직렬화
		var cmd = JsonUtility.FromJson<AttackCommand>(json);

		//요청자와 객체 정보가 같은지 확인(핵 방지)
		if (info.Sender.ActorNumber != cmd.attackerNum)
		{
			if (!GameHelper.IsCurrentTurnAI())//현재 AI 턴 또한 아닌 경우
			{
				Debug.LogError("[ERROR]It is not real Requester");
				return;
			}
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


		//TODO: 게임 종료 검증
		if (MatchResultManager.Instance.TryResolveResultByTreeHP())
		{
			Debug.Log("Match End By Tree HP 0");
			return;
		}

		//계산된 결과를 각 클라이언트에게 브로드캐스트하는 함수 호출
		BroadcastHitResult(cmd.attackerNum, dmg.finalDamage, dmg.convertedToBarrier, dmg.hidden, hp);
	}

	//마스터 클라이언트가 계산 결과를 각 클라이언트에게 브로드캐스트 하는 함수
	private void BroadcastHitResult(int attackNum, int finalDmg, float finalBarrierConverted, bool hidden, float treeHPAfter)
	{
		Debug.Log("Broadcast in");
		Debug.Log("Final Damage : " + finalDmg);

		//플레이어 배열 가져오기
		Player[] playerNums = PhotonNetwork.PlayerList;

		//공격자와 그 외 플레이어 구분
		Player attacker = null;
		List<Player> opposites = new List<Player>();

		foreach (Player player in playerNums)
		{
			if (player.ActorNumber == attackNum)
			{
				attacker = player;
			}
			else
			{
				opposites.Add(player);
			}
		}

		//예외 처리
		if (attacker == null)
		{
			//해당 플레이어가 AI 플레이어인지 확인
			//AI이면 그냥 null 처리
			if (!PlayerManager.Instance.AIPlayerObj.ContainsKey(attackNum))
			{
				Debug.LogError("No Attacker Player Info");
				return;
			}
		}


		//공격자(요청자)에게 전달할 json 객체
		var fullInfo = new AttackResult
		{
			attackerNum = attackNum,
			finalDamage = finalDmg,
			convertedBarrier = finalBarrierConverted,
			hidden = hidden,
			treeHpAfter = treeHPAfter,
		};

		//그외 플레이어들에게 전달할 json 객체
		var maskedInfo = new AttackResult
		{
			attackerNum = attackNum,
			//hidden 여부에 따라서 데미지 정보 공개 혹은 비공개
			finalDamage = hidden ? -1 : finalDmg,
			convertedBarrier = hidden ? -1f : finalBarrierConverted,
			hidden = hidden,
			treeHpAfter = treeHPAfter,
		};

		//직렬화
		string fullInfoJson = JsonUtility.ToJson(fullInfo);
		string maskedInfoJson = JsonUtility.ToJson(maskedInfo);


		//현재 턴이 AI인 경우
		if (GameHelper.IsCurrentTurnAI())
		{
			Debug.Log("AI_OnAttackResult");
			//AI에게 공격 결과 전송
			AI_OnAttackResult(attackNum, fullInfoJson);
			//만약 AI의 상대방(즉, MasterClient)에게 관련 정보를 전달하려면, 추가함수를 구성하도록 설정
		}
		else
		{
			Debug.Log("MOT AI_OnAttackResult");
			//각 요청자와 그외 플레이어들에게 RPC로 결과 전송
			photonView.RPC(nameof(RPC_OnAttackResult), attacker, attacker.ActorNumber, fullInfoJson);
			if (GameManager.Instance.isSoloPlay)    //현재 싱글 플레이하는 중이면
			{
				foreach (var kvp in PlayerManager.Instance.Players)
				{
					//ai 플레이어 찾고
					int actNum = kvp.Value.actorNumber;
					Player p = PhotonNetwork.CurrentRoom.GetPlayer(actNum);

					if (p == null)  //p == ai 플레이어
					{
						//해당 플레이어의 준비 여부 임의로 설정
						PhotonPropertyHelper.SetPlayerProp(actNum, PlayerPropKeys.PDamageProcessCompleted, true);
						TurnManager.Instance.PlayerDamageChecker(attacker.ActorNumber);
					}
				}
			}
			else
			{
				foreach (Player player in opposites)
				{
					photonView.RPC(nameof(RPC_OnAttackResult), player, attacker.ActorNumber, maskedInfoJson);
				}
			}
		}
	}

	[PunRPC]
	private void RPC_OnAttackResult(int attackerNum, string json)
	{
		var res = JsonUtility.FromJson<AttackResult>(json);


		//UI 처리
		if (res.hidden && res.finalDamage < 0)
		{
			//데미지를 가리는 UI 처리 수행
		}
		else
		{
			//기본 처리(데미지 공개)
		}

		if (res.attackerNum == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			float currentTotalDamage = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.TotalDamage);
			float currentBarrier = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageBarrier);

			//데미지 합계 계산
			currentTotalDamage += res.finalDamage;
			//Barrier 값 계산
			currentBarrier = currentBarrier + res.convertedBarrier;
			//변경된 스탯 값 프로퍼티에 업데이트
			PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.TotalDamage, currentTotalDamage);
			Debug.Log($"Player{PhotonNetwork.LocalPlayer.ActorNumber}s TotalDamage : {currentTotalDamage}");
			PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageBarrier, currentBarrier);
			Debug.Log($"Player{PhotonNetwork.LocalPlayer.ActorNumber}s Barrier : {currentBarrier}");
		}
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.PDamageProcessCompleted, true);
		TurnManager.Instance.PlayerDamageChecker(attackerNum);
	}

	private void AI_OnAttackResult(int attackerNum, string json)
	{
		var res = JsonUtility.FromJson<AttackResult>(json);

		float currentTotalDamage = PhotonPropertyHelper.GetPlayerProp<float>(attackerNum, PlayerPropKeys.TotalDamage);
		float currentBarrier = PhotonPropertyHelper.GetPlayerProp<float>(attackerNum, PlayerPropKeys.VillageBarrier);

		//데미지 합계 계산
		currentTotalDamage += res.finalDamage;
		//Barrier 값 계산
		currentBarrier = currentBarrier + res.convertedBarrier;
		//변경된 스탯 값 프로퍼티에 업데이트
		PhotonPropertyHelper.SetPlayerProp(attackerNum, PlayerPropKeys.TotalDamage, currentTotalDamage);
		Debug.Log($"Player{attackerNum}s TotalDamage : {currentTotalDamage}");
		PhotonPropertyHelper.SetPlayerProp(attackerNum, PlayerPropKeys.VillageBarrier, currentBarrier);
		Debug.Log($"Player{attackerNum}s Barrier : {currentBarrier}");

		//AI 플레이어와 다른 플레이어(즉 마스트 클라)의 준비 여부 설정
		PhotonPropertyHelper.SetPlayerProp(attackerNum, PlayerPropKeys.PDamageProcessCompleted, true);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.PDamageProcessCompleted, true);
		TurnManager.Instance.PlayerDamageChecker(attackerNum);
	}

	//즉시 적용되는 아이템 실행 함수
	private void ItemProcessImm(int actorNum, EffectSpec es)
	{
		if (!PhotonNetwork.IsMasterClient) return;
		//EffectContext 발행
		var ctx = new EffectContext(_rng, Debug.Log);

		//ItemEffect 타입 기준 구분
		switch (es.effectType)
		{
			//나무 체력에 추가 HP를 +/- 하는 아이템이라면
			case ItemEffect.DeltaTreeUp:
				float val = es.floatValue1;
				if (Mathf.Approximately(val, 0f))
				{
					ctx.Log?.Invoke("Item Heal Value null Exception");
					return;
				}
				val += ctx.GetTreeHP();
				ctx.SetTreeHP(val);

				ctx.Log?.Invoke($"[ItemProcessImm] TreeHP Changed to {val}");
				break;
			//마을 체력에 추가 HP를 +/- 하는 아이템이라면
			case ItemEffect.DeltaVillageHp:
				float delta = es.floatValue1;
				if (Mathf.Approximately(delta, 0f))
				{
					ctx.Log?.Invoke("Item Village Heal Value null Exception");
					return;
				}
				float cur = ctx.GetPlayerVillageHP(actorNum);
				//아래 코드는 해당 아이템을 사용하면 게임을 바로 지게 되는 요인을 막기 위한 플레이어를 위한 장치
				//도입 여부는 아직 모름
				//if (delta < 0f && cur + delta <= 0f)
				//{
				//	ctx.Log?.Invoke("[ItemProcessImm] Not enough VillageHP for donation");
				//	return;
				//}
				float next = delta + cur;
				ctx.SetPlayerVIllageHP(actorNum, next);

				ctx.Log?.Invoke($"[ItemProcessImm] VillageHP Changed to {next}");
				break;
			//마을 쉴드량에 추가 쉴드를 +/- 하는 아이템이라면
			case ItemEffect.DeltaVillageShield:
				float shield = es.floatValue1;
				if (Mathf.Approximately(shield, 0f))
				{
					ctx.Log?.Invoke("Item Shield Value null Exception");
					return;
				}
				shield += ctx.GetPlayerVillageShield(actorNum);

				ctx.SetPlayerVIllageShield(actorNum, shield);

				ctx.Log?.Invoke($"[ItemProcessImm] Player{actorNum}'s VillageShield Changed to {shield}");
				break;
			//마을 쉴드량에 특정 값(비율)을 곱하는 아이템이라면
			case ItemEffect.MultVillageShield:
				float mult = es.floatValue1;
				if (Mathf.Approximately(mult, 0f))
				{
					ctx.Log?.Invoke("Item Shield Value null Exception");
					return;
				}
				float nextShield = ctx.GetPlayerVillageShield(actorNum) * mult;

				ctx.SetPlayerVIllageShield(actorNum, nextShield);

				ctx.Log?.Invoke($"[ItemProcessImm] Player{actorNum}'s VillageShield Changed to {nextShield}");
				break;
			//플레이어 기력에 추가 기력을 +/- 하는 아이템이라면
			case ItemEffect.DeltaPlayerEng:
				int eng = es.intValue1;
				if (eng == 0)
				{
					ctx.Log?.Invoke("Item Charge Value null Exception");
					return;
				}

				eng += ctx.GetPlayerEng(actorNum);

				ctx.SetPlayerEng(actorNum, eng);

				ctx.Log?.Invoke($"[ItemProcessImm] Player{actorNum}'s Energy Changed to {eng}");
				break;

			case ItemEffect.TransferOpponentShieldPct:
				int targetActNum = getRandomActNum_ExceptMe(actorNum);
				if (targetActNum == -1)
				{
					Debug.LogError("There is only me in this Game...");
					return;
				}
				float VillageShieldPct = es.floatValue1;

				float targetShieldValue = ctx.GetPlayerVillageShield(targetActNum);
				float myShieldValue = ctx.GetPlayerVillageShield(actorNum);
				float deltaValue = targetShieldValue * VillageShieldPct;

				ctx.SetPlayerVIllageShield(actorNum, myShieldValue + deltaValue);
				ctx.SetPlayerVIllageShield(targetActNum, Mathf.Max(0f, targetShieldValue - deltaValue));

				ctx.Log?.Invoke($"[ItemProcessImm] Player{actorNum}'s VillageShield Changed from {myShieldValue} to {myShieldValue + deltaValue}");
				ctx.Log?.Invoke($"[ItemProcessImm] Player{targetActNum}'s VillageShield Changed from {targetShieldValue} to {targetShieldValue - deltaValue}");
				break;
		}
	}

	private int getRandomActNum_ExceptMe(int myActNum)
	{
		var playerList = PhotonNetwork.PlayerList;

		List<int> candidates = new List<int>();

		foreach (var player in playerList)
		{
			if (player.ActorNumber != myActNum)
			{
				candidates.Add(player.ActorNumber);
			}
		}

		if (candidates.Count == 0)
		{
			if (GameManager.Instance.isSoloPlay)
			{
				foreach (int num in PlayerManager.Instance.AIPlayerObj.Keys)
				{
					candidates.Add(num);
				}
			}
			else return -1;
		}

		int randomIndex = _rng.Range(0, candidates.Count);
		return candidates[randomIndex];
	}

	private bool IsMyTurnCheckInMaster(int attackerNum)
	{
		int cur = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
		return cur == attackerNum;
	}
}
