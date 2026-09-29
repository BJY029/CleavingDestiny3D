using Option.Element;
using UnityEngine;
using UnityEngine.Audio;

namespace Option
{
    public class SoundSetting : MonoBehaviour
    {
        public enum SoundType { Master, SFX, BGM, Environment }

        private const float MinVolume = 0.0001f;
        private const float MaxVolume = 1.5f;

        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private SliderWithToggleSetting masterVolumeSetting;
        [SerializeField] private SliderWithToggleSetting sfxVolumeSetting;
        [SerializeField] private SliderWithToggleSetting musicVolumeSetting;
        [SerializeField] private SliderWithToggleSetting environmentVolumeSetting;

        private SettingData settingData;

        public void Initialize(SettingData newSettingData)
        {
            settingData = newSettingData;

            masterVolumeSetting.SetMinMax(MinVolume, MaxVolume);
            sfxVolumeSetting.SetMinMax(MinVolume, MaxVolume);
            musicVolumeSetting.SetMinMax(MinVolume, MaxVolume);
            environmentVolumeSetting.SetMinMax(MinVolume, MaxVolume);

            // 볼륨 슬라이더 리스너 등록
            masterVolumeSetting.AddListener(value => SetVolume(SoundType.Master, value));
            sfxVolumeSetting.AddListener(value => SetVolume(SoundType.SFX, value));
            musicVolumeSetting.AddListener(value => SetVolume(SoundType.BGM, value));
            environmentVolumeSetting.AddListener(value => SetVolume(SoundType.Environment, value));

            // 토글 리스너 등록 (음소거 상태 저장 및 오디오 믹서 적용)
            masterVolumeSetting.AddToggleListener(isOn => SetSoundEnabled(SoundType.Master, isOn));
            sfxVolumeSetting.AddToggleListener(isOn => SetSoundEnabled(SoundType.SFX, isOn));
            musicVolumeSetting.AddToggleListener(isOn => SetSoundEnabled(SoundType.BGM, isOn));
            environmentVolumeSetting.AddToggleListener(isOn => SetSoundEnabled(SoundType.Environment, isOn));

            // 초기 토글 상태 설정
            masterVolumeSetting.SetToggleValue(settingData.isMasterEnabled);
            sfxVolumeSetting.SetToggleValue(settingData.isSfxEnabled);
            musicVolumeSetting.SetToggleValue(settingData.isBGMEnabled);
            environmentVolumeSetting.SetToggleValue(settingData.isEnvironmentEnabled);

            // 초기 볼륨 슬라이더 값 설정
            masterVolumeSetting.SetValue(settingData.masterVolume);
            sfxVolumeSetting.SetValue(settingData.sfxVolume);
            musicVolumeSetting.SetValue(settingData.bgmVolume);
            environmentVolumeSetting.SetValue(settingData.environmentVolume);

            // 초기 볼륨 믹서 적용 (토글이 활성화되어 있으면 저장된 볼륨, 비활성화(음소거)면 MinVolume)
            ApplyMixerVolume(SoundType.Master, settingData.isMasterEnabled ? settingData.masterVolume : MinVolume);
            ApplyMixerVolume(SoundType.SFX, settingData.isSfxEnabled ? settingData.sfxVolume : MinVolume);
            ApplyMixerVolume(SoundType.BGM, settingData.isBGMEnabled ? settingData.bgmVolume : MinVolume);
            ApplyMixerVolume(SoundType.Environment, settingData.isEnvironmentEnabled ? settingData.environmentVolume : MinVolume);
        }

        public void SetSoundEnabled(SoundType type, bool isEnabled)
        {
            switch (type)
            {
                case SoundType.Master:
                    settingData.isMasterEnabled = isEnabled;
                    ApplyMixerVolume(SoundType.Master, isEnabled ? settingData.masterVolume : MinVolume);
                    break;
                case SoundType.SFX:
                    settingData.isSfxEnabled = isEnabled;
                    ApplyMixerVolume(SoundType.SFX, isEnabled ? settingData.sfxVolume : MinVolume);
                    break;
                case SoundType.BGM:
                    settingData.isBGMEnabled = isEnabled;
                    ApplyMixerVolume(SoundType.BGM, isEnabled ? settingData.bgmVolume : MinVolume);
                    break;
                case SoundType.Environment:
                    settingData.isEnvironmentEnabled = isEnabled;
                    ApplyMixerVolume(SoundType.Environment, isEnabled ? settingData.environmentVolume : MinVolume);
                    break;
            }
        }

        public void SetVolume(SoundType type, float value)
        {
            value = Mathf.Clamp(value, MinVolume, MaxVolume);

            switch (type)
            {
                case SoundType.Master:
                    settingData.masterVolume = value;
                    if (settingData.isMasterEnabled)
                        ApplyMixerVolume(SoundType.Master, value);
                    break;
                case SoundType.SFX:
                    settingData.sfxVolume = value;
                    if (settingData.isSfxEnabled)
                        ApplyMixerVolume(SoundType.SFX, value);
                    break;
                case SoundType.BGM:
                    settingData.bgmVolume = value;
                    if (settingData.isBGMEnabled)
                        ApplyMixerVolume(SoundType.BGM, value);
                    break;
                case SoundType.Environment:
                    settingData.environmentVolume = value;
                    if (settingData.isEnvironmentEnabled)
                        ApplyMixerVolume(SoundType.Environment, value);
                    break;
            }
        }

        private void ApplyMixerVolume(SoundType type, float value)
        {
            value = Mathf.Clamp(value, MinVolume, MaxVolume);
            float logVolume = Mathf.Log10(value) * 20f;

            switch (type)
            {
                case SoundType.Master:
                    SetMixerVolume("Master", logVolume);
                    SetWeatherMakerMasterVolume(value);
                    break;
                case SoundType.SFX:
                    SetMixerVolume("SFX", logVolume);
                    break;
                case SoundType.BGM:
                    SetMixerVolume("BGM", logVolume);
                    break;
                case SoundType.Environment:
                    SetMixerVolume("Environment", logVolume);
                    SetWeatherMakerEnvVolume(value);
                    break;
            }
        }

        private void SetMixerVolume(string parameterName, float logVolume)
        {
            if (audioMixer == null)
            {
                Debug.LogError("[SoundSetting] AudioMixer가 할당되지 않았습니다.", this);
                return;
            }

            if (!audioMixer.SetFloat(parameterName, logVolume))
            {
                Debug.LogWarning($"[SoundSetting] 노출된 AudioMixer 매개변수를 찾을 수 없습니다: {parameterName}");
            }
        }

        private void SetWeatherMakerMasterVolume(float volume)
        {
            if (!WeatherMakerAudioBridge.HasInstance) return;

            WeatherMakerAudioBridge.instance.SetMasterVolume(volume);
        }

        private void SetWeatherMakerEnvVolume(float volume)
        {
            if (!WeatherMakerAudioBridge.HasInstance) return;

            WeatherMakerAudioBridge.instance.SetEnvironmentVolume(volume);
        }
    }
}