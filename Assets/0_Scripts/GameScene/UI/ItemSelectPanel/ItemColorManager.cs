using System.Collections.Generic;
using UnityEngine;

public class ItemColorManager : MonoBehaviour
{
    public static ItemColorManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    //아이템 등급에 할당할 색상을 관리하는 매니져
    public List<ItemColorSetting> Colors;
    //ItemColorSetting 이라는 스크립터블 오브젝트로, 각 희귀도에 따른 아이템 색상 값을 관리한다.
    private Dictionary<ItemClass, ItemColorSetting> ItemColors = new Dictionary<ItemClass, ItemColorSetting>();

    void Start()
    {
        foreach (var C in Colors)
        {
            ItemColors.Add(C.itemClass, C);
        }
    }

    public Color GetNormalColor(ItemClass rarity)
    {
        return ItemColors[rarity].BasicColor;
    }

    public Color GetDarkerColor(ItemClass rarity)
    {
        return ItemColors[rarity].DarkColor;
    }

    public Color GetBriteColor(ItemClass rarity)
    {
        return ItemColors[rarity].BriteColor;
    }
}
