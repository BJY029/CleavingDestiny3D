
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VillageBuilding : MonoBehaviour
{
    public VillageUpgradeIndex buildingType;

    public TextMeshProUGUI levelText;

    public Button upgradeButton;
    public TextMeshProUGUI upgradeCostText;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public void Start()
    {
        SetLevelAndCostText();
        CheckCostIsEnough(VillageManager.Instance.GetMyGold());

        VillageManager.Instance.OnGoldChanged.AddListener(CheckCostIsEnough);

        // 임시 텍스트 설정
        nameText.text = $"Village_{buildingType}_Name";
        descriptionText.text = $"Village_{buildingType}_Description";
    }

    void SetLevelAndCostText()
    {
        int currentLevel = VillageStat.GetUpgradeLevel(buildingType);
        levelText.text = $"Level: {currentLevel}";

        int upgradeCost = VillageStat.GetLevelUpgradedCost(buildingType);
        upgradeCostText.text = $"Upgrade Cost: {upgradeCost} Gold";
    }

    void CheckCostIsEnough(int gold)
    {
        int upgradeCost = VillageStat.GetLevelUpgradedCost(buildingType);

        if (gold >= upgradeCost)
        {
            upgradeCostText.color = Color.green;
            upgradeButton.interactable = true;
        }
        else
        {
            upgradeCostText.color = Color.red;
            upgradeButton.interactable = false;
        }
    }

    public void OnClickUpgradeButton()
    {
        VillageManager.Instance.TryUpgradeLevel(buildingType);
        SetLevelAndCostText();
    }
}