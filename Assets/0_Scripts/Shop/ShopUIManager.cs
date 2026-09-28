using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Option;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance;

    [Header("Branch Info")]
    [SerializeField] private TextMeshProUGUI branchCountText;

    [Header("MainShop")]
    [SerializeField] private GameObject BackGround;

    [Header("Exit")]
    [SerializeField] private Button ExitBtn;

    public bool IsShopOpen => BackGround != null && BackGround.activeSelf;

    private void OnEnable()
    {
        PlayerProfile.OnBranchCountChanged += UpdateBranchCount;

        UpdateBranchCount(PlayerProfile.BranchCount);
    }

    private void OnDisable()
    {
        PlayerProfile.OnBranchCountChanged -= UpdateBranchCount;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ExitBtn.onClick.AddListener(ExitShopUI);
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
}
