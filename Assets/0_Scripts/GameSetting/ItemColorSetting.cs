using UnityEngine;

[CreateAssetMenu(fileName = "ItemColorSetting", menuName = "Scriptable Objects/ItemColorSetting")]
public class ItemColorSetting : ScriptableObject
{
    public ItemClass itemClass;
    public Color BasicColor;
    public Color DarkColor;
    public Color BriteColor;
}
