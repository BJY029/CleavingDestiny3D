using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSetting", menuName = "Scriptable Objects/PlayerSetting")]
public class PlayerSetting : ScriptableObject
{
    [Header("Village Stats")]
    public float villageHP = 5000f;
    public float villageBarrier = 0f;
    public float barrierConversionRate = 0.3f;
    public int initialGold = 100;
    public float initialTotalDamage = 0f;
    public int initialDayGoldIncome = 50;
    public float initialBarrierArmor = 0f;


    [Header("Combat Stats")]
    public int maxAtkPow = 1100;
    public int minAtkPow = 900;

    [Header("Energy Stats")]
    public int initialEnergy = 5;
    public int maxEnergy = 5;
    public int energyIncomePerDay = 5;
    public int carryOverEnergy = 0;
    public float regenEnergyPerDay = 10f;

    [Header("Item Weight Defaults")]
    public float commonWeight = 50f;
    public float heroWeight = 30f;
    public float rareWeight = 20f;
    public float legendaryWeight = 10f;

    [Header("Initial Upgrades")]
    public int[] initialVillageUpgrades = new int[5] { 0, 0, 0, 0, 0 };

    [Header("Inventory & Stats")]
    public int inventoryCapacity = 8;
    public float dayTimeDamage = 0; // 기존 roomSetting에 있던 것 이동
}
