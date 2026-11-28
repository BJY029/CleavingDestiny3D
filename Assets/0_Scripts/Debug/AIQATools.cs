#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

// Editor-only hooks. Observing a decision must never evaluate it a second time.
[Serializable]
public sealed class AIQAOfferRule
{
    public bool enabled;
    public bool repeat;
    public bool force;
    public int turnIndex = -1;
    public string[] itemIds = new string[3];

    public bool IsDue(int requestedTurn, bool consumed) => enabled && (repeat || !consumed) &&
        (turnIndex < 0 || turnIndex == requestedTurn);

    public bool TryResolve(IReadOnlyList<ItemSO> registered, IReadOnlyList<ItemSO> allowed,
        out string offer, out string reason)
    {
        offer = null;
        reason = null;
        if (itemIds == null || itemIds.Length != 3)
        {
            reason = "아이템을 정확히 3개 지정하세요.";
            return false;
        }
        var unique = new HashSet<string>(StringComparer.Ordinal);
        foreach (string id in itemIds)
        {
            if (string.IsNullOrWhiteSpace(id) || registered == null || !registered.Any(i => i != null && i.itemId == id))
            {
                reason = $"ItemDB에 등록되지 않은 아이템: {id ?? "(비어 있음)"}";
                return false;
            }
            if (!unique.Add(id))
            {
                reason = $"선택지는 서로 다른 아이템 3개여야 합니다: {id}";
                return false;
            }
            if (!force && (allowed == null || !allowed.Any(i => i != null && i.itemId == id)))
            {
                reason = $"현재 등장 제한에 걸린 아이템: {id}. QA 강제 지정으로 별도 시험할 수 있습니다.";
                return false;
            }
        }
        offer = string.Join("|", itemIds);
        return true;
    }
}

public static class AIQAOfferOverrides
{
    private const string Prefix = "CleavingDestiny.AIQA.Offer.";
    private static readonly Dictionary<string, string> applied = new();
    private static object room;
    private static readonly List<string> events = new();
    public static IReadOnlyList<string> Events => events;

    private static string Key(bool ai) => Prefix + (ai ? "AI" : "Human");
    public static AIQAOfferRule GetRule(bool ai)
    {
        string json = SessionState.GetString(Key(ai), "");
        return string.IsNullOrEmpty(json) ? new AIQAOfferRule() : JsonUtility.FromJson<AIQAOfferRule>(json) ?? new AIQAOfferRule();
    }

    public static void SetRule(bool ai, AIQAOfferRule rule)
    {
        SessionState.SetString(Key(ai), JsonUtility.ToJson(rule));
        SessionState.SetBool(Key(ai) + ".Consumed", false);
        SessionState.SetString(Key(ai) + ".Status", rule.enabled ? "예약됨 — 다음 제안 생성부터 적용" : "비활성");
        // Already generated offers remain immutable, even when a new rule is armed.
    }

    public static string GetStatus(bool ai) => SessionState.GetString(Key(ai) + ".Status", "비활성");
    public static bool IsConsumed(bool ai) => SessionState.GetBool(Key(ai) + ".Consumed", false);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnPlayStarted()
    {
        room = null;
        ResetConsumption();
    }

    private static void ResetConsumption()
    {
        applied.Clear();
        events.Clear();
        foreach (bool ai in new[] { false, true })
        {
            SessionState.SetBool(Key(ai) + ".Consumed", false);
            SessionState.SetString(Key(ai) + ".Status", GetRule(ai).enabled ? "예약됨" : "비활성");
        }
    }

