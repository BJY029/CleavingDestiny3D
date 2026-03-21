using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village.Building
{

    public class ShopUI : VillageBuilldingUI
    {
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
            buyButton.onClick.AddListener(OnClickBuyButton); // 구매 버튼은 업그레이드 버튼과 동일한 로직으로 처리

            // 아이템 프리팹 생성
            int itemCount = VillageStat.VillageBalance.ShopItemCount;
            shopItems = new ShopItem[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                ShopItem newItem = Instantiate(shopItemPrefab, shopItemContainer);
                newItem.ParentShopUI = this; // 생성된 아이템에 ShopUI 참조 설정
                shopItems[i] = newItem;
                newItem.gameObject.SetActive(false); // 초기에는 비활성화
            }
        }

        public override void SetBuildingUI(VillageType buildingType)
        {
            base.SetBuildingUI(buildingType);

            RefreshShopStatusUI();

            // 상점은 처음 열 때 아이템을 세팅해주고, 이후에는 새로고침 버튼을 눌렀을 때 아이템이 세팅되도록 함
            if (IsFirstShopOpen)
            {
                IsFirstShopOpen = false;
                ReloadShop();
            }
        }

        // 상점 물품 새로고침 + 돈 소모
        public void OnClickReloadButton()
        {
            int reloadCost = VillageStat.VillageBalance.GetShopReloadCost(reloadCount);
            if (VillageSystem.VillageLogic.GetMyGold() < reloadCost)
            {
                RefreshShopStatusUI();
                return;
            }

            VillageSystem.VillageLogic.AddGold(-reloadCost);

            reloadCount++;
            ReloadShop();

            RefreshStatusUI();
            RefreshShopStatusUI();
        }

        // 상점 물품 새로고침
        private void ReloadShop()
        {
            Debug.Log("ShopUI: Shop Reloaded");
            ShopItemSelect(null); // 아이템 선택 해제

            foreach (var item in shopItems)
            {
                if (item == null) continue;
                var itemData = ItemDB.Instance.Get("1000");
                item.gameObject.SetActive(true);
                item.SetShopItem(itemData);
            }
        }

        private void RefreshShopStatusUI()
        {
            int currentLevel = VillageStat.GetVillageLevel(currentBuildingType);
            float rareAppearanceChance = VillageStat.VillageBalance.ShopRareItemChanceBase + (currentLevel * VillageStat.VillageBalance.ShopRareItemChanceMultiplier);
            shopEffectText.SetText(LocalizationManager.Instance.GetText(CSV_Type.Village, "Shop_Effect"), currentLevel, rareAppearanceChance * 100f);

            int reloadCost = VillageStat.VillageBalance.GetShopReloadCost(reloadCount);
            reloadCostText.SetText(LocalizationManager.Instance.GetText(CSV_Type.Village, "Shop_Reload"), reloadCost);
            reloadButton.interactable = VillageSystem.VillageLogic.GetMyGold() >= reloadCost;

            if (selectedItemIndex < 0 || selectedItemIndex >= shopItems.Length)
            {
                itemDescriptionText.SetText(string.Empty);
                buyButton.interactable = false;
            }
            else
            {
                // 선택된 아이템의 설명을 가져와서 UI에 표시
                ShopItem selectedItem = shopItems[selectedItemIndex];
                if (selectedItem != null)
                {
                    itemDescriptionText.SetText(selectedItem.GetItemDescription());

                    // TODO: 아이템 구매 가능 여부에 따라 구매 버튼 활성화/비활성화 처리 (예: 골드 부족 시 비활성화)
                    buyButton.interactable = true; // 아이템이 선택되면 구매 버튼 활성화
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
            selectedItemIndex = -1; // 일단 선택된 아이템 인덱스 초기화
            // 클릭된 아이템이 shopItems 배열에서 몇 번째 인덱스인지 찾기
            for (int i = 0; i < shopItems.Length; i++)
            {
                bool isSelectedItem = shopItems[i] == shopItem;
                shopItems[i].SetSelected(isSelectedItem); // 클릭된 아이템은 선택 상태로, 나머지는 선택 해제
                if (isSelectedItem)
                {
                    selectedItemIndex = i;
                }
            }

            RefreshShopStatusUI();
        }
    }
}