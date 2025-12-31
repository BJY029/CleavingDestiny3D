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

	//플레이어 프로퍼티 값들
	private int currentEnergy;
    private int currentMaxEnergy;
	private int currentCarryOverEnergy;
    private int currentMaxAtkDamage;
    private int currentMinAtkDamage;
    private float currentVillageHP;
    private float currentTotalDamage;
    private float currentBarrier;
    private float currentConBarrier;

	//플레이어 프로퍼티 값 불러오기
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

	//UI 업데이트
	public void SetPlayerStatusUI()
    {
		//if (!photonView.IsMine) return;
		GetCurrentPlayerStatus();
        PlayerCanvasController.Instance.updatePlayerStatus(
            currentEnergy.ToString(), currentVillageHP.ToString(), currentTotalDamage.ToString(), currentBarrier.ToString());
    }

	//턴 전환시 실행될 스탯 값 초기화
	//RPC로 모든 플레이어에게 적용
	public void initPlayerStatus()
	{
		photonView.RPC(nameof(RPC_initPlayerStatus), RpcTarget.All);
	}

	[PunRPC]
	public void RPC_initPlayerStatus()
	{
		GetCurrentPlayerStatus();
		currentEnergy = currentMaxEnergy + currentCarryOverEnergy;
		currentTotalDamage = CommonDefine.defaultTotalDamage;
		currentBarrier = CommonDefine.defaultVillageBarrier;

		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.Energy, currentEnergy);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.CarryOverEnergy, CommonDefine.defaultCarryOverEnergy);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage, currentTotalDamage);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier, currentBarrier);
	}

	//스탯 값이 변경될 때마다 프로퍼티에 적용
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


	//To Do: 아이템 관련 기력 함수
	//UI 처리도 진행

	//나무 HIT 발생시 호출 될 함수
	//Hit을 누른 시점의 강도 값을 인자로 받는다.
	public int HitAction(float damageRatio)
    {
		GetCurrentPlayerStatus();

		//Hit Damager 계산
		int HitDamage = currentMinAtkDamage + Mathf.RoundToInt((currentMaxAtkDamage - currentMinAtkDamage) * (damageRatio / 100));
		//Total Damage 계산
        currentTotalDamage += HitDamage;
		//Barrier 값 계산
        currentBarrier = currentTotalDamage * (1 + currentConBarrier);
		//변경된 스탯 값 프로퍼티에 업데이트
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage, currentTotalDamage);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier, currentBarrier);
		//Hit 데미지 반환
		return HitDamage;
    }

	//웨이브 변경 시 실행할 마을 데미지 함수
    public void DamagedVillage(float damage)
    {
		GetCurrentPlayerStatus();

		//방벽 먼저 제거
		damage -= currentBarrier;
		if(damage < 0) damage = 0;
		//마을 데미지 입히기
        currentVillageHP -= damage;

		//종료 조건 확인
        if(currentVillageHP <= 0)
        {
            currentVillageHP = 0;
            //게임 종료
            Debug.Log("Game End By VillageHP 0");
        }
		//프로퍼티 업데이트
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageHP, currentVillageHP);
	}
}
