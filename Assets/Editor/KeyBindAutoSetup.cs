using System;
using System.Collections.Generic;
using System.Linq;
using Option;
using Option.Element;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CleavingDestiny.Editor
{
    [InitializeOnLoad]
    public static class KeyBindAutoSetup
    {
        private const string SETUP_KEY = "KeyBindAutoSetup_Executed_v4";
        private const string PREFAB_PATH = "Assets/3_Prefabs/Option/OptionCanvas.prefab";
        private const string ELEMENT_PREFAB_PATH = "Assets/3_Prefabs/Option/KeyBindElement.prefab";
        private const string INPUT_ACTION_PATH = "Assets/InputSystem_Actions.inputactions";

        static KeyBindAutoSetup()
        {
            EditorApplication.delayCall += () =>
            {
                if (EditorPrefs.GetBool(SETUP_KEY, false)) return;
                EditorPrefs.SetBool(SETUP_KEY, true);

                ExecuteSetup();
            };
        }

        [MenuItem("Tools/CleavingDestiny/Setup KeyBind Option in OptionCanvas")]
        [MenuItem("CONTEXT/KeyBindOption/Setup All KeyBind Items")]
        public static void ExecuteSetup()
        {
            GameObject elementPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ELEMENT_PREFAB_PATH);
            if (elementPrefab == null)
            {
                Debug.LogError($"[KeyBindAutoSetup] Cannot find KeyBindElement prefab at '{ELEMENT_PREFAB_PATH}'");
                return;
            }

            var allSubAssets = AssetDatabase.LoadAllAssetsAtPath(INPUT_ACTION_PATH);
            var actionRefs = allSubAssets.OfType<InputActionReference>().ToList();
            var inputAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(INPUT_ACTION_PATH);

            GameObject root = PrefabUtility.LoadPrefabContents(PREFAB_PATH);
            if (root == null)
            {
                Debug.LogError($"[KeyBindAutoSetup] Cannot load prefab at '{PREFAB_PATH}'");
                return;
            }

            try
            {
                var keyBindOption = root.GetComponentInChildren<KeyBindOption>(true);
                if (keyBindOption == null)
                {
                    Debug.LogError("[KeyBindAutoSetup] KeyBindOption component not found in OptionCanvas.prefab");
                    return;
                }

                var optionManager = root.GetComponent<OptionManager>();
                if (optionManager != null)
                {
                    optionManager.KeyBindOption = keyBindOption;
                }

                // Scroll View 내부의 Content RectTransform 탐색
                Transform contentTransform = null;
                var scrollRect = keyBindOption.GetComponentInChildren<ScrollRect>(true);
                if (scrollRect != null && scrollRect.content != null)
                {
                    contentTransform = scrollRect.content;
                }
                else
                {
                    var grid = keyBindOption.GetComponentInChildren<GridLayoutGroup>(true);
                    if (grid != null)
                    {
                        contentTransform = grid.transform;
                    }
                }

                if (contentTransform == null)
                {
                    Debug.LogError("[KeyBindAutoSetup] Content transform not found under KeyBindOption!");
                    return;
                }

                // 기존 Content 내의 임시 자식 오브젝트 정리
                var existingChildren = new List<GameObject>();
                for (int i = 0; i < contentTransform.childCount; i++)
                {
                    existingChildren.Add(contentTransform.GetChild(i).gameObject);
                }
                foreach (var child in existingChildren)
                {
                    UnityEngine.Object.DestroyImmediate(child);
                }

                // 등록할 모든 14개 액션 설정
                var configs = new (string textId, string actionName, CompositePart composite)[]
                {
                    ("Key_Move_Forward", "Move", CompositePart.Up),
                    ("Key_Move_Backward", "Move", CompositePart.Down),
                    ("Key_Move_Left", "Move", CompositePart.Left),
                    ("Key_Move_Right", "Move", CompositePart.Right),
                    ("Key_Sprint", "Sprint", CompositePart.None),
                    ("Key_Jump", "Jump", CompositePart.None),
                    ("Key_Interact", "Interact", CompositePart.None),
                    ("Key_Tab", "Tab", CompositePart.None),
                    ("Key_GuideBook", "GuideBook", CompositePart.None),
                    ("Key_QuickSlot1", "QuickSlot1", CompositePart.None),
                    ("Key_QuickSlot2", "QuickSlot2", CompositePart.None),
                    ("Key_QuickSlot3", "QuickSlot3", CompositePart.None),
                    ("Key_QuickSlot4", "QuickSlot4", CompositePart.None),
                    ("Key_QuickSlot5", "QuickSlot5", CompositePart.None),
                };

                var items = new List<KeyBindItem>();

                foreach (var cfg in configs)
                {
                    InputActionReference matchedRef = actionRefs.FirstOrDefault(r =>
                        r != null && r.action != null &&
                        r.action.name.Equals(cfg.actionName, StringComparison.OrdinalIgnoreCase));

                    if (matchedRef == null && inputAsset != null)
                    {
                        var act = inputAsset.FindAction($"Player/{cfg.actionName}");
                        if (act != null)
                        {
                            matchedRef = InputActionReference.Create(act);
                        }
                    }

                    if (matchedRef == null)
                    {
                        Debug.LogWarning($"[KeyBindAutoSetup] Could not find action reference for {cfg.actionName}");
                    }

                    // KeyBindElement 프리팹 인스턴스화
                    GameObject elementGo = (GameObject)PrefabUtility.InstantiatePrefab(elementPrefab, contentTransform);
                    elementGo.name = $"KeyBindElement_{cfg.textId}";
                    var elementComp = elementGo.GetComponent<KeyBindElement>();

                    var item = new KeyBindItem
                    {
                        localizedName = new LocalizedString(CSV_Type.Option, cfg.textId),
                        actionReference = matchedRef,
                        compositePart = cfg.composite,
                        customBindingIndex = -1,
                        element = elementComp
                    };

                    items.Add(item);
                }

                keyBindOption.KeyBindItems = items;

                PrefabUtility.SaveAsPrefabAsset(root, PREFAB_PATH);
                Debug.Log($"[KeyBindAutoSetup] Successfully setup all {items.Count} KeyBind items in {PREFAB_PATH}!");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
