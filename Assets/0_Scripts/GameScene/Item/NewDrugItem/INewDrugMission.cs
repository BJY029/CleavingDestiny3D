public interface INewDrugMission
{
    string MissionName { get; }
    string MissionDesc { get; }

    bool IsSuccess { get; }
    bool IsFailed { get; }

    void Init(NewDrugMissionContext context);
    void OnGameEvent(NewDrugGameEvent gameEvent);
}
