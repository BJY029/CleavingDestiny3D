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
    [Tooltip("Legacy field. Farm base income comes from PlayerSetting.energyIncomePerDay.")]
    private float energyIncomeBase = 5;
    public float EnergyIncomeBase => energyIncomeBase;

    [SerializeField]
    private float energyIncomeMultiplier = 2f;
    public float EnergyIncomeMultiplier => energyIncomeMultiplier;

    [SerializeField]
    [Tooltip("Legacy field. Farm base capacity comes from PlayerSetting.maxEnergy.")]
    private float energyMaxBase = 10f;
    public float EnergyMaxBase => energyMaxBase;

    [SerializeField]
    private float energyMaxMultiplier = 2f;
    public float EnergyMaxMultiplier => energyMaxMultiplier;


    public int GetFarmMaxEnergy(PlayerSetting playerSetting, int level)
    {
        return playerSetting.maxEnergy + GetFarmEnergyBonus(level, energyMaxMultiplier);
    }

    public int GetFarmEnergyIncome(PlayerSetting playerSetting, int level)
    {
        return playerSetting.energyIncomePerDay + GetFarmEnergyBonus(level, energyIncomeMultiplier);
    }

    private int GetFarmEnergyBonus(int level, float multiplier)
    {
        int L = Mathf.Clamp(level, 0, Mathf.Max(0, maxLevel - 1));
        return Mathf.RoundToInt(multiplier * L * (L + 3f) / 4f);
    }

    [Header("Forge (Axe Damage)")]

    [SerializeField]
    [Tooltip("Legacy field. Forge base damage comes from PlayerSetting.minAtkPow / maxAtkPow.")]
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

    public (int min, int max) GetAxeRangeDamage(PlayerSetting playerSetting, int level)
    {
        int upgrades = Mathf.Clamp(level, 0, Mathf.Max(0, maxLevel - 1));
        return (
            playerSetting.minAtkPow - Mathf.RoundToInt(upgrades * axeDamageLevelMultiplier * axeDamageMinMultiplier),
            playerSetting.maxAtkPow + Mathf.RoundToInt(upgrades * axeDamageLevelMultiplier * axeDamageMaxMultiplier)
        );
    }

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
    private float shopRareChanceBase = 0.05f; // 기준 확률에 표시 레벨별 증가분을 더한다.
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

    // level은 내부 저장 기준(0부터)이며 화면에 표시되는 레벨은 level + 1이다.
    public (float common, float hero, float rare, float legendary) GetShopRarityChances(int level)
    {
        int displayLevel = Mathf.Clamp(level, 0, maxLevel - 1) + 1;
        float rare = Mathf.Clamp01(shopRareChanceBase + displayLevel * shopRareChanceMultiplier);
        float hero = displayLevel >= shopHeroMinLevel
            ? Mathf.Clamp01(shopHeroChanceBase + (displayLevel - shopHeroMinLevel) * shopHeroChanceMultiplier)
            : 0f;
        float legendary = displayLevel >= shopLegendaryMinLevel
            ? Mathf.Clamp01(shopLegendaryChanceBase + (displayLevel - shopLegendaryMinLevel) * shopLegendaryChanceMultiplier)
            : 0f;
        float total = rare + hero + legendary;
        if (total > 1f) return (0f, hero / total, rare / total, legendary / total);
        return (1f - total, hero, rare, legendary);
    }

    [SerializeField]
    private int shopReloadCost = 50;

    [SerializeField]
    private int shopReloadCostIncrement = 20;
    public int GetShopReloadCost(int reloadCount)
    {
        return shopReloadCost + (reloadCount * shopReloadCostIncrement);
    }

    [Header("Shop Item Prices")]
    [SerializeField] private int priceCommon = 100;
    [SerializeField] private int priceHero = 300;
    [SerializeField] private int priceRare = 600;
    [SerializeField] private int priceLegendary = 1200;

    public int GetItemPrice(ItemClass itemClass) => itemClass switch
    {
        ItemClass.Common => priceCommon,
        ItemClass.Hero => priceHero,
        ItemClass.Rare => priceRare,
        ItemClass.Legendary => priceLegendary,
        _ => priceCommon
    };
}
