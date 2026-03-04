using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village.Building
{

    public class VillageBuilldingUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Building Info Texts")]
        [SerializeField] private TextMeshProUGUI titleText;

        [SerializeField] private TextMeshProUGUI descriptionText;

        [SerializeField] BuildingEffect[] buildingEffectList;

        [Header("Economy Texts")]
        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;

        [Header("Buttons")]
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button exitButton;

        private VillageType currentBuildingType;

        IVillageStatProvider VillageStat => VillageSystem.VillageStat;


        public Action OnExitButtonClicked;

        void Start()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false);

            exitButton.onClick.AddListener(OnExitButton);
            upgradeButton.onClick.AddListener(OnUpgradeButton);
        }

        public void SetBuildingUI(VillageType buildingType)
        {
            currentBuildingType = buildingType;

            // 레벨 가져오기
            int currentLevel = VillageStat.GetVillageLevel(currentBuildingType);
            Debug.Log($"Setting UI for {currentBuildingType}, Level: {currentLevel}");

            // 현재 레벨 기준 다음 업그레이드 비용 가져오기
            int nextUpgradeCost = VillageStat.GetLevelUpgradedCost(currentBuildingType);
            int currentGold = VillageSystem.VillageLogic.GetMyGold();

            // Localization 키 생성
            string titleKey = $"{currentBuildingType}_Title";
            string descKey = $"{currentBuildingType}_Desc";
            // string effectFormatKey = $"{currentBuildingType}_Effect";

            // UI 텍스트 업데이트
            titleText.SetText(new LocalizedString(CSV_Type.Village, titleKey));
            descriptionText.SetText(new LocalizedString(CSV_Type.Village, descKey));

            for (int i = 0; i < buildingEffectList.Length; i++)
            {
                var effect = buildingEffectList[i];
                // i가 currentLevel 이하일 때만 라인 활성화
                effect.SetEffectLineEnabled(i <= currentLevel);

                // i가 현재 레벨과 딱 맞을 때만 이펙트 활성화
                effect.SetEffectActivated(i == currentLevel);

                // 효과 설명 업데이트
                string effectKey = $"{currentBuildingType}_Effect";
                if (!string.IsNullOrEmpty(effectKey))
                {
                    LocalizedString effectDesc = new(CSV_Type.Village, effectKey);
                    var effectValue = GetBuildingParams(currentBuildingType, i);
                    if (effectValue.Item2 < 0f)
                    {
                        effect.EffectValueText.SetText(effectDesc, effectValue.Item1);
                    }
                    else
                    {
                        effect.EffectValueText.SetText(effectDesc, effectValue.Item1, effectValue.Item2);
                    }
                }
            }

            currentGoldText.SetText("{0}", currentGold);

            // 업그레이드 버튼 설정
            UpdateUpgradeButton(nextUpgradeCost, currentGold);
        }

        private void UpdateUpgradeButton(int nextUpgradeCost, int currentGold)
        {
            bool canUpgrade = currentGold >= nextUpgradeCost;
            LocalizedString upgradeCostStr = new LocalizedString(CSV_Type.Village, canUpgrade ? "Upgrade_Cost" : "Upgrade_Cost_Not_Enough");
            upgradeCostText.SetText(upgradeCostStr, nextUpgradeCost);

            // 업그레이드 버튼 활성화 여부 설정
            upgradeButton.interactable = canUpgrade;
        }

        private (float, float) GetBuildingParams(VillageType type, int level)
        {
            return type switch
            {
                VillageType.Mine => (VillageStat.GetGoldIncomePerDay(level), -1f),
                VillageType.Farm => (VillageStat.GetMaxEnergy(level), VillageStat.GetEnergyIncomePerDay(level)),
                VillageType.Barrier => (VillageStat.GetBarrierArmor(level), -1f),
                VillageType.Forge => VillageStat.GetAxeRangeDamage(level),
                // TODO: 상점 미구현
                _ => (0f, 0f)
            };
        }

        public async Awaitable ShowBuildingUI(float duration = 0.5f)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            await Tween.Alpha(canvasGroup, 1f, duration, ease: Easing.Standard(Ease.InCubic));
            canvasGroup.interactable = true;
        }

        public async Awaitable HideBuildingUI(float duration = 0.5f)
        {
            await Tween.Alpha(canvasGroup, 0f, duration, ease: Easing.Standard(Ease.OutCubic));
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.gameObject.SetActive(false);
        }

        private void OnExitButton()
        {
            OnExitButtonClicked?.Invoke();
        }

        private void OnUpgradeButton()
        {
            if (VillageSystem.VillageLogic.TryUpgradeLevel(currentBuildingType))
            {
                int currentLevel = VillageStat.GetVillageLevel(currentBuildingType);
                int currentGold = VillageSystem.VillageLogic.GetMyGold();

                currentGoldText.SetText("{0}", currentGold);
                UpdateUpgradeButton(VillageStat.GetLevelUpgradedCost(currentBuildingType), currentGold);

                // 업그레이드 성공 시 UI 갱신
                for (int i = 0; i < buildingEffectList.Length; i++)
                {
                    var effect = buildingEffectList[i];
                    // i가 currentLevel 이하일 때만 라인 활성화
                    effect.SetEffectLineEnabled(i <= currentLevel);

                    // i가 현재 레벨과 딱 맞을 때만 이펙트 활성화
                    effect.SetEffectActivated(i == currentLevel);
                }
            }
        }

    }
}