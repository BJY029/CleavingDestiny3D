using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using DigitalRuby.WeatherMaker;
using System.Collections;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public static GameStarter instance;

    [SerializeField] private float mapIntroductionDuration = 7f;

    [Header("Turn 정하기")]
    [SerializeField] private StickGameController stickGameController;
    [SerializeField] private float turnResultDisplayDuration = 3f;

    [Header("실제 Game")]
    [Tooltip("턴 정하기가 끝나기 전까지 비활성화할 게임 시스템")]
    [SerializeField] private GameObject mainGameCanvas;

    [Header("Game Theme")]
    [SerializeField] private GameThemeCatalogSO themeCatalog;
    [SerializeField] WeatherMakerWeatherZoneScript globalWeatherZone;
    private GameThemeSO currentGameTheme;
    private Coroutine applyThemeCor;

    private GameStartPhase currentPhase = GameStartPhase.None;
    public GameStartPhase CurrentPhase => currentPhase;
    private double phaseStartTime;
    private double PhaseDuration;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private IEnumerator Start()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) yield break;

        bool hasTheme = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GameStartRoomKeys.ThemeID);
        bool hasPhase = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GameStartRoomKeys.Phase);

        if (hasTheme) yield return StartCoroutine(ApplyCurrentRoomThemeRoutine());
        if (hasPhase) ApplyCurrentRoomPhase();

        if (!PhotonNetwork.IsMasterClient) yield break;

        if (!hasTheme || !hasPhase) InitializeGameStart();
    }

    private void InitializeGameStart()
    {
        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null) return;

        if (themeCatalog == null) return;
        GameThemeSO selectedTheme = themeCatalog.GetRandomTheme();

        if (selectedTheme == null) return;

        double now = PhotonNetwork.Time;
        ExitGames.Client.Photon.Hashtable props = new()
        {
            {GameStartRoomKeys.ThemeID, selectedTheme.ThemeId},
            {GameStartRoomKeys.Phase, (byte)GameStartPhase.MapIntroduction},
            {GameStartRoomKeys.PhaseStartTime, now},
            {GameStartRoomKeys.PhaseDuration, (double)mapIntroductionDuration},
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private void Update()
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;

        double now = PhotonNetwork.Time;

        switch (currentPhase)
        {
            case GameStartPhase.MapIntroduction:
                if (now >= phaseStartTime + PhaseDuration)
                {
                    SetPhase(GameStartPhase.TurnSelection);
                }
                break;
            case GameStartPhase.TurnSelection:
                CheckTurnSelectionFinished(now);
                break;
            case GameStartPhase.TurnResult:
                if (now >= phaseStartTime + PhaseDuration)
                    SetPhase(GameStartPhase.MainGame);
                break;
        }
    }

    private void SetPhase(GameStartPhase phase, double duration = 0d)
    {
        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null) return;

        ExitGames.Client.Photon.Hashtable properties = new()
        {
            {GameStartRoomKeys.Phase, (byte)phase},
            {GameStartRoomKeys.PhaseStartTime, PhotonNetwork.Time},
            {GameStartRoomKeys.PhaseDuration, duration}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        bool themeChanged = propertiesThatChanged.ContainsKey(GameStartRoomKeys.ThemeID);
        bool phaseChanged = propertiesThatChanged.ContainsKey(GameStartRoomKeys.Phase);

        if (themeChanged)
        {
            if (applyThemeCor != null) StopCoroutine(applyThemeCor);
            applyThemeCor = StartCoroutine(ApplyThemeThenPhaseRoutine(phaseChanged));

            return;
        }

        if (phaseChanged) ApplyCurrentRoomPhase();
    }

    private void ApplyCurrentRoomPhase()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) return;

        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        if (!properties.TryGetValue(GameStartRoomKeys.Phase, out object phaseValue)) return;

        currentPhase = (GameStartPhase)Convert.ToByte(phaseValue);

        if (properties.TryGetValue(GameStartRoomKeys.PhaseStartTime, out object startValue))
            phaseStartTime = Convert.ToDouble(startValue);

        if (properties.TryGetValue(GameStartRoomKeys.PhaseDuration, out object durationValue))
            PhaseDuration = Convert.ToDouble(durationValue);

        ApplyPhase();
    }

    private IEnumerator ApplyCurrentRoomThemeRoutine()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) yield break;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameStartRoomKeys.ThemeID, out object themeIDValue))
            yield break;

        string themeId = themeIDValue as string;

        if (string.IsNullOrWhiteSpace(themeId))
        {
            Debug.LogError("Theme ID is NULL");
            yield break;
        }

        if (themeCatalog == null || !themeCatalog.TryGetTheme(themeId, out GameThemeSO theme))
        {
            Debug.LogError("Can't find theme info");
            yield break;
        }

        currentGameTheme = theme;

        ApplyGameSettings(theme);

        yield return StartCoroutine(ApplyWeatherThemeRoutine(theme));
    }

    private IEnumerator ApplyThemeThenPhaseRoutine(bool applyPhaseAfterTheme)
    {
        yield return StartCoroutine(ApplyCurrentRoomThemeRoutine());

        if (applyPhaseAfterTheme) ApplyCurrentRoomPhase();

        applyThemeCor = null;
    }

    private void ApplyGameSettings(GameThemeSO theme)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[GameStarter] GameManager가 없습니다.");
            return;
        }

        if (theme.PlayerData == null)
        {
            Debug.LogError($"[{theme.name}] PlayerData가 없습니다.");
            return;
        }

        if (theme.RoomData == null)
        {
            Debug.LogError($"[{theme.name}] RoomData가 없습니다.");
            return;
        }

        GameManager.Instance.playerDefaultSetting = theme.PlayerData;
        GameManager.Instance.roomDefaultSetting = theme.RoomData;
    }

    private void ApplyWeatherTheme(GameThemeSO theme)
    {
        if (applyThemeCor != null) StopCoroutine(applyThemeCor);

        applyThemeCor = StartCoroutine(ApplyWeatherThemeRoutine(theme));
    }

    private IEnumerator ApplyWeatherThemeRoutine(GameThemeSO theme)
    {
        float timeout = 10f;
        float elapsed = 0f;

        while (WeatherMakerScript.Instance == null ||
            WeatherMakerDayNightCycleManagerScript.Instance == null ||
            globalWeatherZone == null)
        {
            elapsed += Time.unscaledDeltaTime;

            if (elapsed >= timeout)
            {
                Debug.LogError("[GameStarter] Weather Maker 초기화 대기 초과");
                applyThemeCor = null;
                yield break;
            }

            yield return null;
        }

        if (theme.WeatherProfile == null)
        {
            Debug.LogError($"[{theme.name}] Weather Profile이 없습니다.");
            applyThemeCor = null;
            yield break;
        }

        WeatherMakerScript weatherMaker = WeatherMakerScript.Instance;
        WeatherMakerProfileScript oldProfile = weatherMaker.LastLocalProfile;
        WeatherMakerProfileScript newProfile = theme.WeatherProfile;

        globalWeatherZone.gameObject.SetActive(true);

        globalWeatherZone.SingleProfile = newProfile;

        weatherMaker.RaiseWeatherProfileChanged(oldProfile, newProfile, 0f, 0f, true, null);

        yield return null;
        yield return new WaitForEndOfFrame();

        applyThemeCor = null;
    }

    private void ApplyPhase()
    {
        switch (currentPhase)
        {
            case GameStartPhase.MapIntroduction:
                EnterMapIntroduction();
                AudioManager.Instance.PlaySfx2D("Map_Intro");
                break;
            case GameStartPhase.TurnSelection:
                EnterTurnSelection();
                break;
            case GameStartPhase.PlayerPreparation:
                EnterPlayerPreparation();
                break;
            case GameStartPhase.TurnResult:
                AudioManager.Instance.PlaySfx2D("Turn_Result");
                EnterTurnResult();
                break;
            case GameStartPhase.MainGame:
                AudioManager.Instance.PlayBgmFadeIn("Game_Default_BGM", 2f);
                EnterMainGame();
                break;
        }
    }

    private void EnterMapIntroduction()
    {
        CameraSwitchManager.Instance.MainCameraOn();
        mainGameCanvas.transform.localScale = Vector3.zero;

        if (stickGameController != null)
        {
            stickGameController.EndStickGame();
        }

        if (currentGameTheme == null)
        {
            Debug.LogError("[GameStarter] 현재 GameThemeSO가 없습니다. Clear로 강제 설정합니다.");
            GamePrepareCanvasController.instance.SetPrepareCanvasAsWeather(GameTheme.Clear);
            return;
        }

        GamePrepareCanvasController.instance.SetPrepareCanvasAsWeather(currentGameTheme.ThemeType);
    }

    private void EnterTurnSelection()
    {
        //TODO : 캔버스 끄기
        GamePrepareCanvasController.instance.SetUnActive();
        CameraSwitchManager.Instance.BranchCameraOn();

        if (stickGameController == null)
        {
            Debug.LogError("[GameStartFlowController] StickGameController가 없습니다.");
            return;
        }

        stickGameController.BeginStickGame();
    }

    private void EnterPlayerPreparation()
    {
        if (PlayerManager.Instance == null) return;

        PlayerManager.Instance.BeginPrepareStartGame();
    }

    private void EnterTurnResult()
    {
        if (GamePrepareCanvasController.instance == null) return;

        int firstActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
        bool isFirstTurn = PhotonNetwork.LocalPlayer.ActorNumber == firstActor;


        CameraSwitchManager.Instance.MainCameraOn();
        GamePrepareCanvasController.instance.ShowBranchGameResult(isFirstTurn);
    }

    private void EnterMainGame()
    {
        if (GamePrepareCanvasController.instance != null) GamePrepareCanvasController.instance.SetUnActive();

        mainGameCanvas.transform.localScale = Vector3.one;
        CameraSwitchManager.Instance.PlayerCameraOn();
        CameraSwitchManager.Instance.GameCameraToggle(false);
        TimeManager.instance.StartTurnTimer();
    }

    private void CheckTurnSelectionFinished(double now)
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (!props.TryGetValue(StickGameRoomKeys.SelectionResolved, out object resolvedValue)
        || !(bool)resolvedValue) return;

        if (!props.TryGetValue(StickGameRoomKeys.SelectionResolvedTime, out object resolvedTimeValue))
            return;

        double resolvedTime = System.Convert.ToDouble(resolvedTimeValue);

        if (now >= resolvedTime + turnResultDisplayDuration)
            SetPhase(GameStartPhase.PlayerPreparation);
    }
}
