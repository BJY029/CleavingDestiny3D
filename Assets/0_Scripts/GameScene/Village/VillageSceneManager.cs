using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageSceneManager : MonoBehaviourPunCallbacks
{
    public event Action OnVillagePhaseStarted;
    public event Action OnVillagePhaseEnded;
    public event Action OnPlayerReadyListUpdated;

    private float _startTime = -1.0f;
    private float _endTime = -1.0f;
    private bool _isPhaseActive = false;

    //싱글 플레이 모드 확인용 플래그
    private bool IsSinglePlayer => !PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom || PhotonNetwork.OfflineMode;

    void Start()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.SetVillageSceneManager(this);
        }
    }

    private void Update()
    {
        // MasterClient만 타이머를 체크해 마을 페이즈일 때 타이머가 다 진행되면 페이즈 종료
        if (!IsMasterAndPhase()) return;

        if ((float)PhotonNetwork.Time >= _endTime)
        {
            EndPhaseLogic();
        }
    }

    // 마스터 클라이언트가 페이즈를 종료시키는 공통 로직
    private void EndPhaseLogic()
    {
        _endTime = -1.0f;
        _isPhaseActive = false;
        OnVillagePhaseEnded?.Invoke();
    }

    private bool IsMasterAndPhase()
    {
        return _isPhaseActive && PhotonNetwork.IsMasterClient;
    }

    // 플레이어 속성 변경 감지
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 어떤 플레이어가 준비 상태를 변경했는지 감지
        bool isVillageReadyChanged = changedProps.ContainsKey(PlayerPropKeys.PlayerVillageReady);
        if (isVillageReadyChanged)
        {
            OnPlayerReadyListUpdated?.Invoke();

            if (IsMasterAndPhase())
            {
                CheckAllPlayersReady();
            }
        }
    }

    private void CheckAllPlayersReady()
    {
        // 모든 플레이어가 준비되었는지 확인
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.TryGetValue(PlayerPropKeys.PlayerVillageReady, out object isReadyObj) || !(bool)isReadyObj)
            {
                return;
            }
        }

        Debug.Log("All players are ready in Village. Ending phase early.");
        EndPhaseLogic();
    }

    /// <summary>
    /// UI 버튼에서 호출: 내 준비 상태를 변경
    /// </summary>
    public void SetLocalPlayerReady(bool isReady)
    {
        PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.PlayerVillageReady, isReady);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue(RoomPropKeys.IsVillageUpgradePhase, out object isPhaseActiveObj))
        {
            bool isPhase = (bool)isPhaseActiveObj;
            TurnManager.Instance.isUpgradePhase = isPhase;
            if (isPhase) StartVillagePhase();
            else EndVillagePhase();
        }

        if (propertiesThatChanged.TryGetValue(RoomPropKeys.VillageUpgradeStartEndTime, out object value)
        && value is Vector2 times)
        {
            _startTime = times.x;
            _endTime = times.y;
        }
    }

    public void StartVillagePhase()
    {
        _isPhaseActive = true;
        SetLocalPlayerReady(false);

        // UniTask의 Fire-and-Forget
        LoadVillageSceneAsync().Forget();
    }

    public void EndVillagePhase()
    {
        _isPhaseActive = false;
        _endTime = -1.0f;

        UnloadVillageSceneAsync().Forget();
    }

    /// <summary>
    /// 페이드 효과와 함께 마을 씬을 로드합니다.
    /// </summary>
    public async UniTask LoadVillageSceneAsync()
    {
        if (SceneManager.GetSceneByName(CommonDefine.VILLAGESCENE).isLoaded) return;

        // 1. 미리 로딩 시작 (활성화 대기)
        var loadOp = SceneManager.LoadSceneAsync(CommonDefine.VILLAGESCENE, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        // 2. 로딩 중에 페이드 인 동시 진행
        await FadeCanvas.Instance.FadeInAsync(1f);

        GameCanvasController.Instance.SetActiveCanvas(false);
        PlayerCanvasController.Instance.SetActiveCanvas(false);

        await UniTask.Delay(300, cancellationToken: destroyCancellationToken); // 0.3초 대기

        // 3. 씬 활성화
        loadOp.allowSceneActivation = true;
        await loadOp.ToUniTask(cancellationToken: destroyCancellationToken); // 로딩 완료 대기

        await FadeCanvas.Instance.FadeOutAsync(1f);

        OnVillagePhaseStarted?.Invoke();
    }

    /// <summary>
    /// 페이드 효과와 함께 마을 씬을 언로드합니다.
    /// </summary>
    public async UniTask UnloadVillageSceneAsync()
    {
        if (!SceneManager.GetSceneByName(CommonDefine.VILLAGESCENE).isLoaded) return;

        // 1. 화면 가리기 (완료될 때까지 대기)
        await FadeCanvas.Instance.FadeInAsync(1f);

        await UniTask.Delay(300, cancellationToken: destroyCancellationToken);

        // 2. 화면이 가려진 상태에서 언로드
        // ToUniTask()는 작업 완료를 보장합니다.
        await SceneManager.UnloadSceneAsync(CommonDefine.VILLAGESCENE).ToUniTask(cancellationToken: destroyCancellationToken);

        // 3. UI 복구
        GameCanvasController.Instance.SetActiveCanvas(true);
        PlayerCanvasController.Instance.SetActiveCanvas(true);

        // 4. 화면 밝히기
        await FadeCanvas.Instance.FadeOutAsync(1f);
    }
}