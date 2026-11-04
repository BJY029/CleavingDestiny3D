using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterSkinState { Locked, Owned, Equipped }

public class CharacterShopController : MonoBehaviour
{
    [SerializeField] private CharacterSkinCatalogSO catalog;
    [SerializeField] private CharacterShopItemUI itemPrefab;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Image EquippedCharacterIcon;
    [SerializeField] private TextMeshProUGUI EquippedCharacterName;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button retryButton;

    private readonly List<CharacterShopItemUI> itemUIs = new();
    private UGSManager ugs;
    private bool isBuilt, isBuying, isLoading;

    //현재 작업이 유효한지 확인하는 번호
    //각 함수에서 로컬 변수 token과 함께 사용됨. token은 작업을 시작할 때 기억해 둔 세대번호
    //token != activation이면 비동기 작업 중 대기가 시작한 뒤 사용자 인터렉션으로 상점 상태가 바뀐 것. 
    // 이 경우, 비동기 작업 결괄르 반영하지 않도록 한다.
    private int activation;

    public bool IsShopReady { get; private set; }
    public bool IsBusy => isBuying || isLoading || !IsShopReady;

    private void OnEnable()
    {
        ugs = UGSManager.Instance;
        ugs.WalletChanged += HandleWalletChanged;
        PlayerProfile.OnCharacterSkinChanged += RefreshAll;

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLoad);

