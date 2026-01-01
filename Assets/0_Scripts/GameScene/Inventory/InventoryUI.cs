using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	[SerializeField] private Inventory inventory;
	[SerializeField] private InventoryEachSlot[] inventorySlots;

	private void OnEnable()
	{
		inventory.OnChanged += Refresh;
		Refresh();
	}

	private void OnDisable()
	{
		inventory.OnChanged -= Refresh;
	}

	private void Refresh()
	{
		var slots = inventory.Slots;
		for(int i = 0; i < inventorySlots.Length; i++)
		{
			if (i >= slots.Count) break;
			inventorySlots[i].Set(slots[i].item);	
		}
	}
}
