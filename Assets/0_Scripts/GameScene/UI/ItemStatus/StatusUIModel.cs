using System;
using System.Collections.Generic;
using System.Linq;
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
    //private readonly Dictionary<string, ItemStatusInfo> _dict = new();
    private readonly List<(string, ItemStatusInfo)> _info = new();

    private string Key(int owner, string statusId) => $"{owner}:{statusId}";

    public List<ItemStatusInfo> GetAllForOwner(int ownerActNum)
    {
        var list = new List<ItemStatusInfo>();

        foreach (var kv in _info)
        {
            if (kv.Item2.ownerActNum == ownerActNum)
                list.Add(kv.Item2);
        }
        return list;
    }

    public bool GetStatusInfoInstance(int ownerActNum, string statusId, out ItemStatusInfo info)
    {
        string key = Key(ownerActNum, statusId);

        for (int i = 0; i < _info.Count; i++)
        {
            if (_info[i].Item1 == key)
            {
                info = _info[i].Item2;
                return true;
            }
        }
        info = default;
        return false;
    }

    private void SortInfo()
    {
        _info.Sort((a, b) => b.Item2.remainingTurns.CompareTo(a.Item2.remainingTurns));
    }

    public void Client_Add(ItemStatusInfo info)
    {
        string key = Key(info.ownerActNum, info.statusId);
        int index = _info.FindIndex(x => x.Item1 == key);

        if (index != -1)
        {
            var ItemInfo = _info[index].Item2;
            ItemInfo.stackCount++;
            _info[index] = (key, ItemInfo);
            Debug.Log($"Item Stack Increase : {info.statusId}");
        }
        else
        {
            _info.Add((key, info));
            Debug.Log($"New Item Added : {info.statusId}");
        }

        SortInfo();
        StatusOnChanged?.Invoke();
    }

    public void Client_Update(ItemStatusInfo info)
    {
        string key = Key(info.ownerActNum, info.statusId);
        int index = _info.FindIndex(x => x.Item1 == key);
        if (index != -1)
        {
            _info[index] = (key, info);

            SortInfo(); //정렬 적용
            StatusOnChanged?.Invoke();
            Debug.Log($"Item status removed : {info.statusId}");
        }
        else
        {
            Debug.Log("None match item ERROR");
        }
    }

    public void Client_Remove(int ownerActNum, string statusId)
    {
        string key = Key(ownerActNum, statusId);
        int index = _info.FindIndex(x => x.Item1 == key);
        if (index != -1)
        {
            _info.RemoveAt(index);

            SortInfo(); //정렬 적용
            StatusOnChanged?.Invoke();
            Debug.Log($"Item status removed : {statusId}");
        }
        else
        {
            Debug.Log("None match item ERROR");
        }
    }
}
