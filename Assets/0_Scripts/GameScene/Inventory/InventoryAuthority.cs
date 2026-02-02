using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class InventoryAuthority : MonoBehaviourPunCallbacks
{
	public static InventoryAuthority Instance;
	private WorldInventorySlot wis;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	//3개 중 하나를 선택한 경우 호출 될 함수
	public void RequestTakeOffer(string itemId)
	{
		photonView.RPC(nameof(RPC_TakeOffer), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, itemId);
	}

	public void RequestUseItem(int slotIdx, WorldInventorySlot wi)
	{
		wis = wi;
		photonView.RPC(nameof(RPC_UseItem), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, slotIdx);
	}

	public void RequestSteelItem(int FromActor, int ToActor, int SelectedSlotIdx, WorldInventorySlot wi)
	{
		wis = wi;
		photonView.RPC(nameof(RPC_SteelItem), RpcTarget.MasterClient, FromActor, ToActor, SelectedSlotIdx);
	}

	//UID 기반으로 아이템을 삭제하는 함수
	public void DeleteItemByUID(int ActNum, int UID)
	{
		photonView.RPC(nameof(RPC_DeleteItem), RpcTarget.MasterClient, ActNum, UID);
	}

	[PunRPC]
	void RPC_DeleteItem(int actor, int UID, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		//요청자 INV 정보 가져오기
		string Inv = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(actor));
		int InvCap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(actor));
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actor);

		//요청자 INV 정보 검증
		var slots = ItemInfoSerializer.Decode(Inv, InvCap);

		//uniqueId 기반으로 슬롯 IDX 값 받아오기
		int removaldx = ItemInfoSerializer.TryFindIndexByUniqueId(slots, UID);

		//사용한 아이템 인벤토리에서 제거
		slots[removaldx] = (0, null);
		photonView.RPC(nameof(RPC_RefreshItemInv), player);

		//아이템 사용 UI 띄우기
		//PlayerCanvasController.Instance.PopUpItemNotify(item.itemId, player);

		//슬롯 Info 정보 업데이트
		string updatedItemSlots = ItemInfoSerializer.Encode(slots);

		string InvKey = ItemPropKeys.INV(actor);
		//새롭게 업데이트할 프로퍼티
		var newProps = new ExitGames.Client.Photon.Hashtable
		{
			{InvKey, updatedItemSlots }
		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);
	}

	[PunRPC]
	void RPC_TakeOffer(int actor, string itemId, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient) return;
		if (info.Sender == null || info.Sender.ActorNumber != actor) return;

		//턴 검증
		int turnActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
		if (turnActor != actor) return;

		//offer 검증
		string offer = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.OFFER(actor));
		if (string.IsNullOrEmpty(offer) || !Contains(offer, itemId)) return;

		//인벤토리 크기 받아오기
		int cap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(actor));

		//직렬화된 인벤토리 정보 역직렬화해서 가져오기
		string invSlots = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(actor));
		var slots = ItemInfoSerializer.Decode(invSlots, cap);

		//다음 아이템 고유 아이디 가져오기
		int nextUid = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.NEXT_UID);
		//플레이어 아이템 인벤토리 첫 칸에 해당 아이템 삽입 시도하기
		if (!ItemInfoSerializer.TryAddFirstEmpty(slots, (nextUid, itemId)))
		{
			//실패시
			Debug.LogError("Item Insertion ERROR");
			return;
		}

		//삽입 결과, 변경 결과 프로퍼티로 한번에 업데이트
		var ht = new ExitGames.Client.Photon.Hashtable
		{
			{ItemPropKeys.INV(actor),ItemInfoSerializer.Encode(slots)},
			{ItemPropKeys.NEXT_UID, nextUid + 1},
			{ItemPropKeys.OFFER(actor), "" },

		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);


		string inv = "";
		foreach (var x in slots)
		{
			if (x.itemID == null)
			{
				inv += " _ ";
			}
			else
			{
				inv += $" {x.itemID} ";
			}
		}
		Debug.Log($"Player{actor}'s inventory : {inv}");
	}

	[PunRPC]
	void RPC_UseItem(int requestActor, int slotIdx, PhotonMessageInfo info)
	{
		//Master 검증
		if (!PhotonNetwork.IsMasterClient) return;

		//Turn 검증
		int turnActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
		if (turnActor != requestActor)
		{
			Debug.LogError("Requester isn't Current turn");
			return;
		}

		//요청자 INV 정보 가져오기
		string Inv = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(turnActor));
		int InvCap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(turnActor));

		//요청자 INV 정보 검증
		var slots = ItemInfoSerializer.Decode(Inv, InvCap);
		int itemCnt = 0;
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].itemID != null) itemCnt++;
		}

		if (slots == null || slots.Length != InvCap)
		{
			Debug.LogError("Slots Info Error");
			return;
		}
		if (slotIdx < 0 || slotIdx >= slots.Length)
		{
			Debug.LogError("Slot Index Info Error");
			return;
		}

		//슬롯 Idx 검증
		string itemId = slots[slotIdx].itemID;
		int uniqueId = slots[slotIdx].uniqueId;

		//uniqueId 기반으로 슬롯 IDX 값 받아오기
		int removaldx = ItemInfoSerializer.TryFindIndexByUniqueId(slots, uniqueId);
		//IDX 다르면 에러
		if (removaldx != slotIdx)
		{
			Debug.Log("SlotIdx Error");
			return;
		}

		ItemSO item = ItemDB.Instance.Get(itemId);


		Player player = PhotonNetwork.CurrentRoom.GetPlayer(requestActor);

		//희생 아이템이면서, 아이템 갯수가 1개 이하인 경우
		if (itemCnt <= 1 && item.itemId == "2002")
		{
			photonView.RPC(nameof(RPC_ShowWarning), player, UI_CSV.UI_Warning_NotEnoughItem);
			return;
		}

		//Check whether you are trying to use an item that can only be used once per turn or day.
		if (!ItemHandlingSystem.instance.CheckItemAvaiable(requestActor, itemId))
		{
			photonView.RPC(nameof(RPC_ShowWarning), player, UI_CSV.UI_Warning_NotAvaiable);
			return;
		}

		//Check player's energy and prevent to use item
		int playerEng = PhotonPropertyHelper.GetPlayerProp<int>(player, PlayerPropKeys.Energy);
		if (playerEng - item.itemCost < 0)
		{
			photonView.RPC(nameof(RPC_ShowWarning), player, UI_CSV.UI_Warning_Energy);
			photonView.RPC(nameof(RPC_SetItemSlotUI), player, false);
			return;
		}

		//기력량 업데이트
		PhotonPropertyHelper.SetPlayerProp(player, PlayerPropKeys.Energy, playerEng - item.itemCost);


		//사용한 아이템 인벤토리에서 제거
		slots[slotIdx] = (0, null);
		photonView.RPC(nameof(RPC_SetItemSlotUI), player, true);

		//아이템 사용 UI 띄우기
		PlayerCanvasController.Instance.PopUpItemNotify(item.itemId, player);

		//슬롯 Info 정보 업데이트
		string updatedItemSlots = ItemInfoSerializer.Encode(slots);

		string InvKey = ItemPropKeys.INV(requestActor);
		//새롭게 업데이트할 프로퍼티
		var newProps = new ExitGames.Client.Photon.Hashtable
		{
			{InvKey, updatedItemSlots }
		};

		//검증 프로퍼티
		var expected = new ExitGames.Client.Photon.Hashtable
		{
			{InvKey,  Inv},
			{RoomPropKeys.CurrentTurnActor, turnActor },
		};

		//현재 expected 프로퍼티인 경우에만 newProps로 업데이트
		PhotonNetwork.CurrentRoom.SetCustomProperties(newProps, expected);
		string inv = "";
		foreach (var x in slots)
		{
			if (x.itemID == null)
			{
				inv += " _ ";
			}
			else
			{
				inv += $" {x.itemID} ";
			}
		}
		Debug.Log($"Player{turnActor}'s inventory : {inv}");

		//TODO: 아이템 효과 적용
		//MasterClient가 효과를 확정하고 Room 프로퍼티 업데이트
		ItemHandlingSystem.instance.AddItemStatusInstance(turnActor, item, uniqueId);
	}

	[PunRPC]
	public void RPC_SteelItem(int FromActor, int ToActor, int SelectedSlotIdx, PhotonMessageInfo info)
	{
		//Master 검증
		if (!PhotonNetwork.IsMasterClient) return;

		//인벤토리 주인 INV 정보 가져오기
		string FromInv = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(FromActor));
		int FromInvCap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(FromActor));
		//훔치는 Actor Inv 정보 가져오기
		string ToInv = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(ToActor));
		int ToInvCap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(ToActor));


		//인벤토리 주인 INV 정보 검증
		var slots = ItemInfoSerializer.Decode(FromInv, FromInvCap);
		if (slots == null || slots.Length != FromInvCap)
		{
			Debug.LogError("Slots Info Error");
			return;
		}
		if (SelectedSlotIdx < 0 || SelectedSlotIdx >= slots.Length)
		{
			Debug.LogError("Slot Index Info Error");
			return;
		}

		//인벤토리 주인 슬롯 Idx 검증
		string itemId = slots[SelectedSlotIdx].itemID;
		int uniqueId = slots[SelectedSlotIdx].uniqueId;

		//uniqueId 기반으로 슬롯 IDX 값 받아오기
		int removaldx = ItemInfoSerializer.TryFindIndexByUniqueId(slots, uniqueId);
		//IDX 다르면 에러
		if (removaldx != SelectedSlotIdx)
		{
			Debug.Log("SlotIdx Error");
			return;
		}

		//훔치는 아이템 가져오기
		ItemSO item = ItemDB.Instance.Get(itemId);
		//인벤토리 주인 Player 객체
		Player FromPlayer = PhotonNetwork.CurrentRoom.GetPlayer(FromActor);
		Player ToPlayer = PhotonNetwork.CurrentRoom.GetPlayer(ToActor);

		//빼앗긴 아이템 인벤토리에서 제거
		slots[SelectedSlotIdx] = (0, null);
		photonView.RPC(nameof(RPC_SetItemSlotUI), FromPlayer, true);

		//아이템 빼앗김 UI 띄우기
		PlayerCanvasController.Instance.PopUpItemStolenNotify(item.itemId, FromPlayer, ToPlayer);

		//인벤토리 주인 슬롯 Info 정보 업데이트
		string updatedItemSlots = ItemInfoSerializer.Encode(slots);

		string InvKey = ItemPropKeys.INV(FromActor);
		//새롭게 업데이트할 프로퍼티
		var newProps = new ExitGames.Client.Photon.Hashtable
		{
			{InvKey, updatedItemSlots }
		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);



		//훔치는 Actor의 인벤토리 정보 역직렬화해서 가져오기
		var TargetSlots = ItemInfoSerializer.Decode(ToInv, ToInvCap);

		//훔치는 플레이어 아이템 인벤토리 첫 칸에 해당 아이템 삽입 시도하기
		if (!ItemInfoSerializer.TryAddFirstEmpty(TargetSlots, (uniqueId, itemId)))
		{
			//실패시
			Debug.LogError("Item Insertion ERROR");
			return;
		}

		//삽입 결과, 변경 결과 프로퍼티로 한번에 업데이트
		var ht = new ExitGames.Client.Photon.Hashtable
		{
			{ItemPropKeys.INV(ToActor),ItemInfoSerializer.Encode(TargetSlots)},
			{ItemPropKeys.OFFER(ToActor), "" },

		};
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);


		//Debug
		string inv = "";
		foreach (var x in slots)
		{
			if (x.itemID == null)
			{
				inv += " _ ";
			}
			else
			{
				inv += $" {x.itemID} ";
			}
		}
		Debug.Log($"Player{FromActor}'s inventory : {inv}");

		inv = "";
		foreach (var x in TargetSlots)
		{
			if (x.itemID == null)
			{
				inv += " _ ";
			}
			else
			{
				inv += $" {x.itemID} ";
			}
		}
		Debug.Log($"Player{ToActor}'s inventory : {inv}");
	}

	[PunRPC]
	public void RPC_SetItemSlotUI(bool success)
	{
		wis?.SetCurrentItemNull(success);
		wis = null;
	}

	[PunRPC]
	public void RPC_RefreshItemInv()
	{
		GameObject PlayerInv = PlayerStatus.Instance.GetPlayerInventory();
		WorldInventory WI = PlayerInv.GetComponent<WorldInventory>();
		WI.RefreshInv();
	}

	[PunRPC]
	public void RPC_ShowWarning(string textId)
	{
		PlayerCanvasController.Instance.SetWarningTextActive(textId);
	}

	//선택된 아이템이 실제 제안된 아이템 목록에 있는지 확인
	bool Contains(string offer, string id)
	{
		var p = offer.Split('|');
		for (int i = 0; i < p.Length; i++)
		{
			if (p[i] == id) return true;
		}
		return false;
	}
}
