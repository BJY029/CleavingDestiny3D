using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Village.Building
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI itemGoldText;

        ItemSO currentItem;

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
    }
}