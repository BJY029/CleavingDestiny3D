using System;
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

        [FormerlySerializedAs("villageBuilldingUIPrefab")] public VillageBuilldingUI villageBuildingUIPrefab;

        public bool isScaleOnPointer = true;

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
                transform.localScale = Vector3.one * 1.1f;
            }
            OnVillagePointerEnterExit?.Invoke(this, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isScaleOnPointer)
            {
                transform.localScale = Vector3.one;
            }
            OnVillagePointerEnterExit?.Invoke(this, false);
        }

        public void SetInteractionEnabled(bool enabled)
        {
            if (!enabled)
            {
                transform.localScale = Vector3.one;
                OnVillagePointerEnterExit?.Invoke(this, false);
            }

            this.enabled = enabled;
        }

        public void RestoreHover()
        {
            if (!isActiveAndEnabled) return;

            if (isScaleOnPointer)
            {
                transform.localScale = Vector3.one * 1.1f;
            }
            OnVillagePointerEnterExit?.Invoke(this, true);
        }

        public static string GetBuildingName(VillageType type)
        {
            return LocalizationManager.Instance.GetText(CSV_Type.Village, $"{type}_Title");
        }

    }
}
