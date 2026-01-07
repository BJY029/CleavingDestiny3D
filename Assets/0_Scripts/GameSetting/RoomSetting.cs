using UnityEngine;

[CreateAssetMenu(fileName = "RoomSetting", menuName = "Scriptable Objects/RoomSetting")]
public class RoomSetting : ScriptableObject
{
    [Header("Default Tree Stats")]
    public float treeHP = 300000f;
    public float treeAtkPow = 1500f;

    [Header("Default Game Flow")]
    public int startDay = 1;
    public int initialTurn = 0;
    public int initialTurnIndex = 1;
    public GamePhaseValue initialPhase = GamePhaseValue.DAY;
    public int maxWave = 3;
    public int initialWave = 0;

    [Header("Items")]
    public int initialUID = 1;
    public int itemOfferCount = 3;

    [Header("Village Settings")]
    public float villageUpgradeLimitedTime = 60f;
}

