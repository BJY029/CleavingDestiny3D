using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Village.Building
{
    public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI itemGoldText;
        [SerializeField] Image selectedItemHighlight;
        public CanvasGroup canvasGroup; // 아이템의 상호작용 가능 여부를 제어하기 위한 CanvasGroup

        public ShopUI ParentShopUI { get; set; } // 아이템이 속한 ShopUI 참조

        ItemSO currentItem;
        public bool IsEmpty => currentItem == null; // 아이템이 없는 경우 true 반환
        public bool IsSelected { get; private set; } // 아이템이 선택된 상태인지 여부

        public void SetShopItem(ItemSO item)
        {
            currentItem = item;
            itemIcon.sprite = item.Icon; // 아이템 아이콘 설정
            itemGoldText.text = $"{100} <color=yellow>G</color>";   // TODO 아이템 가격 설정 - 일단 고정값으로 100골드 표시
        }

        public string GetItemDescription()
        {
            if (currentItem == null)
                return string.Empty;

            return LocalizationManager.Instance.GetText(CSV_Type.Item, currentItem.itemDesc_ID); // 아이템 설명 반환
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            selectedItemHighlight.gameObject.SetActive(selected); // 선택된 아이템 강조 표시 활성화/비활성화
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsEmpty)
                return;

            transform.localScale = Vector3.one * 1.1f; // 아이템이 있는 경우 마우스 오버 시 크기 증가
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsEmpty)
                return;

            transform.localScale = Vector3.one; // 마우스가 아이템에서 벗어날 때 원래 크기로 돌아감
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsEmpty)
                return;

            ParentShopUI.ShopItemSelect(this); // 클릭된 아이템을 ShopUI로 전달하여 처리
        }
    }
}