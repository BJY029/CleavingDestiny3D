using UnityEngine;

public class TreeInteractable : MonoBehaviour, ILookInteractable
{
	public void OnLookEnter(PlayerController pc)
	{
		PlayerCanvasController.Instance.SetHitTextActive();
		pc.isLookingAtTree = true;
	}

	public void OnLookExit(PlayerController pc)
	{
		PlayerCanvasController.Instance.SetHitTextUnActive();
		pc.isLookingAtTree = false;
	}

	public void OnInteract(PlayerController pc)
	{
		if (!PlayerCanvasController.Instance.selecting) return;
		pc.TryHit();
	}
}
