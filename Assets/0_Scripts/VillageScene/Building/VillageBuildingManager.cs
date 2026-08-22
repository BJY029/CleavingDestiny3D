using System.Collections.Generic;
using System.Threading.Tasks;
using Potan.CoreUtils;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Village.Building
{

    public class VillageBuildingManager : MonoBehaviour
    {
        private const float FadeInDuration = 0.3f;
        private const float TransitionDuration = 0.25f;
        private const float ExitTransitionDuration = 0.3f;

        public VillageBuilding[] villageBuildings;
        [SerializeField] CinemachineCamera cinemachineCamera;
        [SerializeField] Canvas builidingUICanvas;
        private CinemachineBrain brain;
        private readonly List<RaycastResult> _raycastResults = new();

        // 현재 활성화된 UI
        VillageBuilldingUI currentBuildingUI;
        private bool _isExiting;

        public bool IsBuildingOpen => currentBuildingUI != null;

        // 프리팹(Key)과 생성된 인스턴스(Value)를 매핑하여 관리하는 캐시
        private Dictionary<VillageBuilldingUI, VillageBuilldingUI> _uiInstanceCache = new Dictionary<VillageBuilldingUI, VillageBuilldingUI>();

        private void Start()
        {
            brain = CinemachineBrain.GetActiveBrain(0);

            // buildingUI.OnExitButtonClicked += ExitBuilding;

            foreach (var building in villageBuildings)
            {
                building.OnVillageClicked += OnBuildingClicked;
            }
        }

        private void OnEnable()
        {
            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnMenuKeyDown += TryExitBuilding;
            }
        }

        private void OnDisable()
        {
            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnMenuKeyDown -= TryExitBuilding;
                KeyInteractManager.Instance.SetMenuInputEnabled(true);
            }
        }

        private async void OnBuildingClicked(VillageBuilding building)
        {
            DevLog.Log("BuildingManager received click from: " + building.buildingType, this);

            if (currentBuildingUI != null)
            {
                DevLog.Log("A building UI is already open. Ignoring click.");
                return;
            }

            // 해당 건물의 UI 프리팹이 이미 생성된 적 있는지 확인
            if (!_uiInstanceCache.TryGetValue(building.villageBuildingUIPrefab, out currentBuildingUI))
            {
                // 없다면 새로 생성하고 캐시에 등록
                currentBuildingUI = Instantiate(building.villageBuildingUIPrefab, builidingUICanvas.transform);
                currentBuildingUI.OnExitButtonClicked += ExitBuilding;

                _uiInstanceCache.Add(building.villageBuildingUIPrefab, currentBuildingUI);
            }

            // 재사용되는 UI이므로 비활성화 상태일 수 있음
            currentBuildingUI.gameObject.SetActive(true);

            // 카메라를 해당 빌딩으로 이동
            cinemachineCamera.transform.position = building.cameraFocusPoint.position + new Vector3(0, 0, -5);
            cinemachineCamera.gameObject.SetActive(true);

            var fadeCanvas = FadeCanvas.Instance;
            await fadeCanvas.FadeInAsync(FadeInDuration);

            // 재사용된 UI에 현재 클릭된 건물의 타입 데이터를 새로 주입 (중요)
            currentBuildingUI.SetBuildingUI(building.buildingType);

            await currentBuildingUI.ShowBuildingUI(TransitionDuration);
            await fadeCanvas.FadeOutAsync(TransitionDuration);
        }

        public async void ExitBuilding()
        {
            if (currentBuildingUI == null || _isExiting) return;

            _isExiting = true;
            KeyInteractManager.Instance?.SetMenuInputEnabled(false);
            foreach (var building in villageBuildings)
            {
                building.SetInteractionEnabled(false);
            }

            try
            {
                var fadeCanvas = FadeCanvas.Instance;
                await fadeCanvas.FadeInAsync(ExitTransitionDuration);
                await currentBuildingUI.HideBuildingUI(ExitTransitionDuration);

                cinemachineCamera.gameObject.SetActive(false);

                await fadeCanvas.FadeOutAsync(ExitTransitionDuration);
                currentBuildingUI = null;

                while (brain != null && brain.IsBlending)
                {
                    await Awaitable.NextFrameAsync();
                }
            }
            finally
            {
                foreach (var building in villageBuildings)
                {
                    building.SetInteractionEnabled(true);
                }
                RestoreHoverUnderCursor();
                _isExiting = false;
                KeyInteractManager.Instance?.SetMenuInputEnabled(true);
            }
        }

        private bool TryExitBuilding()
        {
            if (!IsBuildingOpen) return false;

            ExitBuilding();
            return true;
        }

        private void RestoreHoverUnderCursor()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null) return;

            var pointerEventData = new PointerEventData(eventSystem) { position = Input.mousePosition };
            eventSystem.RaycastAll(pointerEventData, _raycastResults);

            if (_raycastResults.Count > 0)
            {
                _raycastResults[0].gameObject.GetComponentInParent<VillageBuilding>()?.RestoreHover();
            }

            _raycastResults.Clear();
        }
    }
}
