using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public static GameStarter instance;

    [SerializeField] private float mapIntroductionDuration = 7f;

    [Header("Turn 정하기")]
    [SerializeField] private StickGameController stickGameController;
    [SerializeField] private float turnResultDisplayDuration = 3f;

    [Header("실제 Game")]
    [Tooltip("턴 정하기가 끝나기 전까지 비활성화할 게임 시스템")]
    [SerializeField] private GameObject mainGameSystemRoot;

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

    private void Start()
    {
        ApplyCurrentRoomPhase();

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom
        && !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GameStartRoomKeys.Phase))
            SetPhase(GameStartPhase.MapIntroduction, mapIntroductionDuration);
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
        }
    }

    private void SetPhase(GameStartPhase phase, double duration = 0d)
    {
        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null) return;

        Hashtable properties = new()
        {
            {GameStartRoomKeys.Phase, (byte)phase},
            {GameStartRoomKeys.PhaseStartTime, PhotonNetwork.Time},
            {GameStartRoomKeys.PhaseDuration, duration}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged.ContainsKey(GameStartRoomKeys.Phase))
            ApplyCurrentRoomPhase();
    }

    private void ApplyCurrentRoomPhase()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) return;

        Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        if (!properties.TryGetValue(GameStartRoomKeys.Phase, out object phaseValue)) return;

        currentPhase = (GameStartPhase)Convert.ToByte(phaseValue);

        if (properties.TryGetValue(GameStartRoomKeys.PhaseStartTime, out object startValue))
            phaseStartTime = Convert.ToDouble(startValue);

        if (properties.TryGetValue(GameStartRoomKeys.PhaseDuration, out object durationValue))
            PhaseDuration = Convert.ToDouble(durationValue);

        ApplyPhase();
    }

    private void ApplyPhase()
    {
        switch (currentPhase)
        {
            case GameStartPhase.MapIntroduction:
                EnterMapIntroduction();
                break;
            case GameStartPhase.TurnSelection:
                EnterTurnSelection();
                break;
            case GameStartPhase.PlayerPreparation:
                EnterPlayerPreparation();
                break;
            case GameStartPhase.MainGame:
                EnterMainGame();
                break;
        }
    }

    private void EnterMapIntroduction()
    {
        CameraSwitchManager.Instance.MainCameraOn();

        if (stickGameController != null)
        {
            stickGameController.EndStickGame();
        }
        //TODO : 캔버스 켜기
    }

    private void EnterTurnSelection()
    {
        //TODO : 캔버스 끄기

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

    private void EnterMainGame()
    {
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
