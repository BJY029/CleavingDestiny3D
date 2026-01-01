using UnityEngine;
using UnityEngine.UI;

public class InventoryEachSlot : MonoBehaviour
{
    public Image icon;
    public Text ItemName;
    public Text ItemDescription;
    
    public void Set(ItemSO item)
    {
        if (item == null)
        {
            icon.sprite = null;
            icon.enabled = false;
            ItemName.text = string.Empty;
            ItemDescription.text = string.Empty;
            return;
        }

        icon.enabled = true;
        icon.sprite = item.icon;
        ItemName.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.displayName_ID);
        ItemDescription.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);
    }
}