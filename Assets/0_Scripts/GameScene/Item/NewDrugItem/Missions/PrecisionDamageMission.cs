using UnityEngine;

public class PrecisionDamageMission : INewDrugMission
{
    public string MissionName => "정밀 타격 실험";

    public string MissionDesc => $"연속된 2턴 동안 나무에게 준 데미지 차이를 {threshold} 이하로 맞추세요.";

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
            return;
        }

        previousTurnDamage = currentTrunDamage;
    }
}
