using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIconView : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI RemainTurnText;
    public TextMeshProUGUI StackText;

    public void Bind(Sprite sprite, int remainingTurns, int stackCount)
    {
        icon.sprite = sprite;
        RemainTurnText.text = remainingTurns > 0 ? remainingTurns.ToString() : "";
        StackText.text = stackCount > 1 ? stackCount.ToString() : "";
    }

    public void BindMasked(Sprite maskSprite, int remainingTurns, int stackCount)
    {
        icon.sprite = maskSprite;
        RemainTurnText.text = remainingTurns > 0 ? remainingTurns.ToString() : "";
        StackText.text = stackCount > 1 ? stackCount.ToString() : "";
    }
}
