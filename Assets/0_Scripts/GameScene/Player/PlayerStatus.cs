using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

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

	// 플레이어 프로퍼티 변수
	private int currentEnergy;
	private int currentMaxEnergy;
	private int currentCarryOverEnergy;
	private int currentMaxAtkDamage;
	private int currentMinAtkDamage;
	private float currentVillageHP;
	private float currentTotalDamage;
	private float currentBarrier;
	private float currentConBarrier;

	// 플레이어 프로퍼티 값 불러오기
	public void GetCurrentPlayerStatus()
	{
		currentCarryOverEnergy = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.CarryOverEnergy);
		currentEnergy = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.Energy);
		currentMaxEnergy = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MaxEnergy);
		currentMaxAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MaxAtkPow);
		currentMinAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MinAtkPow);
		currentVillageHP = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageHP);
		currentBarrier = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier);
		currentConBarrier = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.BarrierConversionRate);
		currentTotalDamage = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage);
	}

	// UI 업데이트
	public void SetPlayerStatusUI()
	{
		GetCurrentPlayerStatus();
		PlayerCanvasController.Instance.updatePlayerStatus(
			currentEnergy.ToString(), currentVillageHP.ToString(), currentTotalDamage.ToString(), currentBarrier.ToString());
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
		// ScriptableObject 기반 초기화
		var playerSet = GameManager.Instance.playerDefaultSetting;

		currentEnergy = (currentMaxEnergy == 0 ? playerSet.initialEnergy : currentMaxEnergy) + currentCarryOverEnergy;
		currentTotalDamage = 0;
		currentBarrier = playerSet.villageBarrier;

		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.Energy, currentEnergy);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.CarryOverEnergy, playerSet.carryOverEnergy);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage, currentTotalDamage);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier, currentBarrier);
	}

	// 플레이어 프로퍼티가 변경될 때 UI에 반영
	public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
	{
		if (target != PhotonNetwork.LocalPlayer) return;

		if (changedProps.ContainsKey(PlayerPropKeys.Energy) ||
			changedProps.ContainsKey(PlayerPropKeys.VillageHP) ||
			changedProps.ContainsKey(PlayerPropKeys.TotalDamage) ||
			changedProps.ContainsKey(PlayerPropKeys.VillageBarrier))
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
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage, currentTotalDamage);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier, currentBarrier);

		// 계산된 데미지 반환
		return HitDamage;
	}

	// 마을이 데미지를 입었을 때 처리 함수
	public void DamagedVillage(float damage)
	{
		GetCurrentPlayerStatus();

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

		// 프로퍼티 업데이트
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageHP, currentVillageHP);
	}
}
