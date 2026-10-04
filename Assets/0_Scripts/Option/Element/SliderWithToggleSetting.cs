using System;
using UnityEngine;
using UnityEngine.UI;

namespace Option.Element
{
    public class SliderWithToggleSetting : BaseSliderSetting
    {
        [SerializeField] private Toggle toggle;
        private Action<bool> onToggleChanged;
        private float cachedSliderValue = 1f;

        protected override void Awake()
        {
            base.Awake();

            if (toggle == null)
            {
                toggle = GetComponentInChildren<Toggle>();
            }

            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }
        }

        public void AddToggleListener(Action<bool> listener)
        {
            onToggleChanged -= listener;
            onToggleChanged += listener;
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                slider.interactable = true;
                SetValue(cachedSliderValue);
            }
            else
            {
                cachedSliderValue = slider.value;
                slider.interactable = false;
                slider.SetValueWithoutNotify(slider.minValue);
            }

            onToggleChanged?.Invoke(isOn);
        }

        public override void SetValue(float value)
        {
            cachedSliderValue = value;

            if (toggle != null && !toggle.isOn)
            {
                slider.SetValueWithoutNotify(slider.minValue);
                return;
            }

            base.SetValue(value);
        }

        public void SetToggleValue(bool isOn)
        {
            if (toggle == null)
            {
                toggle = GetComponentInChildren<Toggle>();
            }

            if (toggle != null)
            {
                toggle.isOn = isOn;
                if (!isOn)
                {
                    slider.interactable = false;
                    slider.SetValueWithoutNotify(slider.minValue);
                }
                else
                {
                    slider.interactable = true;
                }
            }
        }
    }
}
