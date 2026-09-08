using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Village.Building;

namespace Village
{
    public class VillageUIManager : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI goldText;

        [Header("UIs")]
        private CanvasGroup canvasGroup;

        public Image villageTimer;
        public Image outsideTimer;
        [SerializeField] private TextMeshProUGUI outsideTimerText;
        [SerializeField] private float urgentTimerThreshold = 10f;
        [SerializeField] private Color urgentTimerColor = Color.red;

        public CanvasGroup villageNamePanel;
        private RectTransform villageNameTextRect;
        private Canvas villageCanvas;
        public TextMeshProUGUI villageNameText;
        
        [Header("References")]
        public VillageBuildingManager buildingManager;
        [SerializeField] private VillageBuilding compassBuilding;
        [SerializeField] private VillageBuilding outsideVillageBuilding;
        [SerializeField] private VillageStatusUI villageStatusUI;

        public Camera villageCam;

        private float startTime;
        private float endTime;
        private float duration;
        private bool isUpgradePhase;
        private Color villageTimerDefaultColor;
        private Color outsideTimerDefaultColor;
        private Color outsideTimerTextDefaultColor;
        private int lastOutsideTimerSecond = -1;

        public override void OnEnable()
        {
            base.OnEnable();

            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnTabKeyDown += ToggleVillageStatusPanel;
            }

            SyncRoomProperties();
        }

        public override void OnDisable()
        {
            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnTabKeyDown -= ToggleVillageStatusPanel;
            }

            base.OnDisable();
        }

        public void ToggleVillageStatusPanel()
        {
            if (buildingManager.IsBuildingOpen) return;

            if (villageStatusUI.IsOpen)
            {
                villageStatusUI.Close();
            }
            else
            {
                villageStatusUI.Open();
            }
        }

        public void Init()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            villageTimerDefaultColor = villageTimer.color;
            outsideTimerDefaultColor = outsideTimer.color;
            if (outsideTimerText != null)
                outsideTimerTextDefaultColor = outsideTimerText.color;
            // 씬 로드 시 초기 상태 설정
            SetCanvasState(true);
            villageNameTextRect = villageNamePanel.GetComponent<RectTransform>();
            villageCanvas = villageNamePanel.GetComponentInParent<Canvas>();

            foreach (var building in buildingManager.villageBuildings)
            {
                building.OnVillagePointerEnterExit += OnVillagePointerEnterExit;
            }
            compassBuilding.OnVillagePointerEnterExit += OnVillagePointerEnterExit;
            outsideVillageBuilding.OnVillagePointerEnterExit += OnVillagePointerEnterExit;
            
            villageNamePanel.alpha = 0f;

            // 씬 로드 시점에 이미 설정된 RoomProperties를 동기화
            SyncRoomProperties();

            // OpenVillageBtn.onClick.AddListener(OnClickOpenVillage);
            // CloseVillageBtn.onClick.AddListener(OnClickCloseVillage);
        }

        private void OnVillagePointerEnterExit(VillageBuilding building, bool isEnter)
        {
            if (isEnter)
            {
                if (building == compassBuilding || building == outsideVillageBuilding)
                {
                    string textId = building == compassBuilding ? "Compass_Title" : "Village_Enter_Title";
                    villageNameText.SetText(LocalizationManager.Instance.GetText(CSV_Type.Village, textId));
                    PositionVillageName(building);
                    villageNamePanel.alpha = 1f;
                    return;
                }

                int level = VillageSystem.VillageStat.GetVillageLevel(building.buildingType);
                int upgradeCost = VillageSystem.VillageStat.GetLevelUpgradedCost(building.buildingType, level);
                int currentGold = VillageSystem.VillageLogic.GetMyGold();
                bool canUpgrade = upgradeCost <= 0 || currentGold >= upgradeCost;

                villageNameText.SetText(
                    canUpgrade ? building.HoverTextFormat : building.NotEnoughGoldHoverTextFormat,
                    level + 1,
                    upgradeCost);
                PositionVillageName(building);
                villageNamePanel.alpha = 1f;
            }
            else
            {
                villageNamePanel.alpha = 0f;
            }
        }

        private void PositionVillageName(VillageBuilding building)
        {
            if (villageNameTextRect.parent is not RectTransform parentRect) return;

            Vector2 screenPosition = villageCam.WorldToScreenPoint(building.transform.position);
            Camera uiCamera = villageCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : villageCanvas.worldCamera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPosition, uiCamera, out Vector2 localPosition))
            {
                villageNameTextRect.localPosition = localPosition;
            }
        }

        private void Update()
        {
            if (!isUpgradePhase || duration <= 0f) return;

            float now = (float)PhotonNetwork.Time;
            float remain = Mathf.Max(0f, endTime - now);

            float warningDuration = Mathf.Clamp(urgentTimerThreshold, 0.001f, duration);
            bool isUrgent = remain <= warningDuration;
            float fillAmount = Mathf.Clamp01(remain / (isUrgent ? warningDuration : duration));
            villageTimer.fillAmount = fillAmount;
            villageTimer.color = isUrgent ? urgentTimerColor : villageTimerDefaultColor;
            outsideTimer.fillAmount = fillAmount;
            outsideTimer.color = isUrgent ? urgentTimerColor : outsideTimerDefaultColor;

            if (outsideTimerText != null)
                outsideTimerText.color = isUrgent ? urgentTimerColor : outsideTimerTextDefaultColor;

            int remainingSeconds = Mathf.CeilToInt(remain);
            if (outsideTimerText != null && remainingSeconds != lastOutsideTimerSecond)
            {
                outsideTimerText.SetText($"{remainingSeconds / 60:00}:{remainingSeconds % 60:00}");
                lastOutsideTimerSecond = remainingSeconds;
            }
        }

        // 기존 SetActiveCanvas와 RPC를 하나로 통합 및 단순화
        public void SetCanvasState(bool active)
        {
            if (active)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // 마을 페이즈 종료 시 (씬 언로드 전) 호출될 수도 있음
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            SyncRoomProperties(propertiesThatChanged);
        }

        private void SyncRoomProperties(Hashtable properties = null)
        {
            if (!PhotonNetwork.InRoom) return;

            var roomProps = properties ?? PhotonNetwork.CurrentRoom.CustomProperties;

            if (roomProps.TryGetValue(RoomPropKeys.VillageUpgradeStartEndTime, out object value)
                && value is Vector2 times)
            {
                startTime = times.x;
                endTime = times.y;
                duration = Mathf.Max(0.001f, endTime - startTime);
            }

            if (roomProps.TryGetValue(RoomPropKeys.IsVillageUpgradePhase, out object isPhaseObj)
                && isPhaseObj is bool isPhase)
            {
                isUpgradePhase = isPhase;
            }
        }
    }
}
