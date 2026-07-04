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
    public float turnTime = 40f;

    [Header("Items")]
    public int initialUID = 1;
    public int itemOfferCount = 3;
    public int lockpickCount = 0;
    public int lockCount = 1;
    public float common_reduction_rate = 0.3f;
    public float hero_reduction_rate = 0.4f;
    public float rare_reduction_rate = 0.5f;
    public float legendary_reduction_rate = 0.6f;

    [Header("Village Settings")]
    public float villagePhaseTime = 60f;

    [Header("Match Control")]
    public int LoserActNum = -1;
    public string MatchEndReason = MatchResultReason.None.ToString();
    public int ResolvedTurnIdx = -1;
}

