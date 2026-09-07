using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public int version = 1;

    public int branchCount;
    public List<string> ownedAxeSkinIdx = new();
    public string equippedAxeSkinId;
}
