using UnityEngine;

public class PrecisionDamageMission : INewDrugMission
{
    public string MissionName => LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_NAME_PRECISIONDAMAGE");

    public string MissionDesc => LocalizationManager.Instance.GetFormatText(CSV_Type.Mission, "M_DESC_PRECISIONDAMAGE", threshold);

    public bool IsSuccess { get; private set; }

    public bool IsFailed { get; private set; }

    private int threshold;

    private int currentTrunDamage;
    private int previousTurnDamage;

    private bool hasPreviousTurnDamage;

    public PrecisionDamageMission(int threshold)
    {
        this.threshold = threshold;
    }

    public void Init(NewDrugMissionContext context)
    {
        IsSuccess = false;
        IsFailed = false;

        currentTrunDamage = 0;
        previousTurnDamage = 0;
        hasPreviousTurnDamage = false;
    }

    public void OnGameEvent(NewDrugGameEvent gameEvent)
    {
        if (IsSuccess || IsFailed) return;

        switch (gameEvent.Type)
        {
            case NewDrugGameEventType.TurnStarted:
                currentTrunDamage = 0;
                break;
            case NewDrugGameEventType.TreeDamaged:
                currentTrunDamage += gameEvent.DamageAmount;
                break;
            case NewDrugGameEventType.TurnEnded:
                CheckDamageDifference();
                break;
        }
    }

    private void CheckDamageDifference()
    {
        if (!hasPreviousTurnDamage)
        {
            previousTurnDamage = currentTrunDamage;
            hasPreviousTurnDamage = true;
            return;
        }

        int diff = Mathf.Abs(previousTurnDamage - currentTrunDamage);

        if (diff <= threshold)
        {
            IsSuccess = true;
        }
        else IsFailed = true;

        //previousTurnDamage = currentTrunDamage;
    }
}
