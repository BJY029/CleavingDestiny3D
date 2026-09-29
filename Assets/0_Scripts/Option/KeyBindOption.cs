using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Option
{
    public enum CompositePart
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    [System.Serializable]
    public class KeyBindItem
    {
        [Tooltip("화면에 표시될 액션의 다국어 텍스트 설정 (Option 테이블)")]
        public LocalizedString localizedName = new LocalizedString(CSV_Type.Option, "");

        public string GetDisplayName()
        {
            string text = localizedName.ToString();
            if (!string.IsNullOrEmpty(text))
                return text;

            if (!string.IsNullOrEmpty(localizedName.textID))
                return localizedName.textID;

            return actionReference != null && actionReference.action != null
                ? actionReference.action.name
                : string.Empty;
        }

        [Tooltip("연결할 InputActionReference (.inputactions 파일에서 액션 선택)")]
        public InputActionReference actionReference;

        [Tooltip("WASD 같은 2D Vector 복합키인 경우 해당 방향 선택 (단일 키는 None)")]
        public CompositePart compositePart = CompositePart.None;

        [Tooltip("특정 바인딩 인덱스를 직접 지정할 경우 입력 (-1이면 자동 감지)")]
        public int customBindingIndex = -1;

        [Tooltip("화면의 UI Element")]
        public KeyBindElement element;

        public InputAction GetAction()
        {
            if (KeyInteractManager.Instance != null && KeyInteractManager.Instance.InputActions != null)
            {
                if (actionReference != null && actionReference.action != null)
                {
                    // 1. Action ID로 검색
                    var found = KeyInteractManager.Instance.InputActions.asset.FindAction(actionReference.action.id);
                    if (found != null) return found;

                    // 2. Action 이름으로 검색
                    found = KeyInteractManager.Instance.InputActions.asset.FindAction(actionReference.action.name);
                    if (found != null) return found;
                }
            }

            return actionReference != null ? actionReference.action : null;
        }

        /// <summary>
        /// 액션 내에서 실제로 리바인딩 대상이 될 bindingIndex를 계산합니다.
        /// </summary>
        public int GetEffectiveBindingIndex()
        {
            var action = GetAction();
            if (action == null) return -1;

            // 1. 복합키(Composite) 파트 검색 (WASD 등) - customBindingIndex보다 우선 처리
            if (compositePart != CompositePart.None)
            {
                string targetPart = compositePart.ToString().ToLower();

                // 1-1: 키보드 복합키 파트 우선 검색
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var b = action.bindings[i];
                    if (b.isPartOfComposite && b.name.Equals(targetPart, StringComparison.OrdinalIgnoreCase))
                    {
                        string p = !string.IsNullOrEmpty(b.effectivePath) ? b.effectivePath : b.path;
                        if (p.Contains("Keyboard") || (b.groups != null && b.groups.Contains("Keyboard")))
                            return i;
                    }
                }

                // 1-2: 키보드 구분이 없을 경우 이름이 일치하는 첫 복합키 파트
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    if (action.bindings[i].isPartOfComposite && action.bindings[i].name.Equals(targetPart, StringComparison.OrdinalIgnoreCase))
                        return i;
                }
            }

            // 2. 단일 키인 경우: 키보드 일반 키(numpad 제외) 우선 검색 (QuickSlot1, Sprint, Jump 등)
            for (int i = 0; i < action.bindings.Count; i++)
            {
                var b = action.bindings[i];
                if (!b.isComposite && !b.isPartOfComposite)
                {
                    string p = !string.IsNullOrEmpty(b.effectivePath) ? b.effectivePath : b.path;
                    if (p.Contains("Keyboard") && !p.Contains("numpad"))
                        return i;
                }
            }

            // 3. 단일 키: numpad 포함 모든 키보드 바인딩 검색
            for (int i = 0; i < action.bindings.Count; i++)
            {
                var b = action.bindings[i];
                if (!b.isComposite && !b.isPartOfComposite)
                {
                    string p = !string.IsNullOrEmpty(b.effectivePath) ? b.effectivePath : b.path;
                    if (p.Contains("Keyboard"))
                        return i;
                }
            }

            // 4. 직접 지정한 인덱스가 유효한 경우 (compositePart가 None일 때)
            if (customBindingIndex >= 0 && customBindingIndex < action.bindings.Count)
                return customBindingIndex;

            // 5. Fallback: 첫 번째 비복합 바인딩 또는 0번
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (!action.bindings[i].isComposite)
                    return i;
            }

            return 0;
        }
    }

    public class KeyBindOption : MonoBehaviour
    {
        [Header("Key Bind Items")]
        [SerializeField] private List<KeyBindItem> keyBindItems = new();
        public List<KeyBindItem> KeyBindItems
        {
            get => keyBindItems;
            set => keyBindItems = value;
        }

        private SettingData _settingData;
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;
        private KeyBindItem _currentRebindingItem;
        private Action _cancelMenuAction;

        private void Awake()
        {
            _cancelMenuAction = CancelRebinding;
        }

        private void OnEnable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged += RefreshUI;
            }

            if (_settingData != null)
            {
                RefreshUI();
            }
        }

        private void OnDisable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= RefreshUI;
            }

            CancelRebinding();
        }

        private void OnDestroy()
        {
            CancelRebinding();
        }

        public void Initialize(SettingData newSettingData)
        {
            _settingData = newSettingData;

            // 저장된 키 바인딩 오버라이드가 있으면 적용
            if (!string.IsNullOrEmpty(_settingData.keyRebinds))
            {
                KeyInteractManager.Instance?.LoadBindingOverrides(_settingData.keyRebinds);
            }

            RefreshUI();
        }

        private string GetCleanDisplayString(InputAction action, int targetIndex)
        {
            if (action == null || targetIndex < 0 || targetIndex >= action.bindings.Count)
                return "-";

            string str = action.GetBindingDisplayString(targetIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
            if (!string.IsNullOrEmpty(str))
            {
                if (str.StartsWith("Hold ", StringComparison.OrdinalIgnoreCase))
                {
                    str = str.Substring(5).Trim();
                }
            }
            return str;
        }

        public void RefreshUI()
        {
            foreach (var item in keyBindItems)
            {
                if (item.element == null) continue;

                var action = item.GetAction();
                if (action == null) continue;

                int targetIndex = item.GetEffectiveBindingIndex();

                string displayString = GetCleanDisplayString(action, targetIndex);

                KeyBindItem capturedItem = item;
                item.element.Initialize(capturedItem.localizedName, displayString, () => StartRebinding(capturedItem), capturedItem.GetDisplayName());
            }
        }

        public void StartRebinding(KeyBindItem item)
        {
            if (item == null || item.element == null) return;

            var action = item.GetAction();
            if (action == null) return;

            // 이미 진행 중인 리바인딩이 있으면 취소
            CancelRebinding();

            int targetIndex = item.GetEffectiveBindingIndex();
            if (targetIndex < 0 || targetIndex >= action.bindings.Count) return;

            _currentRebindingItem = item;
            item.element.SetListening(true);

            // ESC 메뉴 뒤로가기 스택에 취소 액션 등록 (ESC 누르면 메뉴가 닫히지 않고 리바인딩만 취소)
            KeyInteractManager.Instance?.PushMenuAction(_cancelMenuAction);

            action.Disable();

            _rebindOperation = action.PerformInteractiveRebinding(targetIndex)
                .WithControlsExcluding("Mouse") // 마우스 클릭/이동으로 오작동 방지
                .WithCancelingThrough("<Keyboard>/escape") // ESC 누르면 취소
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation =>
                {
                    FinishRebinding(item, action, targetIndex, operation);
                })
                .OnCancel(operation =>
                {
                    CancelRebinding();
                });

            _rebindOperation.Start();
        }

        private void FinishRebinding(KeyBindItem item, InputAction action, int targetIndex, InputActionRebindingExtensions.RebindingOperation operation)
        {
            action.Enable();
            operation?.Dispose();
            _rebindOperation = null;

            KeyInteractManager.Instance?.RemoveMenuAction(_cancelMenuAction);

            string keyString = GetCleanDisplayString(action, targetIndex);
            item.element.SetKeyBind(keyString);
            item.element.SetListening(false);

            _currentRebindingItem = null;

            SaveRebinds();
        }

        public void CancelRebinding()
        {
            if (_rebindOperation != null)
            {
                _rebindOperation.Cancel();
                _rebindOperation.Dispose();
                _rebindOperation = null;
            }

            KeyInteractManager.Instance?.RemoveMenuAction(_cancelMenuAction);

            if (_currentRebindingItem != null)
            {
                var action = _currentRebindingItem.GetAction();
                action?.Enable();

                int targetIndex = _currentRebindingItem.GetEffectiveBindingIndex();
                string keyString = GetCleanDisplayString(action, targetIndex);

                _currentRebindingItem.element?.SetKeyBind(keyString);
                _currentRebindingItem.element?.SetListening(false);
                _currentRebindingItem = null;
            }
        }

        private void SaveRebinds()
        {
            if (_settingData == null) return;

            string rebindsJson = string.Empty;
            if (KeyInteractManager.Instance != null && KeyInteractManager.Instance.InputActions != null)
            {
                rebindsJson = KeyInteractManager.Instance.SaveBindingOverrides();
            }
            else
            {
                var action = _currentRebindingItem?.GetAction();
                if (action != null && action.actionMap != null && action.actionMap.asset != null)
                {
                    rebindsJson = action.actionMap.asset.SaveBindingOverridesAsJson();
                }
            }

            _settingData.keyRebinds = rebindsJson;
            OptionManager.Instance?.SaveSetting().Forget();
        }

        public void ResetToDefault()
        {
            CancelRebinding();

            KeyInteractManager.Instance?.ResetAllBindingOverrides();

            if (_settingData != null)
            {
                _settingData.keyRebinds = string.Empty;
                OptionManager.Instance?.SaveSetting().Forget();
            }

            RefreshUI();
        }
    }
}