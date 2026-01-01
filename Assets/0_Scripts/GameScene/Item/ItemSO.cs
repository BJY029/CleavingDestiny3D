using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemId;       //고유 ID
    public string displayName_ID;  //UI에서 보여지는 이름 CSV ID
    public string itemDesc_ID;     //아이템 설명 CSV ID
    public Sprite icon;         //UI 스프라이트
    public ItemType type;       //아이템 Type
    public int itemCost;        //필요한 기력량
    public ItemClass itemClass; //아이템 등급
}
