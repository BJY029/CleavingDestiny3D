using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace Village.Building
{

    public class VillageBuildingManager : MonoBehaviour
    {
        [SerializeField] VillageBuilding[] villageBuildings;
        [SerializeField] CinemachineCamera cinemachineCamera;
        [SerializeField] FadeCanvas fadeCanvas;
        VillageBuilldingUI buildingUI;

        void Start()
        {
            buildingUI = GetComponent<VillageBuilldingUI>();

            buildingUI.OnExitButtonClicked += ExitBuilding;

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
            buildingUI.SetBuildingUI(building.buildingType);
            await buildingUI.ShowBuildingUI(0.5f);
            await fadeCanvas.FadeOut(1f);
        }


        public async void ExitBuilding()
        {
            await fadeCanvas.FadeIn(0.5f);
            await buildingUI.HideBuildingUI(0.5f);

            cinemachineCamera.gameObject.SetActive(false);

            await fadeCanvas.FadeOut(1f);

        }
    }
}