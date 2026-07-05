using UnityEngine;

public class SpendStaminaInOneTurnMission : INewDrugMission
{
    public string MissionName => "고녿도 기력 반응";

    public string MissionDesc => $"단 1턴 안에 기력을 {requiredStamina} 이상 소모하세요.";

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
