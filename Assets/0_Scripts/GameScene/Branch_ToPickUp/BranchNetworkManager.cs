using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System;

public class BranchNetworkManager : MonoBehaviourPunCallbacks
{
    public static BranchNetworkManager Instance { get; private set; }

    [SerializeField] private BranchPool branchPool;

    private readonly Dictionary<int, Vector3> activeBranchIds = new();

    public Dictionary<int, Vector3> GetActiveBranchSnapshot()
    {
        return new Dictionary<int, Vector3>(activeBranchIds);
    }

    public event Action<int> OnBranchRemoved;

    private int nextBranchId;

    public int ActiveCount => activeBranchIds.Count;
    public int PrefabCount => branchPool != null ? branchPool.PrefabCount : 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void MasterSpawnBranch(int prefabIndex, Vector3 position, Quaternion rotation)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int branchId = nextBranchId++;

        photonView.RPC(nameof(RPC_SpawnBranch), RpcTarget.All, branchId, prefabIndex, position, rotation);
    }

    public bool TryCollectBranchByAI(int branchId, Vector3 aiPos)
    {
        if (!activeBranchIds.TryGetValue(branchId, out Vector3 branchPosition))
            return false;

        float distance = Vector3.Distance(aiPos, branchPosition);

        if (distance > 3f) return false;

        if (!TryConsumeBranch(branchId)) return false;

        RemoveBranchByAI(branchId);

        return true;
    }

    public void RequestPickUp(int branchId)
    {
        if (branchId < 0) return;


        photonView.RPC(nameof(RPC_RequestPickUp), RpcTarget.MasterClient, branchId);
    }

    private void RemoveBranchByAI(int branchId)
    {
        branchPool.Return(branchId);
    }

    [PunRPC]
    private void RPC_SpawnBranch(int branchId, int prefabIndex, Vector3 position, Quaternion rotation)
    {
        nextBranchId = Mathf.Max(nextBranchId, branchId + 1);

        if (activeBranchIds.ContainsKey(branchId)) return;
        activeBranchIds.Add(branchId, position);

        BranchPickUp branch = branchPool.Get(branchId, prefabIndex, position, rotation);

        if (branch != null) return;

        activeBranchIds.Remove(branchId);
        Debug.LogError($"[BranchNetworkManager] Branch Spawn 실패 : {branchId}");
    }

    [PunRPC]
    private void RPC_RequestPickUp(int branchId, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int winnerActorNumber = info.Sender.ActorNumber;

        if (!TryConsumeBranch(branchId)) return;

        photonView.RPC(nameof(RPC_ConfirmPickUp), RpcTarget.All, branchId, winnerActorNumber);
    }

    [PunRPC]
    private void RPC_ConfirmPickUp(int branchId, int winnerActorNumber)
    {
        activeBranchIds.Remove(branchId);

        branchPool.Return(branchId);

        if (PhotonNetwork.LocalPlayer.ActorNumber == winnerActorNumber)
        {
            GameSessionData.AddBranch();

            AudioManager.Instance.PlaySfx2D("PickupBranch");

            Debug.Log($"[Branch] 획득 성공 / 이번 판: {GameSessionData.CollectedBranchCount}");
        }
    }

    private bool TryConsumeBranch(int branchId)
    {
        if (!activeBranchIds.Remove(branchId))
            return false;

        OnBranchRemoved?.Invoke(branchId);

        return true;
    }
}
