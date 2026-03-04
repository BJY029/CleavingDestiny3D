using UnityEngine;

[CreateAssetMenu(fileName = "VillageBalanceData", menuName = "Scriptable Objects/VillageBalanceData")]
public class VillageBalanceData : ScriptableObject
{
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


}
