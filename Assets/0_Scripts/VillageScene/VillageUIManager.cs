using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village
{
    public class VillageUIManager : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI goldText;

        [Header("UIs")]
        public Slider TimeSlider;
        public Button OpenVillageBtn;
        public Button CloseVillageBtn;
        public GameObject VillagePanel;
        private CanvasGroup canvasGroup;

        private float startTime;
        private float endTime;
        private float duration;
        private bool isUpgradePhase;

        public void Init()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            // 씬 로드 시 초기 상태 설정
            SetCanvasState(true);

            // OpenVillageBtn.onClick.AddListener(OnClickOpenVillage);
            // CloseVillageBtn.onClick.AddListener(OnClickCloseVillage);
        }

        private void Update()
        {
            if (!isUpgradePhase) return;

            float now = (float)PhotonNetwork.Time;
            float remain = endTime - now;
            TimeSlider.value = remain / duration;
        }


        public void OnClickAddGoldButton()
        {
            VillageSystem.VillageLogic.AddGold(100);
        }

        public void OnClickOpenVillage()
        {
            VillagePanel.SetActive(true);
            OpenVillageBtn.gameObject.SetActive(false);
        }

        public void OnClickCloseVillage()
        {
            VillagePanel.SetActive(false);
            OpenVillageBtn.gameObject.SetActive(true);
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
                VillagePanel.SetActive(false);
                OpenVillageBtn.gameObject.SetActive(true);

                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(RoomPropKeys.VillageUpgradeStartEndTime, out object value)
                && value is Vector2 times)
            {
                startTime = times.x;
                endTime = times.y;
                duration = endTime - startTime;
            }

            if (propertiesThatChanged.ContainsKey(RoomPropKeys.IsVillageUpgradePhase))
            {
                isUpgradePhase = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
            }
        }
    }
}
