using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(AudioDataSO))]
public class AudioDataSOEditor : Editor
{
    private string searchString = "";
    private HashSet<string> duplicatedIDs = new HashSet<string>();

    public override void OnInspectorGUI()
    {
        AudioDataSO myTarget = (AudioDataSO)target;

        serializedObject.Update();

        // 중복 검사
        CheckDuplicatedIDs(myTarget);

        // 헤더 정보
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🎵 Audio Data SO Manager", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("오디오 키 ID와 클립을 관리하는 데이터 에셋입니다. 중복된 ID가 없도록 주의하세요.", MessageType.Info);
        EditorGUILayout.Space();

        // 검색 바
        DrawSearchBar();

        SerializedProperty listProp = serializedObject.FindProperty("audioDatas");

        if (listProp == null)
        {
            EditorGUILayout.HelpBox("audioDatas 리스트를 찾을 수 없습니다.", MessageType.Error);
            return;
        }

        // 리스트 드로잉
        EditorGUILayout.BeginVertical();

        int drawCount = 0;
        for (int i = 0; i < listProp.arraySize; i++)
        {
            SerializedProperty elementProp = listProp.GetArrayElementAtIndex(i);

            SerializedProperty idProp = elementProp.FindPropertyRelative("id");
            SerializedProperty clipProp = elementProp.FindPropertyRelative("clip");
            SerializedProperty randomClipsProp = elementProp.FindPropertyRelative("randomClips");
            SerializedProperty useRandomClipProp = elementProp.FindPropertyRelative("useRandomClip");
            SerializedProperty volumeProp = elementProp.FindPropertyRelative("volume");
            SerializedProperty pitchProp = elementProp.FindPropertyRelative("pitch");
            SerializedProperty useRandomPitchProp = elementProp.FindPropertyRelative("useRandomPitch");
            SerializedProperty pitchRangeProp = elementProp.FindPropertyRelative("pitchRange");
            SerializedProperty is3DProp = elementProp.FindPropertyRelative("is3D");

            string idValue = idProp.stringValue;

            // 검색 필터 적용
            if (!string.IsNullOrEmpty(searchString) && idValue.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            drawCount++;

            // 스타일 정의
            bool isDuplicated = duplicatedIDs.Contains(idValue) && !string.IsNullOrEmpty(idValue);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // 중복 경고
            if (isDuplicated)
            {
                EditorGUILayout.HelpBox($"중복 경고: '{idValue}' ID가 중복 등록되었습니다!", MessageType.Error);
            }

            EditorGUILayout.BeginHorizontal();

            // ID 프로퍼티
            EditorGUILayout.PropertyField(idProp, new GUIContent("ID"));

            // 삭제 버튼
            Color previousBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.9f, 0.3f, 0.3f);

            bool deleteRequested = GUILayout.Button("✕", GUILayout.Width(25));

            GUI.backgroundColor = previousBackgroundColor;

            if (deleteRequested)
            {
                listProp.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            // 상세 데이터 필드들
            DrawClipSettings(clipProp, randomClipsProp, useRandomClipProp);

            EditorGUILayout.Space(3f);

            EditorGUILayout.PropertyField(volumeProp, new GUIContent("Volume"));

            DrawPitchSettings(pitchProp, useRandomPitchProp, pitchRangeProp);

            EditorGUILayout.PropertyField(is3DProp, new GUIContent("Is 3D Sound"));

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (drawCount == 0 && listProp.arraySize > 0)
        {
            EditorGUILayout.HelpBox("검색어와 일치하는 오디오 데이터가 없습니다.", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        // 추가 버튼
        if (GUILayout.Button("✚ Add New Audio Data", GUILayout.Height(30)))
        {
            AddNewAudioData(listProp);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        GUILayout.Label("검색 ID:", GUILayout.Width(50));

        searchString = EditorGUILayout.TextField(searchString);

        if (GUILayout.Button("초기화", GUILayout.Width(50)))
        {
            searchString = "";
            GUI.FocusControl(null);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    private void DrawClipSettings(SerializedProperty clipProp, SerializedProperty randomClipsProp, SerializedProperty useRandomClipProp)
    {
        EditorGUILayout.LabelField("Audio Clip", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(clipProp, new GUIContent("Default Clip"));
        EditorGUILayout.PropertyField(useRandomClipProp, new GUIContent("Use Random Clip"));

        using (new EditorGUI.DisabledScope(!useRandomClipProp.boolValue))
        {
            EditorGUILayout.PropertyField(randomClipsProp, new GUIContent("Random Clips"), true);
        }

        if (useRandomClipProp.boolValue &&
            clipProp.objectReferenceValue == null &&
            randomClipsProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox("랜덤 재생을 사용하려면 기본 클립 또는 Random Clips를 등록해야 합니다.", MessageType.Warning);
        }
    }

    private void DrawPitchSettings(SerializedProperty pitchProp, SerializedProperty useRandomPitchProp, SerializedProperty pitchRangeProp)
    {
        EditorGUILayout.Space(3f);
        EditorGUILayout.LabelField("Pitch", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(pitchProp, new GUIContent("Default Pitch"));
        EditorGUILayout.PropertyField(useRandomPitchProp, new GUIContent("Use Random Pitch"));

        if (!useRandomPitchProp.boolValue)
            return;

        Vector2 range = pitchRangeProp.vector2Value;

        float min = Mathf.Clamp(Mathf.Min(range.x, range.y), 0.1f, 3f);
        float max = Mathf.Clamp(Mathf.Max(range.x, range.y), 0.1f, 3f);

        EditorGUILayout.BeginHorizontal();
        min = EditorGUILayout.FloatField("Min", min);
        max = EditorGUILayout.FloatField("Max", max);
        EditorGUILayout.EndHorizontal();

        min = Mathf.Clamp(min, 0.1f, 3f);
        max = Mathf.Clamp(max, min, 3f);

        EditorGUILayout.MinMaxSlider(ref min, ref max, 0.1f, 3f);

        pitchRangeProp.vector2Value = new Vector2(min, max);
    }

    private void AddNewAudioData(SerializedProperty listProp)
    {
        int newIndex = listProp.arraySize;

        listProp.InsertArrayElementAtIndex(newIndex);

        SerializedProperty newElement = listProp.GetArrayElementAtIndex(newIndex);

        newElement.FindPropertyRelative("id").stringValue = "New_Audio_Key";
        newElement.FindPropertyRelative("clip").objectReferenceValue = null;

        SerializedProperty randomClipsProp = newElement.FindPropertyRelative("randomClips");
        randomClipsProp.arraySize = 0;

        newElement.FindPropertyRelative("useRandomClip").boolValue = false;
        newElement.FindPropertyRelative("volume").floatValue = 1f;
        newElement.FindPropertyRelative("pitch").floatValue = 1f;
        newElement.FindPropertyRelative("useRandomPitch").boolValue = false;
        newElement.FindPropertyRelative("pitchRange").vector2Value = new Vector2(0.9f, 1.1f);
        newElement.FindPropertyRelative("is3D").boolValue = false;
    }

    private void CheckDuplicatedIDs(AudioDataSO so)
    {
        duplicatedIDs.Clear();
        HashSet<string> checkedIDs = new HashSet<string>();
        foreach (var data in so.audioDatas)
        {
            if (string.IsNullOrEmpty(data.id)) continue;
            if (checkedIDs.Contains(data.id))
            {
                duplicatedIDs.Add(data.id);
            }
            else
            {
                checkedIDs.Add(data.id);
            }
        }
    }

    // GUI 배경 텍스처 생성을 위한 헬퍼
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
