using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
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

	public Dictionary<int, bool> selectedNewDrugItem = new Dictionary<int, bool>();
	public bool hasSelectedNewDrugItem(int actorNum)
	{ return (selectedNewDrugItem.ContainsKey(actorNum) && selectedNewDrugItem[actorNum]); }

	private void MarkSelectedNewDrugItem(int actorNum, string itemId)
	{
		if (itemId != "3001") return;
		selectedNewDrugItem[actorNum] = true;
		Debug.Log($"[NewDrugDevelopment] Actor {actorNum} selected new drug item.");
	}

	//3개 중 하나를 선택한 경우 호출 될 함수
	public void RequestTakeOffer(string itemId)
	{
		photonView.RPC(nameof(RPC_TakeOffer), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, itemId);
	}

	public void RequestBuyShopItem(int actor, string itemId, int price)
	{
		photonView.RPC(nameof(RPC_BuyShopItem), RpcTarget.MasterClient, actor, itemId, price);
	}

	public void RequestUseItem(int slotIdx, int ActNum, WorldInventorySlot wi)
	{
		wis = wi;
		photonView.RPC(nameof(RPC_UseItem), RpcTarget.MasterClient, ActNum, slotIdx);
	}

	public void RequestUseNewDrugItem(int ActNum, InventoryNewDrug nd)
	{
		photonView.RPC(nameof(RPC_UseNewDrugItem), RpcTarget.MasterClient, ActNum);
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

	// [MasterClient 전용] 아이템을 인벤토리에 추가하는 공통 로직
	// 상점 구매, 턴 제안 수락 등 모든 '아이템 획득' 상황에서 재사용됩니다.
	private bool Master_AddItemToInventory(int actor, string itemId, ExitGames.Client.Photon.Hashtable extraProps = null)
	{
		// 1. 해당 플레이어의 인벤토리 용량과 현재 데이터를 RoomProperties에서 가져옵니다.
		int cap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(actor));
		string invSlotsStr = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(actor));

		// 2. 문자열 형식의 데이터를 (UniqueId, ItemId) 배열로 복원(Decode)합니다.
		var slots = ItemInfoSerializer.Decode(invSlotsStr, cap);

		// 3. 인벤토리가 가득 찼는지 검사합니다.
		if (ItemInfoSerializer.isFullInventory(slots)) return false;

		// 4. 아이템에 부여할 고유 식별자(Next UID)를 가져옵니다.
		int nextUid = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.NEXT_UID);

		// 5. 첫 번째 빈 슬롯에 아이템을 삽입합니다.
		if (!ItemInfoSerializer.TryAddFirstEmpty(slots, (nextUid, itemId))) return false;

		// 6. 업데이트할 RoomProperties 해시테이블 구성
		var ht = new ExitGames.Client.Photon.Hashtable
		{
			{ ItemPropKeys.INV(actor), ItemInfoSerializer.Encode(slots) }, // 갱신된 인벤토리 문자열
			{ ItemPropKeys.NEXT_UID, nextUid + 1 }                         // 다음 아이템을 위한 UID 증가
		};

		// 7. 추가로 업데이트할 프로퍼티(예: Offer 비우기)가 있다면 병합합니다.
		if (extraProps != null)
		{
			foreach (var key in extraProps.Keys) ht[key] = extraProps[key];
		}

		// 8. 서버(Photon Room)에 프로퍼티 설정을 요청하여 모든 클라이언트에 동기화합니다.
		PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

		return true;
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
		if (player != null)
			photonView.RPC(nameof(RPC_RefreshItemInv), player);
		//AI �÷��̾��� ���� ��û�� ����
		else if (GameManager.Instance.isSoloPlay)
			RefreshAIItemInv();

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

		// 아이템 추가 시도 및 Offer 비우기
		var extra = new ExitGames.Client.Photon.Hashtable { { ItemPropKeys.OFFER(actor), "" } };
		if (!Master_AddItemToInventory(actor, itemId, extra))
		{
			Debug.LogError("Item Insertion ERROR (Inventory Full?)");
			photonView.RPC(nameof(RPC_ShowWarning), info.Sender, UI_CSV.UI_Warning_FullInv);
			return;
		}

		MarkSelectedNewDrugItem(actor, itemId);

		Debug.Log($"Player{actor} took offer: {itemId}");
	}

	[PunRPC]
	void RPC_BuyShopItem(int actor, string itemId, int price, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		// 클라이언트가 이미 가격을 지불하고 호출했다고 가정하고, 인벤토리에 아이템만 추가합니다.
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actor);

		if (Master_AddItemToInventory(actor, itemId))
		{
			MarkSelectedNewDrugItem(actor, itemId);
			Debug.Log($"[InventoryAuthority] Actor {actor} bought {itemId} for {price}G (Gold deducted by client)");
		}
		else
		{
			// 아이템 추가 실패 (인벤토리 가득 참)
			Debug.LogError("[InventoryAuthority] Item Insertion ERROR (Inventory Full?)");
		}
	}

	[PunRPC]
	void RPC_UseNewDrugItem(int requestActor, PhotonMessageInfo info)
	{
		//Master 검증
		if (!PhotonNetwork.IsMasterClient) return;

		//Turn 검증
		int turnActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
		if (turnActor != requestActor)
		{
			if (!GameManager.Instance.isSoloPlay)
			{
				Debug.LogError("Requester isn't Current turn");
				return;
			}
		}

		bool hasNewDrugItem = PhotonPropertyHelper.GetRoomProp<bool>(ItemPropKeys.INV_NEWDRUG(requestActor));
		if (!hasNewDrugItem)
		{
			Debug.LogError("Requester doesn't have NEW DRUG Itme!");
			return;
		}

		ItemSO item = ItemDB.Instance.Get("5000");
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(requestActor);

		photonView.RPC(nameof(RPC_OffNewDrug), player);

		PlayerCanvasController.Instance.PopUpItemNotify(item.itemId, player);
		string InvKey = ItemPropKeys.INV_NEWDRUG(requestActor);
		//새롭게 업데이트할 프로퍼티
		var newProps = new ExitGames.Client.Photon.Hashtable
		{
			{InvKey, false }
		};

		//검증 프로퍼티
		var expected = new ExitGames.Client.Photon.Hashtable
		{
			{InvKey,  true},
			{RoomPropKeys.CurrentTurnActor, turnActor },
		};

		//현재 expected 프로퍼티인 경우에만 newProps로 업데이트
		PhotonNetwork.CurrentRoom.SetCustomProperties(newProps, expected);

		//TODO: 아이템 효과 적용
		//MasterClient가 효과를 확정하고 Room 프로퍼티 업데이트
		ItemHandlingSystem.instance.AddItemStatusInstance(requestActor, item, -1);
		//NotifyItemUsedForNewDrugMission(turnActor, item);
	}

	[PunRPC]
	private void RPC_OffNewDrug()
	{
		int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
		if (PlayerManager.Instance.PlayersInv.TryGetValue((actNum), out WorldInventory MyInv))
		{
			MyInv.ToggleNewDrugPosition(false);
		}
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
			if (!GameManager.Instance.isSoloPlay)
			{
				Debug.LogError("Requester isn't Current turn");
				return;
			}
		}

		//요청자 INV 정보 가져오기
		string Inv = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(requestActor));
		int InvCap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(requestActor));

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
			if (player != null)
				photonView.RPC(nameof(RPC_ShowWarning), player, UI_CSV.UI_Warning_NotEnoughItem);
			return;
		}

		//Check whether you are trying to use an item that can only be used once per turn or day.
		if (!ItemHandlingSystem.instance.CheckItemAvaiable(requestActor, itemId))
		{
			if (player != null)
				photonView.RPC(nameof(RPC_ShowWarning), player, UI_CSV.UI_Warning_NotAvaiable);
			return;
		}

		//Check player's energy and prevent to use item
		int playerEng = PhotonPropertyHelper.GetPlayerProp<int>(requestActor, PlayerPropKeys.Energy);
		if (playerEng - item.itemCost < 0)
		{
			if (player != null)
			{
				photonView.RPC(nameof(RPC_ShowWarning), player, UI_CSV.UI_Warning_Energy);
				photonView.RPC(nameof(RPC_SetItemSlotUI), player, false);
			}
			return;
		}

		//기력량 업데이트
		PhotonPropertyHelper.SetPlayerProp(requestActor, PlayerPropKeys.Energy, playerEng - item.itemCost);


		//사용한 아이템 인벤토리에서 제거
		slots[slotIdx] = (0, null);
		if (player != null)
			photonView.RPC(nameof(RPC_SetItemSlotUI), player, true);
		else
		{
			wis.SetCurrentItemNull(true);
			wis = null;
		}

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
		Debug.Log($"Player{requestActor}'s inventory : {inv}");

		//TODO: 아이템 효과 적용
		//MasterClient가 효과를 확정하고 Room 프로퍼티 업데이트
		ItemHandlingSystem.instance.AddItemStatusInstance(requestActor, item, uniqueId);
		NotifyItemUsedForNewDrugMission(requestActor, item);
	}

	private void NotifyItemUsedForNewDrugMission(int actorNum, ItemSO usedItem)
	{
		if (!PhotonNetwork.IsMasterClient) return;
		if (NewDrugMissionManager.instance == null) return;
		if (usedItem == null) return;

		// 신약 개발 아이템 자체는 미션 시작 트리거라서 제외
		if (usedItem.itemId == "3001")
			return;

		NewDrugMissionManager.instance.ReceiveGameEvent(new NewDrugGameEvent
		{
			Type = NewDrugGameEventType.ItemUsed,
			ActorNumber = actorNum,
			UsedItem = usedItem,
			TurnIndex = GetCurrentTurnIndex(),
			WaveIndex = GetCurrentWaveIndex()
		});

		if (usedItem.itemCost > 0)
		{
			NewDrugMissionManager.instance.ReceiveGameEvent(new NewDrugGameEvent
			{
				Type = NewDrugGameEventType.StaminaSpent,
				ActorNumber = actorNum,
				StaminaAmount = usedItem.itemCost,
				TurnIndex = GetCurrentTurnIndex(),
				WaveIndex = GetCurrentWaveIndex()
			});
		}
	}

	private int GetCurrentTurnIndex()
	{
		return PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
	}

	private int GetCurrentWaveIndex()
	{
		return PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
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

	//AI �÷��̾� �κ��丮 ���� �Լ�
	public void RefreshAIItemInv()
	{
		GameObject AIInv = PlayerStatus.Instance.GetAIInventory();
		WorldInventory WI = AIInv?.GetComponent<WorldInventory>();
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
