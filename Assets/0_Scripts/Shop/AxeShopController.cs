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

public enum AxePurchaseResult
{
    Success,
    InvalidSkin,
    AlreadyOwned,
    NotEnoughBranch
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

    private void OnEnable()
    {
        PlayerProfile.OnBranchCountChanged += HandleBranchCountChanged;
        PlayerProfile.OnAxeSkinChanged += HandleAxeSkinChanged;

        if (!isBuilt) BuildShop();

        RefreshAll();
    }

    private void OnDisable()
    {
        PlayerProfile.OnBranchCountChanged -= HandleBranchCountChanged;
        PlayerProfile.OnAxeSkinChanged -= HandleAxeSkinChanged;
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

    public void TryPurchase(AxeSkinSO skin)
    {
        if (skin == null) return;

        AxePurchaseResult result = PlayerProfile.TryPurchaseAxeSkin(skin.SkinId, skin.Price);

        switch (result)
        {
            case AxePurchaseResult.Success:
                SaveManager.Save();
                Debug.Log($"[AxeShop] 구매 성공 : {skin.DisplayName}");
                break;
            case AxePurchaseResult.NotEnoughBranch:
                Debug.Log("[AxeShop] 나뭇가지가 부족합니다.");
                break;
            case AxePurchaseResult.AlreadyOwned:
                Debug.Log("[AxeShop] 이미 보유한 스킨입니다.");
                break;
            case AxePurchaseResult.InvalidSkin:
                Debug.LogError("[AxeShop] 잘못된 스킨 정보입니다.");
                break;
        }
    }

    public void TryEquip(AxeSkinSO skin)
    {
        if (skin == null) return;

        if (!PlayerProfile.EquipAxeSkin(skin.SkinId))
            return;

        SaveManager.Save();

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

    private void RefreshAll()
    {
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
