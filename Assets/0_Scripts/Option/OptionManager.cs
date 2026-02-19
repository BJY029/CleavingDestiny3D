using System.IO;
using Option.Element;
using UnityEngine;

namespace Option
{
    public class OptionManager : MonoBehaviour
    {
        public static OptionManager Instance { get; private set; }

        internal SettingData settingData;

        GameObject optionMenu;

        [SerializeField] CategorySwapper categorySwapper;

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
            categorySwapper.SwapCategory(0);
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

        void SaveSetting()
        {
            string json = JsonUtility.ToJson(settingData);
            File.WriteAllText(Application.persistentDataPath + "/setting.json", json);
        }

        void LoadSetting()
        {
            string path = Application.persistentDataPath + "/setting.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                settingData = JsonUtility.FromJson<SettingData>(json);
            }
            else
            {
                settingData = new SettingData(); // 기본 설정으로 초기화
            }
        }
    }
}