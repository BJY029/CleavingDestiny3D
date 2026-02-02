using UnityEngine;

using System;

[Serializable]
public struct ItemStatusInfo
{
    public string itemId;
    public string statusId;
    public int ownerActNum;
    public int sourceActNum;
    public int remainingTurns;

    public bool isHiddenToEnemy;
    public int stackCount;
}
