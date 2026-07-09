using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using System;
using ExitGames.Client.Photon;
using System.Text;
using Village;

public class OfferAuthority : MonoBehaviourPunCallbacks
{
	public static OfferAuthority Instance;

	// 상점 리롤 결과 수신 시 발생하는 이벤트 (대상ActorNum, nonce, 아이템ID배열)
	public event Action<int, int, string[]> OnShopRerollReceived;

	private static readonly HashSet<string> AI_MODE_BANNED_ITEM_IDS = new HashSet<string>
	{
		"3001", "4006"
	};

	private void Awake()
	{
		// 씬 기반 싱글톤: 기존 인스턴스가 있다면 파괴하고 현재 것을 등록
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void OnDestroy()
	{
		// 인스턴스가 파괴될 때 static 참조 해제
		if (Instance == this)
		{
			Instance = null;
		}
	}

	// 아이템 등급별 가중치를 담는 구조체
	public readonly struct RarityWeights
	{
		public readonly float Common;
		public readonly float Hero;
		public readonly float Rare;
		public readonly float Legendary;

		public RarityWeights(float common, float hero, float rare, float legendary)
		{
			Common = common;
			Hero = hero;
			Rare = rare;
			Legendary = legendary;
		}

		public float GetWeight(ItemClass itemClass) => itemClass switch
		{
			ItemClass.Common => Common,
			ItemClass.Hero => Hero,
			ItemClass.Rare => Rare,
			ItemClass.Legendary => Legendary,
			_ => 0f
		};
	}

	private List<string> currentOffer = new List<string>();
	private List<(double key, string itemId)> scored = new List<(double key, string itemId)>();

	public int GetItemRollSeed(int turnActor, int turnIndex)
	{
		int roomSeed = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.RoomSeed);
		return roomSeed ^ (turnIndex * 73856093) ^ (turnActor * 19349663) ^ 12345;
	}

	// 클라이언트 요청에 응답
	public void RequestShopReroll(int turnActor, int turnIndex, int shopNonce)
	{
		int shopLevel = VillageSystem.VillageStat.GetVillageLevel(VillageType.Shop);
		photonView.RPC(nameof(RPC_RequestShopReroll), RpcTarget.MasterClient, turnActor, turnIndex, shopNonce, shopLevel);
	}

