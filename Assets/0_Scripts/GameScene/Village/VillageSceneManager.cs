using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageSceneManager : MonoBehaviourPunCallbacks
{
    public event Action OnVillagePhaseStarted;
    public event Action OnVillagePhaseEnded;

    private float _startTime = -1.0f;
    private float _endTime = -1.0f;
    private bool _isPhaseActive = false;

    void Start()
    {
        // TurnManager가 있다면 등록
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.SetVillageSceneManager(this);
        }
    }

    private void Update()
    {
        // 마스터 클라이언트만 시간을 체크하여 페이즈 종료를 결정함
        if (!PhotonNetwork.IsMasterClient) return;
        if (!_isPhaseActive || _endTime < 0) return;

        // 시간 체크
        if ((float)PhotonNetwork.Time >= _endTime)
        {
            _endTime = -1.0f;
            _isPhaseActive = false;
            // TurnManager에게 종료 알림
            OnVillagePhaseEnded?.Invoke();
        }
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

        // 마을 페이즈 관련 시간 정보가 업데이트 되었을 때 로컬 변수 동기화
        if (propertiesThatChanged.TryGetValue(RoomPropKeys.VillageUpgradeStartEndTime, out object value)
        && value is Vector2 times)
        {
            _startTime = times.x;
            _endTime = times.y;
        }
    }

    /// <summary>
    /// 마을 페이즈를 시작합니다. (Fire-and-forget)
    /// </summary>
    public void StartVillagePhase()
    {
        _isPhaseActive = true;
        _ = LoadVillageSceneAsync();
    }

    /// <summary>
    /// 마을 페이즈를 종료합니다. (Fire-and-forget)
    /// </summary>
    public void EndVillagePhase()
    {
        _isPhaseActive = false;
        _endTime = -1.0f;
        _ = UnloadVillageSceneAsync();
    }

    /// <summary>
    /// 페이드 효과와 함께 마을 씬을 로드합니다.
    /// </summary>
    public async Awaitable LoadVillageSceneAsync()
    {
        if (SceneManager.GetSceneByName(CommonDefine.VILLAGESCENE).isLoaded) return;

        // 미리 로딩 시작 (활성화는 안 함)
        var loadOp = SceneManager.LoadSceneAsync(CommonDefine.VILLAGESCENE, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        // 페이드 인 (화면 어두워짐)
        await FadeCanvas.Instance.FadeInAsync(1f);

        // 메인 UI 숨기기
        GameCanvasController.Instance.SetActiveCanvas(false);
        PlayerCanvasController.Instance.SetActiveCanvas(false);

        await Awaitable.WaitForSecondsAsync(0.3f);

        // 씬 활성화 및 완료 대기
        loadOp.allowSceneActivation = true;
        await loadOp;

        // 씬 전환 완료 후 페이드 아웃은 마을 씬 내부 초기화 로직이나 여기서 수행
        await FadeCanvas.Instance.FadeOutAsync(1f);

        OnVillagePhaseStarted?.Invoke();
    }

    /// <summary>
    /// 페이드 효과와 함께 마을 씬을 언로드합니다.
    /// </summary>
    public async Awaitable UnloadVillageSceneAsync()
    {
        if (!SceneManager.GetSceneByName(CommonDefine.VILLAGESCENE).isLoaded) return;
        var unloadOp = SceneManager.UnloadSceneAsync(CommonDefine.VILLAGESCENE);
        unloadOp.allowSceneActivation = false;

        // 페이드 인 (화면 가리기)
        await FadeCanvas.Instance.FadeInAsync(1f);

        await Awaitable.WaitForSecondsAsync(0.3f);

        unloadOp.allowSceneActivation = true;
        await unloadOp;

        // 메인 UI 복구
        GameCanvasController.Instance.SetActiveCanvas(true);
        PlayerCanvasController.Instance.SetActiveCanvas(true);

        // 페이드 아웃 (화면 밝아짐)
        await FadeCanvas.Instance.FadeOutAsync(1f);
    }
}