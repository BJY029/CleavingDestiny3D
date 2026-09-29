using System;
using UnityEngine;
using UnityEngine.UI;

namespace Option.Element
{
    public abstract class BaseSliderSetting : MonoBehaviour
    {
        [SerializeField] protected Slider slider;
        protected Action<float> onValueChanged;

        protected virtual void Awake()
        {
            if (slider == null)
            {
                slider = GetComponentInChildren<Slider>();
            }
            slider.onValueChanged.AddListener(OnSliderValueChangedInternal);
        }

        public void AddListener(Action<float> listener)
        {
            onValueChanged -= listener;
            onValueChanged += listener;
        }

        public virtual void SetValue(float value)
        {
            slider.value = value;
        }

        public virtual void SetValueWithoutNotify(float value)
        {
            if (slider == null)
            {
                slider = GetComponentInChildren<Slider>();
            }
            if (slider != null)
            {
                slider.SetValueWithoutNotify(value);
            }
        }

        public void SetMinMax(float min, float max)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }

        protected virtual void OnSliderValueChangedInternal(float value)
        {
            onValueChanged?.Invoke(value);
        }
    }
}