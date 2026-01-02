using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InventoryAuthority : MonoBehaviourPunCallbacks
{
	public static InventoryAuthority Instance;

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

	[PunRPC]
	void RPC_TakeOffer(int actor, string itemId, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient) return;
		if(info.Sender == null || info.Sender.ActorNumber != actor) return;

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
		if(!ItemInfoSerializer.TryAddFirstEmpty(slots, (nextUid, itemId)))
		{
			//실패시
			Debug.LogError("Item Insertion ERROR");
			return;
		}

		//삽입 결과, 변경 결과 프로퍼티로 업데이트
		PhotonPropertyHelper.SetRoomProp(ItemPropKeys.INV(actor), ItemInfoSerializer.Encode(slots));
		PhotonPropertyHelper.SetRoomProp(ItemPropKeys.NEXT_UID, nextUid + 1);
		PhotonPropertyHelper.SetRoomProp(ItemPropKeys.OFFER(actor), "");

		Debug.Log($"Player{actor}'s inventory : {invSlots}");
	}

	//선택된 아이템이 실제 제안된 아이템 목록에 있는지 확인
	bool Contains(string offer, string id)
	{
		var p = offer.Split('|');
		for(int i = 0; i <  p.Length; i++)
		{
			if (p[i] == id) return true;
		}
		return false;
	}
}
