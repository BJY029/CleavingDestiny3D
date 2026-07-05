using UnityEngine;

public class ReachDefenseInOneTurnMission : INewDrugMission
{
    public string MissionName => "방어 안정화 실험";

    public string MissionDesc => $"단 1턴 안에 마을 방어력을 {requiredDefenseAmount} 이상 증가시키세요.";

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

        currentTurnDefenseGain = 0;
    }

    public void OnGameEvent(NewDrugGameEvent gameEvent)
    {
        if (IsSuccess || IsFailed) return;

        switch (gameEvent.Type)
        {
            case NewDrugGameEventType.TurnStarted:
                currentTurnDefenseGain = 0;
                break;

            case NewDrugGameEventType.DefenseChanged:
                currentTurnDefenseGain += gameEvent.DefneseDelta;

                if (currentTurnDefenseGain >= requiredDefenseAmount)
                {
                    IsSuccess = true;
                }

                break;

            case NewDrugGameEventType.TurnEnded:
                currentTurnDefenseGain = 0;
                break;
        }
    }
}
