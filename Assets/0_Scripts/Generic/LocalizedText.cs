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

    [Tooltip("텍스트 포맷에 주입할 Action 이름 목록 (비워둘 시 LocalizationManager의 기본 매핑 사용)")]
    [SerializeField] private string[] keyActionNames;

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
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
        KeyInteractManager.OnKeyBindingsChanged += UpdateText;
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            // 비활성화될 때 구독 해제
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
        KeyInteractManager.OnKeyBindingsChanged -= UpdateText;
    }

    public void UpdateText()
    {
        if (!Application.isPlaying) return;

        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            if (textComponent == null) return;
        }

        if (string.IsNullOrEmpty(textID)) return;

        if (LocalizationManager.Instance != null && LocalizationManager.Instance.IsLoaded)
        {
            if (keyActionNames != null && keyActionNames.Length > 0)
            {
                textComponent.SetText(LocalizationManager.Instance.GetFormatTextWithKeys(tableType, textID, keyActionNames));
            }
            else
            {
                textComponent.SetText(LocalizationManager.Instance.GetText(tableType, textID));
            }
        }
    }
}
