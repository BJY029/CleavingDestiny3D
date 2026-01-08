using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village
{

    public class VillageBuilding : MonoBehaviour
    {
        public VillageType buildingType;

        public TextMeshProUGUI levelText;

        public Button upgradeButton;
        public TextMeshProUGUI upgradeCostText;

        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;

        public void Start()
        {
            SetLevelAndCostText();
            CheckCostIsEnough(VillageSystem.VillageLogic.GetMyGold());

            VillageSystem.VillageLogic.OnGoldChanged += CheckCostIsEnough;

            // 임시 텍스트 설정
            nameText.SetText($"Village_{buildingType}_Name");
            descriptionText.SetText($"Village_{buildingType}_Description");
        }

        void SetLevelAndCostText()
        {

            int currentLevel = VillageStatManager.Instance.GetVillageLevel(buildingType);
            levelText.SetText("Level: {0}", currentLevel);

            int upgradeCost = VillageStatManager.Instance.GetLevelUpgradedCost(buildingType);
            upgradeCostText.SetText("Upgrade Cost: {0} Gold", upgradeCost);
        }

        void CheckCostIsEnough(int gold)
        {
            int upgradeCost = VillageStatManager.Instance.GetLevelUpgradedCost(buildingType);

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
            VillageSystem.VillageLogic.TryUpgradeLevel(buildingType);
            SetLevelAndCostText();
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (VillageSystem.VillageLogic != null)
            {
                VillageSystem.VillageLogic.OnGoldChanged -= CheckCostIsEnough;
            }
        }
    }
}