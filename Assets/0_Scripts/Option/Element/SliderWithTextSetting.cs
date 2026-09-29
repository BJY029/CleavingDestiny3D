using TMPro;
using UnityEngine;

namespace Option.Element
{
    public class SliderWithTextSetting : BaseSliderSetting
    {
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private int decimalPlaces = 0;

        protected override void Awake()
        {
            base.Awake();
            if (valueText == null)
            {
                valueText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void Start()
        {
            if (slider != null)
            {
                UpdateText(slider.value);
            }
        }

        public void SetDecimalPlaces(int places)
        {
            decimalPlaces = Mathf.Max(0, places);
            if (slider != null)
            {
                UpdateText(slider.value);
            }
        }

        public override void SetValue(float value)
        {
            base.SetValue(value);
            UpdateText(value);
        }

        protected override void OnSliderValueChangedInternal(float value)
        {
            UpdateText(value);
            base.OnSliderValueChangedInternal(value);
        }

        private void UpdateText(float value)
        {
            if (valueText == null) return;

            if (decimalPlaces <= 0)
            {
                valueText.SetText("{0}", Mathf.RoundToInt(value));
            }
            else
            {
                valueText.text = value.ToString($"F{decimalPlaces}");
            }
        }
    }
}