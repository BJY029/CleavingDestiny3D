using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Village.Building
{
    public class VillageBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public VillageType buildingType;
        public Transform cameraFocusPoint;

        public string BuildingName { get; private set; }
        public string HoverTextFormat { get; private set; }
        public string NotEnoughGoldHoverTextFormat { get; private set; }
        
        public Action<VillageBuilding> OnVillageClicked;
        public Action<VillageBuilding, bool> OnVillagePointerEnterExit;

        [FormerlySerializedAs("villageBuilldingUIPrefab")] public VillageBuildingUI villageBuildingUIPrefab;

        public bool isScaleOnPointer = true;

        [Header("Hover Tween Settings")]
        [SerializeField] private float hoverScaleRatio = 1.08f;
        [SerializeField] private float scaleDuration = 0.15f;
        [SerializeField] private Ease hoverEase = Ease.OutQuad;
        [SerializeField] private Ease exitEase = Ease.OutQuad;

        private Vector3 _originalScale = Vector3.one;
        private Tween _scaleTween;

        [Header("Sounds")]
        [SerializeField] private string enterSound = "village_doorbell";
        [SerializeField] private string exitSound = "village_doorbell";

        public void PlayEnterSound()
        {
            if (!string.IsNullOrEmpty(enterSound) && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx2D(enterSound);
            }
        }

        public void PlayExitSound()
        {
            if (!string.IsNullOrEmpty(exitSound) && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx2D(exitSound);
            }
        }

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        private void OnDisable()
        {
            _scaleTween.Stop();
            transform.localScale = _originalScale;
        }

        private void Start()
        {
            BuildingName = GetBuildingName(buildingType);
            HoverTextFormat = $"{BuildingName}  Lv. {{0}}  {{1}} Gold";
            NotEnoughGoldHoverTextFormat = $"{BuildingName}  Lv. {{0}}  <color=#FF0000>{{1}} Gold</color>";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnVillageClicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isScaleOnPointer)
            {
                AnimateScale(_originalScale * hoverScaleRatio, hoverEase);
            }
            OnVillagePointerEnterExit?.Invoke(this, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isScaleOnPointer)
            {
                AnimateScale(_originalScale, exitEase);
            }
            OnVillagePointerEnterExit?.Invoke(this, false);
        }

        public void SetInteractionEnabled(bool enabled)
        {
            if (!enabled)
            {
                _scaleTween.Stop();
                transform.localScale = _originalScale;
                OnVillagePointerEnterExit?.Invoke(this, false);
            }

            this.enabled = enabled;
        }

        public void RestoreHover()
        {
            if (!isActiveAndEnabled) return;

            if (isScaleOnPointer)
            {
                AnimateScale(_originalScale * hoverScaleRatio, hoverEase);
            }
            OnVillagePointerEnterExit?.Invoke(this, true);
        }

        private void AnimateScale(Vector3 targetScale, Ease ease)
        {
            _scaleTween.Stop();
            _scaleTween = Tween.Scale(transform, targetScale, scaleDuration, ease, useUnscaledTime: true);
        }

        public static string GetBuildingName(VillageType type)
        {
            return LocalizationManager.Instance.GetText(CSV_Type.Village, $"{type}_Title");
        }

    }
}
