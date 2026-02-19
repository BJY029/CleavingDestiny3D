
using UnityEngine;

namespace Option
{
    [System.Serializable]
    public class SettingData
    {
        // GamePlay Settings
        public int resolutionIndex = 0;
        public int screenModeIndex = 0;
        public int fpsLimitIndex = 0;
        public bool vSync = true;
        public int languageIndex = 0;
        public float fov = 60f;
        public bool invertY = false;
        public float mouseSensitivity = 1f;


        // Sound Settings
        public float masterVolume = 0.5f;
        public float sfxVolume = 0.5f;
        public float musicVolume = 0.5f;

        // keybindings
        // TODO
    }
}