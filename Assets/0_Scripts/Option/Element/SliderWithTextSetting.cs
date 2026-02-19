using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Option.Element
{
    public class SliderWithTextSetting : MonoBehaviour
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI valueText;
        Action<float> onValueChanged;


        void Start()
        {
            if (slider == null)
            {
                slider = GetComponentInChildren<Slider>();
            }
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        public void AddListener(Action<float> listener)
        {
            onValueChanged -= listener;
            onValueChanged += listener;
        }

        public void SetValue(float value)
        {
            slider.value = value;
            if (valueText != null)
            {
                valueText.SetText("{0}", Mathf.RoundToInt(value));
            }
        }

        public void SetMinMax(float min, float max)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }

        void OnSliderValueChanged(float value)
        {
            if (valueText != null)
            {
                valueText.SetText("{0}", Mathf.RoundToInt(value));
            }
            onValueChanged?.Invoke(value);
        }
    }
}