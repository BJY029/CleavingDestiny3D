using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Option.Element
{
    public class DropdownSetting : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown dropdown;
        Action<int> onValueChanged;

        List<string> options;
        CSV_Type currentTableType;
        bool isLocalizedOptions = false;

        public int OptionsCount { get; private set; }

        void Start()
        {
            if (dropdown == null)
            {
                dropdown = GetComponentInChildren<TMP_Dropdown>();
            }
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        void OnDestroy()
        {
            if (LocalizationManager.Instance != null)
                LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
        }

        void UpdateText()
        {
            if (!isLocalizedOptions || options == null) return;

            dropdown.ClearOptions();
            var localizedOptions = new List<string>(options.Count);
            foreach (var key in options)
            {
                localizedOptions.Add(LocalizationManager.Instance.GetText(currentTableType, key));
            }
            dropdown.AddOptions(localizedOptions);
            dropdown.RefreshShownValue();
        }

        public void AddListener(Action<int> listener)
        {
            // 기존 이벤트 제거 후 추가 (중복 방지)
            onValueChanged -= listener;
            onValueChanged += listener;
        }

        public void SetOptions(List<string> newOptions)
        {
            isLocalizedOptions = false;
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;

            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
            OptionsCount = newOptions.Count;
        }

        public void SetLocalizedOptions(List<string> keyOptions, CSV_Type tableType)
        {
            isLocalizedOptions = true;
            options = keyOptions;
            currentTableType = tableType;

            OptionsCount = keyOptions.Count;

            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
            LocalizationManager.Instance.OnLanguageChanged += UpdateText;

            UpdateText();
        }

        public void SetSelectedIndex(int index)
        {
            dropdown.value = index;
        }

        void OnDropdownValueChanged(int value)
        {
            onValueChanged?.Invoke(value);
        }

        public ReadOnlySpan<char> GetOptionText(int index)
        {
            return dropdown.options[index].text;
        }
    }
}