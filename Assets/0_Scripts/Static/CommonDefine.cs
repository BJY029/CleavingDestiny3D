using System.Collections.Generic;
using UnityEngine;

public class CommonDefine
{
    //Scene name
    public const string LOBBYSCENE = "LobbyScene";
    public const string GAMESCENE = "GameScene";

    //Layer name
    public const string TREELAYER = "Tree";

    //아이템 선택 제안 수
    public const int itemOfferCnt = 3;

    //Room Property 기본 값
    public const float defaultTreeHP = 300000f;
    public const float defaultTreeAtkPow = 1500f;
    public const int defaultStartDay = 1;
    public const int defaultTurn = 0;
    public const int defaultTurnIndex = 1;
    public const GamePhaseValue defaultPhaseValue = GamePhaseValue.DAY;
    public const int defaultMaxWave = 3;
    public const int defaultWave = 0;
    //Room Property 일부인 Item/inventory 관련 기본 값
	public const int defaultInventoryCapacity = 8;
	public const int defaultUID = 1;

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

    //아이템 등급 기반 기본 등장 확률(각자 플레이어 프로퍼티에 저장)
    public const float defaultCommonItemWeight = 50f;
    public const float defaultHeroItemWeight = 30f;
    public const float defaultRareItemWeight = 20f;
    public const float defaultLegendaryItemWeight = 10f;
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
    public const string RoomSeed = "RoomSeed";
    public const string TurnOrder = "TurnInfo";
    public const string CurrentTurn = "CurrentTurn";
    public const string CurrentTurnActor = "CurrentTurnActor";
    public const string TurnIndex = "TrunIndex";
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

    public const string NEXT_UID = "NEXT_UID";
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
    Common, Hero, Rare, Legendary
}