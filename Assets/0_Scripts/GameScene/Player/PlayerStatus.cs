using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
	public static PlayerStatus Instance;
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	//플레이어의 인벤토리
	private GameObject myInventory;
	private GameObject AIInventory;
	// 플레이어 프로퍼티 변수
	private int currentEnergy;
	private int currentMaxEnergy;
	private int currentCarryOverEnergy;
	private int currentMaxAtkDamage;
	private int currentMinAtkDamage;
	private float currentVillageHP;
	private float maxVillageHp;
	private float currentTotalDamage;
	private float currentBarrier;
	private float currentConBarrier;
	private float currentTreeDmgMulit;

	private bool IsSinglePlayer => !PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom || PhotonNetwork.OfflineMode;

	public float GetMaxVillageHp() { return PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MaxVillageHP); }
	public float GetCurrentVillageHP() { return PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageHP); }
	public float GetCurrentBarrier() { return PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageBarrier); }

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
		var props = PhotonNetwork.LocalPlayer.CustomProperties;

		currentCarryOverEnergy = GetValue<int>(props, PlayerPropKeys.CarryOverEnergy);
		currentEnergy = GetValue<int>(props, PlayerPropKeys.Energy);
		currentMaxEnergy = GetValue<int>(props, PlayerPropKeys.MaxEnergy);
		currentMaxAtkDamage = GetValue<int>(props, PlayerPropKeys.MaxAtkPow);
		currentMinAtkDamage = GetValue<int>(props, PlayerPropKeys.MinAtkPow);

		currentVillageHP = GetValue<float>(props, PlayerPropKeys.VillageHP);
		currentBarrier = GetValue<float>(props, PlayerPropKeys.VillageBarrier);
		currentConBarrier = GetValue<float>(props, PlayerPropKeys.BarrierConversionRate);
		currentTotalDamage = GetValue<float>(props, PlayerPropKeys.TotalDamage);

		currentTreeDmgMulit = GetValue<float>(props, PlayerPropKeys.TreeAtkMulti);

		maxVillageHp = GetValue<float>(props, PlayerPropKeys.MaxVillageHP);
	}

	public void GetCurrentPlayerStatus(int aiNumber)
	{
		var props = PhotonNetwork.CurrentRoom.CustomProperties;

		string attachedKey = $"_{aiNumber}";

		currentCarryOverEnergy = GetValue<int>(props, PlayerPropKeys.CarryOverEnergy + attachedKey);
		currentEnergy = GetValue<int>(props, PlayerPropKeys.Energy + attachedKey);
		currentMaxEnergy = GetValue<int>(props, PlayerPropKeys.MaxEnergy + attachedKey);
		currentMaxAtkDamage = GetValue<int>(props, PlayerPropKeys.MaxAtkPow + attachedKey);
		currentMinAtkDamage = GetValue<int>(props, PlayerPropKeys.MinAtkPow + attachedKey);

		currentVillageHP = GetValue<float>(props, PlayerPropKeys.VillageHP + attachedKey);
		currentBarrier = GetValue<float>(props, PlayerPropKeys.VillageBarrier + attachedKey);
		currentConBarrier = GetValue<float>(props, PlayerPropKeys.BarrierConversionRate + attachedKey);
		currentTotalDamage = GetValue<float>(props, PlayerPropKeys.TotalDamage + attachedKey);

		currentTreeDmgMulit = GetValue<float>(props, PlayerPropKeys.TreeAtkMulti + attachedKey);

		maxVillageHp = GetValue<float>(props, PlayerPropKeys.MaxVillageHP + attachedKey);
	}

	// 안전하게 값을 꺼내는 유틸리티 함수
	private T GetValue<T>(ExitGames.Client.Photon.Hashtable props, string key)
	{
		if (props.TryGetValue(key, out object value))
		{
			return (T)value;
		}
		return default(T);
	}

	// UI 업데이트
	public void SetPlayerStatusUI()
	{
		GetCurrentPlayerStatus();
		PlayerCanvasController.Instance.updatePlayerStatus(
			currentEnergy.ToString(), currentVillageHP.ToString(), currentTotalDamage.ToString(), currentBarrier.ToString(), currentTreeDmgMulit.ToString());
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
		GetCurrentPlayerStatus();
		var playerSet = GameManager.Instance.playerDefaultSetting;

		Debug.Log($"Current Village HP : {currentVillageHP}");

		currentEnergy = (currentMaxEnergy == 0 ? playerSet.initialEnergy : currentMaxEnergy) + currentCarryOverEnergy;
		currentTotalDamage = 0;
		currentBarrier = playerSet.villageBarrier;

		// 변경 사항을 하나의 Hashtable에 담습니다.
		ExitGames.Client.Photon.Hashtable newProps = new ExitGames.Client.Photon.Hashtable
	{
		{ PlayerPropKeys.Energy, currentEnergy },
		{ PlayerPropKeys.CarryOverEnergy, playerSet.carryOverEnergy },
		{ PlayerPropKeys.TotalDamage, currentTotalDamage },
		{ PlayerPropKeys.VillageBarrier, currentBarrier }
	};

		// 한 번의 호출로 모든 프로퍼티를 동기화합니다. 
		// (콜백도 1번만 발생하며, 저장된 값으로 온전히 동기화됨)
		PhotonNetwork.LocalPlayer.SetCustomProperties(newProps);

		Debug.LogWarning($"PLAYER// Energe : {currentEnergy}, TotalDamage : {currentTotalDamage}, Barrier : {currentBarrier}");

		SetPlayerStatusUI();

		if (PhotonNetwork.IsMasterClient && IsSinglePlayer)
		{
			foreach (int aiNum in PlayerManager.Instance.AIPlayerObj.Keys)
			{
				InitAIStatus(aiNum);
			}
		}
	}

	public void InitAIStatus(int actNum)
	{
		GetCurrentPlayerStatus(actNum);
		var playerSet = GameManager.Instance.playerDefaultSetting;

		Debug.Log($"Current AI Village HP : {currentVillageHP}");

		currentEnergy = (currentMaxEnergy == 0 ? playerSet.initialEnergy : currentMaxEnergy) + currentCarryOverEnergy;
		currentTotalDamage = 0;
		currentBarrier = playerSet.villageBarrier;

		// AI 상태 업데이트도 묶어서 처리합니다.
		ExitGames.Client.Photon.Hashtable newAIProps = new ExitGames.Client.Photon.Hashtable
	{
		{ PlayerPropKeys.Energy, currentEnergy },
		{ PlayerPropKeys.CarryOverEnergy, playerSet.carryOverEnergy },
		{ PlayerPropKeys.TotalDamage, currentTotalDamage },
		{ PlayerPropKeys.VillageBarrier, currentBarrier }
	};

		// AI 프로퍼티를 룸 프로퍼티에 저장하는 로직이라면 아래처럼 적용
		PhotonNetwork.CurrentRoom.SetCustomProperties(newAIProps);
		// (만약 PhotonPropertyHelper 안에 AI를 위한 일괄 처리 함수가 있다면 그걸 사용하셔도 됩니다.)

		Debug.LogWarning($"AI// Energe : {currentEnergy}, TotalDamage : {currentTotalDamage}, Barrier : {currentBarrier}");
	}

	// 플레이어 프로퍼티가 변경될 때 UI에 반영
	public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
	{
		if (target != PhotonNetwork.LocalPlayer) return;

		if (changedProps.ContainsKey(PlayerPropKeys.Energy) ||
			changedProps.ContainsKey(PlayerPropKeys.VillageHP) ||
			changedProps.ContainsKey(PlayerPropKeys.TotalDamage) ||
			changedProps.ContainsKey(PlayerPropKeys.VillageBarrier) ||
			changedProps.ContainsKey(PlayerPropKeys.TreeAtkMulti))
		{
			SetPlayerStatusUI();
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
		// 배리어 수치 계산
		currentBarrier = currentTotalDamage * (1 + currentConBarrier);

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
		damage *= currentTreeDmgMulit;

		Debug.Log("Final Tree Damage : " + damage);
		// 배리어로 데미지 경감
		damage -= currentBarrier;
		if (damage < 0) damage = 0;

		// 남은 데미지를 마을 체력에서 차감
		currentVillageHP -= damage;

		// 게임 오버 확인
		if (currentVillageHP <= 0)
		{
			currentVillageHP = 0;
			// 패배 또는 게임 종료 로직
			Debug.Log("Game End By VillageHP 0");
		}

		Debug.Log($"Final Village HP : {currentVillageHP}");
		// 프로퍼티 업데이트
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageHP, currentVillageHP);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VDamageProcessCompleted, true);
		InitTreeAtkMultRate();

		//데미지 처리가 끝났다고 MasterClient에게 송신
		TurnManager.Instance.TreeDamageChecker();
	}

	public void DamagedVillage(float damage, int aiNumber)
	{
		GetCurrentPlayerStatus(aiNumber);

		damage *= currentTreeDmgMulit;
		damage -= currentBarrier;
		if (damage < 0) damage = 0;

		currentVillageHP -= damage;

		if (currentVillageHP <= 0)
		{
			currentVillageHP = 0;
		}

		PhotonPropertyHelper.SetPlayerProp(aiNumber, PlayerPropKeys.VillageHP, currentVillageHP);
		PhotonPropertyHelper.SetPlayerProp(aiNumber, PlayerPropKeys.VDamageProcessCompleted, true);

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
