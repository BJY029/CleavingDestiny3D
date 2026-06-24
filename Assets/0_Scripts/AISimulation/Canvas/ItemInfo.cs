using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public Image ItemImg;
    public TMP_Text ItemId;
    public TMP_Text ItemName;
    public TMP_Text ItemDesc;
    public TMP_Text ItemCost;

    [HideInInspector]
    public int UID;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void InitInfo(ItemSO item)
    {
        ItemImg.sprite = item.Icon;
        ItemId.text = item.itemId;
        ItemName.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.displayName_ID);
        ItemDesc.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);
        ItemCost.text = item.itemCost.ToString();
    }

    public void PlayItemUsed()
    {
        animator.Play("ItemUsedAnim");
    }

    public void RemoveInfo()
    {
        Destroy(this.gameObject);
    }
}
