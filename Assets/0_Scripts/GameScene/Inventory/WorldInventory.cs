using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class WorldInventory : MonoBehaviourPunCallbacks
{
	//인벤토리 소유자 정보
	int owner = -1;
	//해당 인벤토리의 슬롯들 
    [SerializeField]private List<WorldInventorySlot> slots = new();

	private void Awake()
	{
		//자식 오브젝트에서 슬롯 찾아서 리스트에 삽입
		slots = GetComponentsInChildren<WorldInventorySlot>(true)
			.OrderBy(s => s.slotIndex).ToList();
	}

	private void Start()
	{
		//인벤토리 소유자 초기화
		owner = photonView.OwnerActorNr;
		//슬롯 초기화
		RefreshInv();
	}

	public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		//소유자가 아니면 처리 안함
		if (owner <= 0) return;

		//소유자의 인벤토리 정보
		string invKey = ItemPropKeys.INV(owner);
		string capKey = ItemPropKeys.INV_CAPACITY(owner);

		//소유자의 인벤토리 정보가 변경된 경우
		if(propertiesThatChanged.ContainsKey(invKey) || propertiesThatChanged.ContainsKey(capKey))
		{
			//슬롯 초기화
			RefreshInv();
		}
	}

	//슬롯을 최신 정보로 업데이트 하는 함수
	public void RefreshInv()
	{
		//인벤토리 주인이 아닌 경우 수행 안함
		if (owner <= 0) return;

		//내 actor 번호에 해당되는 INV 정보 가져오기
		int capacity = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(owner));
		string invStr = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(owner));

		//인벤토리 슬롯 가져오기
		var invSlots = ItemInfoSerializer.Decode(invStr, capacity);

		//확장성 고려
		int n = Mathf.Min(slots.Count, capacity);

		//각 인벤토리 슬롯 설정
		for(int i = 0; i < n; i++)
		{
			//해당 아이템 정보 가져오기
			ItemSO item = ItemDB.Instance.Get(invSlots[i].itemID);
			//슬롯 정보 설정(owner 정보도 함께 넘긴다.)
			slots[i].SetSlot(item, owner);
		}

		// (확장성 고려)남은 슬롯 비우기
		for (int i = n; i < slots.Count; i++)
			slots[i].SetSlot(null, owner);
	}
}
