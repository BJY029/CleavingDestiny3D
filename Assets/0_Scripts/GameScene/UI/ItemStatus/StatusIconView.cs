using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIconView : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI RemainTurnText;
    public TextMeshProUGUI StackText;
    public Image ActiveTimingImg;
    public Sprite NightSprite;

    public void Bind(Sprite sprite, int remainingTurns, int stackCount, TriggerMask mask)
    {
        icon.sprite = sprite;

        if (mask == TriggerMask.OnTreeDamage || mask == TriggerMask.OnVillageStart)
        {
            ActiveTimingImg.gameObject.SetActive(true);
            ActiveTimingImg.sprite = NightSprite;
            RemainTurnText.text = "";
        }
        else
        {
            RemainTurnText.text = remainingTurns > 0 ? remainingTurns.ToString() : "";
            ActiveTimingImg.sprite = null;
            ActiveTimingImg.gameObject.SetActive(false);
        }
        StackText.text = stackCount > 0 ? stackCount.ToString() : "";
    }

    public void BindMasked(Sprite maskSprite, int remainingTurns, int stackCount, TriggerMask mask)
    {
        icon.sprite = maskSprite;
        if (mask == TriggerMask.OnTreeDamage || mask == TriggerMask.OnVillageStart)
        {
            ActiveTimingImg.gameObject.SetActive(true);
            ActiveTimingImg.sprite = NightSprite;
            RemainTurnText.text = "";
        }
        else
        {
            RemainTurnText.text = remainingTurns > 0 ? remainingTurns.ToString() : "";
            ActiveTimingImg.sprite = null;
            ActiveTimingImg.gameObject.SetActive(false);
        }
        StackText.text = stackCount > 0 ? stackCount.ToString() : "";
    }
}
