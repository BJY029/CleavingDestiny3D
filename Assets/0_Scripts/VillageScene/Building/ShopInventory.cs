using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Potan.CoreUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Village.Building
{
    public class ShopInventory : MonoBehaviourPunCallbacks
    {
        [Header("Slot Settings")]
        [SerializeField] private Transform slotContainer;

        [SerializeField] private List<ShopInventorySlot> slotList;

        [Header("Tooltip UI")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshProUGUI tooltipNameText;
        [SerializeField] private TextMeshProUGUI tooltipDescText;
        
        [Header("Capacity UI")]
        [SerializeField] private TextMeshProUGUI capacityText;
        
        private void Awake()
        {
            tooltipPanel.SetActive(false);

            foreach (var slot in slotList)
            {
                slot.Init(this);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            RefreshInventory();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            HideTooltip();
        }
        
        public void RefreshInventory()
        {
            if (!PhotonNetwork.InRoom) return;

            int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
            string invStr = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(myActor));
            int capacity = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(myActor));

            var decodedSlots = ItemInfoSerializer.Decode(invStr, capacity);
            int itemCount = 0;

            for (int i = 0; i < slotList.Count; i++)
            {
                var (uniqueId, itemId) = decodedSlots[i];
                ItemSO item = null;

                if (uniqueId > 0 && !string.IsNullOrEmpty(itemId) && itemId != "_")
                {
                    item = ItemDB.Instance.Get(itemId);
                    if (item != null) itemCount++;
                }

                slotList[i].gameObject.SetActive(true);
                slotList[i].SetItem(item);
            }

            capacityText.SetText("{0}/{1}", itemCount, capacity);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (!PhotonNetwork.InRoom) return;

            int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
            if (propertiesThatChanged.ContainsKey(ItemPropKeys.INV(myActor)) ||
                propertiesThatChanged.ContainsKey(ItemPropKeys.INV_CAPACITY(myActor)))
            {
                RefreshInventory();
            }
        }

        public void OnSlotPointerEnter(ShopInventorySlot slot)
        {
            if (slot == null || slot.IsEmpty || slot.CurrentItem == null) return;

            ShowTooltip(slot.CurrentItem);
        }

        public void OnSlotPointerExit(ShopInventorySlot slot)
        {
            HideTooltip();
        }

        private void ShowTooltip(ItemSO item)
        {
            if (item == null) return;

            string itemName = LocalizationManager.Instance.GetText(CSV_Type.Item, item.displayName_ID);
            string itemDesc = LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);

            if (tooltipNameText != null) tooltipNameText.SetText(itemName);
            if (tooltipDescText != null) tooltipDescText.SetText(itemDesc);
            if (tooltipPanel != null) tooltipPanel.SetActive(true);
        }

        private void HideTooltip()
        {
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
        }
    }

    
}
