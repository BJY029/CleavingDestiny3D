using UnityEngine;

public class SpendStaminaInOneTurnMission : INewDrugMission
{
    public string MissionName => LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_NAME_SPENDSTAMINA");
    public string MissionDesc => LocalizationManager.Instance.GetFormatText(CSV_Type.Mission, "M_DESC_SPENDSTAMINA", requiredStamina);
    public bool IsSuccess { get; private set; }

    public bool IsFailed { get; private set; }

    private int requiredStamina;
    private int currentTurnStaminaSpent;
    public SpendStaminaInOneTurnMission(int requiredStamina)
    {
        this.requiredStamina = requiredStamina;
    }

    public void Init(NewDrugMissionContext context)
    {
        IsSuccess = false;
        IsFailed = false;

        currentTurnStaminaSpent = 0;
    }

    public void OnGameEvent(NewDrugGameEvent gameEvent)
    {
        if (IsSuccess || IsFailed) return;

        switch (gameEvent.Type)
        {
            case NewDrugGameEventType.TurnStarted:
                currentTurnStaminaSpent = 0;
                break;
            case NewDrugGameEventType.StaminaSpent:
                currentTurnStaminaSpent += gameEvent.StaminaAmount;

                if (currentTurnStaminaSpent >= requiredStamina) IsSuccess = true;
                break;
            case NewDrugGameEventType.TurnEnded:
                currentTurnStaminaSpent = 0;
                break;
        }
    }
}
