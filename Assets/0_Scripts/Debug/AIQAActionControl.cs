#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
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
    public string id;
    public bool enabled;
    public AIQAPhase phase = AIQAPhase.InventoryReady;
    public AIQAUseMode mode;
    public string itemId;
    public int uniqueId;
    public int useCount = 1;
    public List<int> subsequentUniqueIds = new();
    public int sacrificeUid;
    public int turnIndex = -1;
    public bool repeat;
    public bool pauseOnReach = true;

    public int UseCount => Mathf.Clamp(useCount, 1, 20);
    public int GetUseUid(int index) => index == 0 ? Mathf.Max(0, uniqueId) :
        subsequentUniqueIds != null && index - 1 < subsequentUniqueIds.Count
            ? Mathf.Max(0, subsequentUniqueIds[index - 1]) : 0;

    public bool ReservesUid(int uid)
    {
        for (int i = 0; i < UseCount; i++)
            if (GetUseUid(i) == 0 || GetUseUid(i) == uid) return true;
        return false;
    }

    public bool IsDue(AIQAPhase currentPhase, int turn, bool consumed) => enabled &&
        phase != AIQAPhase.None && phase == currentPhase && (turnIndex < 0 || turnIndex == turn) &&
        (repeat || !consumed);
}

[Serializable]
public sealed class AIQAReservationPlan
{
    public List<AIQAUseReservation> reservations = new();
    public bool useInventoryAfterReservations = true;

    public static AIQAReservationPlan FromLegacy(AIQAUseReservation rule)
    {
        var plan = new AIQAReservationPlan();
        if (rule != null) { rule.id = "legacy"; plan.reservations.Add(rule); }
        return plan;
    }
}

