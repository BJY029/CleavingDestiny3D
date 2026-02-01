using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StatusSyncHub : MonoBehaviourPun
{
    public static StatusSyncHub instance;

    private void Awake()
    {
        if (instance == null) instance = null;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Master calls after adding 1 state; Requests all clients to add status for UI
    /// </summary>
    /// <param name="info"></param>
    public void Master_BroadcastAdd(ItemStatusInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_UI_AddStatus), RpcTarget.All, JsonUtility.ToJson(info));
    }

    /// <summary>
    /// Master calls after change of remaingTurns/stack
    /// </summary>
    /// <param name="info"></param>
    public void Master_BroadcastUpdate(ItemStatusInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_UI_UpdateStatus), RpcTarget.All, JsonUtility.ToJson(info));
    }

    public void Master_BroadcastRemove(int ownerActNum, string itemId, int uniqueId)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC(nameof(RPC_UI_RemoveStatus), RpcTarget.All, ownerActNum, itemId, uniqueId.ToString());
    }

    [PunRPC]
    private void RPC_UI_AddStatus(string json, PhotonMessageInfo msgInfo)
    {
        if (!msgInfo.Sender.IsMasterClient) return;

        var info = JsonUtility.FromJson<ItemStatusInfo>(json);
        StatusUIModel.instance.Client_Add(info);
    }

    [PunRPC]
    private void RPC_UI_UpdateStatus(string json, PhotonMessageInfo msgInfo)
    {
        if (!msgInfo.Sender.IsMasterClient) return;

        var info = JsonUtility.FromJson<ItemStatusInfo>(json);
        StatusUIModel.instance.Client_Update(info);
    }

    [PunRPC]
    private void RPC_UI_RemoveStatus(int ownerActNum, string itemId, string uniqueId, PhotonMessageInfo msgInfo)
    {
        if (!msgInfo.Sender.IsMasterClient) return;

        StatusUIModel.instance.Client_Remove(ownerActNum, itemId, uniqueId);
    }
}
