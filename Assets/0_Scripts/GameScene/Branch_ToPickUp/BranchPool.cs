using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BranchPool : MonoBehaviour
{
    [SerializeField] private BranchPickUp[] branchPrefabs;
    [SerializeField] private int poolSizePerPrefab = 8;

    private readonly Dictionary<int, Queue<BranchPickUp>> inactiveBranches = new();
    private readonly Dictionary<int, BranchPickUp> activeBranches = new();

    public int ActiveCount => activeBranches.Count;
    public int PrefabCount => branchPrefabs?.Length ?? 0;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (branchPrefabs == null || branchPrefabs.Length == 0)
        {
            Debug.LogError("[BranchPool] Branch Prefab이 없습니다.");
            return;
        }

        for (int prefabIndex = 0; prefabIndex < branchPrefabs.Length; prefabIndex++)
        {
            Queue<BranchPickUp> pool = new();
            inactiveBranches.Add(prefabIndex, pool);

            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                BranchPickUp branch = Instantiate(branchPrefabs[prefabIndex], transform);

                branch.Initialize(this, prefabIndex);
                branch.gameObject.SetActive(false);

                pool.Enqueue(branch);
            }
        }
    }

    public BranchPickUp Get(int branchId, int prefabIndex, Vector3 position, Quaternion rotation)
    {
        if (activeBranches.ContainsKey(branchId)) return activeBranches[branchId];

        if (!inactiveBranches.TryGetValue(prefabIndex, out Queue<BranchPickUp> pool))
        {
            Debug.LogError($"[BranchPool] 존재하지 않는 Prefab Index : {prefabIndex}");
            return null;
        }

        if (pool.Count == 0)
        {
            Debug.LogWarning($"[BranchPool] Prefab {prefabIndex} Pool 부족");
            return null;
        }

        BranchPickUp branch = pool.Dequeue();

        branch.transform.SetPositionAndRotation(position, rotation);
        branch.PrepareForSpawn(branchId);

        activeBranches.Add(branchId, branch);

        branch.gameObject.SetActive(true);

        return branch;
    }

    public void Return(int branchId)
    {
        if (!activeBranches.TryGetValue(branchId, out BranchPickUp branch)) return;

        activeBranches.Remove(branchId);

        int prefabIndex = branch.PrefabIndex;

        branch.ResetForPool();
        branch.gameObject.SetActive(false);
        branch.transform.SetParent(transform);

        inactiveBranches[prefabIndex].Enqueue(branch);
    }

    public bool isActive(int branchId)
    {
        return activeBranches.ContainsKey(branchId);
    }
}
