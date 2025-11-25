using UnityEngine;

public class CommonDefine
{
    //Scene name
    public const string LOBBYSCENE = "LobbyScene";
    public const string GAMESCENE = "GameScene";

    //Layer name
    public const string TREELAYER = "Tree";

    //Room Property 기본 값
    public const float defaultTreeHP = 10000f;
    public const float defaultTreeAtkPow = 25f;
    public const int defaultStartDay = 1;
    public const int defaultTurn = 0;
    public const GamePhaseValue defaultPhaseValue = GamePhaseValue.DAY;
    public const int defaultMaxWave = 3;
    public const int defaultWave = 0;

    //Player Property 기본 값
    public const float defaultVillageHP = 1000f;
    public const float defaultVillageBarrier = 0f;
    public const int defaultVillageGold = 100;
    public const float defaultPlayerAtkPow = 20f;
    public const float defaultPlayerEnergy = 50f;
    public const float defaultPlayerMaxEnergy = 50f;
    public const float defaultPlayerRegenEnergyPerDay = 10f;
    public const float defaultDayTimeDamage = 0f;

    //Village 업그레이드 레벨(광산/집/대장간/상점/농장)
    public static readonly int[] defaultVillageUpgrades = new int[5] { 0, 0, 0, 0, 0 };

    // Village 업그레이드 효과 값
    public const int villageIncomePerLevel = 50; // 광산 레벨당 골드 수입 증가량
    public const int villageMaxEnergyPerLevel = 20; // 집 레벨당 최대 에너지 증가량
    public const float villageAtkPowerPerLevel = 5f; // 대장간 레벨당 공격력 보너스 증가량
    public const int villageEnergyRegenPerDay = 20; // 농장 레벨당 하루 에너지 회복량

    // Vilage 업그레이드 가격과 업그레이드 배율
    public static readonly int[] villageUpgradeBaseCost = new int[5] { 100, 150, 200, 250, 150 };
    public static readonly float villageUpgradeCostMultiplier = 2f;
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
    //public const string Weather = "Weather";
}

//플레이어 CustomProperty Key
public static class PlayerPropKeys
{
    //public const string Name = "Name";
    public const string VillageHP = "VillageHP";
    public const string VillageBarrier = "VillageBarrier";
    public const string VillageUpgrades = "VillageUpgrades";
    public const string Gold = "Gold";
    public const string AtkPow = "AtkPow";
    public const string Energy = "Energy";
    public const string MaxEnergy = "MaxEnergy";
    public const string DayTimeDamage = "DayTimeDamage";
    public const string MyTurn = "MyTurn";
}

//VillageUpgrades Property에 저장될 리스트 객체의 각 인덱스 정의
//ex) 리스트 3번째 인덱스에 저장된 값이 5라면, Shop의 업그레이드 레벨이 5임을 뜻함
public enum VillageUpgradeIndex
{
    Mine = 0,
    House = 1,
    Forge = 2,
    Shop = 3,
    Farm = 4,
}

public enum GamePhaseValue
{
    DAY, NIGHT, END
}