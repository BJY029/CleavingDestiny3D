using UnityEngine;

public class NewDrugGameEvent
{
    public NewDrugGameEventType Type;
    public PlayerController Actor;

    public int TurnIndex;
    public int WaveIndex;

    public int DamageAmount;
    public int StaminaAmount;

    public int DefenseValue;
    public int DefneseDelta;

    public ItemSO UsedItem;
}
