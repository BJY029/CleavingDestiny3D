using UnityEngine;
using UnityEngine.UI;
using System;

namespace Option.Element
{
    public class SliderWithToggleSetting : BaseSliderSetting
    {
        [SerializeField] Toggle toggle;
        public Action<bool> onToggleChanged;

        void Start()
        {
            if (toggle == null)
            {
                toggle = GetComponentInChildren<Toggle>();
            }
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void AddToggleListener(Action<bool> listener)
        {
            onToggleChanged -= listener;
            onToggleChanged += listener;
        }

        float cachedSliderValue = 1f;

        void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                base.SetValue(cachedSliderValue);
                slider.interactable = true;
            }
            else
            {
                cachedSliderValue = slider.value;
                slider.interactable = false;
            }
            onToggleChanged?.Invoke(isOn);
        }

        public override void SetValue(float value)
        {
            base.SetValue(value);
            if (toggle != null && toggle.isOn)
            {
                cachedSliderValue = value;
            }
        }
    }
}
