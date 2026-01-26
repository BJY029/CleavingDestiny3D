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
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI currentEffectText;
        [SerializeField] private TextMeshProUGUI nextLevelText;
        [SerializeField] private TextMeshProUGUI nextEffectText;

        [Header("Economy Texts")]
        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private TextMeshProUGUI upgradeCostText;

        [Header("Buttons")]
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button exitButton;

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
            // 관련 데이터 가져오기
            int currentLevel = VillageStat.GetVillageLevel(buildingType);
            int nextLevel = currentLevel + 1;
            int nextUpgradeCost = VillageStat.GetLevelUpgradedCost(buildingType, currentLevel);
            string currentEffect = VillageStat.GetLevelDescriptionID(buildingType, currentLevel);
            string nextEffect = VillageStat.GetLevelDescriptionID(buildingType, nextLevel);
            int currentGold = VillageSystem.VillageLogic.GetMyGold();

            // UI 텍스트 업데이트
            titleText.SetText(buildingType.ToString());
            currentLevelText.SetText("레벨: {0}", currentLevel);
            currentEffectText.SetText("효과:" + currentEffect);
            nextLevelText.SetText("다음 레벨: {0}", nextLevel);
            nextEffectText.SetText("다음 효과:" + nextEffect);
            currentGoldText.SetText("보유 골드: {0}", currentGold);
            upgradeCostText.SetText("업그레이드 비용: {0}", nextUpgradeCost);

            // 업그레이드 버튼 활성화 여부 설정
            upgradeButton.interactable = currentGold >= nextUpgradeCost;
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

        }

    }
}