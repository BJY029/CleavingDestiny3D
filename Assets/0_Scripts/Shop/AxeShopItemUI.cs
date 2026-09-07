using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AxeShopItemUI : MonoBehaviour
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

    private AxeSkinSO skin;
    private AxeShopController shopController;

    public void Initialize(AxeSkinSO axeSkin, AxeShopController controller)
    {
        skin = axeSkin;
        shopController = controller;

        iconImage.sprite = skin.Icon;
        nameText.text = skin.DisplayName;
        priceText.text = skin.Price.ToString();

        buyButton.onClick.AddListener(HandleBuyClicked);
        equipButton.onClick.AddListener(HandleEquipClicked);
    }

    public void Refresh()
    {
        if (skin == null) return;

        AxeSkinState state = shopController.GetSkinState(skin);

        switch (state)
        {
            case AxeSkinState.Locked:
                SetLockState();
                break;
            case AxeSkinState.Owned:
                SetOwnedState();
                break;
            case AxeSkinState.Equipped:
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

        buyButton.interactable = PlayerProfile.BranchCount >= skin.Price;
    }

    private void SetOwnedState()
    {
        buyButton.interactable = false;
        equipButton.interactable = true;
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
        buyButton.onClick.RemoveListener(HandleEquipClicked);
    }
}
