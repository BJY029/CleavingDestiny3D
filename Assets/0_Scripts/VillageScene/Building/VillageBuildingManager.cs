
using Unity.Cinemachine;
using UnityEngine;

namespace Village.Building
{

    public class VillageBuildingManager : MonoBehaviour
    {
        public VillageBuilding[] villageBuildings;
        public CinemachineCamera cinemachineCamera;

        void Start()
        {
            foreach (var building in villageBuildings)
            {
                building.OnVillageClicked += OnBuildingClicked;
            }
        }

        private void OnBuildingClicked(VillageBuilding building)
        {
            Debug.Log("BuildingManager received click from: " + building.buildingType);
            // 카메라를 해당 빌딩으로 이동
            cinemachineCamera.transform.position = building.cameraFocusPoint.position + new Vector3(0, 0, -5);
            cinemachineCamera.gameObject.SetActive(true);
        }
    }
}