        BuildShop();
        LoadShop(false);
    }

    private void OnDisable()
    {
        activation++;

        if (ugs != null)
            ugs.WalletChanged -= HandleWalletChanged;

        PlayerProfile.OnCharacterSkinChanged -= RefreshAll;

        if (retryButton != null)
            retryButton.onClick.RemoveListener(RetryLoad);
    }

    public void RetryLoad() => LoadShop(true);

    private async void LoadShop(bool refreshWallet)
    {
        if (isLoading || isBuying) return;

        int token = ++activation;
        isLoading = true;
        IsShopReady = false;

        SetStatus("Shop_Loading", false);
        RefreshAll();

        try
        {
            await ugs.LoadShopDataAsync(refreshWallet);

            if (this == null || !isActiveAndEnabled || token != activation) return;

            IsShopReady = catalog != null && catalog.DefaultSkin != null && ugs.IsWalletReady;

            SyncOwnership();
            SetStatus(IsShopReady ? "" : "CharacterShop_ConfigError", !IsShopReady);
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            if (this != null && isActiveAndEnabled && token == activation)
                SetStatus("Shop_LoadFailed", true);
        }
        finally
        {
            if (this != null)
            {
                isLoading = false;

                if (isActiveAndEnabled)
                {
                    if (token != activation) LoadShop(false);
                    else RefreshAll();
                }
            }
        }
    }

    private void BuildShop()
    {
        if (isBuilt) return;
        if (catalog == null || itemPrefab == null || contentRoot == null) return;

        var ids = new HashSet<string>();

        AddItem(catalog.DefaultSkin, ids);

        foreach (var skin in catalog.Skins)
            AddItem(skin, ids);

        isBuilt = true;
    }

    private void AddItem(CharacterSkinSO skin, HashSet<string> ids)
    {
        if (skin == null || !ids.Add(skin.SkinId)) return;

        var item = Instantiate(itemPrefab, contentRoot);
        item.Initialize(skin, this);
        itemUIs.Add(item);
    }

    public CharacterSkinState GetSkinState(CharacterSkinSO skin)
    {
        if (skin == null || !PlayerProfile.OwnsCharacterSkin(skin.SkinId))
            return CharacterSkinState.Locked;

        return PlayerProfile.EquippedCharacterSkinId == skin.SkinId ? CharacterSkinState.Equipped : CharacterSkinState.Owned;
    }

    public bool TryGetPrice(CharacterSkinSO skin, out AxeShopPrice price)
    {
        price = null;

        return skin != null && IsShopReady && catalog.TryGetSkin(skin.SkinId, out _) &&
            ugs.TryGetSkinPrice(skin.SkinId, out price) && price.Currency == "coins";
    }

    public bool CanPurchase(CharacterSkinSO skin) => !IsBusy && skin != null &&
        !PlayerProfile.OwnsCharacterSkin(skin.SkinId) && TryGetPrice(skin, out var price) &&
        ugs.PlayerData.TryGetValue("coins", out int coins) && coins >= price.Price;

    public async void TryPurchase(CharacterSkinSO skin)
    {
        if (!CanPurchase(skin)) return;

        int token = activation;
        isBuying = true;

        SetStatus("Shop_Purchasing", false);
        RefreshAll();

        try
        {
            var result = await ugs.PurchaseSkinAsync(skin.SkinId);

            if (this == null || !isActiveAndEnabled) return;

            SyncOwnership();

            switch (result.status)
            {
                case "purchased":
                    SetStatus("Shop_PurchaseComplete", false);
                    AudioManager.Instance?.PlaySfx2D("CoinPurchase");
                    break;

                case "already_owned":
                    SetStatus("CharacterShop_AlreadyOwned", false);
                    break;

                case "insufficient_funds":
                    SetStatus("Shop_NotEnoughCoins", false);
                    break;

                default:
                    SetStatus("Shop_NoSaleInfo", false);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            try
            {
                await ugs.LoadPlayerData();

                if (this != null && isActiveAndEnabled)
                {
                    SyncOwnership();
                    SetStatus(PlayerProfile.OwnsCharacterSkin(skin.SkinId) ? "Shop_PurchaseRecovered" : "Shop_PurchaseFailed", false);
                }
            }
            catch (Exception refreshError)
            {
                Debug.LogException(refreshError);

                if (this != null)
                {
                    IsShopReady = false;

                    if (isActiveAndEnabled)
                        SetStatus("Shop_VerificationFailed", true);
                }
            }
        }
        finally
        {
            if (this != null)
            {
                isBuying = false;

                if (isActiveAndEnabled)
                {
                    if (token != activation)
                    {
                        RetryLoad();
                    }
                    else
                    {
                        SyncOwnership();
                        RefreshAll();
                    }
                }
            }
        }
    }

    public void TryEquip(CharacterSkinSO skin)
    {
        if (IsBusy || skin == null || !catalog.TryGetSkin(skin.SkinId, out _) || !PlayerProfile.EquipCharacterSkin(skin.SkinId)) return;

        SaveManager.Save();
        AudioManager.Instance?.PlaySfx2D("EquipItem");
        SetStatus("CharacterShop_Equipped", false);
    }

    private void HandleWalletChanged()
    {
        SyncOwnership();
        RefreshAll();
    }

    private void SyncOwnership()
    {
        if (ugs == null || !ugs.IsWalletReady || catalog == null) return;

        PlayerProfile.ApplyCharacterOwnership(catalog, ugs.OwnedItems);
        SaveManager.Save();
    }

    private string GetLocalString(string id)
    {
        if (string.IsNullOrEmpty(id)) return "";
        return LocalizationManager.Instance.GetText(CSV_Type.Shop, id);
    }

    private void SetStatus(string id, bool retry)
    {
        if (statusText != null)
            statusText.text = GetLocalString(id);

        if (retryButton != null)
            retryButton.gameObject.SetActive(retry);
    }

    private void RefreshAll()
    {
        if (!isActiveAndEnabled) return;

        if (catalog != null && catalog.TryGetSkin(PlayerProfile.EquippedCharacterSkinId, out var skin))
        {
            if (EquippedCharacterIcon != null)
                EquippedCharacterIcon.sprite = skin.Icon;

            if (EquippedCharacterName != null)
                EquippedCharacterName.text = skin.DisplayName;
        }

        foreach (var item in itemUIs)
        {
            if (item != null)
                item.Refresh();
        }
    }
}