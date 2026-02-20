using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private CSV_Type tableType;
    public CSV_Type TableType
    {
        get => tableType;
        set
        {
            if (tableType != value)
            {
                tableType = value;
                UpdateText();
            }
        }
    }

    [SerializeField] private string textID;
    public string TextID
    {
        get => textID;
        set
        {
            if (textID != value)
            {
                textID = value;
                UpdateText();
            }
        }
    }

    private void Reset()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        UpdateText();
        // 활성화될 때만 이벤트 구독
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            // 비활성화될 때 구독 해제
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    private void UpdateText()
    {
        if (textComponent != null)
        {
            textComponent.SetText(LocalizationManager.Instance.GetText(tableType, textID));
        }
    }
}