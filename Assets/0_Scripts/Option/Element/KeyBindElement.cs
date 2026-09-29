using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Option
{
    public class KeyBindElement : MonoBehaviour
    {
        [SerializeField] private Button keyBindButton;
        [SerializeField] private TextMeshProUGUI keyBindText;
        [SerializeField] private TextMeshProUGUI actionNameText;

        private Action onKeyBindButtonClicked;

        public void Initialize(LocalizedString localizedName, string keyBind, Action onClicked, string fallbackName = null)
        {
            if (actionNameText != null)
            {
                if (actionNameText.TryGetComponent<LocalizedText>(out var localizedText) && !string.IsNullOrEmpty(localizedName.textID))
                {
                    localizedText.TableType = localizedName.tableType;
                    localizedText.TextID = localizedName.textID;
                    localizedText.UpdateText();
                }
                else
                {
                    string text = localizedName.ToString();
                    if (!string.IsNullOrEmpty(text))
                    {
                        actionNameText.text = text;
                    }
                    else if (!string.IsNullOrEmpty(fallbackName))
                    {
                        actionNameText.text = fallbackName;
                    }
                    else if (!string.IsNullOrEmpty(localizedName.textID))
                    {
                        actionNameText.text = localizedName.textID;
                    }
                }
            }

            if (keyBindText != null)
                keyBindText.text = keyBind;

            onKeyBindButtonClicked = onClicked;
        }

        public void Initialize(string newActionName, string keyBind, Action onClicked)
        {
            if (actionNameText != null)
                actionNameText.text = newActionName;

            if (keyBindText != null)
                keyBindText.text = keyBind;

            onKeyBindButtonClicked = onClicked;
        }

        public void Initialize(string newActionName, string keyBind, Action<string> onKeyBindButtonClickedWithName)
        {
            Initialize(newActionName, keyBind, () => onKeyBindButtonClickedWithName?.Invoke(newActionName));
        }

        public void SetKeyBind(string keyBind)
        {
            if (keyBindText != null)
                keyBindText.text = keyBind;
        }

        public void SetListening(bool isListening)
        {
            if (keyBindText != null && isListening)
            {
                string listeningPrompt = "...";
                if (LocalizationManager.Instance != null && LocalizationManager.Instance.IsLoaded)
                {
                    string text = LocalizationManager.Instance.GetText(CSV_Type.Option, "Key_Rebind_Listening");
                    if (!string.IsNullOrEmpty(text))
                    {
                        listeningPrompt = text;
                    }
                }
                keyBindText.text = listeningPrompt;
            }

            if (keyBindButton != null)
            {
                keyBindButton.interactable = !isListening;
            }
        }

        private void Awake()
        {
            if (keyBindButton != null)
            {
                keyBindButton.onClick.AddListener(OnClickKeyBindButton);
            }
        }

        private void OnClickKeyBindButton()
        {
            onKeyBindButtonClicked?.Invoke();
        }
    }
}
