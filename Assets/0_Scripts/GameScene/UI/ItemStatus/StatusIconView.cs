using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIconView : MonoBehaviour
{
    [Header("UIs")]
    public Image icon;
    //public TextMeshProUGUI RemainTurnText;
    public TextMeshProUGUI StackText;
    public Image ActiveTimingImg;
    public Image ItemType;

    [Header("Sprites")]
    public Sprite DaySprite;
    public Sprite NightSprite;
    public Sprite DamageSprite;
    public Sprite DefenceSprite;
    public Sprite HealSprite;
    public Sprite GimmickSprite;

    public void Bind(Sprite sprite, int stackCount, TriggerMask mask, ItemType type)
    {
        icon.sprite = sprite;

        SetActiveTimingSprite(mask);
        SetItemTypeSprite(type);
        StackText.text = stackCount > 0 ? stackCount.ToString() : "";
    }

    public void BindMasked(Sprite maskSprite, int stackCount, TriggerMask mask, ItemType type)
    {
        icon.sprite = maskSprite;
        SetActiveTimingSprite(mask);
        SetItemTypeSprite(type);
        StackText.text = stackCount > 0 ? stackCount.ToString() : "";
    }

    private void SetActiveTimingSprite(TriggerMask mask)
    {
        switch (mask)
        {
            case TriggerMask.OnTreeDamage:
            case TriggerMask.OnVillageStart:
                ActiveTimingImg.sprite = NightSprite;
                break;
            default:
                ActiveTimingImg.sprite = DaySprite;
                break;
        }
    }

    private void SetItemTypeSprite(ItemType type)
    {
        switch (type)
        {
            case global::ItemType.Damage:
                ItemType.sprite = DamageSprite;
                break;
            case global::ItemType.Defence:
                ItemType.sprite = DefenceSprite;
                break;
            case global::ItemType.Heal:
                ItemType.sprite = HealSprite;
                break;
            case global::ItemType.Gimmick:
                ItemType.sprite = GimmickSprite;
                break;
        }
    }
}
