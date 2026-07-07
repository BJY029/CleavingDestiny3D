using UnityEngine;

public class ReachDefenseInOneTurnMission : INewDrugMission
{
    public string MissionName => LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_NAME_REACHDEFENSE");
    public string MissionDesc => LocalizationManager.Instance.GetFormatText(CSV_Type.Mission, "M_DESC_REACHDEFENSE", requiredDefenseAmount);
    public bool IsSuccess { get; private set; }

    public bool IsFailed { get; private set; }

    private float requiredDefenseAmount;
    private float currentTurnDefenseGain;

    public ReachDefenseInOneTurnMission(float requiredDefenseAmount)
    {
        this.requiredDefenseAmount = requiredDefenseAmount;
    }

    public void Init(NewDrugMissionContext context)
    {
        IsSuccess = false;
        IsFailed = false;

        currentTurnDefenseGain = 0f;
    }

    public void OnGameEvent(NewDrugGameEvent gameEvent)
    {
        if (IsSuccess || IsFailed) return;

        switch (gameEvent.Type)
        {
            case NewDrugGameEventType.TurnStarted:
                currentTurnDefenseGain = 0f;
                break;

            case NewDrugGameEventType.DefenseChanged:
                currentTurnDefenseGain += gameEvent.DefneseDelta;

                if (currentTurnDefenseGain >= requiredDefenseAmount)
                {
                    IsSuccess = true;
                }

                break;

            case NewDrugGameEventType.TurnEnded:
                if (currentTurnDefenseGain < requiredDefenseAmount)
                    IsFailed = true;
                break;
        }
    }
}
