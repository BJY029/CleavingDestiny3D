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
        FullScreenMode fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        private bool _isInitialized = false;

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

            // 해상도 목록이 비어있는 경우 Fallback 추가
            if (resolutions.Count == 0)
            {
                Resolution fallback = new Resolution
                {
                    width = Screen.width > 0 ? Screen.width : 1920,
                    height = Screen.height > 0 ? Screen.height : 1080
                };
                resolutions.Add(fallback);
                options.Add(fallback.width + " x " + fallback.height);
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

            // 언어 옵션 설정
            List<string> languageKeys = new List<string>();
            foreach (Language lang in Enum.GetValues(typeof(Language)))
            {
                languageKeys.Add(LocalizationManager.Instance.GetLanguageName(lang));
            }
            languageSetting.SetOptions(languageKeys);

            // 최초 생성(저장 파일 없음) 시 모니터에 맞춘 스마트 기본값 적용
            if (OptionManager.Instance != null && !OptionManager.Instance.isInitialized)
            {
                SetDefaultValues();
            }

            if (!_isInitialized)
            {
                _isInitialized = true;

                resoulutionSetting.AddListener(OnResolutionChanged);
                screenModeSetting.AddListener(OnScreenModeChanged);
                fpsLimitSetting.AddListener(OnFpsLimitChanged);
                vSyncSetting.AddListener(OnVSyncChanged);
                languageSetting.AddListener(OnLanguageChanged);

                // FOV 설정
                fovSetting.SetMinMax(60, 90);
                fovSetting.AddListener(value =>
                {
                    settingData.fov = value;
                });

                // Y축 반전 설정
                invertYSetting.AddListener(isOn =>
                {
                    settingData.invertY = isOn;
                });

                // 마우스 감도 설정
                mouseSensitivitySetting.SetMinMax(0.2f, 3f);
                mouseSensitivitySetting.SetDecimalPlaces(1);
                mouseSensitivitySetting.AddListener(value =>
                {
                    settingData.mouseSensitivity = value;
                });
            }

            // UI 값 동기화
            resoulutionSetting.SetValueWithoutNotify(settingData.resolutionIndex);
            screenModeSetting.SetValueWithoutNotify(settingData.screenModeIndex);
            fpsLimitSetting.SetValueWithoutNotify(settingData.fpsLimitIndex);
            vSyncSetting.SetValueWithoutNotify(settingData.vSync);
            languageSetting.SetValueWithoutNotify((int)LocalizationManager.Instance.currentLanguage);
            fovSetting.SetValueWithoutNotify(settingData.fov);
            invertYSetting.SetValueWithoutNotify(settingData.invertY);
            mouseSensitivitySetting.SetValueWithoutNotify(settingData.mouseSensitivity);

            // 저장된 설정(해상도, FPS 제한 등)을 이번 실행에 적용
            ApplySavedSettings();
        }

        /// <summary>
        /// 모니터의 가로/세로 해상도와 화면비에 가장 가까운 지원 해상도 인덱스를 검색합니다.
        /// </summary>
        public int FindClosestResolutionIndex(int targetWidth, int targetHeight)
        {
            if (resolutions == null || resolutions.Count == 0) return 0;

            if (targetWidth <= 0 || targetHeight <= 0)
            {
                targetWidth = Screen.currentResolution.width > 0 ? Screen.currentResolution.width : Screen.width;
                targetHeight = Screen.currentResolution.height > 0 ? Screen.currentResolution.height : Screen.height;
            }

            if (targetWidth <= 0 || targetHeight <= 0)
            {
                targetWidth = 1920;
                targetHeight = 1080;
            }

            // 1. 완전 일치 (width && height) 검색
            for (int i = 0; i < resolutions.Count; i++)
            {
                if (resolutions[i].width == targetWidth && resolutions[i].height == targetHeight)
                {
                    return i;
                }
            }

            // 2. 화면비(Aspect Ratio) 및 픽셀 차이 기반 가장 가까운 해상도 검색
            float targetAspect = (float)targetWidth / targetHeight;
            int bestIndex = 0;
            float minDifference = float.MaxValue;

            for (int i = 0; i < resolutions.Count; i++)
            {
                var r = resolutions[i];
                float rAspect = (float)r.width / r.height;
                float aspectDiff = Mathf.Abs(rAspect - targetAspect);

                // 가로/세로 픽셀 차이
                float pixelDiff = Mathf.Abs(r.width - targetWidth) + Mathf.Abs(r.height - targetHeight);

                // 화면비가 다르면 큰 패널티 (동일 비율 우선)
                float score = pixelDiff + (aspectDiff > 0.05f ? 100000f : 0f);

                if (score < minDifference)
                {
                    minDifference = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        /// <summary>
        /// 모니터 주사율에 가장 가까운 FPS 제한 인덱스를 찾습니다.
        /// </summary>
        public int FindClosestFpsLimitIndex(double refreshRate)
        {
            if (fpsLimitSetting == null || fpsLimitSetting.OptionsCount == 0) return 0;

            int targetFps = Mathf.RoundToInt((float)refreshRate);
            if (targetFps <= 0) targetFps = 60;

            int bestIndex = fpsLimitSetting.OptionsCount - 1; // Default: Unlimited
            int minDiff = int.MaxValue;

            // Unlimited를 제외한 숫자 옵션들과 비교
            for (int i = 0; i < fpsLimitSetting.OptionsCount - 1; i++)
            {
                string text = fpsLimitSetting.GetOptionText(i).ToString();
                if (int.TryParse(text, out int fps))
                {
                    int diff = Mathf.Abs(fps - targetFps);
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                        bestIndex = i;
                    }
                }
            }

            // 모니터 주사율과 5Hz 이내로 일치하는 프리셋이 있으면 해당 프리셋 선택
            if (minDiff <= 5)
            {
                return bestIndex;
            }

            // 그 외에는 Unlimited 선택
            return fpsLimitSetting.OptionsCount - 1;
        }

        /// <summary>
        /// 기본 마우스 감도(1.0f)를 반환합니다.
        /// </summary>
        public float CalculateDefaultMouseSensitivity()
        {
            return 1.0f;
        }

        private void SetDefaultValues()
        {
            // 1. 해상도: 모니터 현재 해상도에 가장 가까운 값
            settingData.resolutionIndex = FindClosestResolutionIndex(Screen.currentResolution.width, Screen.currentResolution.height);

            // 2. 화면 모드: 전체화면 (0)
            settingData.screenModeIndex = 0;

            // 3. FPS 제한: 모니터 주사율에 가장 가까운 값
            settingData.fpsLimitIndex = FindClosestFpsLimitIndex(Screen.currentResolution.refreshRateRatio.value);

            // 4. VSync: false (반응성 우선)
            settingData.vSync = false;

            // 5. FOV: 60
            settingData.fov = 60f;

            // 6. Y축 반전: false
            settingData.invertY = false;

            // 7. 마우스 감도: 기본값 1.0f
            settingData.mouseSensitivity = 1.0f;
        }

        /// <summary>
        /// 게임플레이 설정을 모니터 최적값 및 기본값으로 초기화하고 즉시 화면 및 설정에 반영합니다.
        /// </summary>
        public void ResetToDefault(SettingData targetData = null)
        {
            if (targetData != null)
            {
                settingData = targetData;
            }

            if (settingData == null) return;

            // 해상도 확인 팝업이 떠있었다면 닫기
            checkPanel?.Dismiss();

            // 기본값 계산 및 설정
            SetDefaultValues();

            // UI 갱신 (확인 팝업이나 중복 콜백 방지를 위해 SetValueWithoutNotify 사용)
            resoulutionSetting.SetValueWithoutNotify(settingData.resolutionIndex);
            screenModeSetting.SetValueWithoutNotify(settingData.screenModeIndex);
            fpsLimitSetting.SetValueWithoutNotify(settingData.fpsLimitIndex);
            vSyncSetting.SetValueWithoutNotify(settingData.vSync);
            languageSetting.SetValueWithoutNotify((int)LocalizationManager.Instance.currentLanguage);
            fovSetting.SetValueWithoutNotify(settingData.fov);
            invertYSetting.SetValueWithoutNotify(settingData.invertY);
            mouseSensitivitySetting.SetValueWithoutNotify(settingData.mouseSensitivity);

            // 실제 엔진/화면에 직접 적용
            ApplySavedSettings();
        }

        private void ApplySavedSettings()
        {
            // 1. 화면 모드 적용
            if (settingData.screenModeIndex < 0 || settingData.screenModeIndex > 2)
            {
                settingData.screenModeIndex = 0; // Default: FullScreen
            }
            OnScreenModeChanged(settingData.screenModeIndex);

            // 2. 해상도 적용 (유효성 검사 후 다이렉트 적용)
            if (settingData.resolutionIndex < 0 || settingData.resolutionIndex >= resolutions.Count)
            {
                settingData.resolutionIndex = FindClosestResolutionIndex(Screen.currentResolution.width, Screen.currentResolution.height);
            }
            SetResolutionDirect(settingData.resolutionIndex);

            // 3. FPS 제한 적용
            if (settingData.fpsLimitIndex < 0 || settingData.fpsLimitIndex >= fpsLimitSetting.OptionsCount)
            {
                settingData.fpsLimitIndex = FindClosestFpsLimitIndex(Screen.currentResolution.refreshRateRatio.value);
            }
            ApplyFpsLimit(settingData.fpsLimitIndex);

            // 4. VSync 적용
            OnVSyncChanged(settingData.vSync);
        }

        private void OnResolutionChanged(int index)
        {
            int prevIndex = settingData.resolutionIndex;
            SetResolutionDirect(index);

            checkPanel.ShowWithTimeout(
                checkResolutionMessage,
                null,
                () =>
                {
                    // 취소 시 이전 해상도로 되돌리기
                    SetResolutionDirect(prevIndex);
                    resoulutionSetting.SetValueWithoutNotify(prevIndex);
                },
                15 // 15초 타임아웃
            );
        }

        private void SetResolutionDirect(int index)
        {
            if (index < 0 || index >= resolutions.Count) return;
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
            ApplyFpsLimit(index);
        }

        private void ApplyFpsLimit(int index)
        {
            settingData.fpsLimitIndex = index;
            if (index == fpsLimitSetting.OptionsCount - 1) // "Unlimited" 선택 시
            {
                Application.targetFrameRate = -1; // FPS 제한 해제
            }
            else
            {
                string text = fpsLimitSetting.GetOptionText(index).ToString();
                if (int.TryParse(text, out int fps))
                {
                    Application.targetFrameRate = fps;
                }
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