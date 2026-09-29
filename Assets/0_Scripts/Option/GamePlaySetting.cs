using System;
using System.Collections.Generic;
using System.Linq;
using Option.Element;
using UnityEngine;

namespace Option
{
    public class GamePlaySetting : MonoBehaviour
    {
        [Header("Gameplay Setting Elements")]
        [SerializeField] DropdownSetting resoulutionSetting;
        [SerializeField] DropdownSetting screenModeSetting;
        [SerializeField] DropdownSetting fpsLimitSetting;
        [SerializeField] ToggleSetting vSyncSetting;
        [SerializeField] DropdownSetting languageSetting;
        [SerializeField] SliderWithTextSetting fovSetting;
        [SerializeField] ToggleSetting invertYSetting;
        [SerializeField] SliderWithTextSetting mouseSensitivitySetting;

        [Header("Check Panel")]
        [SerializeField] CheckPanel checkPanel;

        // private SettingData SettingData => OptionManager.Instance.settingData;
        private SettingData settingData;
        
        List<Resolution> resolutions;
        FullScreenMode fullScreenMode;

        LocalizedString checkResolutionMessage = new LocalizedString(CSV_Type.Option, "Button_CheckResol");

        public void Initialize(SettingData newSettingData)
        {
            settingData = newSettingData;
            
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

            // 화면 모드 설정
            List<string> screenModeKeys = new List<string> { "Screen_FullScreen", "Screen_Borderless", "Screen_Windowed" };
            screenModeSetting.SetLocalizedOptions(screenModeKeys, CSV_Type.Option);

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

            if (!OptionManager.Instance.isInitialized)
            {
                SetDefaultValues();
            }

            resoulutionSetting.SetSelectedIndex(settingData.resolutionIndex);
            resoulutionSetting.AddListener(OnResolutionChanged);

            screenModeSetting.SetSelectedIndex(settingData.screenModeIndex);
            screenModeSetting.AddListener(OnScreenModeChanged);

            fpsLimitSetting.SetSelectedIndex(settingData.fpsLimitIndex);
            fpsLimitSetting.AddListener(OnFpsLimitChanged);

            // VSync 설정
            vSyncSetting.SetValueWithoutNotify(settingData.vSync);
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
            fovSetting.SetValue(settingData.fov);
            fovSetting.AddListener(value =>
            {
                settingData.fov = value;
                // 게임 씬에서 카메라에 적용해야함.
            });

            // Y축 반전 설정
            invertYSetting.SetValueWithoutNotify(settingData.invertY);
            invertYSetting.AddListener(isOn =>
            {
                settingData.invertY = isOn;
                // 게임 씬에서 입력 처리 시 적용해야함.
            });

            // 마우스 감도 설정
            mouseSensitivitySetting.SetMinMax(0.1f, 10f);
            mouseSensitivitySetting.SetValue(settingData.mouseSensitivity);
            mouseSensitivitySetting.AddListener(value =>
            {
                settingData.mouseSensitivity = value;
                // 게임 씬에서 입력 처리 시 적용해야함.
            });

            // 저장된 설정(해상도,FPS제한 등)을 이번 실행에 적용
            ApplySavedSettings();
        }

        private void SetDefaultValues()
        {
            // 해상도 초기값: 현재 화면 해상도 찾기
            string currentRes = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
            int foundIndex = resolutions.FindIndex(r => (r.width + " x " + r.height) == currentRes);
            settingData.resolutionIndex = (foundIndex != -1) ? foundIndex : resolutions.Count - 1;

            // 화면 모드 초기값 (FullScreenMode 기준)
            if (Screen.fullScreenMode == FullScreenMode.Windowed) settingData.screenModeIndex = 2;
            else if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) settingData.screenModeIndex = 1;
            else settingData.screenModeIndex = 0;

            // FPS 제한: Unlimited
            settingData.fpsLimitIndex = fpsLimitSetting.OptionsCount - 1;

            // VSync 등은 SettingData 기본값을 따름
        }

        private void ApplySavedSettings()
        {
            // 1. 화면 모드 적용
            // 인덱스 유효성 검사 (0, 1, 2)
            if (settingData.screenModeIndex < 0 || settingData.screenModeIndex > 2)
            {
                settingData.screenModeIndex = 0; // Default: FullScreen
            }
            OnScreenModeChanged(settingData.screenModeIndex);

            // 2. 해상도 적용
            // 해상도 인덱스가 유효하지 않다면 현재 해상도로 재설정
            if (settingData.resolutionIndex < 0 || settingData.resolutionIndex >= resolutions.Count)
            {
                // 현재 화면 해상도와 일치하는 인덱스 찾기
                string currentRes = Screen.currentResolution.width + " x " + Screen.currentResolution.height;
                int foundIndex = -1;
                for (int i = 0; i < resoulutionSetting.OptionsCount; i++)
                {
                    if (resoulutionSetting.GetOptionText(i) == currentRes)
                    {
                        foundIndex = i;
                        break;
                    }
                }
                settingData.resolutionIndex = (foundIndex != -1) ? foundIndex : resolutions.Count - 1;
            }
            SetResolution(settingData.resolutionIndex);

            // 3. FPS 제한 적용
            if (settingData.fpsLimitIndex < 0 || settingData.fpsLimitIndex >= fpsLimitSetting.OptionsCount)
            {
                settingData.fpsLimitIndex = fpsLimitSetting.OptionsCount - 1; // Default: Unlimited
            }
            OnFpsLimitChanged(settingData.fpsLimitIndex);

            // 4. VSync 적용
            OnVSyncChanged(settingData.vSync);

        }


        private void OnResolutionChanged(int index)
        {
            SetResolution(index);

            checkPanel.ShowWithTimeout(
                checkResolutionMessage,
                null,
                () =>
                {
                    // 취소 시 이전 해상도로 되돌리기
                    int prevIndex = settingData.resolutionIndex;
                    var prevReso = resolutions[prevIndex];
                    Screen.SetResolution(prevReso.width, prevReso.height, fullScreenMode);
                    resoulutionSetting.SetSelectedIndex(prevIndex);
                },
                15 // 15초 타임아웃
            );
        }

        private void SetResolution(int index)
        {
            settingData.resolutionIndex = index;
            var reso = resolutions[index];
            Screen.SetResolution(reso.width, reso.height, fullScreenMode);
        }

        private void OnScreenModeChanged(int index)
        {
            settingData.screenModeIndex = index;
            fullScreenMode = (FullScreenMode)(index + 1); // FullScreenMode.ExclusiveFullScreen을 건너뛰기 위해 +1
            Screen.fullScreenMode = fullScreenMode;
        }

        private void OnFpsLimitChanged(int index)
        {
            settingData.fpsLimitIndex = index;
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
            settingData.vSync = isOn;
            QualitySettings.vSyncCount = isOn ? 1 : 0;
        }

        private void OnLanguageChanged(int index)
        {
            LocalizationManager.Instance.SetLanguage((Language)index);
        }
    }
}