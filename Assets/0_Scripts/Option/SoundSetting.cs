using Option.Element;
using UnityEngine;
using UnityEngine.Audio;

namespace Option
{

    public class SoundSetting : MonoBehaviour
    {
        public enum SoundType { Master, SFX, BGM }

        [SerializeField] AudioMixer audioMixer;

        [SerializeField] SliderWithToggleSetting masterVolumeSetting;
        [SerializeField] SliderWithToggleSetting sfxVolumeSetting;
        [SerializeField] SliderWithToggleSetting musicVolumeSetting;


        private SettingData SettingData => OptionManager.Instance.settingData;

        public void Initialize()
        {
            masterVolumeSetting.SetMinMax(0.0001f, 1.5f);
            sfxVolumeSetting.SetMinMax(0.0001f, 1.5f);
            musicVolumeSetting.SetMinMax(0.0001f, 1.5f);

            masterVolumeSetting.AddListener(value => SetVolume(SoundType.Master, value));
            sfxVolumeSetting.AddListener(value => SetVolume(SoundType.SFX, value));
            musicVolumeSetting.AddListener(value => SetVolume(SoundType.BGM, value));

            // 초기 볼륨 설정
            // Debug.Log($"Initializing Sound Settings: Master={SettingData.masterVolume}, SFX={SettingData.sfxVolume}, BGM={SettingData.bgmVolume}");
            masterVolumeSetting.SetValue(SettingData.masterVolume);
            sfxVolumeSetting.SetValue(SettingData.sfxVolume);
            musicVolumeSetting.SetValue(SettingData.bgmVolume);
        }

        public void SetVolume(SoundType type, float value)
        {
            // 볼륨을 로그 스케일로 변환
            float logVolume = Mathf.Log10(value) * 20f;
            switch (type)
            {
                case SoundType.Master:
                    audioMixer.SetFloat("Master", logVolume);
                    SettingData.masterVolume = value;
                    break;
                case SoundType.SFX:
                    audioMixer.SetFloat("SFX", logVolume);
                    SettingData.sfxVolume = value;
                    break;
                case SoundType.BGM:
                    audioMixer.SetFloat("BGM", logVolume);
                    SettingData.bgmVolume = value;
                    break;
            }
        }

    }
}