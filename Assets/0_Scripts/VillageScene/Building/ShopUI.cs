using System;
using System.Collections.Generic;
using Photon.Pun;
using Potan.CoreUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village.Building
{
    public class ShopUI : VillageBuildingUI
    {
        int shopNonceCounter = 0; // 상점 리롤 시마다 증가하는 nonce 카운터
        private int lastSentNonce = -1; // 내가 보낸 마지막 nonce 추적
        private bool isWaitingForResult = false; // 서버 응답 대기 상태

        [SerializeField] ShopItem shopItemPrefab;

        [SerializeField] private Button reloadButton;
        [SerializeField] private Button buyButton;

        [Header("Shop Item UI Elements")]
        [SerializeField] private ShopInventory shopInventory;
        [SerializeField] private TextMeshProUGUI reloadCostText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private TextMeshProUGUI shopEffectText;
        [SerializeField] private Transform shopItemContainer;

        private bool IsFirstShopOpen = true;
        ShopItem[] shopItems;
        int selectedItemIndex = -1;
        int reloadCount = 0;

        protected override void Awake()
        {
            base.Awake();

            reloadButton.onClick.AddListener(OnClickReloadButton);
            buyButton.onClick.AddListener(OnClickBuyButton);

            // 아이템 프리팹 생성
            int itemCount = VillageStat.VillageBalance.ShopItemCount;
            shopItems = new ShopItem[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                ShopItem newItem = Instantiate(shopItemPrefab, shopItemContainer);
                newItem.ParentShopUI = this;
                shopItems[i] = newItem;
                newItem.gameObject.SetActive(false);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            OfferAuthority.Instance.OnShopRerollReceived += OnRandonShopItemReceived;
            VillageSystem.VillageLogic.OnGoldChanged += OnGoldChanged;
        }

        private void OnGoldChanged(int gold)
        {
            RefreshStatusUI(); // 골드가 변경될 때마다 상점 UI 상태 갱신
        }

        public override void OnDisable()
        {
            if (OfferAuthority.Instance != null)
                OfferAuthority.Instance.OnShopRerollReceived -= OnRandonShopItemReceived;
            if (VillageSystem.VillageLogic != null)
                VillageSystem.VillageLogic.OnGoldChanged -= OnGoldChanged;
            base.OnDisable();
        }

        void OnRandonShopItemReceived(int targetActor, int shopNonce, string[] itemIds)
        {
            // 내 ActorNumber에 대한 응답인지 확인
            if (targetActor != PhotonNetwork.LocalPlayer.ActorNumber) return;

            // 최신 응답인지 검증 (네트워크 순서 꼬임 방지)
            if (shopNonce < lastSentNonce) return;

            DevLog.Log($"ShopUI received random shop items for nonce {shopNonce}", this);

            isWaitingForResult = false;

            for (int i = 0; i < shopItems.Length; i++)
            {
                if (i < itemIds.Length)
                {
                    var itemData = ItemDB.Instance.Get(itemIds[i]);
                    shopItems[i].SetShopItem(itemData);
                    shopItems[i].gameObject.SetActive(true);

                    // 로딩 효과 해제
                    var cg = shopItems[i].GetComponent<CanvasGroup>();
                    if (cg != null) cg.alpha = 1.0f;
                }
                else
                {
                    shopItems[i].gameObject.SetActive(false);
                }
            }

            RefreshStatusUI();
        }

        public override void SetBuildingUI(VillageType buildingType)
        {
            base.SetBuildingUI(buildingType);

            if (IsFirstShopOpen)
            {
                IsFirstShopOpen = false;
                RequestNewItems();
            }

            RefreshStatusUI();
        }

        public void OnClickReloadButton()
        {
            if (isWaitingForResult) return;

            int reloadCost = VillageStat.VillageBalance.GetShopReloadCost(reloadCount);
            if (VillageSystem.VillageLogic.GetMyGold() < reloadCost)
            {
                RefreshStatusUI();
                return;
            }

            VillageSystem.VillageLogic.AddGold(-reloadCost);
            reloadCount++;

            RequestNewItems();

            // 공용 UI(골드 텍스트)와 상점 UI 즉시 갱신
            RefreshStatusUI();
        }

        private void RequestNewItems()
        {
            isWaitingForResult = true;
            lastSentNonce = ++shopNonceCounter;

            int turnIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex) + 1;
            OfferAuthority.Instance.RequestShopReroll(PhotonNetwork.LocalPlayer.ActorNumber, turnIndex, lastSentNonce);

            // 로딩 중 효과 표시
            foreach (var item in shopItems)
            {
                if (item == null) continue;
                item.canvasGroup.alpha = 0.5f;
            }

            ShopItemSelect(null); // 리롤 시 선택 해제
        }

        public override void RefreshStatusUI()
        {
            base.RefreshStatusUI();
            shopInventory?.RefreshInventory();

            int currentLevel = VillageStat.GetVillageLevel(currentBuildingType) + 1; // 레벨은 0부터 시작하므로 +1

            float rareChance = VillageStat.VillageBalance.ShopRareChanceBase + (currentLevel * VillageStat.VillageBalance.ShopRareChanceMultiplier);
            float heroChance = 0f;
            if (currentLevel >= VillageStat.VillageBalance.ShopHeroMinLevel)
            {
                int heroLevelOffset = currentLevel - VillageStat.VillageBalance.ShopHeroMinLevel;
                heroChance = VillageStat.VillageBalance.ShopHeroChanceBase + (heroLevelOffset * VillageStat.VillageBalance.ShopHeroChanceMultiplier);
            }

            float legendaryChance = 0f;
            if (currentLevel >= VillageStat.VillageBalance.ShopLegendaryMinLevel)
            {
                int legendaryLevelOffset = currentLevel - VillageStat.VillageBalance.ShopLegendaryMinLevel;
                legendaryChance = VillageStat.VillageBalance.ShopLegendaryChanceBase + (legendaryLevelOffset * VillageStat.VillageBalance.ShopLegendaryChanceMultiplier);
            }



            shopEffectText.SetText(
                LocalizationManager.Instance.GetText(CSV_Type.Village, "Shop_Effect"),
                rareChance * 100f,
                heroChance * 100f,
                legendaryChance * 100f
            );

            int reloadCost = VillageStat.VillageBalance.GetShopReloadCost(reloadCount);
            int currentGold = VillageSystem.VillageLogic.GetMyGold();
            bool isInventoryFull = IsMyInventoryFull();

            reloadCostText.SetText(LocalizationManager.Instance.GetText(CSV_Type.Village, "Shop_Reload"), reloadCost);

            // 대기 중이 아니고 돈이 충분할 때만 버튼 활성화
            reloadButton.interactable = !isWaitingForResult && currentGold >= reloadCost;

            // 선택된 아이템이 있을 때만 설명과 구매 버튼 활성화
            if (selectedItemIndex < 0 || selectedItemIndex >= shopItems.Length)
            {
                itemDescriptionText.SetText(string.Empty);
                buyButton.interactable = false;
            }
            else
            {
                ShopItem selectedItem = shopItems[selectedItemIndex];
                if (selectedItem != null)
                {
                    itemDescriptionText.SetText(selectedItem.GetItemDescription());
                    // 구매 버튼은 대기 중이 아니고, 아이템 가격보다 골드가 충분할 때만 활성화

                    buyButton.interactable = !isWaitingForResult && !isInventoryFull && currentGold >= selectedItem.Price;
                }
                else
                {
                    // 선택된 아이템이 null인 경우 설명 초기화 및 구매 버튼 비활성화
                    itemDescriptionText.SetText(string.Empty);
                    buyButton.interactable = false;
                }
            }
        }

        // 구매 버튼(Buy Button) 클릭 시 실행되는 함수
        private void OnClickBuyButton()
        {
            // 1. 현재 선택된 아이템이 유효한지 검사합니다.
            if (selectedItemIndex < 0 || selectedItemIndex >= shopItems.Length) return;

            // 인벤토리가 가득 찼다면 구매를 막습니다.
            if (IsMyInventoryFull())
            {
                RefreshStatusUI();
                return;
            }

            ShopItem selectedItem = shopItems[selectedItemIndex];
            if (selectedItem == null || selectedItem.IsEmpty) return;

            // 2. 아이템 정보(ID, 가격)를 가져옵니다.
            int price = selectedItem.Price;
            string itemId = selectedItem.ItemData.itemId;

            // 3. 골드가 충분한지 확인합니다. 부족하다면 UI를 갱신하여 구매 불가 상태를 반영합니다.
            if (VillageSystem.VillageLogic.GetMyGold() < price)
            {
                RefreshStatusUI(); // UI 갱신 (버튼 비활성화 등)
                return;
            }

            // 골드 제거
            VillageSystem.VillageLogic.AddGold(-price);

            // 4. [서버 요청] 인벤토리 권한을 관리하는 Singleton 인스턴스에 구매를 요청합니다.
            // 이 요청은 RPC를 통해 MasterClient에서 최종 검증 및 처리가 이루어집니다.
            if (InventoryAuthority.Instance != null)
            {
                InventoryAuthority.Instance.RequestBuyShopItem(
                    PhotonNetwork.LocalPlayer.ActorNumber,
                    itemId,
                    price
                );

                // 5. [UI 처리] 구매 요청을 보낸 후 즉시 아이템 선택을 해제하여 중복 클릭을 방지합니다.
                shopItems[selectedItemIndex].SetShopItem(null); // 선택된 아이템 UI 초기화
                ShopItemSelect(null);
                RefreshStatusUI();
            }
            else
            {
                DevLog.LogError("InventoryAuthority Instance is null! GameScene이 제대로 로드되었는지 확인하세요.", this);
            }
        }

        private bool IsMyInventoryFull()
        {
            int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
            string playerInv = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(myActor));
            int playerInvCap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(myActor));

            return ItemInfoSerializer.isFullInventory(ItemInfoSerializer.Decode(playerInv, playerInvCap));
        }

        public void ShopItemSelect(ShopItem shopItem)
        {
            selectedItemIndex = -1;
            for (int i = 0; i < shopItems.Length; i++)
            {
                bool isSelectedItem = shopItems[i] == shopItem;
                shopItems[i].SetSelected(isSelectedItem);
                if (isSelectedItem) selectedItemIndex = i;
            }
            RefreshStatusUI();
        }
    }
}
