using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Village.Building
{
    public class VillageBuilding : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public VillageType buildingType;
        public Transform cameraFocusPoint;

        public Action<VillageBuilding> OnVillageClicked;

        public VillageBuilldingUI villageBuilldingUIPrefab;

        public bool isScaleOnPointer = true;

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
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isScaleOnPointer)
            {
                transform.localScale = Vector3.one;

            }
        }

    }
}