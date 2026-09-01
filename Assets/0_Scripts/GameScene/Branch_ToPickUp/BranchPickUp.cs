using UnityEngine;

public class BranchPickUp : MonoBehaviour, ITurnIndependentInteractable
{
    private bool isCollected;


    public void OnInteract(IPlayerAction pc)
    {
        if (isCollected) return;

        PlayerController playerCtrl = pc as PlayerController;

        if (playerCtrl == null) return;

        isCollected = true;

        GameSessionData.AddBranch();

        Debug.Log($"[BranchPickup] 나뭇가지 획득 / 이번 판: {GameSessionData.CollectedBranchCount}");

        Destroy(gameObject);
    }

    public void OnLookEnter(IPlayerAction pc)
    {
        Debug.Log("감지");
        PlayerController playerCtrl = pc as PlayerController;

        if (playerCtrl == null) return;

        //나뭇가지 줍기 UI 표시
    }

    public void OnLookExit(IPlayerAction pc)
    {
        Debug.Log("감지해제");
        //상호작용 UI 숨김
    }
}
