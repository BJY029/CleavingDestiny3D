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

    private string Key(int owner, string itemId, string uniqudId) => $"{owner}:{itemId}:{uniqudId}";

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

    public void Client_Add(ItemStatusInfo info)
    {
        _dict[Key(info.ownerActNum, info.itemId, info.uniqudId.ToString())] = info;
        StatusOnChanged?.Invoke();
    }

    public void Client_Update(ItemStatusInfo info)
    {
        _dict[Key(info.ownerActNum, info.itemId, info.uniqudId.ToString())] = info;
        StatusOnChanged?.Invoke();
    }

    public void Client_Remove(int ownerActNum, string itemId, string uniqueId)
    {
        _dict.Remove(Key(ownerActNum, itemId, uniqueId));
        StatusOnChanged?.Invoke();
    }
}
