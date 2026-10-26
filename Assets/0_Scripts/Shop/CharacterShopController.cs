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
    private int activation;
    public bool IsShopReady { get; private set; }
    public bool IsBusy => isBuying || isLoading || !IsShopReady;

    private void OnEnable()
    {
        ugs = UGSManager.Instance;
        ugs.WalletChanged += HandleWalletChanged;
        PlayerProfile.OnCharacterSkinChanged += RefreshAll;
        if (retryButton != null) retryButton.onClick.AddListener(RetryLoad);
        BuildShop();
        RetryLoad();
    }

    private void OnDisable()
    {
        activation++;
        if (ugs != null) ugs.WalletChanged -= HandleWalletChanged;
        PlayerProfile.OnCharacterSkinChanged -= RefreshAll;
        if (retryButton != null) retryButton.onClick.RemoveListener(RetryLoad);
    }

    public async void RetryLoad()
    {
        if (isLoading || isBuying) return;
        int token = ++activation;
        isLoading = true;
        IsShopReady = false;
        SetStatus("상점 정보를 불러오는 중…", false);
        RefreshAll();
        try
        {
            await ugs.LoadSkinCatalogAsync();
            await ugs.LoadPlayerData();
            if (this == null || !isActiveAndEnabled || token != activation) return;
            IsShopReady = catalog != null && catalog.DefaultSkin != null && ugs.IsWalletReady;
            SyncOwnership();
            SetStatus(IsShopReady ? "" : "기본 캐릭터 설정을 확인하세요.", !IsShopReady);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (this != null && isActiveAndEnabled && token == activation)
                SetStatus("상점 조회 실패 · 다시 시도해 주세요.", true);
        }
        finally
        {
            if (this != null)
            {
                isLoading = false;
                if (isActiveAndEnabled)
                {
                    if (token != activation) RetryLoad();
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
        foreach (var skin in catalog.Skins) AddItem(skin, ids);
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
        if (skin == null || !PlayerProfile.OwnsCharacterSkin(skin.SkinId)) return CharacterSkinState.Locked;
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
        SetStatus("구매 처리 중…", false);
        RefreshAll();
        try
        {
            var result = await ugs.PurchaseSkinAsync(skin.SkinId);
            if (this == null || !isActiveAndEnabled) return;
            SyncOwnership();
            switch (result.status)
            {
                case "purchased": SetStatus("구매 완료 · 장착 버튼으로 선택하세요.", false); break;
                case "already_owned": SetStatus("이미 보유한 캐릭터입니다.", false); break;
                case "insufficient_funds": SetStatus("Coin이 부족합니다.", false); break;
                default: SetStatus("판매 정보가 없습니다.", false); break;
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
                    SetStatus(PlayerProfile.OwnsCharacterSkin(skin.SkinId) ? "구매 상태를 복구했습니다." : "구매 실패 · 다시 시도해 주세요.", false);
                }
            }
            catch (Exception refreshError)
            {
                Debug.LogException(refreshError);
                if (this != null)
                {
                    IsShopReady = false;
                    if (isActiveAndEnabled) SetStatus("구매 확인 실패 · 잔액을 다시 조회해 주세요.", true);
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
                    else { SyncOwnership(); RefreshAll(); }
                }
            }
        }
    }

    public void TryEquip(CharacterSkinSO skin)
    {
        if (IsBusy || skin == null || !catalog.TryGetSkin(skin.SkinId, out _) || !PlayerProfile.EquipCharacterSkin(skin.SkinId)) return;
        SaveManager.Save();
        SetStatus("캐릭터를 장착했습니다.", false);
    }

    private void HandleWalletChanged() { SyncOwnership(); RefreshAll(); }
    private void SyncOwnership()
    {
        if (ugs == null || !ugs.IsWalletReady || catalog == null) return;
        PlayerProfile.ApplyCharacterOwnership(catalog, ugs.OwnedItems);
        SaveManager.Save();
    }
    private void SetStatus(string message, bool retry)
    {
        if (statusText != null) statusText.text = message;
        if (retryButton != null) retryButton.gameObject.SetActive(retry);
    }
    private void RefreshAll()
    {
        if (!isActiveAndEnabled) return;
        if (catalog != null && catalog.TryGetSkin(PlayerProfile.EquippedCharacterSkinId, out var skin))
        {
            if (EquippedCharacterIcon != null) EquippedCharacterIcon.sprite = skin.Icon;
            if (EquippedCharacterName != null) EquippedCharacterName.text = skin.DisplayName;
        }
        foreach (var item in itemUIs) if (item != null) item.Refresh();
    }
}