	[PunRPC]
	void RPC_RequestShopReroll(int turnActor, int turnIndex, int shopNonce, int shopLevel)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		string[] resultOffer = GetShopOffer(turnActor, turnIndex, shopNonce, shopLevel);
		SendShopRerollResult(turnActor, shopNonce, resultOffer);
	}

	/// <summary>
	/// 상점 아이템 목록을 생성합니다. (MasterClient 전용)
	/// </summary>
	public string[] GetShopOffer(int turnActor, int turnIndex, int shopNonce, int shopLevel)
	{
		if (VillageSystem.VillageStat == null) return Array.Empty<string>();

		var b = VillageSystem.VillageStat.VillageBalance;

		float rareW = b.ShopRareChanceBase + (shopLevel - 1) * b.ShopRareChanceMultiplier;

		float heroW = (shopLevel >= b.ShopHeroMinLevel)
			? b.ShopHeroChanceBase + (shopLevel - b.ShopHeroMinLevel) * b.ShopHeroChanceMultiplier
			: 0f;

		float legendaryW = (shopLevel >= b.ShopLegendaryMinLevel)
			? b.ShopLegendaryChanceBase + (shopLevel - b.ShopLegendaryMinLevel) * b.ShopLegendaryChanceMultiplier
			: 0f;

		RarityWeights shopWeights = new RarityWeights(1.0f, heroW, rareW, legendaryW);

		List<ItemSO> items = ItemDB.Instance.GetItemsList();
		items = FilterNewDrugDevelopmentItem(turnActor, items);
		items = FilterAIModeBannedItems(items);
		int randSeed = GetItemRollSeed(turnActor, turnIndex + shopNonce);

		List<string> resultOffer = new List<string>();
		Pick(randSeed, items, in shopWeights, b.ShopItemCount, resultOffer);

		return resultOffer.ToArray();
	}

	private void SendShopRerollResult(int turnActor, int shopNonce, string[] offerArray)
	{
		Player targetPlayer = PhotonNetwork.CurrentRoom.GetPlayer(turnActor);
		if (targetPlayer != null)
		{
			photonView.RPC(nameof(RPC_ReceiveShopRerollResult), targetPlayer, turnActor, shopNonce, offerArray);
		}
	}

	[PunRPC]
	void RPC_ReceiveShopRerollResult(int turnActor, int shopNonce, string[] offerArray)
	{
		Debug.Log($"Received shop reroll result for Actor {turnActor}, nonce {shopNonce}: {string.Join(", ", offerArray)}");

		// 이벤트 호출 (구독자가 있을 경우에만 실행)
		OnShopRerollReceived?.Invoke(turnActor, shopNonce, offerArray);
	}


	//각 턴에 호출될 아이템 3개 뽑기 함수
	public string MakeOfferForTurn(int turnActor, int turnIndex)
	{
		if (!PhotonNetwork.IsMasterClient) return string.Empty;

		// 플레이어 인벤토리 확인
		string playerInv = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(turnActor));
		int playerInvCap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(turnActor));
		if (ItemInfoSerializer.isFullInventory(ItemInfoSerializer.Decode(playerInv, playerInvCap)))
		{
			return ERROR.FULL_INV.ToString();
		}

		List<ItemSO> items = ItemDB.Instance.GetItemsList();
		items = FilterNewDrugDevelopmentItem(turnActor, items);
		items = FilterAIModeBannedItems(items);
		int randSeed = GetItemRollSeed(turnActor, turnIndex);

		RarityWeights playerWeights = new(
			PhotonPropertyHelper.GetPlayerProp<float>(turnActor, PlayerPropKeys.Item_CommonWeight),
			PhotonPropertyHelper.GetPlayerProp<float>(turnActor, PlayerPropKeys.Item_HeroWeight),
			PhotonPropertyHelper.GetPlayerProp<float>(turnActor, PlayerPropKeys.Item_RareWeight),
			PhotonPropertyHelper.GetPlayerProp<float>(turnActor, PlayerPropKeys.Item_LegendaryWeight)
		);

		Pick(randSeed, items, in playerWeights, 3, currentOffer);

		return string.Join("|", currentOffer);
	}

	// MasterClient만 수행
	public void Pick(int randomSeed, List<ItemSO> items, in RarityWeights weights, int count, List<string> itemCache)
	{
		itemCache.Clear();
		scored.Clear();
		var rng = new System.Random(randomSeed);

		foreach (var it in items)
		{
			if (string.IsNullOrEmpty(it.itemId)) continue;

			float rw = weights.GetWeight(it.itemClass);
			if (rw <= 0f || it.itemWeight <= 0f) continue;

			double w = rw * it.itemWeight;
			double u = 1.0 - rng.NextDouble();
			double key = -Math.Log(u) / w;

			scored.Add((key, it.itemId));
		}

		var picked = scored
			.GroupBy(s => s.itemId)
			.Select(g => g.OrderBy(x => x.key).First())
			.OrderBy(x => x.key)
			.Take(count)
			.Select(x => x.itemId)
			.ToList();

		while (picked.Count < count) picked.Add("Error");
		itemCache.AddRange(picked);
	}

	private List<ItemSO> FilterNewDrugDevelopmentItem(int actorNum, List<ItemSO> items)
	{
		if (items == null) return new List<ItemSO>();

		items = items.Where(item => item != null && item.itemId != "5000").ToList();

		if (InventoryAuthority.Instance == null) return items;

		if (!InventoryAuthority.Instance.hasSelectedNewDrugItem(actorNum)) return items;

		return items.Where(item => item != null && item.itemId != "3001").ToList();
	}

	private List<ItemSO> FilterAIModeBannedItems(List<ItemSO> items)
	{
		if (items == null) return new List<ItemSO>();

		if (!GameManager.Instance.isSoloPlay) return items;

		return items.Where(item =>
		item != null && !AI_MODE_BANNED_ITEM_IDS.Contains(item.itemId)).ToList();
	}
}
