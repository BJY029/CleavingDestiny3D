using System.IO;
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

        GameObject optionMenu;

        [SerializeField] CategorySwapper categorySwapper;
        [SerializeField] GamePlaySetting gamePlaySetting;
        [SerializeField] SoundSetting soundSetting;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadSetting();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            optionMenu = transform.GetChild(0).gameObject;
            optionMenu.SetActive(false);
            categorySwapper.SwapCategory(0);

            gamePlaySetting.Initialize();
            soundSetting.Initialize();
        }
        
        /// <summary>
        /// 옵션 메뉴 활성화/비활성화
        /// </summary>
        /// <param name="isActive"></param>
        public void SetOptionMenu(bool isActive)
        {
            optionMenu.SetActive(isActive);

            if (!isActive)
            {
                SaveSetting();
            }
        }

        async void SaveSetting()
        {
            string json = JsonUtility.ToJson(settingData);
            string path = Application.persistentDataPath + "/setting.json";
            await File.WriteAllTextAsync(path, json);
            DevLog.Log($"Settings saved to: {path}", this);
        }

        void LoadSetting()
        {
            string path = Application.persistentDataPath + "/setting.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                settingData = JsonUtility.FromJson<SettingData>(json);
                isInitialized = true;
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