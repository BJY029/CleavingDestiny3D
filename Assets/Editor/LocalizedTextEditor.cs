using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    private SerializedProperty textComponentProp;
    private SerializedProperty tableTypeProp;
    private SerializedProperty textIDProp;

    private string[] _availableIDs;
    private int _selectedIndex;

    // Manager와 동일한 파싱 규칙 적용
    private readonly string CSV_SPLIT_REGEX = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

    private void OnEnable()
    {
        textComponentProp = serializedObject.FindProperty("textComponent");
        tableTypeProp = serializedObject.FindProperty("tableType");
        textIDProp = serializedObject.FindProperty("textID");

        UpdateIDs();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(textComponentProp);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(tableTypeProp);
        if (EditorGUI.EndChangeCheck())
        {
            UpdateIDs();
        }

        if (_availableIDs != null && _availableIDs.Length > 0)
        {
            _selectedIndex = System.Array.IndexOf(_availableIDs, textIDProp.stringValue);
            if (_selectedIndex == -1) _selectedIndex = 0;

            _selectedIndex = EditorGUILayout.Popup("Text ID", _selectedIndex, _availableIDs);
            textIDProp.stringValue = _availableIDs[_selectedIndex];
        }
        else
        {
            EditorGUILayout.PropertyField(textIDProp);
            EditorGUILayout.HelpBox("ID를 찾을 수 없습니다. (라벨/파일명/헤더 확인)", MessageType.Warning);
        }

        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void UpdateIDs()
    {
        CSV_Type currentType = (CSV_Type)tableTypeProp.enumValueIndex;
        List<string> ids = new List<string>();

        if (currentType == CSV_Type.Preload)
        {
            var preload = Resources.Load<TextAsset>("Preload_CSV");
            if (preload != null) ids.AddRange(ExtractIDs(preload.text));
        }
        else
        {
            // Addressables 설정을 직접 참조하여 라벨로 에셋 찾기
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                // Localization 라벨을 가진 모든 에셋 엔트리 가져오기
                var entries = new List<AddressableAssetEntry>();
                settings.GetAllAssets(entries, false, null, entry => entry.labels.Contains("Localization"));

                foreach (var entry in entries)
                {
                    string path = entry.AssetPath;
                    string fileName = Path.GetFileNameWithoutExtension(path);

                    if (fileName.StartsWith(currentType.ToString(), System.StringComparison.OrdinalIgnoreCase))
                    {
                        var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                        if (asset != null) ids.AddRange(ExtractIDs(asset.text));
                    }
                }
            }
            else
            {
                Debug.LogWarning("[LocalizedTextEditor] AddressableAssetSettings를 찾을 수 없습니다.");
            }
        }

        _availableIDs = ids.Distinct().OrderBy(s => s).ToArray();
    }

    private List<string> ExtractIDs(string csvText)
    {
        List<string> result = new List<string>();
        using StringReader reader = new StringReader(csvText);
        string headerLine = reader.ReadLine();
        if (string.IsNullOrEmpty(headerLine)) return result;

        // 헤더에도 정규표현식과 따옴표 제거 적용
        string[] header = Regex.Split(headerLine, CSV_SPLIT_REGEX);
        int idIndex = -1;
        for (int i = 0; i < header.Length; i++)
        {
            if (header[i].Trim(' ', '\"') == "ID")
            {
                idIndex = i;
                break;
            }
        }

        if (idIndex == -1) return result;

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var cols = Regex.Split(line, CSV_SPLIT_REGEX);
            if (cols.Length > idIndex)
            {
                string id = cols[idIndex].Trim(' ', '\"');
                if (!string.IsNullOrEmpty(id)) result.Add(id);
            }
        }
        return result;
    }
}