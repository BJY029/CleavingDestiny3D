using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

namespace Village.Building
{

    public class VillageBuildingManager : MonoBehaviour
    {
        [SerializeField] VillageBuilding[] villageBuildings;
        [SerializeField] CinemachineCamera cinemachineCamera;
        [SerializeField] Canvas builidingUICanvas;

        // 현재 활성화된 UI
        VillageBuilldingUI currentBuildingUI;

        // 프리팹(Key)과 생성된 인스턴스(Value)를 매핑하여 관리하는 캐시
        private Dictionary<VillageBuilldingUI, VillageBuilldingUI> _uiInstanceCache = new Dictionary<VillageBuilldingUI, VillageBuilldingUI>();

        void Start()
        {
            // buildingUI.OnExitButtonClicked += ExitBuilding;

            foreach (var building in villageBuildings)
            {
                building.OnVillageClicked += OnBuildingClicked;
            }
        }

        private async void OnBuildingClicked(VillageBuilding building)
        {
            Debug.Log("BuildingManager received click from: " + building.buildingType);

            // 해당 건물의 UI 프리팹이 이미 생성된 적 있는지 확인
            if (!_uiInstanceCache.TryGetValue(building.villageBuilldingUIPrefab, out currentBuildingUI))
            {
                // 없다면 새로 생성하고 캐시에 등록
                currentBuildingUI = Instantiate(building.villageBuilldingUIPrefab, builidingUICanvas.transform);
                currentBuildingUI.OnExitButtonClicked += ExitBuilding;

                _uiInstanceCache.Add(building.villageBuilldingUIPrefab, currentBuildingUI);
            }

            // 재사용되는 UI이므로 비활성화 상태일 수 있음
            currentBuildingUI.gameObject.SetActive(true);

            // 카메라를 해당 빌딩으로 이동
            cinemachineCamera.transform.position = building.cameraFocusPoint.position + new Vector3(0, 0, -5);
            cinemachineCamera.gameObject.SetActive(true);

            var fadeCanvas = FadeCanvas.Instance;
            await fadeCanvas.FadeInAsync(1f);

            // 재사용된 UI에 현재 클릭된 건물의 타입 데이터를 새로 주입 (중요)
            currentBuildingUI.SetBuildingUI(building.buildingType);

            await currentBuildingUI.ShowBuildingUI(0.5f);
            await fadeCanvas.FadeOutAsync(1f);
        }


        public async void ExitBuilding()
        {
            if (currentBuildingUI == null) return;

            var fadeCanvas = FadeCanvas.Instance;
            await fadeCanvas.FadeInAsync(0.5f);
            await currentBuildingUI.HideBuildingUI(0.5f);

            cinemachineCamera.gameObject.SetActive(false);

            await fadeCanvas.FadeOutAsync(1f);

        }
    }
}