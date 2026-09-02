using UnityEngine;

public class BranchPickUp : MonoBehaviour, ITurnIndependentInteractable
{
    private BranchPool ownerPool;

    private bool isCollected;
    private bool isPickUpRequested;

    public int BranchId { get; private set; } = -1;
    public int PrefabIndex { get; private set; }

    public void Initialize(BranchPool pool, int prefabIndex)
    {
        ownerPool = pool;
        PrefabIndex = prefabIndex;
    }

    public void PrepareForSpawn(int branchId)
    {
        BranchId = branchId;
        isCollected = false;
        isPickUpRequested = false;
    }

    public void ResetForPool()
    {
        BranchId = -1;
        isCollected = false;
        isPickUpRequested = false;
    }

    public void OnInteract(IPlayerAction pc)
    {
        if (isCollected || isPickUpRequested) return;

        if (pc is not PlayerController) return;

        if (BranchId < 0) return;

        isPickUpRequested = true;

        BranchNetworkManager.Instance.RequestPickUp(BranchId);
    }

    public void OnLookEnter(IPlayerAction pc)
    {
        if (pc is not PlayerController)
            return;

        Debug.Log($"나뭇가지 감지 : {BranchId}");

        // [F] 나뭇가지 줍기 UI 표시
    }

    public void OnLookExit(IPlayerAction pc)
    {
        if (pc is not PlayerController)
            return;

        // 상호작용 UI 숨김
    }
}
