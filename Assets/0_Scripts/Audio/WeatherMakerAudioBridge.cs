using System.Threading;
using Cysharp.Threading.Tasks;
using DigitalRuby.WeatherMaker;
using UnityEngine;
using UnityEngine.SceneManagement;
using Option;

public class WeatherMakerAudioBridge : MonoBehaviour
{
    public static WeatherMakerAudioBridge instance { get; private set; }
    public static bool HasInstance => instance != null;

    private const float MinVolume = 0.0001f;
    private const float MaxVolume = 1.5f;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float environmentVolume = 1f;

    [Header("Weather Maker Ambient")]
    [Tooltip("Weather Maker Sound Zone을 사용하는 경우 활성화합니다.")]
    [SerializeField] private bool useWeatherMakerAmbientSound = true;

    private WeatherMakerAudioManagerScript _weatherMakerAudioManager;
    private CancellationTokenSource _bindCancellationTokenSource;
    private float _weatherAudioFadeMultiplier = 1f;
    private CancellationTokenSource _weatherAudioFadeCancellationTokenSource;

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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        LoadVolumesFromOption();
        RebindWeatherMaker();
    }

    private void OnDestroy()
    {
        CancelBinding();
        CancelWeatherAudioFade();

        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        RebindWeatherMaker();
    }

    private void RebindWeatherMaker()
    {
        CancelBinding();

        _weatherMakerAudioManager = null;

        _bindCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        BindAsync(_bindCancellationTokenSource.Token).Forget();
    }


    private async UniTaskVoid BindAsync(CancellationToken cancellationToken)
    {
        bool canceled = await UniTask
        .WaitUntil(() =>
        WeatherMakerAudioManagerScript.Instance != null, cancellationToken: cancellationToken)
        .SuppressCancellationThrow();

        if (canceled) return;

        _weatherMakerAudioManager = WeatherMakerAudioManagerScript.Instance;

        LoadVolumesFromOption();
        ApplyVolumes();
    }

    private void CancelBinding()
    {
        if (_bindCancellationTokenSource == null) return;

        _bindCancellationTokenSource.Cancel();
        _bindCancellationTokenSource.Dispose();
        _bindCancellationTokenSource = null;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp(volume, MinVolume, MaxVolume);
        ApplyVolumes();
    }

    public void SetEnvironmentVolume(float volume)
    {
        environmentVolume = Mathf.Clamp(volume, MinVolume, MaxVolume);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (_weatherMakerAudioManager == null) return;

        _weatherMakerAudioManager.VolumeModifier = masterVolume * _weatherAudioFadeMultiplier;
        _weatherMakerAudioManager.WeatherVolumeModifier = environmentVolume;
        _weatherMakerAudioManager.AmbientVolumeModifier = useWeatherMakerAmbientSound ? environmentVolume : 0f;
    }

    private void LoadVolumesFromOption()
    {
        if (OptionManager.Instance == null || OptionManager.Instance.settingData == null) return;

        SettingData data = OptionManager.Instance.settingData;

        float targetMaster = data.isMasterEnabled ? data.masterVolume : MinVolume;
        float targetEnv = data.isEnvironmentEnabled ? data.environmentVolume : MinVolume;

        masterVolume = Mathf.Clamp(targetMaster, MinVolume, MaxVolume);
        environmentVolume = Mathf.Clamp(targetEnv, MinVolume, MaxVolume);
    }

    public void MuteWeatherAudio(float fadeDuration = 0.7f)
    {
        //Debug.Log($"[WeatherAudio] 음소거 요청\n{System.Environment.StackTrace}", this);
        StartWeatherAudioFade(0f, fadeDuration);
    }

    public void RestoreWeatherAudio(float fadeDuration = 0.7f)
    {
        _weatherMakerAudioManager = WeatherMakerAudioManagerScript.Instance;
        LoadVolumesFromOption();

        //Debug.Log($"[WeatherAudio] 복원 요청 | Manager={_weatherMakerAudioManager != null}, Master={masterVolume}, Environment={environmentVolume}, Fade={_weatherAudioFadeMultiplier}", this);

        if (_weatherMakerAudioManager == null)
        {
            RebindWeatherMaker();
        }

        StartWeatherAudioFade(1f, fadeDuration);
    }

    private void StartWeatherAudioFade(float targetMultiplier, float duration)
    {
        CancelWeatherAudioFade();

        if (duration <= 0f)
        {
            _weatherAudioFadeMultiplier = targetMultiplier;
            ApplyVolumes();
            return;
        }

        _weatherAudioFadeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

        FadeWeatherAudioAsync(targetMultiplier, duration, _weatherAudioFadeCancellationTokenSource.Token).Forget();
    }

    private async UniTaskVoid FadeWeatherAudioAsync(float targetMultiplier, float duration, CancellationToken cancellationToken)
    {
        float startMultiplier = _weatherAudioFadeMultiplier;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (cancellationToken.IsCancellationRequested) return;

            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsedTime / duration);
            _weatherAudioFadeMultiplier = Mathf.Lerp(startMultiplier, targetMultiplier, progress);

            ApplyVolumes();

            bool canceled = await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();

            if (canceled) return;
        }

        _weatherAudioFadeMultiplier = targetMultiplier;
        ApplyVolumes();

        //Debug.Log($"[WeatherAudio] 페이드 완료 | Target={targetMultiplier}, Manager={_weatherMakerAudioManager != null}, Master={masterVolume}, Environment={environmentVolume}", this);
    }

    private void CancelWeatherAudioFade()
    {
        if (_weatherAudioFadeCancellationTokenSource == null) return;

        _weatherAudioFadeCancellationTokenSource.Cancel();
        _weatherAudioFadeCancellationTokenSource.Dispose();
        _weatherAudioFadeCancellationTokenSource = null;
    }
}
