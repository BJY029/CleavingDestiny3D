using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class BranchNetworkManager : MonoBehaviourPunCallbacks
{
    public static BranchNetworkManager Instance { get; private set; }

    [SerializeField] private BranchPool branchPool;

    private readonly HashSet<int> activeBranchIds = new();

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

    public void RequestPickUp(int branchId)
    {
        if (branchId < 0) return;

        photonView.RPC(nameof(RPC_RequestPickUp), RpcTarget.MasterClient, branchId);
    }

    [PunRPC]
    private void RPC_SpawnBranch(int branchId, int prefabIndex, Vector3 position, Quaternion rotation)
    {
        nextBranchId = Mathf.Max(nextBranchId, branchId + 1);

        if (!activeBranchIds.Add(branchId)) return;

        BranchPickUp branch = branchPool.Get(branchId, prefabIndex, position, rotation);

        if (branch != null) return;

        activeBranchIds.Remove(branchId);
        Debug.LogError($"[BranchNetworkManager] Branch Spawn 실패 : {branchId}");
    }

    [PunRPC]
    private void RPC_RequestPickUp(int branchId, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!activeBranchIds.Remove(branchId)) return;

        int winnerActorNumber = info.Sender.ActorNumber;

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

            Debug.Log($"[Branch] 획득 성공 / 이번 판: {GameSessionData.CollectedBranchCount}");
        }
    }
}
