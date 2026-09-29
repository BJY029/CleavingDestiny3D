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