using System;
using System.Collections.Generic;
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

    private AudioManager _audioManager;
    private bool _started;

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
        if (instance != this) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        StoryCreditController.PlaybackStateChanged += RefreshCurrentSceneBgm;
        if (_started)
            RefreshCurrentSceneBgm();
    }

    private void Start()
    {
        _started = true;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StoryCreditController.PlaybackStateChanged -= RefreshCurrentSceneBgm;
        if (_audioManager != null)
            _audioManager.OnAudioDataLoaded -= RefreshCurrentSceneBgm;
        _audioManager = null;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void RefreshCurrentSceneBgm()
    {
        if (instance != this || !isActiveAndEnabled) return;

        var audioManager = AudioManager.Instance;
        if (_audioManager != audioManager)
        {
            if (_audioManager != null)
                _audioManager.OnAudioDataLoaded -= RefreshCurrentSceneBgm;
            _audioManager = audioManager;
            if (_audioManager != null)
                _audioManager.OnAudioDataLoaded += RefreshCurrentSceneBgm;
        }

        PlayBGMForScene(SceneManager.GetActiveScene());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        RefreshCurrentSceneBgm();
        var sceneBgm = FindSceneBGMData(SceneManager.GetActiveScene().name);
        if (sceneBgm == null || string.IsNullOrWhiteSpace(sceneBgm.bgmID))
            HandleUnregisteredScene();
    }

    private void PlayBGMForScene(Scene scene)
    {
        var story = StoryCreditController.instance;
        if (story != null && story.gameObject.scene == scene && story.isActiveAndEnabled
            && story.IsBlockingSceneBgm)
            return;

        SceneBGMData sceneBGMData = FindSceneBGMData(scene.name);

        if (sceneBGMData == null || string.IsNullOrWhiteSpace(sceneBGMData.bgmID))
            return;

        if (_audioManager == null || _audioManager.IsBgmPlaying(sceneBGMData.bgmID))
            return;

        _audioManager.PlayBgm(sceneBGMData.bgmID);
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

        if (_audioManager != null)
            _audioManager.StopBgm();
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

