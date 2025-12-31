using UnityEngine;

[CreateAssetMenu(fileName = "VillageBalanceData", menuName = "Scriptable Objects/VillageBalanceData")]
public class VillageBalanceData : ScriptableObject
{
    [SerializeField]
    private float energyIncomeBase;
    public float EnergyIncomeBase => energyIncomeBase;

    [SerializeField]
    private float energyIncomeMultiplier;
    public float EnergyIncomeMultiplier => energyIncomeMultiplier;

    [SerializeField]
    private float enemyMaxBase;
    public float EnemyMaxBase => enemyMaxBase;

    [SerializeField]
    private float enemyMaxMultiplier;
    public float EnemyMaxMultiplier => enemyMaxMultiplier;


}
