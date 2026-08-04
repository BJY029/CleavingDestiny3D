using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBGMManager : MonoBehaviour
{
    [Serializable]
    private class SceneBGMData
    {
        [Tooltip("SceneName")]
        public string sceneName;
        [Tooltip("BGM ID")]
        public string bgmID;
    }

    public static SceneBGMManager instance { get; private set; }

    [Header("Scene BGM Settings")]
    [SerializeField] private List<SceneBGMData> sceneBGMDatas = new();

    [Header("Unregistered Scene")]
    [Tooltip("BGM이 등록되지 않은 씬에서 기존 BGM을 정지할지 여부")]
    [SerializeField] private bool stopBGMInUnregisteredScene = true;

    private string _currentBGMId;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayBGMForScene(SceneManager.GetActiveScene());
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        PlayBGMForScene(scene);
    }

    private void PlayBGMForScene(Scene scene)
    {
        SceneBGMData sceneBGMData = FindSceneBGMData(scene.name);

        if (sceneBGMData == null || string.IsNullOrWhiteSpace(sceneBGMData.bgmID))
        {
            HandleUnregisteredScene();
            return;
        }

        if (string.Equals(_currentBGMId, sceneBGMData.bgmID, StringComparison.Ordinal))
            return;

        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.PlayBgm(sceneBGMData.bgmID);
        _currentBGMId = sceneBGMData.bgmID;
    }

    private SceneBGMData FindSceneBGMData(string sceneName)
    {
        foreach (SceneBGMData data in sceneBGMDatas)
        {
            if (data == null) continue;

            if (string.Equals(data.sceneName, sceneName, StringComparison.Ordinal))
                return data;
        }

        return null;
    }

    private void HandleUnregisteredScene()
    {
        if (!stopBGMInUnregisteredScene) return;

        if (string.IsNullOrEmpty(_currentBGMId)) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopBgm();

        _currentBGMId = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HashSet<string> registeredScenes = new(StringComparer.Ordinal);

        foreach (SceneBGMData data in sceneBGMDatas)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.sceneName))
                continue;

            if (!registeredScenes.Add(data.sceneName))
                Debug.LogWarning($"[SceneBgmManager] 중복된 씬이 등록되어 있습니다: {data.sceneName}", this);
        }
    }
#endif
}

