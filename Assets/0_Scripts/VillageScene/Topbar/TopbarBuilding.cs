using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Village.Building;

public class TopbarBuilding : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image highlightImage;
    [SerializeField] private VillageType building;
    public VillageType BuildingType => building;

    private Action<VillageType> onClicked;
    
    public void Init(Action<VillageType> newClicked)
    {
        SetHighlight(false);
        onClicked = newClicked;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onClicked?.Invoke(building);
        }
    }
    
    public void SetHighlight(bool highlight)
    {
        highlightImage.enabled = highlight;
    }
}
