using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StatusIconView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    private ItemStatusInfo statusInfo;
    private bool masked;
    private RectTransform tooltipRoot;
    private TextMeshProUGUI tooltipText;
    private readonly Vector3[] tooltipCorners = new Vector3[4];

    public void SetStatusInfo(ItemStatusInfo info, bool isMasked)
    {
        statusInfo = info;
        masked = isMasked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (masked) return;
        if (tooltipRoot == null)
        {
            var panel = new GameObject("StatusTooltip", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Canvas));
            panel.layer = gameObject.layer;
            tooltipRoot = panel.GetComponent<RectTransform>();
            tooltipRoot.SetParent(transform, false);
            var background = panel.GetComponent<Image>();
            background.color = new Color(0.08f, 0.07f, 0.06f, 0.97f);
            background.raycastTarget = false;
            var tooltipCanvas = panel.GetComponent<Canvas>();
            tooltipCanvas.overrideSorting = true;
            tooltipCanvas.sortingOrder = GetComponentInParent<Canvas>().sortingOrder + 10;
            tooltipText = Instantiate(StackText, tooltipRoot);
            tooltipText.name = "Description";
            tooltipText.raycastTarget = false;
            tooltipText.enableAutoSizing = false;
            tooltipText.fontSize = 20;
            tooltipText.fontStyle = FontStyles.Normal;
            tooltipText.color = Color.white;
            tooltipText.alignment = TextAlignmentOptions.TopLeft;
            tooltipText.textWrappingMode = TextWrappingModes.Normal;
            tooltipText.rectTransform.anchorMin = Vector2.zero;
            tooltipText.rectTransform.anchorMax = Vector2.one;
            tooltipText.rectTransform.offsetMin = new Vector2(12, 12);
            tooltipText.rectTransform.offsetMax = new Vector2(-12, -12);
        }
        tooltipText.text = ItemStatusText.GetStatusDescription(statusInfo);
        tooltipRoot.sizeDelta = new Vector2(380f, tooltipText.GetPreferredValues(tooltipText.text, 356f, 0f).y + 24f);
        tooltipRoot.anchorMin = tooltipRoot.anchorMax = new Vector2(0, 1);
        tooltipRoot.pivot = Vector2.zero;
        tooltipRoot.anchoredPosition = new Vector2(0, 12);
        tooltipRoot.gameObject.SetActive(true);
        tooltipRoot.GetWorldCorners(tooltipCorners);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, tooltipCorners[2]);
        if (topRight.x > Screen.width) tooltipRoot.pivot = new Vector2(1, tooltipRoot.pivot.y);
        if (topRight.y > Screen.height)
        {
            tooltipRoot.anchorMin = tooltipRoot.anchorMax = Vector2.zero;
            tooltipRoot.pivot = new Vector2(tooltipRoot.pivot.x, 1);
            tooltipRoot.anchoredPosition = new Vector2(0, -12);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipRoot != null) tooltipRoot.gameObject.SetActive(false);
    }

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

public static class ItemStatusText
{
    public static string GetExpirationKey(StatusSpec spec)
    {
        if (spec.statusId == "GIM_SPY") return "UI_Status_VillageConsumed";
        if (spec.statusId == "GIM_CURSE") return "UI_Status_ConversionConsumed";
        if (spec.consumeOnTrigger && (spec.triggers & TriggerMask.OnBeforeAttack) != 0)
            return "UI_Status_AttackConsumed";
        return spec.durationType switch
        {
            DurationType.ThisTurn => "UI_Status_TurnEnd",
            DurationType.Turns => "UI_Status_TurnEndsRemaining",
            DurationType.UntilWaveEnd => (spec.triggers & TriggerMask.OnVillageStart) != 0
                ? "UI_Status_VillageConsumed" : "UI_Status_VillageEnd",
            _ => "UI_Status_UntilTriggered"
        };
    }

    public static string GetDurationText(StatusSpec spec, int remainingTurns)
    {
        return LocalizationManager.Instance.GetFormatText(CSV_Type.UI, GetExpirationKey(spec), Mathf.Max(1, remainingTurns));
    }

    public static string GetItemDescription(ItemSO item)
    {
        string text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);
        if (item.effects == null) return text;
        foreach (var effect in item.effects)
        {
            if (effect.effectType != ItemEffect.AddStatus || effect.statusSpce == null) continue;
            text += "\n\n<size=85%>" + GetDurationText(effect.statusSpce, effect.statusSpce.durationTurns) + "</size>";
        }
        return text;
    }

    public static string GetStatusDescription(ItemStatusInfo info)
    {
        var item = ItemDB.Instance.Get(info.itemId);
        if (item == null) return string.Empty;
        string text = "<b>" + LocalizationManager.Instance.GetText(CSV_Type.Item, item.displayName_ID) + "</b>\n" +
            LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);
        if (item.effects == null) return text;
        foreach (var effect in item.effects)
        {
            if (effect.effectType == ItemEffect.AddStatus && effect.statusSpce?.statusId == info.statusId)
                return text + "\n\n" + GetDurationText(effect.statusSpce, info.remainingTurns);
        }
        return text;
    }
}
