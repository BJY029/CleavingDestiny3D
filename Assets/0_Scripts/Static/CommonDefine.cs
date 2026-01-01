using UnityEngine;

public class CommonDefine
{
    //Scene name
    public const string LOBBYSCENE = "LobbyScene";
    public const string GAMESCENE = "GameScene";

    //Layer name
    public const string TREELAYER = "Tree";

    //Room Property 기본 값
    public const float defaultTreeHP = 300000f;
    public const float defaultTreeAtkPow = 1500f;
    public const int defaultStartDay = 1;
    public const int defaultTurn = 0;
    public const GamePhaseValue defaultPhaseValue = GamePhaseValue.DAY;
    public const int defaultMaxWave = 3;
    public const int defaultWave = 0;

    //Player Property 기본 값
    public const float defaultVillageHP = 5000f;
    public const float defaultVillageBarrier = 0f;
    public const float defaultBarrierConversionRate = 0.3f;
    public const int defaultGold = 100;
    public const int defaultPlayerMaxAtkPow = 1100;
    public const int defaultPlayerMinAtkPow = 900;
    public const int defaultPlayerEnergy = 5;
    public const int defaultPlayerMaxEnergy = 5;
    public const int defaultCarryOverEnergy = 0;
    public const float defaultPlayerRegenEnergyPerDay = 10f;
    public const float defaultDayTimeDamage = 0f;
    public const float defaultTotalDamage = 0f;

    //Village 업그레이드 레벨(광산/대장간/상점/농장/방벽)
    public static readonly int[] defaultVillageUpgrades = new int[5] { 0, 0, 0, 0, 0 };
}

public enum PLAYER
{
    P1, P2, P3, P4, NONE,
}

//플레이어 정보 저장 객체
public class RuntimePlayer
{
    public int actorNumber;
    public string playerName;
    public int turnIdx;
    public bool isMyTurn;
}

public class UI_CSV
{
    public const string UI_PVP = "UI_PVP";
    public const string UI_PVE = "UI_PVE";
    public const string UI_EXIT = "UI_EXIT";
    public const string UI_BranchDesc1 = "UI_BranchDesc_1";
    public const string UI_BranchDesc2 = "UI_BranchDesc_2";
    public const string UI_PlayerHit = "UI_PlayerHIT";
    public const string UI_PlayerNHit = "UI_PlayerNotHIT";
}

//서버 CustomProperty Key
public static class RoomPropKeys
{
    public const string TurnOrder = "TurnInfo";
    public const string CurrentTurn = "CurrentTurn";
    public const string NextTurn = "NextTurn";
    public const string CurrentDay = "CurrentDay";
    public const string MaxWaveCnt = "MaxWaveCnt";
    public const string CurrentWave = "CurrentWave";
    public const string GamePhase = "GamePhase"; // "DAY" / "NIGHT" / "END"
    public const string TreeHP = "TreeHP";
    public const string TreeAtkPow = "TreeAtkPow";
    public const string IsVillageUpgradePhase = "IsVillageUpgradePhase"; // bool
    public const string VillageUpgradeStartEndTime = "VillageUpgradeStartEndTime"; //float
    //public const string Weather = "Weather";
}

//플레이어 CustomProperty Key
public static class PlayerPropKeys
{
    //public const string Name = "Name";
    public const string VillageHP = "VillageHP";
    public const string VillageBarrier = "VillageBarrier";
    public const string BarrierConversionRate = "BarrierConversionRate";
    public const string VillageUpgrades = "VillageUpgrades";
    public const string Gold = "Gold";
    public const string MaxAtkPow = "MaxAtkPow";
    public const string MinAtkPow = "MinAtkPow";
    public const string TotalDamage = "TotalDamage";
    public const string Energy = "Energy";
    public const string MaxEnergy = "MaxEnergy";
    public const string CarryOverEnergy = "CarryOverEnergy";
    public const string DayTimeDamage = "DayTimeDamage";
    public const string MyTurn = "MyTurn";
}

// 건물의 종류
public enum VillageType
{
    House = -1, // House는 턴 종료용 -> 업그레이드 대상 아님
    Mine,
    Forge,
    Shop,
    Farm,
    Barrier,
}

public enum GamePhaseValue
{
    DAY, NIGHT, END
}

public enum ItemType
{
    Damage, Defence, Heal, Gimmick
}

public enum ItemClass
{
    Common, Hero, Rere, Legend
}