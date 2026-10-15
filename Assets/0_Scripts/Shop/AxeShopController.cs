using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AxeSkinState
{
    Locked,
    Owned,
    Equipped
}

public class AxeShopController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private AxeSkinCatalogSO catalog;

    [Header("UIs")]
    [SerializeField] private AxeShopItemUI itemPrefab;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Image EquippedAxeIcon;
    [SerializeField] private TextMeshProUGUI EquippedAxeName;

    private readonly List<AxeShopItemUI> itemUIs = new();

    private bool isBuilt;
    private bool isBuying;
    private UGSManager ugs;
    public bool IsShopReady { get; private set; }
    public bool IsBusy => isBuying || !IsShopReady;

    private async void OnEnable()
    {
        PlayerProfile.OnBranchCountChanged += HandleBranchCountChanged;
        PlayerProfile.OnAxeSkinChanged += HandleAxeSkinChanged;
        ugs = UGSManager.Instance;
        ugs.WalletChanged += HandleWalletChanged;
        IsShopReady = false;

        if (!isBuilt) BuildShop();

        RefreshAll();
        try
        {
            await UGSManager.Instance.LoadSkinCatalogAsync();
            if (this != null && isActiveAndEnabled) IsShopReady = UGSManager.Instance.IsWalletReady;
        }
        catch (System.Exception e) { Debug.LogException(e); }
        finally { if (this != null && isActiveAndEnabled) RefreshAll(); }
    }

    private void OnDisable()
    {
        PlayerProfile.OnBranchCountChanged -= HandleBranchCountChanged;
        PlayerProfile.OnAxeSkinChanged -= HandleAxeSkinChanged;
        if (ugs != null) ugs.WalletChanged -= HandleWalletChanged;
    }

    private void BuildShop()
    {
        if (catalog == null || itemPrefab == null || contentRoot == null)
        {
            Debug.LogError("[AxeShopController] 상점 설정이 올바르지 않습니다.", this);
            return;
        }

        foreach (AxeSkinSO skin in catalog.Skins)
        {
            if (skin == null) continue;

            AxeShopItemUI itemUI = Instantiate(itemPrefab, contentRoot);
            itemUI.Initialize(skin, this);

            itemUIs.Add(itemUI);
        }

        isBuilt = true;
    }

    public AxeSkinState GetSkinState(AxeSkinSO skin)
    {
        if (skin == null)
            return AxeSkinState.Locked;

        if (!PlayerProfile.OwnsAxeSkin(skin.SkinId))
            return AxeSkinState.Locked;

        if (PlayerProfile.EquippedAxeSkinId == skin.SkinId)
            return AxeSkinState.Equipped;

        return AxeSkinState.Owned;

    }

    public bool TryGetPrice(AxeSkinSO skin, out AxeShopPrice price)
    {
        price = null;
        return skin != null && IsShopReady && UGSManager.Instance.TryGetSkinPrice(skin.SkinId, out price);
    }

    public bool CanPurchase(AxeSkinSO skin)
    {
        return !IsBusy && TryGetPrice(skin, out var price) &&
            UGSManager.Instance.PlayerData.TryGetValue(price.Currency, out int balance) && balance >= price.Price;
    }

    public async void TryPurchase(AxeSkinSO skin)
    {
        if (skin == null || IsBusy || !TryGetPrice(skin, out _)) return;
        isBuying = true;
        RefreshAll();
        try
        {
            var response = await UGSManager.Instance.PurchaseSkinAsync(skin.SkinId);
            switch (response.status)
            {
                case "purchased":
                    Debug.Log($"[AxeShop] 구매 성공: {skin.DisplayName} / {response.currency} {response.price}");
                    AudioManager.Instance?.PlaySfx2D("WoodPurchase");
                    break;
                case "already_owned":
                    Debug.Log("[AxeShop] 이미 보유한 스킨입니다. 서버 구매 목록을 반영했습니다.");
                    break;
                case "insufficient_funds":
                    Debug.Log("[AxeShop] 구매에 필요한 재화가 부족합니다.");
                    break;
                case "skin_not_found":
                    Debug.LogError("[AxeShop] 서버 SKIN_CATALOG에 없는 스킨입니다: " + skin.SkinId);
                    break;
            }
        }
        catch (System.Exception e) { Debug.LogException(e); }
        finally
        {
            isBuying = false;
            if (this != null) RefreshAll();
        }
    }

    public void TryEquip(AxeSkinSO skin)
    {
        if (skin == null || IsBusy) return;

        if (!PlayerProfile.EquipAxeSkin(skin.SkinId))
            return;

        SaveManager.Save();

        AudioManager.Instance.PlaySfx2D("EquipItem");
        Debug.Log($"[AxeShop] 장착 : {skin.DisplayName}");
    }

    private void HandleBranchCountChanged(int count)
    {
        RefreshAll();
    }

    private void HandleAxeSkinChanged()
    {
        RefreshAll();
    }

    private void HandleWalletChanged() => RefreshAll();

    private void RefreshAll()
    {
        if (catalog == null) return;
        catalog.TryGetSkin(PlayerProfile.EquippedAxeSkinId, out AxeSkinSO skin);

        if (skin != null)
        {
            EquippedAxeIcon.sprite = skin.Icon;
            EquippedAxeName.text = skin.DisplayName;
        }

        foreach (AxeShopItemUI itemUI in itemUIs)
        {
            if (itemUI != null) itemUI.Refresh();
        }
    }
}
