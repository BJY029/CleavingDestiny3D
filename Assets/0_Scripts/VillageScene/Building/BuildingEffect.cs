using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingEffect : MonoBehaviour
{
    [SerializeField] Sprite currentEffectSprite;
    [SerializeField] Sprite enabledCircleSprite;

    Sprite originalEffectSprite;
    Sprite originalCircleSprite;

    [SerializeField] Image effectImage;
    [SerializeField] Image circleImage;
    [SerializeField] GameObject disablePanel;

    [SerializeField] TextMeshProUGUI effectValueText;
    public TextMeshProUGUI EffectValueText => effectValueText;

    void Awake()
    {
        originalEffectSprite = effectImage.sprite;
        originalCircleSprite = circleImage.sprite;
    }

    public void SetEffectLineEnabled(bool enabled)
    {
        circleImage.sprite = enabled ? enabledCircleSprite : originalCircleSprite;
        disablePanel.SetActive(!enabled);
    }

    public void SetEffectActivated(bool active)
    {
        effectImage.sprite = active ? currentEffectSprite : originalEffectSprite;
    }
}
