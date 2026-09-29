using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Option.Element;
using Potan.CoreUtils;
using UnityEngine;

namespace Option
{
    public class OptionManager : MonoBehaviour
    {
        public static OptionManager Instance { get; private set; }

        public SettingData settingData;
        // 설정 데이터가 최초 생성인지 저장 데이터 로드인지 구분하기 위한 변수
        internal bool isInitialized = false;

        private GameObject optionMenu;

        [SerializeField] private CategorySwapper categorySwapper;
        [SerializeField] private GamePlaySetting gamePlaySetting;
        [SerializeField] private SoundSetting soundSetting;
        [SerializeField] private KeyBindOption keyBindOption;
        public KeyBindOption KeyBindOption
        {
            get => keyBindOption;
            set => keyBindOption = value;
        }

        private string settingPath;
        private Action closeAction;

        private void Awake()
        {
            closeAction = () => SetOptionMenu(false);

            if (Instance == null || Instance == this)
            {
                Instance = this;
                settingPath = Path.Join(Application.persistentDataPath, "setting.json");
                LoadSetting();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            KeyInteractManager.Instance?.RemoveMenuAction(closeAction);
        }

        private void Start()
        {
            optionMenu = transform.GetChild(0).gameObject;
            optionMenu.SetActive(false);
            categorySwapper.SetInitialCategory(0);

            if (keyBindOption == null)
            {
                keyBindOption = GetComponentInChildren<KeyBindOption>(true);
            }

            gamePlaySetting.Initialize(settingData);
            soundSetting.Initialize(settingData);
            if (keyBindOption != null)
            {
                keyBindOption.Initialize(settingData);
            }
        }

        public bool IsOptionMenuActive()
        {
            return optionMenu.activeSelf;
        }
        
        /// <summary>
        /// 옵션 메뉴 활성화/비활성화
        /// </summary>
        /// <param name="isActive"></param>
        public void SetOptionMenu(bool isActive)
        {
            optionMenu.SetActive(isActive);

            if (isActive)
            {
                KeyInteractManager.Instance?.PushMenuAction(closeAction);
            }
            else
            {
                KeyInteractManager.Instance?.RemoveMenuAction(closeAction);
                AudioManager.Instance?.PlaySfx2D("ui_button");
                SaveSetting().Forget();
            }
        }

        public async UniTask SaveSetting()
        {
            string json = JsonUtility.ToJson(settingData);
            await File.WriteAllTextAsync(settingPath, json);
            DevLog.Log($"Settings saved to: {settingPath}", this);
        }

        /// <summary>
        /// 모든 키 바인딩 설정을 기본값으로 초기화하고 저장합니다.
        /// 버튼 OnClick 이벤트에 연결하여 사용할 수 있습니다.
        /// </summary>
        public void ResetKeyBinds()
        {
            if (keyBindOption != null)
            {
                keyBindOption.ResetToDefault();
            }
            else
            {
                KeyInteractManager.Instance?.ResetAllBindingOverrides();
                if (settingData != null)
                {
                    settingData.keyRebinds = string.Empty;
                }
                KeyInteractManager.NotifyKeyBindingsChanged();
                SaveSetting().Forget();
            }
        }

        /// <summary>
        /// 게임플레이, 사운드, 키 바인딩을 포함한 모든 옵션 설정을 모니터 최적값 및 기본값으로 초기화하고 저장합니다.
        /// 버튼 OnClick 이벤트에 연결하여 사용할 수 있습니다.
        /// </summary>
        public void ResetAllSettings()
        {
            if (settingData == null)
            {
                settingData = new SettingData();
            }

            // 1. 게임플레이 설정 초기화 (모니터 최적 해상도, 주사율, 비례 마우스 감도 등)
            gamePlaySetting?.ResetToDefault(settingData);

            // 2. 사운드 설정 초기화 (마스터/SFX 0.8, BGM/Env 0.6, 모두 켜짐)
            soundSetting?.ResetToDefault(settingData);

            // 3. 키 바인딩 초기화
            if (keyBindOption != null)
            {
                keyBindOption.ResetToDefault();
            }
            else
            {
                KeyInteractManager.Instance?.ResetAllBindingOverrides();
                settingData.keyRebinds = string.Empty;
                KeyInteractManager.NotifyKeyBindingsChanged();
            }

            SaveSetting().Forget();
            DevLog.Log("All settings have been reset to default and saved.", this);
        }

        /// <summary>
        /// ResetAllSettings()의 별칭 메서드입니다.
        /// </summary>
        public void ResetToDefault() => ResetAllSettings();

        /// <summary>
        /// 게임플레이 설정(해상도, 모니터 주사율, 마우스 감도 등)을 모니터 최적값 및 기본값으로 초기화하고 저장합니다.
        /// 버튼 OnClick 이벤트에 연결하여 사용할 수 있습니다.
        /// </summary>
        public void ResetGamePlaySettings()
        {
            if (settingData == null)
            {
                settingData = new SettingData();
            }
            gamePlaySetting?.ResetToDefault(settingData);
            SaveSetting().Forget();
            DevLog.Log("GamePlay settings have been reset to monitor optimal defaults.", this);
        }

        /// <summary>
        /// 사운드 설정을 기본값(마스터/SFX 0.8, BGM/Env 0.6, 음소거 해제)으로 초기화하고 저장합니다.
        /// 버튼 OnClick 이벤트에 연결하여 사용할 수 있습니다.
        /// </summary>
        public void ResetSoundSettings()
        {
            if (settingData == null)
            {
                settingData = new SettingData();
            }
            soundSetting?.ResetToDefault(settingData);
            SaveSetting().Forget();
            DevLog.Log("Sound settings have been reset to default.", this);
        }

        private void LoadSetting()
        {
            if (File.Exists(settingPath))
            {
                try
                {
                    string json = File.ReadAllText(settingPath);
                    settingData = JsonUtility.FromJson<SettingData>(json);
                    isInitialized = true;
                }
                catch (Exception e)
                {
                    DevLog.LogError($"Error loading settings: {e.Message}", this);
                    isInitialized = false;
                    settingData = new SettingData(); // 기본 설정으로 초기화
                }
            }
            else
            {
                isInitialized = false;
                settingData = new SettingData(); // 기본 설정으로 초기화

                DevLog.Log("No existing settings found. Initialized with default settings.", this);
            }
        }
    }
}