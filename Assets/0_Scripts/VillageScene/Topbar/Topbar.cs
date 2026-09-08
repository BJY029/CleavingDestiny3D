using System;
using System.Collections.Generic;
using UnityEngine;
using Village.Building;

public class Topbar : MonoBehaviour
{
    public List<TopbarBuilding> topbarBuildings;
    [SerializeField] private VillageBuildingManager buildingManager;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (buildingManager == null)
        {
            buildingManager = FindFirstObjectByType<VillageBuildingManager>();
        }

        foreach (var building in topbarBuildings)
        {
            building.Init(OpenBuilding);
        }

        SetActiveBuilding(buildingManager != null ? buildingManager.ActiveBuildingType : null);
    }

    private void OnEnable()
    {
        if (buildingManager != null)
        {
            buildingManager.OnActiveBuildingChanged += SetActiveBuilding;
        }
    }

    private void OnDisable()
    {
        if (buildingManager != null)
        {
            buildingManager.OnActiveBuildingChanged -= SetActiveBuilding;
        }
    }

    private void OpenBuilding(VillageType building)
    {
        _ = buildingManager.OpenOrSwitchBuilding(building);
    }

    private void SetActiveBuilding(VillageType? activeBuilding)
    {
        bool isBuildingView = activeBuilding.HasValue;
        canvasGroup.alpha = isBuildingView ? 1f : 0f;
        canvasGroup.interactable = isBuildingView;
        canvasGroup.blocksRaycasts = isBuildingView;

        foreach (var building in topbarBuildings)
        {
            building.SetHighlight(activeBuilding.HasValue && building.BuildingType == activeBuilding.Value);
        }
    }
}
