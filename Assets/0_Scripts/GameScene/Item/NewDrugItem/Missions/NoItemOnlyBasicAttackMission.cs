public class NoItemOnlyBasicAttackMission : INewDrugMission
{
    public string MissionName => "순수 전투 실험";
    public string MissionDesc => "한 웨이브 동안 아이템을 사용하지 않고 평타만 사용하세요.";

    public bool IsSuccess { get; private set; }
    public bool IsFailed { get; private set; }

    private int checkedTurnCount;
    private bool usedBasicAttackThisTurn;
    private bool usedInvaildActionThisTurn;

    private int startTurnIndex;

    public void Init(NewDrugMissionContext context)
    {
        IsSuccess = false;
        IsFailed = false;

        checkedTurnCount = 0;
        usedBasicAttackThisTurn = false;
        usedInvaildActionThisTurn = false;

        startTurnIndex = context.StartTurnIndex;
    }

    public void OnGameEvent(NewDrugGameEvent gameEvent)
    {
        if (IsSuccess || IsFailed) return;

        switch (gameEvent.Type)
        {
            case NewDrugGameEventType.TurnStarted:
                usedBasicAttackThisTurn = false;
                usedInvaildActionThisTurn = false;
                break;
            case NewDrugGameEventType.BasicAttackUsed:
                usedBasicAttackThisTurn = true;
                break;
            case NewDrugGameEventType.ItemUsed:
            case NewDrugGameEventType.SkillUsed:
                usedInvaildActionThisTurn = true;
                break;
            case NewDrugGameEventType.TurnEnded:
                CheckTurnEnd();
                break;
        }
    }

    private void CheckTurnEnd()
    {
        if (usedInvaildActionThisTurn)
        {
            IsFailed = true;
            return;
        }

        if (!usedBasicAttackThisTurn)
        {
            IsFailed = true;
            return;
        }

        checkedTurnCount++;

        if (checkedTurnCount >= 3) IsSuccess = true;
    }
}
