using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static bool isInitialized;
    private const string SaveFileName = "player_save.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static void Initialize()
    {
        if (isInitialized) return;

        PlayerSaveData saveData = Load();

        PlayerProfile.Initialize(saveData);

        isInitialized = true;
    }

    public static void Save()
    {
        PlayerSaveData saveData = PlayerProfile.CreateSaveData();

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveManager] 저장 완료 : {SavePath}");
    }

    public static PlayerSaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveManager] 저장 파일 없음. 기본 데이터를 생성합니다.");

            PlayerSaveData defaultData = CreateDefaultData();
            SaveData(defaultData);

            return defaultData;
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);

            if (saveData == null) throw new System.Exception("SaveData is Null");

            return saveData;
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"[SaveManager] 저장 데이터 로드 실패 : {exception.Message}");

            return CreateDefaultData();
        }
    }

    private static PlayerSaveData CreateDefaultData()
    {
        return new PlayerSaveData
        {
            branchCount = 0,
            ownedAxeSkinIdx = new() { "axe_basic" },
            equippedAxeSkinId = "axe_basic"
        };
    }

    private static void SaveData(PlayerSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveManager] 기본 저장 데이터 생성 완료 : {SavePath}");
    }
}
