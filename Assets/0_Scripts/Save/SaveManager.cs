using System;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    private const string SaveFileName = "player_save.dat";
    private const string BackupFileName = "player_save.bak";
    private const string TempFileName = "player_save.tmp";

    private static string savePath;
    private static string backupPath;
    private static string tempPath;

    private static bool isInitialized;


    public static void Initialize()
    {
        if (isInitialized) return;

        savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        backupPath = Path.Combine(Application.persistentDataPath, BackupFileName);
        tempPath = Path.Combine(Application.persistentDataPath, TempFileName);

        PlayerSaveData saveData = Load();

        PlayerProfile.Initialize(saveData);

        isInitialized = true;
    }

    public static void Save()
    {
        EnsurePathsInitialized();

        PlayerSaveData saveData = PlayerProfile.CreateSaveData();

        if (!PlayerSaveDataValidator.ValidateAndRepair(saveData))
        {
            Debug.LogError("[SaveManager] 저장 데이터 Validation 실패");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(saveData);
            string encryptedData = SaveCrypto.Encrypt(json);

            WriteTempFile(encryptedData);

            BackupCurrentSave();

            ReplaceMainSave();

            Debug.Log($"[SaveManager] 저장 완료 : {savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 저장 실패 : {e}");
        }
    }

    public static PlayerSaveData Load()
    {
        EnsurePathsInitialized();

        if (TryLoadFromPath(savePath, out PlayerSaveData mainData))
        {
            Debug.Log("[SaveManager] Main Save 로드 성공");
            return mainData;
        }

        Debug.LogWarning("[SaveManager] Main Save 로드 실패");

        if (TryLoadFromPath(backupPath, out PlayerSaveData backupData))
        {
            Debug.LogWarning("[SaveManager] Backup Save로 복구");

            RestoreBackupToMain();

            return backupData;
        }

        Debug.LogWarning("[SaveManager] 사용 가능한 Save 없음. 기본 데이터 생성");

        PlayerSaveData defaultData = CreateDefaultData();

        SaveDataDirectly(defaultData);

        return defaultData;
    }

    private static bool TryLoadFromPath(string path, out PlayerSaveData saveData)
    {
        saveData = null;

        if (!File.Exists(path))
            return false;

        try
        {
            string encryptedData = File.ReadAllText(path);

            if (!SaveCrypto.TryDecrypt(encryptedData, out string json)) return false;

            saveData = JsonUtility.FromJson<PlayerSaveData>(json);

            if (!PlayerSaveDataValidator.ValidateAndRepair(saveData))
            {
                saveData = null;
                return false;
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[SaveManager] Save 로드 실패 ({path}) : {e.Message}");

            saveData = null;

            return false;
        }
    }

    private static void WriteTempFile(string encryptedData)
    {
        if (File.Exists(tempPath)) File.Delete(tempPath);
        File.WriteAllText(tempPath, encryptedData);
    }

    private static void BackupCurrentSave()
    {
        if (!File.Exists(savePath)) return;

        if (!TryLoadFromPath(savePath, out _))
        {
            Debug.LogWarning("[SaveManager] 현재 Main Save가 비정상이므로 Backup하지 않습니다.");
            return;
        }

        File.Copy(savePath, backupPath, true);
    }

    private static void ReplaceMainSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);

        File.Move(tempPath, savePath);
    }

    private static void RestoreBackupToMain()
    {
        try
        {
            File.Copy(backupPath, savePath, true);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveManager] Backup 복구 파일 복사 실패 : {e.Message}");
        }
    }

    private static void SaveDataDirectly(PlayerSaveData saveData)
    {
        if (!PlayerSaveDataValidator.ValidateAndRepair(saveData)) return;

        string json = JsonUtility.ToJson(saveData);
        string encryptedData = SaveCrypto.Encrypt(json);

        File.WriteAllText(savePath, encryptedData);
    }

    private static PlayerSaveData CreateDefaultData()
    {
        return new PlayerSaveData
        {
            version = 1,
            branchCount = 0,
            ownedAxeSkinIdx = new() { "axe_basic" },
            equippedAxeSkinId = "axe_basic"
        };
    }

    private static void EnsurePathsInitialized()
    {
        if (!string.IsNullOrWhiteSpace(savePath)) return;

        savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        backupPath = Path.Combine(Application.persistentDataPath, BackupFileName);
        tempPath = Path.Combine(Application.persistentDataPath, TempFileName);
    }

#if UNITY_EDITOR
    public static void ResetSaveForTest()
    {
        EnsurePathsInitialized();

        DeleteIfExists(savePath);
        DeleteIfExists(backupPath);
        DeleteIfExists(tempPath);

        PlayerSaveData defaultData = CreateDefaultData();

        PlayerProfile.Initialize(defaultData);

        SaveDataDirectly(defaultData);

        Debug.Log(
            $"[SaveManager] 테스트 Save 초기화 완료 / " +
            $"Branch={PlayerProfile.BranchCount} / " +
            $"Axe={PlayerProfile.EquippedAxeSkinId}"
        );
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
#endif
}
