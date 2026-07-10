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
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        GUILayout.Label("검색 ID:", GUILayout.Width(50));
        searchString = EditorGUILayout.TextField(searchString);
        if (GUILayout.Button("초기화", GUILayout.Width(50)))
        {
            searchString = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

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
            SerializedProperty volumeProp = elementProp.FindPropertyRelative("volume");
            SerializedProperty pitchProp = elementProp.FindPropertyRelative("pitch");
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
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            if (isDuplicated)
            {
                boxStyle.normal.background = MakeTex(2, 2, new Color(0.8f, 0.2f, 0.2f, 0.15f));
            }
            else
            {
                boxStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.05f));
            }

            EditorGUILayout.BeginVertical(boxStyle);

            // 중복 경고
            if (isDuplicated)
            {
                EditorGUILayout.HelpBox($"중복 경고: '{idValue}' ID가 중복 등록되었습니다!", MessageType.Error);
            }

            EditorGUILayout.BeginHorizontal();
            
            // ID 프로퍼티
            EditorGUILayout.PropertyField(idProp, new GUIContent("ID"));

            // 삭제 버튼
            GUI.backgroundColor = new Color(0.9f, 0.3f, 0.3f);
            if (GUILayout.Button("✕", GUILayout.Width(25)))
            {
                listProp.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break; // 인덱스 파괴 방지
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            // 상세 데이터 필드들
            EditorGUILayout.PropertyField(clipProp, new GUIContent("Clip"));
            EditorGUILayout.PropertyField(volumeProp, new GUIContent("Volume"));
            EditorGUILayout.PropertyField(pitchProp, new GUIContent("Pitch"));
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
            int newIndex = listProp.arraySize;
            listProp.InsertArrayElementAtIndex(newIndex);
            SerializedProperty newElement = listProp.GetArrayElementAtIndex(newIndex);
            
            // 기본값 초기화
            newElement.FindPropertyRelative("id").stringValue = "New_Audio_Key";
            newElement.FindPropertyRelative("clip").objectReferenceValue = null;
            newElement.FindPropertyRelative("volume").floatValue = 1f;
            newElement.FindPropertyRelative("pitch").floatValue = 1f;
            newElement.FindPropertyRelative("is3D").boolValue = false;
        }

        serializedObject.ApplyModifiedProperties();
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