    public static bool TryGetOffer(int actor, int turnIndex, IReadOnlyList<ItemSO> registered,
        IReadOnlyList<ItemSO> allowed, out string offer)
    {
        offer = null;
        if (!Application.isPlaying || !PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient ||
            GameManager.Instance == null || !GameManager.Instance.isSoloPlay) return false;

        if (room != null && !ReferenceEquals(room, PhotonNetwork.CurrentRoom)) ResetConsumption();
        room = PhotonNetwork.CurrentRoom;
        string request = actor + ":" + turnIndex;
        if (applied.TryGetValue(request, out offer)) return true;

        bool ai = PhotonNetwork.CurrentRoom.GetPlayer(actor) == null;
        AIQAOfferRule rule = GetRule(ai);
        if (!rule.IsDue(turnIndex, IsConsumed(ai))) return false;

        if (!rule.TryResolve(registered, allowed, out offer, out string reason))
        {
            string message = $"Actor {actor} / Turn {turnIndex}: 지정 거절 — {reason} (기본 추첨 사용)";
            SessionState.SetString(Key(ai) + ".Status", message);
            AddEvent(message);
            return false;
        }

        if (applied.Count >= 256) applied.Clear();
        applied[request] = offer;
        if (!rule.repeat) SessionState.SetBool(Key(ai) + ".Consumed", true);
        string result = $"Actor {actor} / Turn {turnIndex}: {offer}" + (rule.force ? " [강제]" : "");
        SessionState.SetString(Key(ai) + ".Status", result + (rule.repeat ? " / 반복 예약 유지" : " / 1회 적용 완료"));
        AddEvent(result);
        Debug.Log("[AI QA ItemOffer] " + result);
        return true;
    }

    private static void AddEvent(string message)
    {
        if (events.Count >= 50) events.RemoveAt(0);
        events.Add(message);
    }
}

[Serializable]
public sealed class AIQAOfferSkipRule
{
    public bool enabled;
    public bool repeat;
    public int turnIndex = -1;

    public bool IsDue(int turn, bool consumed, string offer) => enabled && (repeat || !consumed) &&
        (turnIndex < 0 || turnIndex == turn) && !string.IsNullOrWhiteSpace(offer) && offer != ERROR.FULL_INV.ToString();
}

public static class AIQAOfferSkipSelection
{
    private const string Key = "CleavingDestiny.AIQA.Offer.SkipSelection";
    private static object room;
    public static AIQAOfferSkipRule GetRule()
    {
        string json = SessionState.GetString(Key, "");
        return string.IsNullOrEmpty(json) ? new AIQAOfferSkipRule() : JsonUtility.FromJson<AIQAOfferSkipRule>(json);
    }

    public static string Status => SessionState.GetString(Key + ".Status", "비활성");

    public static void SetRule(AIQAOfferSkipRule rule)
    {
        SessionState.SetString(Key, JsonUtility.ToJson(rule));
        ResetConsumption();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void OnPlayStarted()
    {
        room = null;
        ResetConsumption();
    }

    private static void ResetConsumption()
    {
        SessionState.SetBool(Key + ".Consumed", false);
        SessionState.SetString(Key + ".Status", GetRule().enabled ? "예약됨 — AI 제안 선택 직전에 적용" : "비활성");
    }

    public static ExitGames.Client.Photon.Hashtable CreateSkipUpdate(int actor) => new()
    {
        { ItemPropKeys.OFFER(actor), "" }
    };

    public static bool TrySkip(int actor, int turn, string offer)
    {
        if (!AIQAActionControl.IsCurrent(actor, turn)) return false;
        if (room != null && !ReferenceEquals(room, PhotonNetwork.CurrentRoom)) ResetConsumption();
        room = PhotonNetwork.CurrentRoom;
        var rule = GetRule();
        if (!rule.IsDue(turn, SessionState.GetBool(Key + ".Consumed", false), offer) ||
            PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.OFFER(actor)) != offer) return false;
        // End only this offer. Do not insert an item, allocate a UID or change inventory/energy.
        if (!PhotonNetwork.CurrentRoom.SetCustomProperties(CreateSkipUpdate(actor))) return false;
        if (!rule.repeat) SessionState.SetBool(Key + ".Consumed", true);
        string message = $"Actor {actor} / Turn {turn}: QA 강제 미선택 — {offer} / 아이템 획득 없이 다음 행동 진행";
        SessionState.SetString(Key + ".Status", message + (rule.repeat ? " / 반복 예약 유지" : " / 1회 적용 완료"));
        AIQADiagnostics.Skipped(actor, "제안 선택", message);
        Debug.Log("[AI QA ItemOffer] " + message);
        return true;
    }
}

