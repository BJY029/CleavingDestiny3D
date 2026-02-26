using TMPro;
using UnityEngine;

namespace Option.Element
{
    public class SliderWithTextSetting : BaseSliderSetting
    {
        [SerializeField] TextMeshProUGUI valueText;

        void Start()
        {
            if (valueText != null)
            {
                valueText.SetText("{0}", Mathf.RoundToInt(slider.value));
            }
        }

        public override void SetValue(float value)
        {
            base.SetValue(value);
            if (valueText != null)
            {
                valueText.SetText("{0}", Mathf.RoundToInt(value));
            }
        }

        protected override void OnSliderValueChangedInternal(float value)
        {
            if (valueText != null)
            {
                valueText.SetText("{0}", Mathf.RoundToInt(value));
            }
            base.OnSliderValueChangedInternal(value);
        }
    }
}