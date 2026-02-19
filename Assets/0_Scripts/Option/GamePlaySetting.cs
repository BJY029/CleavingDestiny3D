using System;
using System.Collections.Generic;
using System.Linq;
using Option.Element;
using UnityEngine;

namespace Option
{
    public class GamePlaySetting : MonoBehaviour
    {
        [SerializeField] DropdownSetting resoulutionSetting;
        [SerializeField] DropdownSetting screenModeSetting;
        [SerializeField] DropdownSetting fpsLimitSetting;
        [SerializeField] ToggleSetting vSyncSetting;
        [SerializeField] DropdownSetting languageSetting;
        [SerializeField] SliderWithTextSetting fovSetting;
        [SerializeField] ToggleSetting invertYSetting;
        [SerializeField] SliderWithTextSetting mouseSensitivitySetting;

        private SettingData SettingData => OptionManager.Instance.settingData;

        List<Resolution> resolutions;
        FullScreenMode fullScreenMode;

        void Start()
        {
            double maxFps = 0;

            // 해상도 설정
            var reso = Screen.resolutions;
            resolutions = new List<Resolution>();
            List<string> options = new List<string>();

            // 중복 제거를 위한 HashSet (width x height)
            HashSet<string> uniqueResolutions = new HashSet<string>();

            for (int i = 0; i < reso.Length; i++)
            {
                // 단순히 width x height가 같은지 확인하여 중복 제거
                string resOption = reso[i].width + " x " + reso[i].height;

                if (!uniqueResolutions.Contains(resOption))
                {
                    uniqueResolutions.Add(resOption);
                    resolutions.Add(reso[i]);
                    options.Add(resOption);
                }

                maxFps = Math.Max(maxFps, reso[i].refreshRateRatio.value);
            }
            resoulutionSetting.SetOptions(options);
            resoulutionSetting.SetSelectedIndex(SettingData.resolutionIndex);
            resoulutionSetting.AddListener(OnResolutionChanged);

            // 화면 모드 설정
            List<string> screenModeKeys = new List<string> { "Option_FullScreen", "Option_Borderless", "Option_Windowed" };
            screenModeSetting.SetLocalizedOptions(screenModeKeys, CSV_Type.Option);
            screenModeSetting.SetSelectedIndex(SettingData.screenModeIndex);
            screenModeSetting.AddListener(OnScreenModeChanged);

            // FPS 제한 설정
            int[] fpsList = new int[] { 30, 60, 120, 144, 165, 240 };
            List<string> fpsOptions = new List<string>(fpsList.Length);
            for (int i = 0; i < fpsList.Length; i++)
            {
                if (fpsList[i] <= maxFps)
                    fpsOptions.Add(fpsList[i].ToString());
            }
            fpsOptions.Add("Unlimited");
            fpsLimitSetting.SetOptions(fpsOptions);
            fpsLimitSetting.SetSelectedIndex(SettingData.fpsLimitIndex);
            fpsLimitSetting.AddListener(OnFpsLimitChanged);

            // VSync 설정
            vSyncSetting.SetValue(SettingData.vSync);
            vSyncSetting.AddListener(OnVSyncChanged);

            // 언어 설정
            List<string> languageKeys = new List<string>();
            foreach (Language lang in Enum.GetValues(typeof(Language)))
            {
                languageKeys.Add(LocalizationManager.Instance.GetLanguageName(lang));
            }
            languageSetting.SetOptions(languageKeys);
            languageSetting.SetSelectedIndex((int)LocalizationManager.Instance.currentLanguage);
            languageSetting.AddListener(OnLanguageChanged);

            //  FOV 설정    
            fovSetting.SetMinMax(60, 90);
            fovSetting.SetValue(SettingData.fov);
            fovSetting.AddListener(value =>
            {
                OptionManager.Instance.settingData.fov = value;
                // 게임 씬에서 카메라에 적용해야함.
            });

            // Y축 반전 설정
            invertYSetting.SetValue(SettingData.invertY);
            invertYSetting.AddListener(isOn =>
            {
                OptionManager.Instance.settingData.invertY = isOn;
                // 게임 씬에서 입력 처리 시 적용해야함.
            });

            // 마우스 감도 설정
            mouseSensitivitySetting.SetMinMax(0.1f, 10f);
            mouseSensitivitySetting.SetValue(SettingData.mouseSensitivity);
            mouseSensitivitySetting.AddListener(value =>
            {
                OptionManager.Instance.settingData.mouseSensitivity = value;
                // 게임 씬에서 입력 처리 시 적용해야함.
            });
        }


        private void OnResolutionChanged(int index)
        {
            SettingData.resolutionIndex = index;
            var reso = resolutions[index];
            Screen.SetResolution(reso.width, reso.height, fullScreenMode);
        }

        private void OnScreenModeChanged(int index)
        {
            SettingData.screenModeIndex = index;
            fullScreenMode = (FullScreenMode)(index + 1); // FullScreenMode.ExclusiveFullScreen을 건너뛰기 위해 +1
            Screen.fullScreenMode = fullScreenMode;
        }

        private void OnFpsLimitChanged(int index)
        {
            SettingData.fpsLimitIndex = index;
            if (index == fpsLimitSetting.OptionsCount - 1) // "Unlimited" 선택 시
            {
                Application.targetFrameRate = -1; // FPS 제한 해제
            }
            else
            {
                int fps = (int)double.Parse(fpsLimitSetting.GetOptionText(index));
                Application.targetFrameRate = fps;
            }
        }

        private void OnVSyncChanged(bool isOn)
        {
            SettingData.vSync = isOn;
            QualitySettings.vSyncCount = isOn ? 1 : 0;
        }

        private void OnLanguageChanged(int index)
        {
            SettingData.languageIndex = index;
            LocalizationManager.Instance.currentLanguage = (Language)index;
        }
    }
}