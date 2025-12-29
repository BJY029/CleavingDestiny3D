using UnityEngine;

public class TreeStatus : MonoBehaviour
{
    public static TreeStatus Instance;
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance);
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

    //나무 체력 감소
    public void getHitByPlayer(int HitDamage)
    {
        if (currentTreeHP > HitDamage)
        {
            //게임 종료
            Debug.Log("Game End");
            return;
        }

        currentTreeHP -= HitDamage;
        PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, currentTreeHP);
    }

    //나무 힐
    public void getHealByItem(int HealValue)
    {
        currentTreeHP = (currentTreeHP + HealValue) < CommonDefine.defaultTreeHP ? currentTreeHP + HealValue : CommonDefine.defaultTreeHP;
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, currentTreeHP);
	}

    //마을 공격
    public void attackPlayersVillage()
    {
        //PhotonView로 모든 플레이어 마을에게 데미지 부여
    }
}
