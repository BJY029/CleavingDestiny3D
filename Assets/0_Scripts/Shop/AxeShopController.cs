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
        if (retryButton != null) retryButton.onClick.AddListener(RetryLoad);
        IsShopReady = false;
        SetStatus("상점 정보를 불러오는 중…", false);
        BuildShop();
        RefreshAll();
        LoadShop(false);
    }

    private void OnDisable()
    {
        activation++;
        IsShopReady = false;
        if (ugs != null) ugs.WalletChanged -= HandleWalletChanged;
        PlayerProfile.OnAxeSkinChanged -= RefreshAll;
        if (retryButton != null) retryButton.onClick.RemoveListener(RetryLoad);
    }

    private bool IsCurrent(int token) => this != null && isActiveAndEnabled && token == activation;

    public void RetryLoad() => LoadShop(true);

    private async void LoadShop(bool refreshWallet)
    {
        if (!isActiveAndEnabled || isLoading || isBuying) return;
        int token = ++activation;
        isLoading = true;
        IsShopReady = false;
        SetStatus("상점 정보를 불러오는 중…", false);
        RefreshAll();
        try
        {
            BuildShop();
            await ugs.LoadShopDataAsync(refreshWallet);
            if (!IsCurrent(token)) return;
            IsShopReady = isBuilt && catalog.TryGetSkin("axe_basic", out _) && ugs.IsWalletReady;
            SetStatus(IsShopReady ? "" : "기본 도끼와 상점 설정을 확인하세요.", !IsShopReady);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (IsCurrent(token)) SetStatus("상점 조회 실패 · 다시 시도해 주세요.", true);
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
        if (catalog.TryGetSkin("axe_basic", out var defaultSkin)) AddItem(defaultSkin, ids);
        foreach (var skin in catalog.Skins) AddItem(skin, ids);
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
        if (skin == null || !PlayerProfile.OwnsAxeSkin(skin.SkinId)) return AxeSkinState.Locked;
        return PlayerProfile.EquippedAxeSkinId == skin.SkinId ? AxeSkinState.Equipped : AxeSkinState.Owned;
    }

    public bool TryGetPrice(AxeSkinSO skin, out AxeShopPrice price)
    {
        price = null;
        return skin != null && IsShopReady && ugs != null && catalog.TryGetSkin(skin.SkinId, out _) &&
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
        SetStatus("구매 처리 중…", false);
        RefreshAll();
        try
        {
            var result = await ugs.PurchaseSkinAsync(skin.SkinId);
            if (!IsCurrent(token)) return;
            switch (result.status)
            {
                case "purchased":
                    SetStatus("구매 완료 · 장착 버튼으로 선택하세요.", false);
                    AudioManager.Instance?.PlaySfx2D("WoodPurchase");
                    break;
                case "already_owned": SetStatus("이미 보유한 도끼입니다.", false); break;
                case "insufficient_funds":
                    SetStatus(price.Currency == "coins" ? "Coin이 부족합니다." : "나뭇가지가 부족합니다.", false);
                    break;
                default: SetStatus("판매 정보가 없습니다.", false); break;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            try
            {
                // A lost response can still mean the server completed the purchase.
                await ugs.LoadPlayerData();
                if (IsCurrent(token))
                    SetStatus(PlayerProfile.OwnsAxeSkin(skin.SkinId) ? "구매 상태를 복구했습니다." : "구매 실패 · 다시 시도해 주세요.", false);
            }
            catch (Exception refreshError)
            {
                Debug.LogException(refreshError);
                if (IsCurrent(token))
                {
                    IsShopReady = false;
                    SetStatus("구매 확인 실패 · 잔액을 다시 조회해 주세요.", true);
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
        SetStatus("도끼를 장착했습니다.", false);
    }

    private void HandleWalletChanged() => RefreshAll();
    private void SetStatus(string message, bool retry)
    {
        if (statusText != null) statusText.text = message;
        if (retryButton != null) retryButton.gameObject.SetActive(retry);
    }
    private void RefreshAll()
    {
        if (!isActiveAndEnabled) return;
        if (catalog != null && catalog.TryGetSkin(PlayerProfile.EquippedAxeSkinId, out var skin))
        {
            if (EquippedAxeIcon != null) EquippedAxeIcon.sprite = skin.Icon;
            if (EquippedAxeName != null) EquippedAxeName.text = skin.DisplayName;
        }
        foreach (var item in itemUIs) if (item != null) item.Refresh();
    }
}
