
using UnityEngine;

namespace Option
{
    [System.Serializable]
    public class SettingData
    {
        // GamePlay Settings
        public int resolutionIndex = 0;
        public int screenModeIndex = 0;
        public int fpsLimitIndex = -1;
        public bool vSync = false;
        public float fov = 60f;
        public bool invertY = false;
        public float mouseSensitivity = 1f;


        // Sound Settings
        public float masterVolume = 0.8f;
        public float sfxVolume = 0.8f;
        public float bgmVolume = 0.6f;
        public float environmentVolume = 0.6f;
        
        public bool isMasterEnabled = true;
        public bool isSfxEnabled = true;
        public bool isBGMEnabled = true;
        public bool isEnvironmentEnabled = true;

        // keybindings
        public string keyRebinds = "";
    }
}