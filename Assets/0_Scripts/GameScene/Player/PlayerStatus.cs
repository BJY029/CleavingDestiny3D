using Photon.Pun;
using System.Linq;
using UnityEngine;

public class PlayerStatus : MonoBehaviourPunCallbacks
{
    public static PlayerStatus Instance;
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
	}

	private int currentEnergy;
    private int currentMaxEnergy;
    private int currentMaxAtkDamage;
    private int currentMinAtkDamage;
    private float currentVillageHP;
    private float currentTotalDamage;
    private float currentBarrier;
    private float currentConBarrier;

    public void GetCurrentPlayerStatus()
    {
        currentEnergy = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.Energy);
        currentMaxEnergy = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MaxEnergy);
		currentMaxAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MaxAtkPow);
		currentMinAtkDamage = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MinAtkPow);
		currentVillageHP = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageHP);
        currentBarrier = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier);
        currentConBarrier = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer, PlayerPropKeys.BarrierConversionRate);
	}

    //To Do: 아이템 관련 기력 함수
    //UI 처리도 진행

    //나무 HIT 발생시 호출 될 함수
    public void HitAction()
    {
        int randomHitDamage = Random.Range(currentMaxAtkDamage, currentMinAtkDamage + 1);
        currentTotalDamage += randomHitDamage;
        currentBarrier = currentTotalDamage * (1 + currentConBarrier);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.TotalDamage, currentTotalDamage);
		PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer, PlayerPropKeys.VillageBarrier, currentBarrier);
    }

    //마을 체력 감소 함수(RPC)
    public void DamagedVillage(float damage)
    {
        if(!IsInitializer()) return;

        photonView.RPC(nameof(RPC_DamagedVillage), RpcTarget.All, damage);
    }

    [PunRPC]
    public void RPC_DamagedVillage(float damage)
    {
        damage -= currentBarrier;
        currentVillageHP -= damage;

        if(currentVillageHP < 0)
        {
            //게임 종료
            Debug.Log("Game End By VillageHP 0");
            return;
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
