using Unity.Cinemachine;
using UnityEngine;

namespace Village.Building
{

    public class VillageBuildingManager : MonoBehaviour
    {
        [SerializeField] VillageBuilding[] villageBuildings;
        [SerializeField] CinemachineCamera cinemachineCamera;
        [SerializeField] FadeCanvas fadeCanvas;


        void Start()
        {
            foreach (var building in villageBuildings)
            {
                building.OnVillageClicked += OnBuildingClicked;
            }
        }

        private async void OnBuildingClicked(VillageBuilding building)
        {
            Debug.Log("BuildingManager received click from: " + building.buildingType);
            // 카메라를 해당 빌딩으로 이동
            cinemachineCamera.transform.position = building.cameraFocusPoint.position + new Vector3(0, 0, -5);
            cinemachineCamera.gameObject.SetActive(true);

            await fadeCanvas.FadeIn(1f);
            EnterBuilding(building);
            await fadeCanvas.FadeOut(1f);
        }

        private void EnterBuilding(VillageBuilding building)
        {
            Debug.Log("Entering building: " + building.buildingType);
            // 빌딩 내부로 이동하는 로직 추가
        }
    }
}