// Per-room execution state. Kept separate so scheduling can be checked without a live match.
public sealed class AIQAReservationRuntime
{
    private readonly HashSet<string> consumed = new();
    private readonly HashSet<string> attempts = new();
    private readonly Dictionary<string, int> lastTurns = new();
    private readonly HashSet<string> naturalInventory = new();
    private static string AttemptKey(AIQAUseReservation r, int actor, int turn) => $"{actor}:{turn}:{r.phase}:{r.id}";
    public bool IsDue(AIQAUseReservation r, int actor, int turn, AIQAPhase phase) =>
        r != null && r.IsDue(phase, turn, consumed.Contains(r.id)) && !attempts.Contains(AttemptKey(r, actor, turn));
    public bool TryBegin(AIQAUseReservation r, int actor, int turn, AIQAPhase phase)
    {
        if (!IsDue(r, actor, turn, phase)) return false;
        attempts.Add(AttemptKey(r, actor, turn));
        if (!r.repeat) consumed.Add(r.id);
        lastTurns[r.id] = turn;
        return true;
    }
    public bool IsActive(AIQAUseReservation r, int turn) => r != null && r.enabled &&
        r.mode == AIQAUseMode.ForceUse && r.phase != AIQAPhase.None && !string.IsNullOrEmpty(r.itemId) &&
        (r.turnIndex < 0 || r.turnIndex == turn) &&
        (r.repeat || !consumed.Contains(r.id) || (lastTurns.TryGetValue(r.id, out int last) && last == turn));
    public bool HasActive(AIQAReservationPlan plan, int turn) => plan.reservations.Any(r => IsActive(r, turn));
    public bool HasPending(AIQAReservationPlan plan, int actor, int turn) => plan.reservations.Any(r =>
        IsActive(r, turn) && !attempts.Contains(AttemptKey(r, actor, turn)));
    public bool ShouldSkipInventory(AIQAReservationPlan plan, int actor, int turn) => HasActive(plan, turn) &&
        (HasPending(plan, actor, turn) || !plan.useInventoryAfterReservations || naturalInventory.Contains($"{actor}:{turn}"));
    public bool NeedsFinalInventory(AIQAReservationPlan plan, int actor, int turn) => HasActive(plan, turn) &&
        !ShouldSkipInventory(plan, actor, turn);
    public void MarkNaturalInventory(int actor, int turn) => naturalInventory.Add($"{actor}:{turn}");
    public bool ShouldReserve(AIQAReservationPlan plan, string itemId, int uid, int actor, int turn, bool busy) =>
        (busy || HasPending(plan, actor, turn) || !plan.useInventoryAfterReservations) &&
        plan.reservations.Any(r => IsActive(r, turn) && r.itemId == itemId && r.ReservesUid(uid));
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
    private static AIQAReservationRuntime runtime = new();
    private static object room;
    private static bool stepPending;
    private static int reservationRevision;
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
        return GetPlan().reservations.FirstOrDefault() ?? new AIQAUseReservation();
    }

    public static AIQAReservationPlan GetPlan()
    {
        string json = SessionState.GetString(Prefix + "Plan", "");
        if (!string.IsNullOrEmpty(json)) return JsonUtility.FromJson<AIQAReservationPlan>(json);
        string legacy = SessionState.GetString(Prefix + "Reservation", "");
        return AIQAReservationPlan.FromLegacy(string.IsNullOrEmpty(legacy) ? null : JsonUtility.FromJson<AIQAUseReservation>(legacy));
    }

    public static void SaveSettings(AIQAActionSettings settings)
    {
        SessionState.SetString(Prefix + "Settings", JsonUtility.ToJson(settings));
        if (!settings.enabled) Release();
    }

    public static void Arm(AIQAUseReservation reservation)
        => ArmPlan(AIQAReservationPlan.FromLegacy(reservation));

    public static void ArmPlan(AIQAReservationPlan plan)
    {
        var ids = new HashSet<string>();
        plan.reservations.RemoveAll(r => r == null);
        foreach (var r in plan.reservations)
            if (string.IsNullOrEmpty(r.id) || !ids.Add(r.id)) { r.id = Guid.NewGuid().ToString("N"); ids.Add(r.id); }
        reservationRevision++;
        SessionState.SetString(Prefix + "Plan", JsonUtility.ToJson(plan));
        runtime = new AIQAReservationRuntime();
        Status = $"예약 목록 저장 — 활성 {plan.reservations.Count(r => r.enabled)}개 / 지정 단계 도달 대기";
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
        var plan = GetPlan();
        foreach (var reservation in plan.reservations) reservation.enabled = false;
        ArmPlan(plan);
        stepPending = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        Held = null;
        Busy = stepPending = false;
        runtime = new AIQAReservationRuntime();
        room = null;
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
        return runtime.ShouldReserve(GetPlan(), itemId, uid,
            PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor),
            PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex), Busy);
    }

    public static bool ShouldReserveItem(bool enabled, AIQAUseReservation rule, string itemId, int uid,
        int turn, bool wasConsumed, int lastAttemptedTurn) => enabled && rule.enabled &&
        rule.mode == AIQAUseMode.ForceUse && rule.phase != AIQAPhase.None && rule.itemId == itemId &&
        rule.ReservesUid(uid) && (rule.turnIndex < 0 || rule.turnIndex == turn) &&
        (rule.repeat || !wasConsumed || lastAttemptedTurn == turn);

    public static async UniTask CheckpointAsync(AIController ai, AIQAPhase phase, CancellationToken token)
    {
        if (!GetSettings().enabled) return;
        int actor = ai.PlayerActNum;
        int turn = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
        if (!IsCurrent(actor, turn)) return;
        if (room != null && !ReferenceEquals(room, PhotonNetwork.CurrentRoom))
        {
            runtime = new AIQAReservationRuntime();
            history.Clear();
        }
        room = PhotonNetwork.CurrentRoom;
        var settings = GetSettings();
        var plan = GetPlan();
        bool pause = stepPending || settings.pauseEveryPhase || settings.pauseAt == phase ||
            plan.reservations.Any(r => runtime.IsDue(r, actor, turn, phase) && r.pauseOnReach);
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
        plan = GetPlan();
        Busy = true;
        int revision = reservationRevision;
        try
        {
            foreach (var rule in plan.reservations)
            {
                token.ThrowIfCancellationRequested();
                if (revision != reservationRevision || !GetSettings().enabled || !IsCurrent(actor, turn)) break;
                if (!runtime.TryBegin(rule, actor, turn, phase)) continue;
                int number = plan.reservations.IndexOf(rule) + 1;
                if (rule.mode == AIQAUseMode.Observe)
                {
                    Log($"예약 #{number} / Actor {actor} / Turn {turn}: {phase} 관찰 시작 / {rule.itemId}");
                    continue;
                }
                int completed = await ExecuteUsesAsync(rule,
                    (index, uid) => ai.aiBrain.InventoryManager.TryUseForQAAsync(rule.itemId, uid, rule.sacrificeUid, token),
                    () => revision == reservationRevision && GetSettings().enabled && IsCurrent(actor, turn),
                    (index, result) => Log($"예약 #{number} / Actor {actor} / Turn {turn} / {phase} / {rule.itemId} [{index + 1}/{rule.UseCount}]: {result.message}"), token);
                Log($"예약 #{number} / {rule.itemId}: {completed}/{rule.UseCount}회 확인" +
                    (completed < rule.UseCount ? " — 이 예약의 나머지 회차 중단 (자동 재시도 없음)" : ""));
            }
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

    public static bool SkipNaturalInventory(int actor)
    {
        int turn = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
        if (!GetSettings().enabled || !IsCurrent(actor, turn)) return false;
        var plan = GetPlan();
        if (runtime.ShouldSkipInventory(plan, actor, turn))
        {
            string reason = runtime.HasPending(plan, actor, turn) ? "미실행 강제 사용 예약이 남아 있어 자연 사용 보류" :
                !plan.useInventoryAfterReservations ? "예약 후 추가 인벤토리 사용 건너뛰기 설정" : "예약 후 추가 인벤토리 사용을 이미 진행함";
            Log($"Actor {actor} / Turn {turn}: {reason}");
            AIQADiagnostics.Skipped(actor, "인벤토리 사용", reason);
            return true;
        }
        if (runtime.HasActive(plan, turn)) runtime.MarkNaturalInventory(actor, turn);
        return false;
    }

    public static bool NeedsFinalInventory(int actor)
    {
        int turn = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
        return GetSettings().enabled && IsCurrent(actor, turn) && runtime.NeedsFinalInventory(GetPlan(), actor, turn);
    }

    // Wait for each authoritative use/removal before selecting the next copy.
    public static async UniTask<int> ExecuteUsesAsync(AIQAUseReservation rule,
        Func<int, int, UniTask<AIQAUseResult>> use, Func<bool> canContinue,
        Action<int, AIQAUseResult> onAttempt, CancellationToken token)
    {
        int completed = 0;
        var used = new HashSet<int>();
        for (int i = 0; i < rule.UseCount; i++)
        {
            token.ThrowIfCancellationRequested();
            if (!canContinue()) break;
            int uid = rule.GetUseUid(i);
            var result = uid > 0 && used.Contains(uid)
                ? AIQAUseResult.Rejected("이미 사용한 UID가 다시 지정됨") : await use(i, uid);
            onAttempt(i, result);
            if (!result.confirmed) break;
            used.Add(result.uniqueId);
            completed++;
        }
        return completed;
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
