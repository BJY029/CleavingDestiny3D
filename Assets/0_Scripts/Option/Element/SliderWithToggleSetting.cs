using UnityEngine;
using UnityEngine.UI;
using System;

namespace Option.Element
{
    public class SliderWithToggleSetting : BaseSliderSetting
    {
        [SerializeField] Toggle toggle;
        float cachedSliderValue = 1f;
        
        void Start()
        {
            if (toggle == null)
            {
                toggle = GetComponentInChildren<Toggle>();
            }
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

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
                slider.value = slider.minValue;
                slider.interactable = false;
            }
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
