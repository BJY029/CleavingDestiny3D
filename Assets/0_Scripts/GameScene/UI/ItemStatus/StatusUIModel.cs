using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StatusUIModel : MonoBehaviourPun
{
    public static StatusUIModel instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public Action StatusOnChanged;
    private readonly Dictionary<string, ItemStatusInfo> _dict = new();

    private string Key(int owner, string statusId) => $"{owner}:{statusId}";

    public List<ItemStatusInfo> GetAllForOwner(int ownerActNum)
    {
        var list = new List<ItemStatusInfo>();

        foreach (var kv in _dict)
        {
            if (kv.Value.ownerActNum == ownerActNum)
                list.Add(kv.Value);
        }
        return list;
    }

    public bool GetStatusInfoInstance(int ownerActNum, string statusId, out ItemStatusInfo info)
    {
        string key = Key(ownerActNum, statusId);

        if (_dict.TryGetValue(key, out info))
        {
            return true;
        }
        info = default;
        return false;
    }

    public void Client_Add(ItemStatusInfo info)
    {
        _dict[Key(info.ownerActNum, info.statusId)] = info;
        StatusOnChanged?.Invoke();
        Debug.Log($"New Item Added : {info.statusId}");
    }

    public void Client_Update(ItemStatusInfo info)
    {
        _dict[Key(info.ownerActNum, info.statusId)] = info;
        StatusOnChanged?.Invoke();
        Debug.Log($"Item status updated : {info.statusId}");
    }

    public void Client_Remove(int ownerActNum, string statusId)
    {
        _dict.Remove(Key(ownerActNum, statusId));
        StatusOnChanged?.Invoke();
        Debug.Log($"Item status removed : {statusId}");
    }
}
