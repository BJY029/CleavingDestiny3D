using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private CSV_Type tableType;
    [SerializeField] private string textID;

    private void Reset()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        UpdateText();
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
    }

    private void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    private void UpdateText()
    {
        if (textComponent != null)
        {
            textComponent.text = LocalizationManager.Instance.GetText(tableType, textID);
        }
    }
}