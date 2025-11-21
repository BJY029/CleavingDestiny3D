using UnityEngine;

public class CommonDefine
{
    //Scene name
    public const string LOBBYSCENE = "LobbyScene";
    public const string GAMESCENE = "GameScene";

	//Room Property 기본 값
	public const float defaultTreeHP = 10000f;
	public const float defaultTreeAtkPow = 25f;
	public const int defaultStartDay = 1;
	public const int defaultTurn = 0;
	public const GamePhaseValue defaultPhaseValue = GamePhaseValue.DAY;

	//Player Property 기본 값
	public const float defaultVillageHP = 1000f;
	public const float defaultVillageBarrier = 0f;
	public const int defaultVillageGold = 100;
	public const float defaultPlayerAtkPow = 20f;
	public const float defaultPlayerEnergy = 50f;
	public const float defaultPlayerMaxEnergy = 50f;
	public const float defaultDayTimeDamage = 0f;

	//Village 업그레이드 레벨(광산/집/대장간/상점/농장)
	public static readonly int[] defaultVillageUpgrades = new int[5] { 0, 0, 0, 0, 0 };
}

public enum PLAYER{
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

//서버 CustomProperty Key
public static class RoomPropKeys
{
    public const string CurrentTurn = "CurrentTurn";
    public const string NextTurn = "NextTurn";
    public const string CurrentDay = "CurrentDay";
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