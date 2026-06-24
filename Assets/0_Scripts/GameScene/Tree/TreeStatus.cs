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

	public AudioSource treeAudioSource;

	//���� ���� ����
	private float currentTreeHP;
	private float currentTreeAtkPow;

	//���� ������Ƽ ���� ��������
	public void GetCurrentTreeStatus()
	{
		currentTreeHP = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeHP);
		currentTreeAtkPow = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeAtkPow);
	}

	//UI ������Ʈ
	public void SetTreeStatusUI()
	{
		GetCurrentTreeStatus();
		TreeCanvasController.Instance.UpdateTreeHP(currentTreeHP);
	}

	//���� ������Ƽ ����Ǹ� UI�� �ݿ��ϱ�
	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey(RoomPropKeys.TreeHP))
		{
			SetTreeStatusUI();
		}
	}

	//���� ü�� ����
	public void getHitByPlayer(int HitDamage)
	{
		if (!IsInitializer()) return;
		GetCurrentTreeStatus();

		//HitDamage ��ŭ ���� ü�� ���� ��Ű��
		currentTreeHP -= HitDamage;

		if (currentTreeHP <= 0)
		{
			currentTreeHP = 0;
			//���� ����
			Debug.Log("Game End");
		}
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, currentTreeHP);
	}

	//���� ��
	public void getHealByItem(int HealValue)
	{
		GetCurrentTreeStatus();
		// currentTreeHP = (currentTreeHP + HealValue) < GameManager.Instance.roomDefaultSetting.treeHP ? currentTreeHP + HealValue : GameManager.Instance.roomDefaultSetting.treeHP;

		float afterHeal = currentTreeHP + HealValue;
		currentTreeHP = Mathf.Clamp(afterHeal, 0, PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeMaxHP, afterHeal));
		PhotonPropertyHelper.SetRoomProp(RoomPropKeys.TreeHP, currentTreeHP);
	}

	//���� ���� �������� ��ȯ�ϴ� �Լ�
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
