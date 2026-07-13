using UnityEngine;

public class NewDrugGameEvent
{
    public NewDrugGameEventType Type;
    public PlayerController Actor;

    public int ActorNumber;
    public int TurnOrderIndex;

    public int TurnIndex;//
    public int WaveIndex;//

    public int DamageAmount;//
    public int StaminaAmount;
    public float TreeHPAfter;//

    public float DefenseValue;
    public float DefneseDelta;//

    public ItemSO UsedItem;//
}
