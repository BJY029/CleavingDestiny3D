using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemOfferUIController : MonoBehaviour
{
    private string itemId;
    [Header("UI Components")]
    public Image iconImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemRarity;
    public TextMeshProUGUI itemCost;
    public TextMeshProUGUI itemDesc;
    public Button selectBtn;


    //인자로 넘어온 아이템 정보로 각 정보 채우기
    public void SetItem(string itemId)
    {
        this.itemId = itemId;

        ItemSO item = ItemDB.Instance.Get(itemId);
        if(item == null)
        {
            Debug.LogError("item is null");
            Debug.LogError($"item id : {itemId}");
            return;
        }

		iconImage.sprite = item.Icon;
		itemName.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.displayName_ID);
        itemRarity.text = item.itemClass.ToString();
        itemCost.text = "Cost : " + item.itemCost.ToString();
        itemDesc.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);

        //select 버튼이 리스너 달기
        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(SelectItem);
    }

    //select 버튼이 눌리면 다음 함수 호출
    private void SelectItem()
    {
        ItemOfferCanvasController.instance.SelectedItem(itemId);
    }
}

