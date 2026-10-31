using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AxeSkinState { Locked, Owned, Equipped }

public class AxeShopController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private AxeSkinCatalogSO catalog;

    [Header("UIs")]
    [SerializeField] private AxeShopItemUI itemPrefab;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Image EquippedAxeIcon;
    [SerializeField] private TextMeshProUGUI EquippedAxeName;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button retryButton;

    private readonly List<AxeShopItemUI> itemUIs = new();
    private UGSManager ugs;
    private bool isBuilt, isBuying, isLoading;
    private int activation;

    public bool IsShopReady { get; private set; }
    public bool IsBusy => !isActiveAndEnabled || isBuying || isLoading || !IsShopReady;

    private void OnEnable()
    {
        ugs = UGSManager.Instance;
        ugs.WalletChanged += HandleWalletChanged;
        PlayerProfile.OnAxeSkinChanged += RefreshAll;

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLoad);

        IsShopReady = false;
        SetStatus("Shop_Loading", false);

        BuildShop();
        RefreshAll();
        LoadShop(false);
    }

    private void OnDisable()
    {
        activation++;
        IsShopReady = false;

        if (ugs != null)
            ugs.WalletChanged -= HandleWalletChanged;

        PlayerProfile.OnAxeSkinChanged -= RefreshAll;

        if (retryButton != null)
            retryButton.onClick.RemoveListener(RetryLoad);
    }

    private bool IsCurrent(int token) => this != null && isActiveAndEnabled && token == activation;

    public void RetryLoad() => LoadShop(true);

    private async void LoadShop(bool refreshWallet)
    {
        if (!isActiveAndEnabled || isLoading || isBuying) return;

        int token = ++activation;
        isLoading = true;
        IsShopReady = false;

        SetStatus("Shop_Loading", false);
        RefreshAll();

        try
        {
            await ugs.LoadShopDataAsync(refreshWallet);

            if (!IsCurrent(token)) return;

            IsShopReady = isBuilt && catalog.TryGetSkin("axe_basic", out _) && ugs.IsWalletReady;
            SetStatus(IsShopReady ? "" : "AxeShop_ConfigError", !IsShopReady);
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            if (IsCurrent(token))
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
        if (isBuilt || catalog == null || itemPrefab == null || contentRoot == null) return;

        var ids = new HashSet<string>(StringComparer.Ordinal);

        if (catalog.TryGetSkin("axe_basic", out var defaultSkin))
            AddItem(defaultSkin, ids);

        foreach (var skin in catalog.Skins)
            AddItem(skin, ids);

        isBuilt = true;
    }

    private void AddItem(AxeSkinSO skin, HashSet<string> ids)
    {
        if (skin == null || string.IsNullOrWhiteSpace(skin.SkinId) || !ids.Add(skin.SkinId)) return;

        var item = Instantiate(itemPrefab, contentRoot);
        item.Initialize(skin, this);
        itemUIs.Add(item);
    }

    public AxeSkinState GetSkinState(AxeSkinSO skin)
    {
        if (skin == null || !PlayerProfile.OwnsAxeSkin(skin.SkinId))
            return AxeSkinState.Locked;

        return PlayerProfile.EquippedAxeSkinId == skin.SkinId ? AxeSkinState.Equipped : AxeSkinState.Owned;
    }

    public bool TryGetPrice(AxeSkinSO skin, out AxeShopPrice price)
    {
        price = null;

        return skin != null && IsShopReady && ugs != null &&
            catalog.TryGetSkin(skin.SkinId, out _) &&
            ugs.TryGetSkinPrice(skin.SkinId, out price);
    }

    public bool CanPurchase(AxeSkinSO skin) => !IsBusy && skin != null && ugs.IsWalletReady &&
        !PlayerProfile.OwnsAxeSkin(skin.SkinId) && TryGetPrice(skin, out var price) &&
        ugs.PlayerData.TryGetValue(price.Currency, out int balance) && balance >= price.Price;

    public async void TryPurchase(AxeSkinSO skin)
    {
        if (!CanPurchase(skin) || !TryGetPrice(skin, out var price)) return;

        int token = activation;
        isBuying = true;

        SetStatus("Shop_Purchasing", false);
        RefreshAll();

        try
        {
            var result = await ugs.PurchaseSkinAsync(skin.SkinId);

            if (!IsCurrent(token)) return;

            switch (result.status)
            {
                case "purchased":
                    SetStatus("Shop_PurchaseComplete", false);
                    AudioManager.Instance?.PlaySfx2D("WoodPurchase");
                    break;

                case "already_owned":
                    SetStatus("AxeShop_AlreadyOwned", false);
                    break;

                case "insufficient_funds":
                    SetStatus(price.Currency == "coins" ? "Shop_NotEnoughCoins" : "Shop_NotEnoughSticks", false);
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
                // 응답을 받지 못했더라도 서버에서 구매가 완료되었을 수 있습니다.
                await ugs.LoadPlayerData();

                if (IsCurrent(token))
                    SetStatus(PlayerProfile.OwnsAxeSkin(skin.SkinId) ? "Shop_PurchaseRecovered" : "Shop_PurchaseFailed", false);
            }
            catch (Exception refreshError)
            {
                Debug.LogException(refreshError);

                if (IsCurrent(token))
                {
                    IsShopReady = false;
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
                    if (token != activation) RetryLoad();
                    else RefreshAll();
                }
            }
        }
    }

    public void TryEquip(AxeSkinSO skin)
    {
        if (IsBusy || skin == null || !catalog.TryGetSkin(skin.SkinId, out _) || !PlayerProfile.EquipAxeSkin(skin.SkinId)) return;

        SaveManager.Save();
        AudioManager.Instance?.PlaySfx2D("EquipItem");
        SetStatus("AxeShop_Equipped", false);
    }

    private void HandleWalletChanged() => RefreshAll();

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

        if (catalog != null && catalog.TryGetSkin(PlayerProfile.EquippedAxeSkinId, out var skin))
        {
            if (EquippedAxeIcon != null)
                EquippedAxeIcon.sprite = skin.Icon;

            if (EquippedAxeName != null)
                EquippedAxeName.text = skin.DisplayName;
        }

        foreach (var item in itemUIs)
        {
            if (item != null)
                item.Refresh();
        }
    }
}