using System.Collections.Generic;
using UnityEngine;

public class CommonDefine
{
    //Scene name
    public const string LOBBYSCENE = "LobbyScene";
    public const string GAMESCENE = "GameScene";
    public const string VILLAGESCENE = "VillageScene";

    //Layer name
    public const string TREELAYER = "Tree";

    // 상수들 제거 (ScriptableObject로 이동됨)
}

public enum PLAYER
{
    P1, P2, P3, P4, NONE,
}

public enum ERROR
{
    FULL_INV,
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
    public const string UI_PlayerSpace = "UI_PlayerSpace";
    public const string UI_Load_Finding = "UI_Load_Finding";
    public const string UI_Load_Waiting = "UI_Load_Waiting";
    public const string UI_Load_MatchSuccess = "UI_Load_MatchSuccess";
    public const string UI_Load_Loading = "UI_Load_Loading";
    public const string UI_Load_ReturningToLobby = "UI_Load_ReturningToLobby";
    public const string UI_ItemNotify_Title = "UI_ItemNotify_Title";
    public const string UI_ItemSacrifice_Title = "UI_ItemSacrifice_Title";
    public const string UI_ItemSacrifice_TableDesc = "UI_ItemSacrifice_TableDesc";
    public const string UI_Warning_Energy = "UI_Warning_Energy";
    public const string UI_Warning_FullInv = "UI_Warning_FullInv";
    public const string UI_Warning_NotEnoughItem = "UI_Warning_NotEnoughItem";
    public const string UI_Warning_NotAvaiable = "UI_Warning_NotAvaiable";
    public const string UI_Item_LockPick_Warning = "UI_Item_LockPick_Warning";
    public const string UI_Item_LockPick_Has = "UI_Item_LockPick_Has";
    public const string UI_Exit_Lobby = "UI_Exit_Lobby";
    public const string UI_Exit_Game = "UI_Exit_Game";
}

//서버 CustomProperty Key
public static class RoomPropKeys
{
    public const string AllReady = "AllReady";
    public const string RoomSeed = "RoomSeed";
    public const string TurnTime = "TurnTime";
    public const string TurnOrder = "TurnInfo";
    public const string CurrentTurn = "CurrentTurn";
    public const string CurrentTurnActor = "CurrentTurnActor";
    public const string TurnIndex = "TurnIndex";
    public const string NextTurn = "NextTurn";
    public const string CurrentDay = "CurrentDay";
    public const string MaxWaveCnt = "MaxWaveCnt";
    public const string CurrentWave = "CurrentWave";
    public const string GamePhase = "GamePhase"; // "DAY" / "NIGHT" / "END"
    public const string TreeHP = "TreeHP";
    public const string TreeMaxHP = "TreeMaxHP";
    public const string TreeAtkPow = "TreeAtkPow";
    public const string IsVillageUpgradePhase = "IsVillageUpgradePhase"; // bool
    public const string VillageUpgradeStartEndTime = "VillageUpgradeStartEndTime"; //float
    public const string PlayerTurnStartEndTime = "PlayerTurnStartEndTime";
    public const string VillagePhaseTime = "VillagePhaseTime"; //float
    public const string IsTreeBulkDamage = "IsTreeBulkDamage";
    public const string MatchLoserActor = "MatchLoserActor"; //int
    public const string MatchResultReason = "MatchResultReaon"; //string
    public const string MatchResolveTurnIndex = "MatchResolveTurnIndex"; //int
    //public const string Weather = "Weather";
}

//플레이어 CustomProperty Key
public static class PlayerPropKeys
{
    //public const string Name = "Name";
    public const string IsReady = "IsReady";
    public const string VDamageProcessCompleted = "VDamageProcessCompleted";
    public const string PDamageProcessCompleted = "PDamageProcessCompleted";
    public const string TreeAtkMulti = "TreeAtkMulti";
    public const string VillageHP = "VillageHP";
    public const string MaxVillageHP = "MaxVillageHP";
    public const string VillageBarrier = "VillageBarrier";
    public const string BarrierConversionRate = "BarrierConversionRate";
    public const string BarrierArmor = "BarrierArmor";
    public const string VillageUpgrades = "VillageUpgrades";
    public const string Gold = "Gold";
    public const string DayGoldIncome = "DayGoldIncome";
    public const string MaxAtkPow = "MaxAtkPow";
    public const string MinAtkPow = "MinAtkPow";
    public const string TotalDamage = "TotalDamage";
    //public const string DamageRatio = "DamageRatio";
    public const string Energy = "Energy";
    public const string MaxEnergy = "MaxEnergy";
    public const string CarryOverEnergy = "CarryOverEnergy";
    public const string EnergyIncome = "EnergyIncome";
    public const string DayTimeDamage = "DayTimeDamage";
    public const string MyTurn = "MyTurn";

    // 플레이어 준비 상태 체크용 키
    public const string PlayerVillageReady = "IsPlayerVillageReady";

    public const string Item_CommonWeight = "Item_CommonWeight";
    public const string Item_HeroWeight = "Item_HeroWeight";
    public const string Item_RareWeight = "Item_RareWeight";
    public const string Item_LegendaryWeight = "Item_LegendaryWeight";
}

public static class ItemPropKeys
{
    //해당 플레이어의 인벤토리 데이터가 들어있는 RoomProp 키
    //ex) actor = 2
    //  Key : INV_2
    //  Value : "12:potion|13:bomb|_|21:shield|_"
    public static string INV(int actor) => $"INV_{actor}";

    //해당 플레이어의 인벤 슬롯 용량
    //ex) actor = 3
    //  KEY : INV_CAP_3, VALUE : 8(인벤 크기 8)
    public static string INV_CAPACITY(int actor) => $"INV_CAP_{actor}";

    //해당 플레이어의 턴 시작 시 제공될 아이템 선택지(3개)
    //ex) actor = 1
    // KEY : OFFER_1
    // VALUE : "potion|bomb|shield"
    public static string OFFER(int actor) => $"OFFER_{actor}";

    public static string LOCKPICK(int actor) => $"LOCKPICK_{actor}";

    public static string LOCKCNT(int actor) => $"LOCK_CNT_{actor}";

    public static string COMMON_RATE(int actor) => $"COMMON_RATE_{actor}";
    public static string HERO_RATE(int actor) => $"HERO_RATE_{actor}";
    public static string RARE_RATE(int actor) => $"RARE_RATE_{actor}";
    public static string LEGENDARY_RATE(int actor) => $"LEGENDARY_RATE_{actor}";

    public const string NEXT_UID = "NEXT_UID";
}

// 건물의 종류
public enum VillageType
{
    Mine,
    Forge,
    Shop,
    Farm,
    Barrier,
    Compass,
}

public enum GamePhaseValue
{
    DAY,
    NIGHT_VILLAGE,
    NIGHT_TREEATK,
    END,
}

public enum MatchResultReason
{
    None,
    TreeDestroyed,
    VillageDestroyed,
    PlayerLeft,
    Draw,
}

public enum MatchResultType
{
    None,
    Win,
    Loss,
    Draw
}

public enum LocationCommand
{
    MY_INV,
    MY_INV_ENTRY,
    MY_HIT,
    OPP_INV,
    OPP_INV_ENTRY,
    OPP_HIT,
}