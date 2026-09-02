using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BranchPool : MonoBehaviour
{
    [SerializeField] private BranchPickUp[] branchPrefabs;
    [SerializeField] private int poolSize = 20;

    private readonly Queue<BranchPickUp> inactiveBranchs = new();
    private readonly HashSet<BranchPickUp> activeBranchs = new();

    public int ActiveCount => activeBranchs.Count;

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            BranchPickUp randomBranch = branchPrefabs[Random.Range(0, branchPrefabs.Length)];
            BranchPickUp branch = Instantiate(randomBranch, transform);

            branch.Initialize(this);
            branch.gameObject.SetActive(false);

            inactiveBranchs.Enqueue(branch);
        }
    }

    public BranchPickUp Get(Vector3 position, Quaternion rotation)
    {
        if (inactiveBranchs.Count == 0)
        {
            return null;
        }

        BranchPickUp branch = inactiveBranchs.Dequeue();

        branch.transform.SetPositionAndRotation(position, rotation);
        branch.PrepareForSpawn();

        activeBranchs.Add(branch);

        branch.gameObject.SetActive(true);

        return branch;
    }

    public void Return(BranchPickUp branch)
    {
        if (branch == null) return;

        if (!activeBranchs.Remove(branch)) return;

        branch.gameObject.SetActive(false);
        branch.transform.SetParent(transform);

        inactiveBranchs.Enqueue(branch);
    }
}
