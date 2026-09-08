using System;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace Village
{
    public class VillageStatusUI : MonoBehaviour
    {
        [Header("Panel & Window")]
        [SerializeField] private CanvasGroup statusCanvasGroup;
        [SerializeField] private RectTransform windowContent;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Animation Settings")]
        [SerializeField] private float openDuration = 0.25f;
        [SerializeField] private float closeDuration = 0.18f;
        [SerializeField] private float startScale = 0.85f;
        [SerializeField] private Ease openEase = Ease.OutBack;
        [SerializeField] private Ease closeEase = Ease.InCubic;

        private Sequence animSequence;
        private System.Action _closeAction;
        public bool IsOpen => statusCanvasGroup.gameObject.activeSelf;

        private void Awake()
        {
            _closeAction = Close;

            if (statusCanvasGroup != null)
            {
                statusCanvasGroup.alpha = 0f;
                statusCanvasGroup.interactable = false;
                statusCanvasGroup.blocksRaycasts = false;
                statusCanvasGroup.gameObject.SetActive(false);

                if (windowContent == null)
                {
                    // windowContent가 별도 지정되지 않은 경우 CanvasGroup의 RectTransform 사용
                    windowContent = statusCanvasGroup.GetComponent<RectTransform>();
                }
            }
        }

        private void OnEnable()
        {
            LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDisable()
        {
            KeyInteractManager.Instance?.RemoveMenuAction(_closeAction);

            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
            }
            animSequence.Stop();
        }

        public void Open()
        {
            if (statusCanvasGroup == null) return;

            KeyInteractManager.Instance?.PushMenuAction(_closeAction);

            Refresh();

            animSequence.Stop();

            statusCanvasGroup.gameObject.SetActive(true);
            statusCanvasGroup.alpha = 0f;
            statusCanvasGroup.interactable = false;
            statusCanvasGroup.blocksRaycasts = true;

            if (windowContent != null)
            {
                windowContent.localScale = Vector3.one * startScale;
            }

            animSequence = Sequence.Create()
                .Group(Tween.Alpha(statusCanvasGroup, startValue: 0f, endValue: 1f, duration: openDuration, ease: Ease.OutQuad))
                .Group(windowContent != null
                    ? Tween.Scale(windowContent, startValue: startScale, endValue: 1f, duration: openDuration, ease: openEase)
                    : default)
                .OnComplete(() =>
                {
                    statusCanvasGroup.interactable = true;
                });
        }

        public void Close()
        {
            if (statusCanvasGroup == null) return;

            KeyInteractManager.Instance?.RemoveMenuAction(_closeAction);

            animSequence.Stop();

            statusCanvasGroup.interactable = false;

            animSequence = Sequence.Create()
                .Group(Tween.Alpha(statusCanvasGroup, startValue: statusCanvasGroup.alpha, endValue: 0f, duration: closeDuration, ease: closeEase))
                .Group(windowContent != null
                    ? Tween.Scale(windowContent, endValue: startScale, duration: closeDuration, ease: closeEase)
                    : default)
                .OnComplete(() =>
                {
                    statusCanvasGroup.blocksRaycasts = false;
                    statusCanvasGroup.gameObject.SetActive(false);
                });
        }

        public void Refresh()
        {
            IVillageStatProvider stat = VillageSystem.VillageStat;
            int mineLevel = stat.GetVillageLevel(VillageType.Mine);
            int forgeLevel = stat.GetVillageLevel(VillageType.Forge);
            int shopLevel = stat.GetVillageLevel(VillageType.Shop);
            int farmLevel = stat.GetVillageLevel(VillageType.Farm);
            int barrierLevel = stat.GetVillageLevel(VillageType.Barrier);
            var damage = stat.GetAxeRangeDamage(forgeLevel);

            string content = LocalizationManager.Instance.GetFormatText(
                CSV_Type.Village,
                "Status_Content",
                VillageSystem.VillageLogic.GetMyGold(),
                mineLevel + 1,
                stat.GetGoldIncomePerDay(mineLevel),
                forgeLevel + 1,
                damage.min,
                damage.max,
                shopLevel + 1,
                farmLevel + 1,
                stat.GetMaxEnergy(farmLevel),
                stat.GetEnergyIncomePerDay(farmLevel),
                barrierLevel + 1,
                stat.GetBarrierArmor(barrierLevel));
            statusText.richText = true;
            statusText.SetText(content);
        }

        private void OnLanguageChanged()
        {
            if (statusCanvasGroup != null && statusCanvasGroup.gameObject.activeSelf)
            {
                Refresh();
            }
        }
    }
}
