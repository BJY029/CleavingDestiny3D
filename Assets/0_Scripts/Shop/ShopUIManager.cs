using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance;

    [Header("Branch Info")]
    [SerializeField] private TextMeshProUGUI branchCountText;

    [Header("MainShop")]
    [SerializeField] private GameObject BackGround;

    [Header("Exit")]
    [SerializeField] private Button ExitBtn;

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

    private void UpdateBranchCount(int count)
    {
        branchCountText.text = count.ToString();
    }

    public void EnterShopUI()
    {
        BackGround.SetActive(true);
    }

    private void ExitShopUI()
    {
        BackGround.SetActive(false);
    }
}
