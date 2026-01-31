using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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

	//나무 스탯 정보
	private float currentTreeHP;
	private float currentTreeAtkPow;

	//나무 프로퍼티 정보 가져오기
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

	//나무 프로퍼티 변경되면 UI에 반영하기
	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey(RoomPropKeys.TreeHP))
		{
			SetTreeStatusUI();
		}
	}

	//나무 체력 감소
	public void getHitByPlayer(int HitDamage)
	{
		if (!IsInitializer()) return;
		GetCurrentTreeStatus();

		//HitDamage 만큼 나무 체력 감소 시키기
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
		// currentTreeHP = (currentTreeHP + HealValue) < GameManager.Instance.roomDefaultSetting.treeHP ? currentTreeHP + HealValue : GameManager.Instance.roomDefaultSetting.treeHP;

		float afterHeal = currentTreeHP + HealValue;
		currentTreeHP = Mathf.Clamp(afterHeal, 0, PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeMaxHP, afterHeal));
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, currentTreeHP);
	}

	//마을 공격 데미지를 반환하는 함수
	public float getTreeAtkPow()
	{
		GetCurrentTreeStatus();
		return currentTreeAtkPow;
	}

	private bool IsInitializer() => PhotonNetwork.IsMasterClient;
	//private bool IsInitializer()
	//{
	//	var players = PhotonNetwork.PlayerList;
	//	int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
	//	int minActor = players.Min(p => p.ActorNumber);
	//	return myActor == minActor;
	//}
}
