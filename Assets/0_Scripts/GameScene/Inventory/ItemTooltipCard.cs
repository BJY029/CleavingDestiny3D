using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ItemTooltipCard : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;
    [SerializeField] private Image accent;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI rarity;
    [SerializeField] private TextMeshProUGUI cost;

    [SerializeField, Min(0.01f)] private float fadeDuration = 0.14f;

    public void SetItem(ItemSO item)
    {
        Color color = GetRarityColor(item.itemClass);
        accent.color = color;
        rarity.color = color;
        rarity.text = LocalizationManager.Instance.GetText(CSV_Type.UI, "UI_ItemTooltip_" + item.itemClass);
        cost.text = LocalizationManager.Instance.GetFormatText(CSV_Type.UI, "UI_ItemTooltip_Energy", item.itemCost);
        icon.sprite = item.Icon;
        icon.enabled = icon.sprite != null;
    }

    private static Color GetRarityColor(ItemClass itemClass)
    {
        if (ItemColorManager.instance != null && ItemColorManager.instance.Colors != null)
        {
            var setting = ItemColorManager.instance.Colors.Find(entry => entry != null && entry.itemClass == itemClass);
            if (setting != null) return setting.BriteColor;
        }
        return new Color(0.87f, 0.76f, 0.51f);
    }

    private void OnEnable()
    {
        group.alpha = 0f;
    }

    private void Update()
    {
        if (group.alpha < 1f)
            group.alpha = Mathf.MoveTowards(group.alpha, 1f, Time.unscaledDeltaTime / fadeDuration);
    }
}
