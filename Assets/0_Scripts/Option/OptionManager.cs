using UnityEngine.InputSystem;
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

        internal SettingData settingData;
        // 설정 데이터가 최초 생성인지 저장 데이터 로드인지 구분하기 위한 변수
        internal bool isInitialized = false;

        private GameObject optionMenu;

        [SerializeField] private CategorySwapper categorySwapper;
        [SerializeField] private GamePlaySetting gamePlaySetting;
        [SerializeField] private SoundSetting soundSetting;

        private string settingPath;
        private Action _closeAction;

        private void Awake()
        {
            _closeAction = () => SetOptionMenu(false);

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
            KeyInteractManager.Instance?.RemoveMenuAction(_closeAction);
        }

        private void Start()
        {
            optionMenu = transform.GetChild(0).gameObject;
            optionMenu.SetActive(false);
            categorySwapper.SetInitialCategory(0);

            gamePlaySetting.Initialize();
            soundSetting.Initialize();
        }
        
        private void Update()
        {
            // KeyInteractManager가 없는 씬(예: LobbyScene)에서 옵션창이 활성화되어 있을 때 ESC로 닫기 지원
            if (KeyInteractManager.Instance == null && IsOptionMenuActive())
            {
                if (WasEscapePressed())
                {
                    SetOptionMenu(false);
                }
            }
        }

        private bool WasEscapePressed()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                return true;

            if (Input.GetKeyDown(KeyCode.Escape))
                return true;

            return false;
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
                KeyInteractManager.Instance?.PushMenuAction(_closeAction);
            }
            else
            {
                KeyInteractManager.Instance?.RemoveMenuAction(_closeAction);
                AudioManager.Instance?.PlaySfx2D("ui_button");
                SaveSetting().Forget();
            }
        }

        private async UniTask SaveSetting()
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