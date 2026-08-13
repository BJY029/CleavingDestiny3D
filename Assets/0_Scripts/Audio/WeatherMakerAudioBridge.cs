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

        _weatherMakerAudioManager.VolumeModifier = masterVolume;
        _weatherMakerAudioManager.WeatherVolumeModifier = environmentVolume;
        _weatherMakerAudioManager.AmbientVolumeModifier = useWeatherMakerAmbientSound ? environmentVolume : 0f;
    }

    private void LoadVolumesFromOption()
    {
        if (OptionManager.Instance == null || OptionManager.Instance.settingData == null) return;

        SettingData data = OptionManager.Instance.settingData;

        masterVolume = Mathf.Clamp(data.masterVolume, MinVolume, MaxVolume);
        environmentVolume = Mathf.Clamp(data.environmentVolume, MinVolume, MaxVolume);
    }
}
