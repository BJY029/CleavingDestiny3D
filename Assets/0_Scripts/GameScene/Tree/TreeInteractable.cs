using UnityEngine;

public class TreeInteractable : MonoBehaviour, ILookInteractable
{
	public void OnLookEnter(IPlayerAction pc)
	{
		PlayerCanvasController.Instance.SetHitTextActive();
		pc.isLookingAtTree = true;
	}

	public void OnLookExit(IPlayerAction pc)
	{
		PlayerCanvasController.Instance.SetHitTextUnActive();
		pc.isLookingAtTree = false;
	}

	public void OnInteract(IPlayerAction pc)
	{
		if (!PlayerCanvasController.Instance.selecting) return;
		pc.TryHit();
	}
}
