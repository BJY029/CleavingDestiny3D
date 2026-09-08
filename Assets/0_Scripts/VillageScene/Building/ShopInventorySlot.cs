using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Village.Building
{
    public class ShopInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image iconImage;

        public ItemSO CurrentItem { get; private set; }
        public bool IsEmpty => CurrentItem == null;

        private ShopInventory _parent;

        public void Init(ShopInventory parent)
        {
            _parent = parent;
            SetItem(null);
        }

        public void SetItem(ItemSO item)
        {
            CurrentItem = item;
            if (item != null && item.Icon != null)
            {
                iconImage.sprite = item.Icon;
                iconImage.enabled = true;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
                iconImage.gameObject.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsEmpty) return;
            transform.localScale = Vector3.one * 1.1f;
            _parent?.OnSlotPointerEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = Vector3.one;
            _parent?.OnSlotPointerExit(this);
        }
    }
}