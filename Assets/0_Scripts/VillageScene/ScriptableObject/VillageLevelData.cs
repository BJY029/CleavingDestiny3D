using UnityEngine;

[CreateAssetMenu(fileName = "VillageLevelData", menuName = "Scriptable Objects/VillageLevelData")]
public class VillageLevelData : ScriptableObject
{
    // Village 업그레이드 데이터
    [SerializeField]
    private VillageType villageType;
    public VillageType VillageType => villageType;

    [SerializeField]
    private int max_level;
    public int MaxLevel => max_level;

    [SerializeField, Tooltip("각 레벨 업그레이드에 필요한 비용")]
    private int[] upgradeCosts;

    public bool TryGetUpgradeCost(int level, out int cost)
    {
        if (level < 0 || level >= max_level || level >= upgradeCosts.Length)
        {
            cost = -1;
            return false;
        }
        cost = upgradeCosts[level];
        return true;
    }

    [SerializeField, Tooltip("각 레벨 업그레이드 시 적용되는 효과 값")]
    private int[] effectValues;

    public bool TryGetEffectValue(int level, out int effectValue)
    {
        if (level < 0 || level > max_level)
        {
            effectValue = -1;
            return false;
        }
        else if (effectValues.Length < level && level <= max_level)
        {
            effectValue = level;
            return true;
        }

        effectValue = effectValues[level];
        return true;
    }

    [SerializeField, Tooltip("건물 설명 ID")]
    private string levelDescriptionID;
    public string LevelDescriptionID => levelDescriptionID;
}
