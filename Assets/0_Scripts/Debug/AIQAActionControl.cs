#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public enum AIQAPhase
{
    None,
    BeforeOffer,
    BeforeFirstSteal,
    InventoryReady,
    BeforeSecondSteal,
    AfterSecondSteal,
    SecondInventoryReady,
    BeforeAttack
}

public enum AIQAUseMode { Observe, ForceUse }

[Serializable]
public sealed class AIQAActionSettings
{
    public bool enabled;
    public bool pauseEveryPhase;
    public AIQAPhase pauseAt = AIQAPhase.InventoryReady;
    public bool freezeTimer = true;
}

[Serializable]
public sealed class AIQAUseReservation
{
    public bool enabled;
    public AIQAPhase phase = AIQAPhase.InventoryReady;
    public AIQAUseMode mode;
    public string itemId;
    public int uniqueId;
    public int sacrificeUid;
    public int turnIndex = -1;
    public bool repeat;
    public bool pauseOnReach = true;

    public bool IsDue(AIQAPhase currentPhase, int turn, bool consumed) => enabled &&
        phase != AIQAPhase.None && phase == currentPhase && (turnIndex < 0 || turnIndex == turn) &&
        (repeat || !consumed);
}

public sealed class AIQAActionGate
{
    public int actor;
    public int turn;
    public AIQAPhase phase;
    public bool released;
    public bool ownsTimerPause;
    public TimeManager timer;
}

[Serializable]
public sealed class AIQAUseResult
{
    public bool confirmed;
    public int uniqueId;
    public string message;
    public static AIQAUseResult Rejected(string message) => new() { message = "거절: " + message };
}

[InitializeOnLoad]
public static class AIQAActionControl
{
    private const string Prefix = "CleavingDestiny.AIQA.Action.";
    private static readonly List<string> history = new();
    private static readonly HashSet<string> attempted = new();
    private static object room;
    private static bool consumed;
    private static int attemptedTurn = -1;
    private static bool stepPending;
    public static AIQAActionGate Held { get; private set; }
    public static bool Busy { get; private set; }
    public static string Status { get; private set; } = "대기 중인 AI 없음";
    public static IReadOnlyList<string> History => history;
    public static event Action<int, int, string> ItemApplied;

