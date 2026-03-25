using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village.Building
{
    public class ShopUI : VillageBuilldingUI
    {
        static int shopNonceCounter = 0; // 상점 리롤 시마다 증가하는 nonce 카운터
        private int lastSentNonce = -1; // 내가 보낸 마지막 nonce 추적
        private bool isWaitingForResult = false; // 서버 응답 대기 상태

        [SerializeField] ShopItem shopItemPrefab;

        [SerializeField] private Button reloadButton;
        [SerializeField] private Button buyButton;

        [Header("Shop Item UI Elements")]
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

        void OnEnable()
        {
            OfferAuthority.Instance.OnShopRerollReceived += OnRandonShopItemReceived;
        }

        void OnDisable()
        {
            if (OfferAuthority.Instance != null)
                OfferAuthority.Instance.OnShopRerollReceived -= OnRandonShopItemReceived;
        }

        void OnRandonShopItemReceived(int shopNonce, string[] itemIds)
        {
            // 최신 응답인지 검증 (네트워크 순서 꼬임 방지)
            if (shopNonce < lastSentNonce) return;

            Debug.Log($"ShopUI received random shop items for nonce {shopNonce}");

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

            RefreshShopStatusUI();
        }

        public override void SetBuildingUI(VillageType buildingType)
        {
            base.SetBuildingUI(buildingType);

            if (IsFirstShopOpen)
            {
                IsFirstShopOpen = false;
                RequestNewItems();
            }

            RefreshShopStatusUI();
        }

        public void OnClickReloadButton()
        {
            if (isWaitingForResult) return;

            int reloadCost = VillageStat.VillageBalance.GetShopReloadCost(reloadCount);
            if (VillageSystem.VillageLogic.GetMyGold() < reloadCost)
            {
                RefreshStatusUI(); // 현재 골드 텍스트 갱신
                RefreshShopStatusUI();
                return;
            }

            VillageSystem.VillageLogic.AddGold(-reloadCost);
            reloadCount++;

            RequestNewItems();

            // 공용 UI(골드 텍스트)와 상점 UI 즉시 갱신
            RefreshStatusUI();
            RefreshShopStatusUI();
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
                var cg = item.GetComponent<CanvasGroup>();
                if (cg == null) cg = item.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0.5f;
            }

            ShopItemSelect(null); // 리롤 시 선택 해제
        }

        private void RefreshShopStatusUI()
        {
            int currentLevel = VillageStat.GetVillageLevel(currentBuildingType);
            float rareChance = VillageStat.VillageBalance.ShopRareChanceBase + (currentLevel * VillageStat.VillageBalance.ShopRareChanceMultiplier);
            shopEffectText.SetText(LocalizationManager.Instance.GetText(CSV_Type.Village, "Shop_Effect"), currentLevel, rareChance * 100f);

            int reloadCost = VillageStat.VillageBalance.GetShopReloadCost(reloadCount);
            int currentGold = VillageSystem.VillageLogic.GetMyGold();

            reloadCostText.SetText(LocalizationManager.Instance.GetText(CSV_Type.Village, "Shop_Reload"), reloadCost);

            // 대기 중이 아니고 돈이 충분할 때만 버튼 활성화
            reloadButton.interactable = !isWaitingForResult && currentGold >= reloadCost;

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
                    buyButton.interactable = !isWaitingForResult && currentGold >= 0; // TODO: 가격 조건 추가
                }
                else
                {
                    itemDescriptionText.SetText(string.Empty);
                    buyButton.interactable = false;
                }
            }
        }

        private void OnClickBuyButton()
        {
            Debug.Log("ShopUI: Buy Button Clicked");
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
            RefreshShopStatusUI();
        }
    }
}
