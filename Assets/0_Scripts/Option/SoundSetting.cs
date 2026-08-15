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

        [SerializeField] AudioMixer audioMixer;

        [SerializeField] SliderWithToggleSetting masterVolumeSetting;
        [SerializeField] SliderWithToggleSetting sfxVolumeSetting;
        [SerializeField] SliderWithToggleSetting musicVolumeSetting;
        [SerializeField] SliderWithToggleSetting environmentVolumeSetting;


        private SettingData SettingData => OptionManager.Instance.settingData;

        public void Initialize()
        {
            masterVolumeSetting.SetMinMax(MinVolume, MaxVolume);
            sfxVolumeSetting.SetMinMax(MinVolume, MaxVolume);
            musicVolumeSetting.SetMinMax(MinVolume, MaxVolume);
            environmentVolumeSetting.SetMinMax(MinVolume, MaxVolume);

            masterVolumeSetting.AddListener(value => SetVolume(SoundType.Master, value));
            sfxVolumeSetting.AddListener(value => SetVolume(SoundType.SFX, value));
            musicVolumeSetting.AddListener(value => SetVolume(SoundType.BGM, value));
            environmentVolumeSetting.AddListener(value => SetVolume(SoundType.Environment, value));

            // 초기 볼륨 설정
            // Debug.Log($"Initializing Sound Settings: Master={SettingData.masterVolume}, SFX={SettingData.sfxVolume}, BGM={SettingData.bgmVolume}");
            masterVolumeSetting.SetValue(SettingData.masterVolume);
            sfxVolumeSetting.SetValue(SettingData.sfxVolume);
            musicVolumeSetting.SetValue(SettingData.bgmVolume);
            environmentVolumeSetting.SetValue(SettingData.environmentVolume);
            
            // 볼륨 믹서 설정
            SetVolume(SoundType.Master, SettingData.masterVolume);
            SetVolume(SoundType.SFX, SettingData.sfxVolume);
            SetVolume(SoundType.BGM, SettingData.bgmVolume);
            SetVolume(SoundType.Environment, SettingData.environmentVolume);
        }

        public void SetVolume(SoundType type, float value)
        {
            value = Mathf.Clamp(value, MinVolume, MaxVolume);
            // 볼륨을 로그 스케일로 변환
            float logVolume = Mathf.Log10(value) * 20f;
            switch (type)
            {
                case SoundType.Master:
                    SetMixerVolume("Master", logVolume);
                    SetWeatherMakerMasterVolume(value);

                    SettingData.masterVolume = value;
                    break;
                case SoundType.SFX:
                    SetMixerVolume("SFX", logVolume);

                    SettingData.sfxVolume = value;
                    break;
                case SoundType.BGM:
                    SetMixerVolume("BGM", logVolume);

                    SettingData.bgmVolume = value;
                    break;
                case SoundType.Environment:
                    SetMixerVolume("Environment", logVolume);
                    SetWeatherMakerEnvVolume(value);

                    SettingData.environmentVolume = value;
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
                Debug.LogWarning($"[SoundSetting] 노출된 AudioMixer 매개변수를 찾을 수 없습니다.");
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