[Serializable]
public sealed class AIQACandidate
{
    public string itemId;
    public string nameKey;
    public int uniqueId;
    public int cost;
    public bool scored;
    public float score;
    public string details;
    public string reason;
}

[Serializable]
public sealed class AIQADecision
{
    public int sequence;
    public int actor;
    public int turnIndex;
    public int wave;
    public float time;
    public string phase;
    public bool hasContext;
    public AIContext context;
    public AIQACombatSnapshot combat;
    public AIQAResolvedAttack resolvedAttack;
    public int budget = -1;
    public bool hasThreshold;
    public float threshold;
    public int selectedIndex = -1;
    public string summary;
    public List<AIQACandidate> candidates = new();

    public AIQACandidate Add(ItemSO item, int uid = 0)
    {
        var row = new AIQACandidate
        {
            itemId = item.itemId, nameKey = item.displayName_ID, uniqueId = uid, cost = item.itemCost
        };
        candidates.Add(row);
        return row;
    }

    public void Score(AIQACandidate row, float score, string details = null)
    {
        row.scored = true;
        row.score = score;
        row.details = details;
    }

    public void Complete(int selectedIndex, string summary = null)
    {
        this.selectedIndex = selectedIndex;
        this.summary = summary ?? (selectedIndex < 0 ? "선택 후보 없음" : "판단 선택 — 실행 성공 여부와 별도");
        for (int i = 0; i < candidates.Count; i++)
        {
            var row = candidates[i];
            if (!string.IsNullOrEmpty(row.reason)) continue;
            if (i == selectedIndex) row.reason = "최종 선택 (판단)";
            else if (float.IsNaN(row.score) || float.IsInfinity(row.score)) row.reason = "비정상 점수 — 실제 선택 여부 확인";
            else if (hasThreshold && row.score <= threshold) row.reason = "임계값 이하 (초과해야 선택)";
            else if (selectedIndex >= 0 && row.score == candidates[selectedIndex].score) row.reason = "동점 — 먼저 순회한 후보 우선";
            else row.reason = "더 높은 점수 후보 존재";
        }
    }
}

public static class AIQADiagnostics
{
    private const string CaptureKey = "CleavingDestiny.AIQA.Capture";
    private static readonly List<AIQADecision> history = new();
    private static int nextSequence;
    public static IReadOnlyList<AIQADecision> History => history;
    public static bool CaptureEnabled
    {
        get => SessionState.GetBool(CaptureKey, false);
        set => SessionState.SetBool(CaptureKey, value);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Clear()
    {
        history.Clear();
        // Keep sequence monotonic so an open window cannot select a stale record after clearing.
    }

    public static AIQADecision Begin(int actor, string phase, AIContext context, int budget = -1,
        float? threshold = null)
    {
        if (!CaptureEnabled) return null;
        return new AIQADecision
        {
            actor = actor, phase = phase, context = context, hasContext = true,
            combat = AIQACombatSnapshot.Capture(actor),
            budget = budget, hasThreshold = threshold.HasValue, threshold = threshold ?? 0,
            wave = context.curWaveCnt, time = Time.realtimeSinceStartup,
            turnIndex = PhotonNetwork.InRoom ? PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex) : -1
        };
    }

    public static void Publish(AIQADecision record, int selectedIndex = -1, string summary = null)
    {
        if (record == null) return;
        record.Complete(selectedIndex, summary);
        record.sequence = ++nextSequence;
        if (history.Count >= 200) history.RemoveAt(0);
        history.Add(record);
    }

    public static void Skipped(int actor, string phase, string reason)
    {
        var record = Begin(actor, phase, default);
        if (record != null) record.hasContext = false;
        Publish(record, summary: reason);
    }

    public static void RecordCombat(int actor, string phase, string summary, AIQAResolvedAttack attack = null)
    {
        var record = Begin(actor, phase, default);
        if (record == null) return;
        record.hasContext = false;
        record.resolvedAttack = attack;
        if (record.combat != null) record.wave = record.combat.wave;
        Publish(record, summary: summary);
    }
}
#endif