    static AIQAActionControl()
    {
        AssemblyReloadEvents.beforeAssemblyReload += Release;
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.ExitingPlayMode) Release();
        };
    }

    public static AIQAActionSettings GetSettings()
    {
        string json = SessionState.GetString(Prefix + "Settings", "");
        return string.IsNullOrEmpty(json) ? new AIQAActionSettings() : JsonUtility.FromJson<AIQAActionSettings>(json);
    }

    public static AIQAUseReservation GetReservation()
    {
        string json = SessionState.GetString(Prefix + "Reservation", "");
        return string.IsNullOrEmpty(json) ? new AIQAUseReservation() : JsonUtility.FromJson<AIQAUseReservation>(json);
    }

    public static void SaveSettings(AIQAActionSettings settings)
    {
        SessionState.SetString(Prefix + "Settings", JsonUtility.ToJson(settings));
        if (!settings.enabled) Release();
    }

    public static void Arm(AIQAUseReservation reservation)
    {
        SessionState.SetString(Prefix + "Reservation", JsonUtility.ToJson(reservation));
        consumed = false;
        attemptedTurn = -1;
        Status = reservation.enabled ? "사용 예약 저장 — 지정 단계 도달 대기" : "사용 예약 해제";
    }

    public static void Release()
    {
        if (Held == null) return;
        Held.released = true;
        RestoreTimer(Held);
    }

    public static void Step()
    {
        stepPending = true;
        Release();
    }

    public static void DisableAll()
    {
        var settings = GetSettings();
        settings.enabled = false;
        SaveSettings(settings);
        var reservation = GetReservation();
        reservation.enabled = false;
        Arm(reservation);
        stepPending = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        Held = null;
        Busy = consumed = stepPending = false;
        attemptedTurn = -1;
        room = null;
        attempted.Clear();
        history.Clear();
        Status = "대기 중인 AI 없음";
    }

    public static bool IsCurrent(int actor, int turn) => Application.isPlaying && PhotonNetwork.InRoom &&
        PhotonNetwork.IsMasterClient && GameManager.Instance != null && GameManager.Instance.isSoloPlay &&
        PhotonNetwork.CurrentRoom.GetPlayer(actor) == null &&
        PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor) == actor &&
        PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex) == turn &&
        PhotonPropertyHelper.GetRoomProp<GamePhaseValue>(RoomPropKeys.GamePhase) == GamePhaseValue.DAY &&
        !PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase) &&
        (MatchResultManager.Instance == null || !MatchResultManager.Instance._isResultResolved);

    public static bool ShouldReserveItem(string itemId, int uid)
    {
        if (!GetSettings().enabled || !PhotonNetwork.InRoom) return false;
        var rule = GetReservation();
        return ShouldReserveItem(true, rule, itemId, uid,
            PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex), consumed, attemptedTurn);
    }

    public static bool ShouldReserveItem(bool enabled, AIQAUseReservation rule, string itemId, int uid,
        int turn, bool wasConsumed, int lastAttemptedTurn) => enabled && rule.enabled &&
        rule.mode == AIQAUseMode.ForceUse && rule.phase != AIQAPhase.None && rule.itemId == itemId &&
        (rule.uniqueId <= 0 || rule.uniqueId == uid) && (rule.turnIndex < 0 || rule.turnIndex == turn) &&
        (rule.repeat || !wasConsumed || lastAttemptedTurn == turn);

    public static async UniTask CheckpointAsync(AIController ai, AIQAPhase phase, CancellationToken token)
    {
        if (!GetSettings().enabled) return;
        int actor = ai.PlayerActNum;
        int turn = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
        if (!IsCurrent(actor, turn)) return;
        if (room != null && !ReferenceEquals(room, PhotonNetwork.CurrentRoom))
        {
            consumed = false;
            attemptedTurn = -1;
            attempted.Clear();
            history.Clear();
        }
        room = PhotonNetwork.CurrentRoom;
        var settings = GetSettings();
        var rule = GetReservation();
        bool due = rule.IsDue(phase, turn, consumed);
        bool pause = stepPending || settings.pauseEveryPhase || settings.pauseAt == phase || (due && rule.pauseOnReach);
        stepPending = false;
        Status = $"Actor {actor} / Turn {turn} / {phase} 도달";
        if (pause)
        {
            var gate = new AIQAActionGate { actor = actor, turn = turn, phase = phase };
            Held = gate;
            try
            {
                while (!gate.released && GetSettings().enabled && IsCurrent(actor, turn))
                {
                    if (GetSettings().freezeTimer) PauseTimer(gate);
                    else RestoreTimer(gate);
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            finally
            {
                RestoreTimer(gate);
                if (Held == gate) Held = null;
            }
        }
        token.ThrowIfCancellationRequested();
        if (!IsCurrent(actor, turn)) throw new OperationCanceledException(token);
        if (!GetSettings().enabled) return;

        // Re-read after waiting: QA can replace or cancel the reservation while held.
        rule = GetReservation();
        string key = $"{actor}:{turn}:{phase}";
        if (!rule.IsDue(phase, turn, consumed) || !attempted.Add(key)) return;
        consumed = !rule.repeat;
        attemptedTurn = turn;
        if (rule.mode == AIQAUseMode.Observe)
        {
            Log($"Actor {actor} / Turn {turn}: {phase} 관찰 시작 / {rule.itemId}. 자연 판단은 AI 판단 모니터에서 확인");
            return;
        }
        Busy = true;
        try
        {
            AIQAUseResult result = await ai.aiBrain.InventoryManager.TryUseForQAAsync(rule.itemId, rule.uniqueId, rule.sacrificeUid, token);
            Log($"Actor {actor} / Turn {turn} / {phase} / {rule.itemId}: {result.message}");
        }
        catch (OperationCanceledException)
        {
            Log($"Actor {actor} / Turn {turn}: 사용 예약 실행 취소 — 이미 처리된 소모는 자동 환불하지 않음");
            throw;
        }
        catch (Exception ex)
        {
            Log($"Actor {actor} / Turn {turn}: 사용 처리 예외 — 상태 확인 필요, 자동 재시도 안 함: {ex.Message}");
            Debug.LogException(ex);
        }
        finally { Busy = false; }
    }

    private static void PauseTimer(AIQAActionGate gate)
    {
        if (gate.ownsTimerPause || TimeManager.instance == null || !TimeManager.instance.TurnTimerActivated) return;
        gate.timer = TimeManager.instance;
        gate.timer.PauseMainTurnTimerForMiniGame();
        gate.ownsTimerPause = gate.timer.TurnTimerPaused;
    }

    private static void RestoreTimer(AIQAActionGate gate)
    {
        if (!gate.ownsTimerPause) return;
        gate.ownsTimerPause = false;
        if (gate.timer != null && gate.timer.TurnTimerPaused && IsCurrent(gate.actor, gate.turn))
            gate.timer.ResumeMainTurnTimerAfterMiniGame();
    }

    public static void NotifyItemApplied(int actor, int uid, string itemId)
    {
        AIQADiagnostics.RecordCombat(actor, "아이템 사용 후", $"{itemId} / UID {uid} 처리 알림 시점 — 비동기 프로퍼티 반영 지연 가능");
        ItemApplied?.Invoke(actor, uid, itemId);
    }

    public static void Log(string message)
    {
        Status = message;
        if (history.Count >= 100) history.RemoveAt(0);
        history.Add(message);
        Debug.Log("[AI QA Action] " + message);
    }
}
#endif
