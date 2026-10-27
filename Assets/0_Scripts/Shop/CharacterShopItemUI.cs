using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterShopItemUI : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;

    [Header("State")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private GameObject equippedLabel;
    [SerializeField] private GameObject priceGroup;
    [SerializeField] private GameObject soldoutGroup;

    private CharacterSkinSO skin;
    private CharacterShopController shopController;

    public void Initialize(CharacterSkinSO axeSkin, CharacterShopController controller)
    {
        skin = axeSkin;
        shopController = controller;

        iconImage.sprite = skin.Icon;
        nameText.text = skin.DisplayName;
        priceText.text = "불러오는 중…";

        buyButton.onClick.AddListener(HandleBuyClicked);
        equipButton.onClick.AddListener(HandleEquipClicked);
    }

    public void Refresh()
    {
        if (skin == null) return;
        if (shopController.TryGetPrice(skin, out var price))
            priceText.text = $"{price.Price:N0} Coin";
        // 가격과 구매 가능 여부는 서버 카탈로그만 사용합니다.
        else priceText.text = shopController.IsShopReady ? "판매 정보 없음" : "불러오는 중…";

        CharacterSkinState state = shopController.GetSkinState(skin);

        switch (state)
        {
            case CharacterSkinState.Locked:
                SetLockState();
                break;
            case CharacterSkinState.Owned:
                SetOwnedState();
                break;
            case CharacterSkinState.Equipped:
                SetEquippedState();
                break;
        }
    }

    private void SetLockState()
    {
        equipButton.interactable = false;
        equippedLabel.SetActive(false);
        priceGroup.SetActive(true);
        soldoutGroup.SetActive(false);

        buyButton.interactable = shopController.CanPurchase(skin);
    }

    private void SetOwnedState()
    {
        buyButton.interactable = false;
        equipButton.interactable = !shopController.IsBusy;
        equippedLabel.SetActive(false);

        priceGroup.SetActive(false);
        soldoutGroup.SetActive(true);
    }

    private void SetEquippedState()
    {
        buyButton.interactable = false;
        equipButton.interactable = false;
        equippedLabel.SetActive(true);

        priceGroup.SetActive(false);
        soldoutGroup.SetActive(true);
    }

    private void HandleBuyClicked()
    {
        shopController.TryPurchase(skin);
    }

    private void HandleEquipClicked()
    {
        shopController.TryEquip(skin);
    }

    private void OnDestroy()
    {
        buyButton.onClick.RemoveListener(HandleBuyClicked);
        equipButton.onClick.RemoveListener(HandleEquipClicked);
    }
}
