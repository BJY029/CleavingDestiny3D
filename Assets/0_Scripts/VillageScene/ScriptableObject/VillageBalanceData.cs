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
    private int shopItemCount = 3;
    public int ShopItemCount => shopItemCount;

    [Header("Shop Rarity - Rare")]
    [SerializeField]
    private float shopRareChanceBase = 0.05f; // 레벨 1 기본 확률
    public float ShopRareChanceBase => shopRareChanceBase;

    [SerializeField]
    private float shopRareChanceMultiplier = 0.05f; // 레벨당 증가량
    public float ShopRareChanceMultiplier => shopRareChanceMultiplier;

    [Header("Shop Rarity - Hero")]
    [SerializeField]
    private int shopHeroMinLevel = 3; // 등장 최소 레벨
    public int ShopHeroMinLevel => shopHeroMinLevel;

    [SerializeField]
    private float shopHeroChanceBase = 0.02f; // 최소 레벨 달성 시 기본 확률
    public float ShopHeroChanceBase => shopHeroChanceBase;

    [SerializeField]
    private float shopHeroChanceMultiplier = 0.03f; // 최소 레벨 이후 레벨당 증가량
    public float ShopHeroChanceMultiplier => shopHeroChanceMultiplier;

    [Header("Shop Rarity - Legendary")]
    [SerializeField]
    private int shopLegendaryMinLevel = 5; // 등장 최소 레벨
    public int ShopLegendaryMinLevel => shopLegendaryMinLevel;

    [SerializeField]
    private float shopLegendaryChanceBase = 0.01f; // 최소 레벨 달성 시 기본 확률
    public float ShopLegendaryChanceBase => shopLegendaryChanceBase;

    [SerializeField]
    private float shopLegendaryChanceMultiplier = 0.01f; // 최소 레벨 이후 레벨당 증가량
    public float ShopLegendaryChanceMultiplier => shopLegendaryChanceMultiplier;

    [SerializeField]
    private int shopReloadCost = 50;

    [SerializeField]
    private int shopReloadCostIncrement = 20;
    public int GetShopReloadCost(int reloadCount)
    {
        return shopReloadCost + (reloadCount * shopReloadCostIncrement);
    }
}
