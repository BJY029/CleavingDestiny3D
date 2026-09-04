using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

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
#if UNITY_EDITOR
        UpdateText();
#endif
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        if (string.IsNullOrEmpty(textID)) return;

        EditorApplication.delayCall -= EditorUpdateText;
        EditorApplication.delayCall += EditorUpdateText;
    }

    private void EditorUpdateText()
    {
        if (this == null || textComponent == null) return;
        UpdateText();
    }
#endif

    private void OnEnable()
    {
        UpdateText();
        // 활성화될 때만 이벤트 구독
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        }
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            // 비활성화될 때 구독 해제
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }
    }

    public void UpdateText()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            if (textComponent == null) return;
        }

        if (string.IsNullOrEmpty(textID)) return;

        if (Application.isPlaying && LocalizationManager.Instance != null && LocalizationManager.Instance.IsLoaded)
        {
            textComponent.SetText(LocalizationManager.Instance.GetText(tableType, textID));
        }
#if UNITY_EDITOR
        else
        {
            string editorText = GetEditorPreviewText(tableType, textID);
            if (!string.IsNullOrEmpty(editorText))
            {
                textComponent.text = editorText;
                EditorUtility.SetDirty(textComponent);
            }
        }
#endif
    }

#if UNITY_EDITOR
    private static readonly Dictionary<CSV_Type, Dictionary<string, (string kr, string en)>> _editorCsvCache = new();
    private static readonly Dictionary<CSV_Type, System.DateTime> _editorCsvLastModified = new();

    private static string GetEditorPreviewText(CSV_Type type, string id)
    {
        if (string.IsNullOrEmpty(id)) return string.Empty;

        EnsureEditorCacheLoaded(type);

        if (_editorCsvCache.TryGetValue(type, out var table) && table.TryGetValue(id, out var values))
        {
            string lang = PlayerPrefs.GetString("Language", "KR");
            return lang == "EN" ? values.en : values.kr;
        }

        return id;
    }

    private static void EnsureEditorCacheLoaded(CSV_Type type)
    {
        string filePath = type == CSV_Type.Preload 
            ? "Assets/Resources/Preload_CSV.csv" 
            : $"Assets/Localization/{type}_CSV.csv";

        if (!File.Exists(filePath)) return;

        var lastWrite = File.GetLastWriteTimeUtc(filePath);
        if (_editorCsvCache.ContainsKey(type) && _editorCsvLastModified.TryGetValue(type, out var cachedTime) && cachedTime == lastWrite)
        {
            return;
        }

        var dict = new Dictionary<string, (string kr, string en)>(System.StringComparer.OrdinalIgnoreCase);

        try
        {
            using var reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
            string header = reader.ReadLine();
            if (header != null)
            {
                var headers = header.Split(',');
                int idIdx = -1, krIdx = -1, enIdx = -1;
                for (int i = 0; i < headers.Length; i++)
                {
                    string h = headers[i].Trim();
                    if (h.Equals("ID", System.StringComparison.OrdinalIgnoreCase)) idIdx = i;
                    else if (h.Equals("KR", System.StringComparison.OrdinalIgnoreCase)) krIdx = i;
                    else if (h.Equals("EN", System.StringComparison.OrdinalIgnoreCase)) enIdx = i;
                }

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var columns = ParseCsvLine(line);
                    if (idIdx >= 0 && idIdx < columns.Count)
                    {
                        string rowId = columns[idIdx];
                        string kr = (krIdx >= 0 && krIdx < columns.Count) ? columns[krIdx].Replace("\\n", "\n") : rowId;
                        string en = (enIdx >= 0 && enIdx < columns.Count) ? columns[enIdx].Replace("\\n", "\n") : rowId;

                        if (!string.IsNullOrEmpty(rowId))
                        {
                            dict[rowId] = (kr, en);
                        }
                    }
                }
            }

            _editorCsvCache[type] = dict;
            _editorCsvLastModified[type] = lastWrite;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[LocalizedText] Failed to load CSV preview for {type}: {ex.Message}");
        }
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(line)) return result;

        int start = 0;
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (line[i] == ',' && !inQuotes)
            {
                result.Add(Unquote(line.Substring(start, i - start)));
                start = i + 1;
            }
        }
        result.Add(Unquote(line.Substring(start)));
        return result;
    }

    private static string Unquote(string str)
    {
        str = str.Trim();
        if (str.Length >= 2 && str.StartsWith("\"") && str.EndsWith("\""))
        {
            str = str.Substring(1, str.Length - 2).Replace("\"\"", "\"");
        }
        return str;
    }
#endif
}