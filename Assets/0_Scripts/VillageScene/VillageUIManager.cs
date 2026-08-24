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

        public CanvasGroup villageNamePanel;
        private RectTransform villageNameTextRect;
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
            // 씬 로드 시 초기 상태 설정
            SetCanvasState(true);
            villageNameTextRect = villageNamePanel.GetComponent<RectTransform>();

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
                    villageNameTextRect.anchoredPosition = villageCam.WorldToScreenPoint(building.transform.position);
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
                villageNameTextRect.anchoredPosition = villageCam.WorldToScreenPoint(building.transform.position);
                villageNamePanel.alpha = 1f;
            }
            else
            {
                villageNamePanel.alpha = 0f;
            }
        }

        private void Update()
        {
            if (!isUpgradePhase || duration <= 0f) return;

            float now = (float)PhotonNetwork.Time;
            float remain = Mathf.Max(0f, endTime - now);
            
            villageTimer.fillAmount = Mathf.Clamp01(remain / duration);
            outsideTimer.fillAmount = Mathf.Clamp01(remain / duration);
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
