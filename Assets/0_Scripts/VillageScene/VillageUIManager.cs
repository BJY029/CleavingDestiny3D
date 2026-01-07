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

            VillageSystem.VillageLogic.OnGoldChanged += UpdateGoldText;

            OpenVillageBtn.onClick.AddListener(OnClickOpenVillage);
            CloseVillageBtn.onClick.AddListener(OnClickCloseVillage);
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (VillageSystem.VillageLogic != null)
            {
                VillageSystem.VillageLogic.OnGoldChanged -= UpdateGoldText;
            }
        }

        private void Update()
        {
            if (!isUpgradePhase) return;

            float now = (float)PhotonNetwork.Time;
            float remain = endTime - now;
            TimeSlider.value = remain / duration;
        }

        private void UpdateGoldText(int gold)
        {
            goldText.SetText("Gold: {0}", gold);
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


        public void SetActiveCanvas(bool active)
        {
            photonView.RPC(nameof(RPC_SetActiveCanvas), RpcTarget.All, active);
        }

        [PunRPC]
        private void RPC_SetActiveCanvas(bool active)
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
            if (propertiesThatChanged.ContainsKey(RoomPropKeys.VillageUpgradeStartEndTime))
            {
                float[] times = PhotonPropertyHelper.GetRoomProp<float[]>(RoomPropKeys.VillageUpgradeStartEndTime);
                startTime = times[0];
                endTime = times[1];
                duration = endTime - startTime;
            }

            if (propertiesThatChanged.ContainsKey(RoomPropKeys.IsVillageUpgradePhase))
            {
                isUpgradePhase = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
            }
        }
    }
}