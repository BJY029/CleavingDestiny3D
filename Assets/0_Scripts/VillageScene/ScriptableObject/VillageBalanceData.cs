using UnityEngine;

[CreateAssetMenu(fileName = "VillageBalanceData", menuName = "Scriptable Objects/VillageBalanceData")]
public class VillageBalanceData : ScriptableObject
{
    [Header("General")]
    [SerializeField]
    private int maxLevel = 5;
    public int MaxLevel => maxLevel;

    [Header("Upgrade Cost")]
    [SerializeField]
    private int upgradeCostBase = 100;
    public int UpgradeCostBase => upgradeCostBase;

    // Cost(L) = Base * Multiplier^(L)
    [SerializeField]
    private float upgradeCostMultiplier = 2f;
    public float UpgradeCostMultiplier => upgradeCostMultiplier;

    [Header("Mine (Gold Income)")]
    // Gold(L) = Base * (L^2 + L)
    [SerializeField]
    private int goldIncomeBase = 50;
    public int GoldIncomeBase => goldIncomeBase;

    [Header("Farm (Energy Income & Max Energy)")]
    [SerializeField]
    private float energyIncomeBase = 5;
    public float EnergyIncomeBase => energyIncomeBase;

    [SerializeField]
    private float energyIncomeMultiplier = 3.2f;
    public float EnergyIncomeMultiplier => energyIncomeMultiplier;

    [SerializeField]
    private float energyMaxBase = 10f;
    public float EnergyMaxBase => energyMaxBase;

    [SerializeField]
    private float energyMaxMultiplier = 5f;
    public float EnergyMaxMultiplier => energyMaxMultiplier;


    [Header("Forge (Axe Damage)")]

    [SerializeField]
    private float axeDamageBase = 1000f;
    public float AxeDamageBase => axeDamageBase;

    [SerializeField]
    private float axeDamageLevelMultiplier = 100f;
    public float AxeDamageLevelMultiplier => axeDamageLevelMultiplier;


    [SerializeField]
    private float axeDamageMinMultiplier = 0.5f;
    public float AxeDamageMinMultiplier => axeDamageMinMultiplier;

    [SerializeField]
    private float axeDamageMaxMultiplier = 1.5f;
    public float AxeDamageMaxMultiplier => axeDamageMaxMultiplier;

    [Header("Barrier (Armor)")]
    // Armor(L) = Base * Multiplier^(L-1)
    [SerializeField]
    private float barrierArmorBase = 100f;
    public float BarrierArmorBase => barrierArmorBase;

    [SerializeField]
    private float barrierArmorMultiplier = 2f;
    public float BarrierArmorMultiplier => barrierArmorMultiplier;


    [Header("Shop")]
    [SerializeField]
    private int shopItemCount = 5;
    public int ShopItemCount => shopItemCount;

    [SerializeField]
    private float shopRareItemChanceBase = 0.05f; // 5% at level 1
    public float ShopRareItemChanceBase => shopRareItemChanceBase;

    [SerializeField]
    private float shopRareItemChanceMultiplier = 0.05f; // +5% per level
    public float ShopRareItemChanceMultiplier => shopRareItemChanceMultiplier;

    [SerializeField]
    private int shopReloadCost = 50;

    [SerializeField]
    private int shopReloadCostIncrement = 20;
    public int GetShopReloadCost(int reloadCount)
    {
        return shopReloadCost + (reloadCount * shopReloadCostIncrement);
    }
}
