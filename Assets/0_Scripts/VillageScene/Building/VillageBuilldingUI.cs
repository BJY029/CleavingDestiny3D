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

            // 관련 데이터 가져오기 (인터페이스에 맞게 수정)
            int currentLevel = VillageStat.GetVillageLevel(currentBuildingType);
            int nextLevel = currentLevel + 1;

            // 현재 레벨 기준 다음 업그레이드 비용 가져오기
            int nextUpgradeCost = VillageStat.GetLevelUpgradedCost(currentBuildingType);
            int currentGold = VillageSystem.VillageLogic.GetMyGold();

            // Localization 키 생성
            string titleKey = $"{currentBuildingType}_Title";
            string descKey = $"{currentBuildingType}_Desc";
            string effectFormatKey = $"{currentBuildingType}_Effect";

            // UI 텍스트 업데이트
            titleText.SetText(new LocalizedString(CSV_Type.Village, titleKey));
            descriptionText.SetText(new LocalizedString(CSV_Type.Village, descKey));

            // 레벨 정보 포맷팅
            currentLevelText.SetText(new LocalizedString(CSV_Type.Village, "Building_Level"), currentLevel);
            nextLevelText.SetText(new LocalizedString(CSV_Type.Village, "Building_NextLevel"), nextLevel);

            // 현재 수치 데이터 가져오기 (플레이어 기준)
            object[] currentParams = GetBuildingParams(currentBuildingType);

            // 효과 정보 포맷팅
            string currentEffectFormatted = string.Format(new LocalizedString(CSV_Type.Village, effectFormatKey), currentParams);

            // TODO: 인터페이스에 레벨을 인자로 받아 미래 수치를 가져오는 함수가 없음, 나중에 구현해야함
            string nextEffectFormatted = "???";

            currentEffectText.SetText(currentEffectFormatted);
            nextEffectText.SetText(string.Format(new LocalizedString(CSV_Type.Village, "Building_NextEffect"), nextEffectFormatted));

            // 경제 정보 포맷팅
            currentGoldText.SetText(new LocalizedString(CSV_Type.Village, "Building_CurrentGold"), currentGold);
            upgradeCostText.SetText(new LocalizedString(CSV_Type.Village, "Building_UpgradeCost"), nextUpgradeCost);

            // 업그레이드 버튼 활성화 여부 설정
            upgradeButton.interactable = currentGold >= nextUpgradeCost;
        }

        /// <summary>
        /// IVillageStatProvider의 실제 함수명으로 매칭합니다.
        /// </summary>
        private object[] GetBuildingParams(VillageType type)
        {
            return type switch
            {
                VillageType.Mine => new object[] { VillageStat.GetGoldIncomePerDay() },
                VillageType.Farm => new object[] { VillageStat.GetMaxEnergy(), VillageStat.GetEnergyIncomePerDay() },
                VillageType.Barrier => new object[] { VillageStat.GetBarrierArmor() },
                // TODO: 상점, 대장간은 아직 미구현
                _ => new object[] { 0, 0 }
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
                // 업그레이드 성공 시 UI 갱신
                SetBuildingUI(currentBuildingType);
            }
        }

    }
}