using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Option;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance;

    [Header("Shop Button")]
    [SerializeField] private Button AxeShopButton;
    [SerializeField] private Button CharacterShopButton;

    [Header("Branch Info")]
    [SerializeField] private TextMeshProUGUI branchCountText;

    [Header("Gold Info")]
    [SerializeField] private TextMeshProUGUI goldCountText;

    [Header("MainShop")]
    [SerializeField] private GameObject BackGround;
    [SerializeField] private GameObject AxeShop;
    [SerializeField] private GameObject CharacterShop;

    [Header("Exit")]
    [SerializeField] private Button AxeShopExitBtn;
    [SerializeField] private Button CharacterShopExitBtn;

    public bool IsShopOpen => BackGround != null && BackGround.activeSelf;

    private UGSManager wallet;

    private void UpdateCoinBalance()
    {
        if (goldCountText != null) goldCountText.text = wallet != null && wallet.IsWalletReady && wallet.PlayerData.TryGetValue("coins", out int coins) ? coins.ToString("N0") : "—";
    }

    private void OnEnable()
    {
        PlayerProfile.OnBranchCountChanged += UpdateBranchCount;
        wallet = UGSManager.Instance;
        wallet.WalletChanged += UpdateCoinBalance;
        UpdateCoinBalance();

        UpdateBranchCount(PlayerProfile.BranchCount);
        //UpdateGoldCount(PlayerProfile.GoldCount);
    }

    private void OnDisable()
    {
        PlayerProfile.OnBranchCountChanged -= UpdateBranchCount;
        if (wallet != null) wallet.WalletChanged -= UpdateCoinBalance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        AxeShop.SetActive(true);
        CharacterShop.SetActive(false);

        AxeShopButton.interactable = false;
        CharacterShopButton.interactable = true;

        AxeShopExitBtn.onClick.AddListener(ExitShopUI);
        CharacterShopExitBtn.onClick.AddListener(ExitShopUI);

        AxeShopButton.onClick.AddListener(ChangeShop_Character_Axe);
        CharacterShopButton.onClick.AddListener(ChangeShop_Axe_Character);
    }

    private void OnDestroy()
    {
        KeyInteractManager.Instance?.RemoveMenuAction(ExitShopUI);
    }

    private void Update()
    {
        // 옵션창이 활성화되어 있다면 옵션창에 우선권 양보
        if (OptionManager.Instance != null && OptionManager.Instance.IsOptionMenuActive())
            return;

        if (IsShopOpen && WasEscapePressed())
        {
            ExitShopUI();
        }
    }

    private bool WasEscapePressed()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            return true;

        if (Input.GetKeyDown(KeyCode.Escape))
            return true;

        return false;
    }

    private void UpdateBranchCount(int count)
    {
        branchCountText.text = count.ToString();
    }

    private void UpdateGoldCount(int count)
    {
        goldCountText.text = count.ToString();
    }

    public void EnterShopUI()
    {
        BackGround.SetActive(true);
        KeyInteractManager.Instance?.PushMenuAction(ExitShopUI);
    }

    public void ExitShopUI()
    {
        if (BackGround != null)
        {
            BackGround.SetActive(false);
        }
        AudioManager.Instance?.PlaySfx2D("ui_button");
        KeyInteractManager.Instance?.RemoveMenuAction(ExitShopUI);
    }

    public void ChangeShop_Axe_Character()
    {
        AxeShopButton.interactable = true;
        CharacterShopButton.interactable = false;

        AxeShop.SetActive(false);
        CharacterShop.SetActive(true);
    }

    public void ChangeShop_Character_Axe()
    {
        AxeShopButton.interactable = false;
        CharacterShopButton.interactable = true;

        AxeShop.SetActive(true);
        CharacterShop.SetActive(false);
    }
}
