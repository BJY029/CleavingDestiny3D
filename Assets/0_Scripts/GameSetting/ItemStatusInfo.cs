using UnityEngine;

[CreateAssetMenu(fileName = "ItemStatusInfo", menuName = "Scriptable Objects/ItemStatusInfo")]
public class ItemStatusInfo : ScriptableObject
{
    public string itemId;
    public int uniqudId;
    public int ownerActNum;
    public int sourceActNum;
    public int remainingTurns;

    public bool isHiddenToEnemy;
    public int stackCount; //스택 카운트용(없으면 1)
}
