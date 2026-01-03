using UnityEngine;

public class WorldInventorySlot : MonoBehaviour, ILookInteractable
{
    private int slotIndex;
    private MeshRenderer quadRenderer;
    private ItemSO currentItem;

	private void Start()
	{
		quadRenderer = GetComponent<MeshRenderer>();
	}

	public bool HasItem()
    {
        return currentItem == null ? false : true;
    }

    public void SetItem(ItemSO item, Material material)
    {
        currentItem = item;
    }

    public void OnLookEnter(PlayerController pc)
    {
        if (!HasItem()) return;
        //TooltipUI.Instance.Show()
        //Highlight();
    }

    public void OnLookExit(PlayerController pc)
    {
        //ToolTipUI.Instance.Hide()
        //Highlight();
    }

    public void OnInteract(PlayerController pc)
    {
        if(!HasItem()) return;
        if (!GameHelper.IsMyTurn()) return;
        //InventoryAuthority.Instance.RequestUseItem();
    }

    private void Highlight(bool on)
    {

    }
}
