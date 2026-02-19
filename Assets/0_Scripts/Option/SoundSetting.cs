using Option.Element;
using UnityEngine;

namespace Option
{

    public class SoundSetting : MonoBehaviour
    {
        [SerializeField] SliderWithTextSetting masterVolumeSetting;
        [SerializeField] SliderWithTextSetting sfxVolumeSetting;
        [SerializeField] SliderWithTextSetting musicVolumeSetting;

        void Start()
        {

        }
    }
}