public class NoItemOnlyBasicAttackMission : INewDrugMission
{
    public string MissionName => LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_NAME_BASICATTACK");
    public string MissionDesc => LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_DESC_BASICATTACK");

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
