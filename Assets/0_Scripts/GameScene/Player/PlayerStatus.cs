using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
	public static PlayerStatus Instance;
	private readonly Hashtable _turnInitPropCache = new Hashtable();
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public readonly Dictionary<int, Transform> playerVillageVFXBases = new();
	[SerializeField] private Transform[] VillageVFXBase;

	//플레이어의 인벤토리
	private GameObject myInventory;
	private GameObject AIInventory;
	// 플레이어 프로퍼티 변수
	private int currentGold;
	private int currentDayGoldIncome;
	private int currentEnergy;
	private int currentEnergyIncome;
	private int currentMaxEnergy;
	private int currentCarryOverEnergy;
	private int currentMaxAtkDamage;
	private int currentMinAtkDamage;
	private float currentVillageHP;
	private float maxVillageHp;
	private float currentTotalDamage;
	private float currentBarrier;
	private float currentBarrierArmor;
	private float currentConBarrier;
	private float currentTreeDmgMulit;

	private bool IsSinglePlayer => !PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom || PhotonNetwork.OfflineMode;

	public float GetMaxVillageHp() { return PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MaxVillageHP); }
	public float GetCurrentVillageHP() { return PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageHP); }
	public float GetCurrentBarrier() { return GetCurrentConvertedBarrier() + GetCurrentBarrierArmor(); }
	public float GetCurrentConvertedBarrier() { return PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageBarrier); }
	public float GetCurrentBarrierArmor() { return PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.BarrierArmor); }

	public void SetPlayerInventory(GameObject inv)
	{
		if (inv == null) return;
		myInventory = inv;
	}

	public void SetAIInventory(GameObject inv)
	{
		if (inv == null) return;
		AIInventory = inv;
	}

	public void ApplyVillageVFXBase()
	{
		int[] turnList = PhotonPropertyHelper.GetRoomProp<int[]>("TurnInfo");
		for (int i = 0; i < turnList.Length; i++)
		{
			playerVillageVFXBases[turnList[i]] = VillageVFXBase[i];
		}
	}

	public GameObject GetPlayerInventory()
	{
		return myInventory;
	}

	public GameObject GetAIInventory()
	{
		return AIInventory;
	}

	// 플레이어 프로퍼티 값 불러오기
	public void GetCurrentPlayerStatus()
	{
		ApplyCurrentPlayerStatus(PhotonNetwork.LocalPlayer.CustomProperties);
	}

	public void GetCurrentPlayerStatus(int aiNumber)
	{
		ApplyCurrentPlayerStatus(PhotonNetwork.CurrentRoom.CustomProperties, $"_{aiNumber}");
	}

	private void ApplyCurrentPlayerStatus(Hashtable props, string attachedKey = "")
	{
		currentGold = PhotonPropertyHelper.GetHashtableValue(props, PlayerPropKeys.Gold + attachedKey, 0);
		currentDayGoldIncome = PhotonPropertyHelper.GetHashtableValue(props, PlayerPropKeys.DayGoldIncome + attachedKey, 0);
		currentCarryOverEnergy = PhotonPropertyHelper.GetHashtableValue<int>(props, PlayerPropKeys.CarryOverEnergy + attachedKey);
		currentEnergy = PhotonPropertyHelper.GetHashtableValue<int>(props, PlayerPropKeys.Energy + attachedKey);
		currentEnergyIncome = PhotonPropertyHelper.GetHashtableValue(props, PlayerPropKeys.EnergyIncome + attachedKey, 0);
		currentMaxEnergy = PhotonPropertyHelper.GetHashtableValue<int>(props, PlayerPropKeys.MaxEnergy + attachedKey);
		currentMaxAtkDamage = PhotonPropertyHelper.GetHashtableValue<int>(props, PlayerPropKeys.MaxAtkPow + attachedKey);
		currentMinAtkDamage = PhotonPropertyHelper.GetHashtableValue<int>(props, PlayerPropKeys.MinAtkPow + attachedKey);

		currentVillageHP = PhotonPropertyHelper.GetHashtableValue<float>(props, PlayerPropKeys.VillageHP + attachedKey);
		currentBarrier = PhotonPropertyHelper.GetHashtableValue<float>(props, PlayerPropKeys.VillageBarrier + attachedKey);
		currentBarrierArmor = PhotonPropertyHelper.GetHashtableValue<float>(props, PlayerPropKeys.BarrierArmor + attachedKey);
		currentConBarrier = PhotonPropertyHelper.GetHashtableValue<float>(props, PlayerPropKeys.BarrierConversionRate + attachedKey);
		currentTotalDamage = PhotonPropertyHelper.GetHashtableValue<float>(props, PlayerPropKeys.TotalDamage + attachedKey);

		currentTreeDmgMulit = PhotonPropertyHelper.GetHashtableValue<float>(props, PlayerPropKeys.TreeAtkMulti + attachedKey);

		maxVillageHp = PhotonPropertyHelper.GetHashtableValue<float>(props, PlayerPropKeys.MaxVillageHP + attachedKey);
	}



	private float GetCurrentTotalDefense()
	{
		// 낮 데미지 전환 배리어 + 마을 업그레이드 기본 방어력
		return currentBarrier + currentBarrierArmor;
	}

	/// <summary>
	/// 현재 플레이어 프로퍼티를 기준으로 실제 적용될 마을 피해량을 예측한다.
	/// (트리 배율 적용 후 총 방어력 차감, 최소 0 보정)
	/// </summary>
	/// <param name="incomingDamage">트리의 원본 공격력</param>
	/// <returns>최종 예상 피해량</returns>
	public float GetExpectedVillageDamage(float incomingDamage)
	{
		GetCurrentPlayerStatus();
		return GetExpectedVillageDamageInternal(incomingDamage);
	}

	public float GetExpetedTreePoison(float damage)
	{
		GetCurrentPlayerStatus();
		return damage * currentTreeDmgMulit;
	}

	//incomingDamage는 나무의 원본 독성 데미지 
	private float GetExpectedVillageDamageInternal(float incomingDamage)
	{
		// 실제 마을 피해식과 동일한 계산 순서
		float adjustedDamage = incomingDamage * currentTreeDmgMulit;
		adjustedDamage -= GetCurrentTotalDefense();
		return Mathf.Max(0f, adjustedDamage);
	}

	// UI 업데이트
	public void SetPlayerStatusUIVFX()
	{
		int prvEng = currentEnergy;
		GetCurrentPlayerStatus();
		PlayerCanvasController.Instance.updatePlayerStatus(
			currentEnergy.ToString(), currentVillageHP.ToString(), currentTotalDamage.ToString(), GetCurrentTotalDefense().ToString(), currentTreeDmgMulit.ToString());

		photonView.RPC(nameof(RPC_SetVillageShieldVFX), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, currentBarrier + currentBarrierArmor);

		if (GameStarter.instance.CurrentPhase == GameStartPhase.MainGame)
			photonView.RPC(nameof(RPC_SetEnergyVFX), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, currentEnergy - prvEng);
	}

	[PunRPC]
	public void RPC_SetVillageShieldVFX(int actorNum, float barrier)
	{
		if (!playerVillageVFXBases.TryGetValue(actorNum, out Transform VFXbase))
		{
			Debug.LogError($"[PlayerStatus] There is no VillageVFXBase. ActorNumber={actorNum}");
			return;
		}

		if (barrier <= 0f)
		{
			GameVFXManager.Instance.StopPersistent("VillageShield", actorNum);
			return;
		}

		GameVFXManager.Instance.PlayOrUpdatePersistent("VillageShield", actorNum, VFXbase, 1f, barrier);
	}

	[PunRPC]
	public void RPC_SetEnergyVFX(int actorNum, int value)
	{
		if (!PlayerObjectRegistry.TryGet(actorNum, out PlayerController pc))
		{
			Debug.LogError($"[PlayerStatus] Can't find PlayerController. ActorNumber={actorNum}");
			return;
		}

		if (value <= 0)
		{
			return;
		}

		GameVFXManager.Instance.Play("EnergyUp", pc.EffectPoints.Foot);
	}

	// 턴 전환 시 플레이어 상태 초기화
	public void initPlayerStatus()
	{
		// RPC를 통해 모든 클라이언트에서 실행
		photonView.RPC(nameof(RPC_initPlayerStatus), RpcTarget.All);
	}

	[PunRPC]
	public void RPC_initPlayerStatus()
	{
		ApplyInitStatus(PhotonNetwork.LocalPlayer.ActorNumber, true);

		SetPlayerStatusUIVFX();

		if (PhotonNetwork.IsMasterClient && IsSinglePlayer)
		{
			foreach (int aiNum in PlayerManager.Instance.AIPlayerObj.Keys)
			{
				ApplyInitStatus(aiNum, false);
			}
		}
	}

	public void InitAIStatus(int actNum)
	{
		ApplyInitStatus(actNum, false);
	}

	private void ApplyInitStatus(int actorNumber, bool isLocalPlayer)
	{
		if (isLocalPlayer)
		{
			GetCurrentPlayerStatus();
		}
		else
		{
			GetCurrentPlayerStatus(actorNumber);
		}

		var playerSet = GameManager.Instance.playerDefaultSetting;
		string attachedKey = isLocalPlayer ? string.Empty : $"_{actorNumber}";
		string actorLabel = isLocalPlayer ? "PLAYER" : "AI";
		int maxEnergy = currentMaxEnergy > 0 ? currentMaxEnergy : playerSet.maxEnergy;
		int energyIncome = currentEnergyIncome > 0 ? currentEnergyIncome : playerSet.energyIncomePerDay;
		int dayGoldIncome = currentDayGoldIncome > 0 ? currentDayGoldIncome : playerSet.initialDayGoldIncome;

		Debug.Log($"Current {actorLabel} Village HP : {currentVillageHP}");

		currentGold += dayGoldIncome;
		currentEnergy = Mathf.Min(energyIncome + currentCarryOverEnergy, maxEnergy);
		Debug.LogWarning($"CurrentEng : {energyIncome}(energyIncome) + {currentCarryOverEnergy}(currentCarryOverEnergy) or {maxEnergy}(maxEnergy)");
		currentTotalDamage = 0;
		currentBarrier = 0f;

		CacheInitProps(_turnInitPropCache, attachedKey, playerSet.carryOverEnergy);

		// 골드랑 에너지 인컴 로그
		Debug.Log($"{actorLabel} receives DayGoldIncome : <color=green>{dayGoldIncome}</color>, EnergyIncome : <color=green>{energyIncome}</color>, CarryOverEnergy : <color=green>{playerSet.carryOverEnergy}</color>");

		if (isLocalPlayer)
		{
			PhotonNetwork.LocalPlayer.SetCustomProperties(_turnInitPropCache);
		}
		else
		{
			PhotonNetwork.CurrentRoom.SetCustomProperties(_turnInitPropCache);
		}

		Debug.LogWarning($"{actorLabel}// Gold : {currentGold}, Energe : {currentEnergy}, TotalDamage : {currentTotalDamage}, Barrier : {currentBarrier}");
	}

	private void CacheInitProps(Hashtable props, string attachedKey, int carryOverEnergy)
	{
		props.Clear();
		props[PlayerPropKeys.Gold + attachedKey] = currentGold;
		props[PlayerPropKeys.Energy + attachedKey] = currentEnergy;
		props[PlayerPropKeys.CarryOverEnergy + attachedKey] = /*carryOverEnergy*/0;
		props[PlayerPropKeys.TotalDamage + attachedKey] = currentTotalDamage;
		props[PlayerPropKeys.VillageBarrier + attachedKey] = currentBarrier;
	}

	// 플레이어 프로퍼티가 변경될 때 UI에 반영
	public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
	{
		if (target != PhotonNetwork.LocalPlayer) return;

		if (changedProps.ContainsKey(PlayerPropKeys.Energy) ||
			changedProps.ContainsKey(PlayerPropKeys.VillageHP) ||
			changedProps.ContainsKey(PlayerPropKeys.TotalDamage) ||
			changedProps.ContainsKey(PlayerPropKeys.VillageBarrier) ||
			changedProps.ContainsKey(PlayerPropKeys.BarrierArmor) ||
			changedProps.ContainsKey(PlayerPropKeys.TreeAtkMulti))
		{
			SetPlayerStatusUIVFX();
		}
	}

	// 플레이어 HIT 발생 시 호출될 함수
	// 데미지 비율(damageRatio)을 인자로 받음
	public int HitAction(float damageRatio)
	{
		GetCurrentPlayerStatus();

		// Hit 데미지 계산
		int HitDamage = currentMinAtkDamage + Mathf.RoundToInt((currentMaxAtkDamage - currentMinAtkDamage) * (damageRatio / 100));
		// 누적 데미지 합산
		currentTotalDamage += HitDamage;
		// 낮에 가한 누적 데미지를 전환 비율만큼 배리어로 환산
		currentBarrier = currentTotalDamage * currentConBarrier;

		// 변경된 값을 네트워크 프로퍼티에 업데이트
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.TotalDamage, currentTotalDamage);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageBarrier, currentBarrier);

		// 계산된 데미지 반환
		return HitDamage;
	}


	// 마을이 데미지를 입었을 때 처리 함수
	public void DamagedVillage(float damage)
	{
		GetCurrentPlayerStatus();
		Debug.Log($"Original Tree Damage : {damage}");
		Debug.Log($"Multiplied value : {currentTreeDmgMulit}");
		float adjustedDamage = damage * currentTreeDmgMulit;
		float blockedDamage = Mathf.Min(adjustedDamage, GetCurrentTotalDefense());
		damage = Mathf.Max(0f, adjustedDamage - blockedDamage);
		float hpBefore = currentVillageHP;

		Debug.Log("Final Tree Damage : " + damage);

		// 남은 데미지를 마을 체력에서 차감
		currentVillageHP -= damage;

		// 게임 오버 확인
		if (currentVillageHP <= 0)
		{
			currentVillageHP = 0;
			// 패배 또는 게임 종료 로직
			Debug.Log("Game End By VillageHP 0");
		}
		BattleLogController.AddVillageAttackLog(adjustedDamage, blockedDamage, hpBefore - currentVillageHP);

		Debug.Log($"Final Village HP : {currentVillageHP}");
		// 프로퍼티 업데이트
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageHP, currentVillageHP);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VDamageProcessCompleted, true);

		// 누적 받은 데미지 저장
		float cumulativeReceived = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.CumulativeDamageReceived, 0f);
		cumulativeReceived += damage;
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.CumulativeDamageReceived, cumulativeReceived);
		InitTreeAtkMultRate();

		//데미지 처리가 끝났다고 MasterClient에게 송신
		TurnManager.Instance.TreeDamageChecker();
	}

	public void DamagedVillage(float damage, int aiNumber)
	{
		GetCurrentPlayerStatus(aiNumber);

		damage = GetExpectedVillageDamageInternal(damage);

		currentVillageHP -= damage;

		if (currentVillageHP <= 0)
		{
			currentVillageHP = 0;
		}

		PhotonPropertyHelper.SetPlayerProp(aiNumber, PlayerPropKeys.VillageHP, currentVillageHP);
		PhotonPropertyHelper.SetPlayerProp(aiNumber, PlayerPropKeys.VDamageProcessCompleted, true);

		// 누적 받은 데미지 저장 (AI)
		float cumulativeReceived = PhotonPropertyHelper.GetPlayerProp<float>(aiNumber, PlayerPropKeys.CumulativeDamageReceived, 0f);
		cumulativeReceived += damage;
		PhotonPropertyHelper.SetPlayerProp(aiNumber, PlayerPropKeys.CumulativeDamageReceived, cumulativeReceived);

		TurnManager.Instance.TreeDamageChecker();
	}

	public void InitTreeAtkMultRate()
	{
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.TreeAtkMulti, 1f);
	}

	public void InitTreeAtkMultRate(int aiNumber)
	{
		PhotonPropertyHelper.SetPlayerProp(aiNumber, PlayerPropKeys.TreeAtkMulti, 1f);
	}
}
