using System;
using Potan.CoreUtils;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village.Building
{

    public class VillageBuilldingUI : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup canvasGroup;

        [Header("Building Info Texts")]
        [SerializeField] protected TextMeshProUGUI titleText;

        [SerializeField] protected TextMeshProUGUI descriptionText;

        [SerializeField] BuildingEffect[] buildingEffectList;

        [Header("Economy Texts")]
        [SerializeField] protected TextMeshProUGUI currentGoldText;
        [SerializeField] protected TextMeshProUGUI upgradeCostText;

        [Header("Buttons")]
        [SerializeField] protected Button upgradeButton;
        [SerializeField] protected Button exitButton;

        protected VillageType currentBuildingType;

        protected IVillageStatProvider VillageStat => VillageSystem.VillageStat;


        public Action OnExitButtonClicked;

        protected virtual void Awake()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false);

            exitButton.onClick.AddListener(OnExitButton);
            upgradeButton.onClick.AddListener(OnUpgradeButton);
        }

        public virtual void SetBuildingUI(VillageType buildingType)
        {
            currentBuildingType = buildingType;

            // 레벨 가져오기
            int currentLevel = VillageStat.GetVillageLevel(currentBuildingType);
            DevLog.Log($"Setting UI for {currentBuildingType}, Level: {currentLevel}", this);

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

            // 골드/업그레이드 비용/버튼 상태 갱신
            RefreshStatusUI();
        }

        public virtual void RefreshStatusUI()
        {
            int nextUpgradeCost = VillageStat.GetLevelUpgradedCost(currentBuildingType, VillageStat.GetVillageLevel(currentBuildingType));
            int currentGold = VillageSystem.VillageLogic.GetMyGold();

            currentGoldText.SetText("{0}", currentGold);
            UpdateUpgradeButton(nextUpgradeCost, currentGold);
        }

        private void UpdateUpgradeButton(int nextUpgradeCost, int currentGold)
        {
            if (nextUpgradeCost <= 0)
            {
                upgradeCostText.SetText(new LocalizedString(CSV_Type.Village, "Upgrade_Max_Level"));
                upgradeButton.interactable = false;
                return;
            }

            bool canUpgrade = currentGold >= nextUpgradeCost;
            var upgradeCostStr = new LocalizedString(CSV_Type.Village, canUpgrade ? "Upgrade_Cost" : "Upgrade_Cost_Not_Enough");
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
                // 성공 시 현재 타입 기준으로 전체 UI를 다시 그려, 텍스트/버튼/이펙트 상태를 일관되게 유지
                SetBuildingUI(currentBuildingType);
            }
        }

    }
}