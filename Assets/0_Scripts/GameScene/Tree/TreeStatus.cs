using ExitGames.Client.Photon;
using Photon.Pun;
using System.Linq;
using UnityEngine;

public class TreeStatus : MonoBehaviourPunCallbacks
{
    public static TreeStatus Instance;
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}


	private float currentTreeHP;
    private float currentTreeAtkPow;

    public void GetCurrentTreeStatus()
    {
        currentTreeHP = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeHP);
		currentTreeAtkPow = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeAtkPow);
	}

	//UI 업데이트
	public void SetTreeStatusUI()
	{
		GetCurrentTreeStatus();
		TreeCanvasController.Instance.UpdateTreeHP(currentTreeHP);
	}

	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		if(propertiesThatChanged.ContainsKey(RoomPropKeys.TreeHP))
		{
			SetTreeStatusUI();
		}
	}

	//나무 체력 감소
	public void getHitByPlayer(int HitDamage)
    {
		if (!IsInitializer()) return;
        GetCurrentTreeStatus();

		currentTreeHP -= HitDamage;
		
		if (currentTreeHP <= 0)
        {
            currentTreeHP = 0;
            //게임 종료
            Debug.Log("Game End");
        }
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, currentTreeHP);
	}

    //나무 힐
    public void getHealByItem(int HealValue)
    {
		GetCurrentTreeStatus();
		currentTreeHP = (currentTreeHP + HealValue) < CommonDefine.defaultTreeHP ? currentTreeHP + HealValue : CommonDefine.defaultTreeHP;
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, currentTreeHP);
	}

	//마을 공격
	public float getTreeAtkPow()
	{
		GetCurrentTreeStatus();
		return currentTreeAtkPow;
	}
	private bool IsInitializer()
	{
		var players = PhotonNetwork.PlayerList;
		int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
		int minActor = players.Min(p => p.ActorNumber);
		return myActor == minActor;
	}
}
