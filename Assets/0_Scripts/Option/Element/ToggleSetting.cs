using System;
using UnityEngine;
using UnityEngine.UI;

namespace Option.Element
{
    public class ToggleSetting : MonoBehaviour
    {
        [SerializeField] Toggle toggle;
        Action<bool> onValueChanged;

        void Start()
        {
            if (toggle == null)
            {
                toggle = GetComponentInChildren<Toggle>();
            }
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void AddListener(Action<bool> listener)
        {
            onValueChanged -= listener;
            onValueChanged += listener;
        }

        public void SetValue(bool value)
        {
            toggle.isOn = value;
        }

        void OnToggleValueChanged(bool value)
        {
            onValueChanged?.Invoke(value);
        }
    }
}