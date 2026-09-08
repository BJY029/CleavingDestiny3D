using System.Collections.Generic;
using System.Threading.Tasks;
using Potan.CoreUtils;
using PrimeTween;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Village.Building
{

    public class VillageBuildingManager : MonoBehaviour
    {
        [Header("Transition Timings (Seconds)")]
        [Tooltip("카메라가 건물로 이동/줌인하는 시간")]
        [SerializeField] private float cameraBlendDuration = 0.38f;
        [Tooltip("카메라 이동 후 UI가 뜨기까지의 시차(Stagger)")]
        [SerializeField] private float uiDelay = 0.1f;
        [Tooltip("UI 팝업 페이드 및 스케일 지속 시간")]
        [SerializeField] private float uiShowDuration = 0.28f;
        [Tooltip("건물에서 나갈 때 카메라 및 UI 퇴장 시간")]
        [SerializeField] private float exitDuration = 0.3f;
        [Tooltip("건물 간 다이렉트 전환 시 카메라 글라이딩 시간")]
        [SerializeField] private float switchDuration = 0.28f;
        [Tooltip("건물 간 다이렉트 전환 시 이전 UI 퇴장 시간")]
        [SerializeField] private float switchUiDuration = 0.15f;
        [Tooltip("건물 포커스 시 카메라 정사영 크기 (작을수록 줌인)")]
        [SerializeField] private float focusOrthoSize = 3.8f;

        public VillageBuilding[] villageBuildings;
        [SerializeField] CinemachineCamera cinemachineCamera;
        [FormerlySerializedAs("builidingUICanvas")] [SerializeField] Canvas buildingUICanvas;
        [SerializeField] private Village.Outside.VillageOutsideManager outsideManager;
        private CinemachineBrain brain;
        private readonly List<RaycastResult> _raycastResults = new();

        // 현재 활성화된 UI 및 건물
        VillageBuildingUI currentBuildingUI;
        private VillageBuilding _currentBuilding;
        private bool _isExiting;
        private bool _isSwitching;
        private System.Action _exitBuildingAction;

        public bool IsBuildingOpen => currentBuildingUI != null;
        public VillageType? ActiveBuildingType => _currentBuilding?.buildingType;
        public event System.Action<VillageType?> OnActiveBuildingChanged;

        // 프리팹(Key)과 생성된 인스턴스(Value)를 매핑하여 관리하는 캐시
        private Dictionary<VillageBuildingUI, VillageBuildingUI> _uiInstanceCache = new Dictionary<VillageBuildingUI, VillageBuildingUI>();

        private void Awake()
        {
            _exitBuildingAction = ExitBuilding;
        }

        private void OnEnable()
        {
            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnQuickSlotKeyDown -= HandleQuickSlot;
                KeyInteractManager.Instance.OnQuickSlotKeyDown += HandleQuickSlot;
            }
        }

        private void OnDisable()
        {
            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnQuickSlotKeyDown -= HandleQuickSlot;
                KeyInteractManager.Instance.RemoveMenuAction(_exitBuildingAction);
            }
        }

        private void Start()
        {
            brain = CinemachineBrain.GetActiveBrain(0);
            UpdateBlendDuration(cameraBlendDuration);

            if (outsideManager == null)
            {
                outsideManager = FindFirstObjectByType<Village.Outside.VillageOutsideManager>();
            }

            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnQuickSlotKeyDown -= HandleQuickSlot;
                KeyInteractManager.Instance.OnQuickSlotKeyDown += HandleQuickSlot;
            }

            foreach (var building in villageBuildings)
            {
                building.OnVillageClicked += OnBuildingClicked;
            }
        }

        private void UpdateBlendDuration(float duration)
        {
            if (brain != null)
            {
                var blend = brain.DefaultBlend;
                blend.Time = duration;
                brain.DefaultBlend = blend;
            }
        }

        public VillageBuilding GetBuilding(VillageType type)
        {
            if (villageBuildings == null) return null;
            foreach (var building in villageBuildings)
            {
                if (building != null && building.buildingType == type) return building;
            }
            return null;
        }

        private bool TryGetVillageTypeFromSlot(int slotIndex, out VillageType villageType)
        {
            switch (slotIndex)
            {
                case 1: villageType = VillageType.Mine; return true;
                case 2: villageType = VillageType.Forge; return true;
                case 3: villageType = VillageType.Shop; return true;
                case 4: villageType = VillageType.Farm; return true;
                case 5: villageType = VillageType.Barrier; return true;
                default:
                    villageType = default;
                    return false;
            }
        }

        private void HandleQuickSlot(int slotIndex)
        {
            if (TryGetVillageTypeFromSlot(slotIndex, out var targetType))
            {
                _ = OpenOrSwitchBuilding(targetType);
            }
        }

        public async Awaitable OpenOrSwitchBuilding(VillageType targetType)
        {
            if (_isExiting || _isSwitching) return;

            VillageBuilding targetBuilding = GetBuilding(targetType);
            if (targetBuilding == null) return;

            // 1. 외곽 뷰에 있는 경우: 마을로 복귀 후 건물 진입
            if (outsideManager != null && outsideManager.IsOutsideActive)
            {
                await outsideManager.ReturnToVillage();
            }

            // 2. 이미 같은 건물이 열려있는 경우
            if (IsBuildingOpen && _currentBuilding != null && _currentBuilding.buildingType == targetType)
            {
                return;
            }

            // 3. 다른 건물이 이미 열려있는 경우: 건물 간 다이렉트 전환 (Gliding Switch)
            if (IsBuildingOpen)
            {
                await SwitchBuilding(targetBuilding);
                return;
            }

            // 4. 마을 전경에서 새로 진입하는 경우
            OnBuildingClicked(targetBuilding);
        }

        public async Awaitable SwitchBuilding(VillageBuilding targetBuilding)
        {
            if (_isSwitching || targetBuilding == null) return;
            _isSwitching = true;

            try
            {
                var oldUI = currentBuildingUI;
                _currentBuilding = targetBuilding;
                OnActiveBuildingChanged?.Invoke(ActiveBuildingType);
                Vector3 targetCameraPos = targetBuilding.cameraFocusPoint.position + new Vector3(0, 0, -5);

                // 타겟 UI 인스턴스 가져오기 (없으면 새로 생성)
                if (!_uiInstanceCache.TryGetValue(targetBuilding.villageBuildingUIPrefab, out var nextUI))
                {
                    nextUI = Instantiate(targetBuilding.villageBuildingUIPrefab, buildingUICanvas.transform);
                    nextUI.OnExitButtonClicked += ExitBuilding;
                    _uiInstanceCache.Add(targetBuilding.villageBuildingUIPrefab, nextUI);
                }

                currentBuildingUI = nextUI;

                if (oldUI == nextUI)
                {
                    // 동일한 UI 프리팹 인스턴스를 사용하는 경우: 살짝 페이드 후 내용 교체 및 카메라 이동
                    var cg = nextUI.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        await Tween.Alpha(cg, 0.2f, 0.1f);
                    }

                    nextUI.SetBuildingUI(targetBuilding.buildingType);

                    // 카메라 글라이딩 이동
                    await Tween.Position(cinemachineCamera.transform, targetCameraPos, switchDuration, Ease.InOutCubic);

                    if (cg != null)
                    {
                        await Tween.Alpha(cg, 1f, 0.15f);
                    }
                }
                else
                {
                    // 서로 다른 UI 프리팹 인스턴스인 경우 (예: 일반 건물 ↔ 상점)
                    if (oldUI != null)
                    {
                        await oldUI.HideBuildingUI(switchUiDuration);
                    }

                    await Tween.Position(cinemachineCamera.transform, targetCameraPos, switchDuration, Ease.InOutCubic);

                    nextUI.SetBuildingUI(targetBuilding.buildingType);
                    await nextUI.ShowBuildingUI(uiShowDuration * 0.8f);
                }
            }
            finally
            {
                _isSwitching = false;
            }
        }

        private async void OnBuildingClicked(VillageBuilding building)
        {
            DevLog.Log("BuildingManager received click from: " + building.buildingType, this);

            if (currentBuildingUI != null || _isExiting || _isSwitching)
            {
                DevLog.Log("A building UI is already open or exiting. Ignoring click.");
                return;
            }

            // 해당 건물의 UI 프리팹이 이미 생성된 적 있는지 확인
            if (!_uiInstanceCache.TryGetValue(building.villageBuildingUIPrefab, out currentBuildingUI))
            {
                // 없다면 새로 생성하고 캐시에 등록
                currentBuildingUI = Instantiate(building.villageBuildingUIPrefab, buildingUICanvas.transform);
                currentBuildingUI.OnExitButtonClicked += ExitBuilding;

                _uiInstanceCache.Add(building.villageBuildingUIPrefab, currentBuildingUI);
            }

            _currentBuilding = building;
            OnActiveBuildingChanged?.Invoke(ActiveBuildingType);

            // 재사용된 UI에 현재 클릭된 건물의 타입 데이터를 새로 주입 (중요)
            currentBuildingUI.SetBuildingUI(building.buildingType);

            // ESC 스택에 닫기 Action 등록
            KeyInteractManager.Instance?.PushMenuAction(_exitBuildingAction);

            // 1. 카메라 이동 & 줌인 시작
            UpdateBlendDuration(cameraBlendDuration);
            cinemachineCamera.transform.position = building.cameraFocusPoint.position + new Vector3(0, 0, -5);
            cinemachineCamera.Lens.OrthographicSize = focusOrthoSize;
            cinemachineCamera.gameObject.SetActive(true);

            // 2. 카메라가 건물로 다가가는 움직임을 시각적으로 느낄 수 있도록 짧은 시차(Stagger) 부여
            if (uiDelay > 0f)
            {
                await Awaitable.WaitForSecondsAsync(uiDelay);
            }

            // 3. UI를 부드럽게 팝업
            await currentBuildingUI.ShowBuildingUI(uiShowDuration);
        }

        public async void ExitBuilding()
        {
            if (currentBuildingUI == null || _isExiting || _isSwitching) return;

            _isExiting = true;
            _currentBuilding = null;
            OnActiveBuildingChanged?.Invoke(null);
            KeyInteractManager.Instance?.RemoveMenuAction(_exitBuildingAction);
            KeyInteractManager.Instance?.SetMenuInputEnabled(false);
            foreach (var building in villageBuildings)
            {
                building.SetInteractionEnabled(false);
            }

            try
            {
                // 카메라를 마을 원래 전경으로 복귀
                UpdateBlendDuration(exitDuration);
                cinemachineCamera.gameObject.SetActive(false);

                // UI를 부드럽게 페이드아웃
                await currentBuildingUI.HideBuildingUI(exitDuration * 0.8f);
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
