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


	private int currentEnergy;
    private int currentMaxEnergy;
	private int currentCarryOverEnergy;
    private int currentMaxAtkDamage;
    private int currentMinAtkDamage;
    private float currentVillageHP;
    private float currentTotalDamage;
    private float currentBarrier;
    private float currentConBarrier;

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
	public int HitAction(float damageRatio)
    {
		GetCurrentPlayerStatus();

		int randomHitDamage = currentMinAtkDamage + Mathf.RoundToInt((currentMaxAtkDamage - currentMinAtkDamage) * (damageRatio / 100));
        currentTotalDamage += randomHitDamage;
        currentBarrier = currentTotalDamage * (1 + currentConBarrier);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage, currentTotalDamage);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier, currentBarrier);
		return randomHitDamage;
    }

    public void DamagedVillage(float damage)
    {
		GetCurrentPlayerStatus();

		damage -= currentBarrier;
		if(damage < 0) damage = 0;
        currentVillageHP -= damage;

        if(currentVillageHP <= 0)
        {
            currentVillageHP = 0;
            //게임 종료
            Debug.Log("Game End By VillageHP 0");
        }
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageHP, currentVillageHP);
	}

	private bool IsInitializer()
	{
		var players = PhotonNetwork.PlayerList;
		int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
		int minActor = players.Min(p => p.ActorNumber);
		return myActor == minActor;
	}
}
