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

        private void Start()
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(buildingType.ToString() + " clicked!");
            OnVillageClicked?.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = Vector3.one * 1.1f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
        }

    }
}