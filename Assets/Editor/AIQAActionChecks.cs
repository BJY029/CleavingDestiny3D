using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public static class AIQAActionChecks
{
    public static List<string> Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Run outside Play Mode.");
        var passed = new List<string>();
        var rng = UnityEngine.Random.state;
        var rule = new AIQAUseReservation
        {
            enabled = true, itemId = "4009", phase = AIQAPhase.BeforeAttack, mode = AIQAUseMode.ForceUse
        };
        Check(rule.IsDue(AIQAPhase.BeforeAttack, 10, false), "reservation triggers at selected phase", passed);
        Check(!rule.IsDue(AIQAPhase.InventoryReady, 10, false), "reservation ignores other phases", passed);
        Check(!rule.IsDue(AIQAPhase.BeforeAttack, 10, true), "one-shot does not trigger after consumption", passed);
        rule.repeat = true;
        Check(rule.IsDue(AIQAPhase.BeforeAttack, 11, true), "repeat can trigger on a later turn", passed);
        rule.turnIndex = 12;
        Check(!rule.IsDue(AIQAPhase.BeforeAttack, 11, false) && rule.IsDue(AIQAPhase.BeforeAttack, 12, false),
            "specific TurnIndex is respected", passed);
        rule.turnIndex = -1;
        rule.repeat = false;

        Check(AIQAActionControl.ShouldReserveItem(true, rule, "4009", 40, 10, false, -1),
            "forced item is withheld from natural consumption before target phase", passed);
        Check(!AIQAActionControl.ShouldReserveItem(true, rule, "1000", 40, 10, false, -1),
            "other item IDs remain available", passed);
        Check(AIQAActionControl.ShouldReserveItem(true, rule, "4009", 40, 10, true, 10),
            "duplicate copies stay withheld after the attempt in the same turn", passed);
        Check(!AIQAActionControl.ShouldReserveItem(true, rule, "4009", 40, 11, true, 10),
            "one-shot withholding ends on the following turn", passed);
        Check(!AIQAActionControl.ShouldReserveItem(false, rule, "4009", 40, 10, false, -1),
            "disabled QA does not reserve items", passed);
        rule.mode = AIQAUseMode.Observe;
        Check(!AIQAActionControl.ShouldReserveItem(true, rule, "4009", 40, 10, false, -1),
            "observation mode does not alter natural item availability", passed);
        rule.mode = AIQAUseMode.ForceUse;
        rule.uniqueId = 40;
        Check(AIQAActionControl.ShouldReserveItem(true, rule, "4009", 40, 10, false, -1) &&
            !AIQAActionControl.ShouldReserveItem(true, rule, "4009", 41, 10, false, -1),
            "UID-specific withholding does not block other copies", passed);
        rule.turnIndex = 12;
        Check(!AIQAActionControl.ShouldReserveItem(true, rule, "4009", 40, 11, false, -1),
            "future turn reservation does not withhold items on earlier turns", passed);
        rule.phase = AIQAPhase.None;
        Check(!rule.IsDue(AIQAPhase.None, 12, false), "None cannot execute a reservation", passed);

        var slots = new (int uniqueId, string itemID)[] { (10, "1000"), (11, "4009"), (12, "4009"), (0, null) };
        Check(AIInventoryManager.FindOwnedQAItemIndex(slots, "4009", 0) == 1,
            "default selection uses first owned copy", passed);
        Check(AIInventoryManager.FindOwnedQAItemIndex(slots, "4009", 12) == 2,
            "UID request selects the exact duplicate slot", passed);
        Check(AIInventoryManager.FindOwnedQAItemIndex(slots, "4009", 10) == -1,
            "UID belonging to another item is rejected", passed);
        Check(AIInventoryManager.FindOwnedQAItemIndex(slots, "4009", 99) == -1 &&
            AIInventoryManager.FindOwnedQAItemIndex(slots, null, 0) == -1,
            "missing UID or empty ID cannot select a slot", passed);
        var copy = JsonUtility.FromJson<AIQAUseReservation>(JsonUtility.ToJson(rule));
        Check(copy.uniqueId == 40 && copy.turnIndex == 12 && copy.itemId == "4009" && copy.mode == AIQAUseMode.ForceUse,
            "reservation data round trips through session JSON", passed);

        var rejected = AIQAUseResult.Rejected("기력 부족");
        Check(!rejected.confirmed && rejected.message.Contains("거절"), "rejection is never reported as confirmed use", passed);
        Check(JsonUtility.ToJson(rng) == JsonUtility.ToJson(UnityEngine.Random.state),
            "reservation matching does not consume random state", passed);
        CheckSequences(passed);
        CheckPlans(passed);
        return passed;
    }

    private static void CheckPlans(List<string> passed)
    {
        var a = new AIQAUseReservation { id = "a", enabled = true, itemId = "1000", mode = AIQAUseMode.ForceUse, uniqueId = 10 };
        var b = new AIQAUseReservation { id = "b", enabled = true, itemId = "1002", mode = AIQAUseMode.ForceUse, uniqueId = 11 };
        var late = new AIQAUseReservation { id = "late", enabled = true, itemId = "2001", mode = AIQAUseMode.ForceUse, phase = AIQAPhase.BeforeAttack };
        var plan = new AIQAReservationPlan { reservations = new List<AIQAUseReservation> { a, b, late } };
        var state = new AIQAReservationRuntime();
        Check(state.ShouldSkipInventory(plan, 2, 10) && !state.NeedsFinalInventory(plan, 2, 10),
            "natural inventory waits for all current-turn forced reservations", passed);
        Check(state.ShouldReserve(plan, "1000", 10, 2, 10, false) && state.ShouldReserve(plan, "1002", 11, 2, 10, false) &&
            !state.ShouldReserve(plan, "1002", 12, 2, 10, false), "all queued exact copies are protected", passed);
        var order = new List<string>();
        foreach (var r in plan.reservations)
            if (state.TryBegin(r, 2, 10, AIQAPhase.InventoryReady)) order.Add(r.itemId);
        Check(string.Join("|", order) == "1000|1002", "same-phase reservations run in list order without taking a later phase", passed);
        Check(!state.TryBegin(a, 2, 10, AIQAPhase.InventoryReady) && !state.TryBegin(b, 2, 10, AIQAPhase.InventoryReady),
            "each queued reservation attempts only once per turn and phase", passed);
        Check(state.ShouldSkipInventory(plan, 2, 10), "later-stage reservation keeps natural inventory deferred", passed);
        Check(state.TryBegin(late, 2, 10, AIQAPhase.BeforeAttack) && state.NeedsFinalInventory(plan, 2, 10) &&
            !state.ShouldSkipInventory(plan, 2, 10), "final reservation enables the requested inventory pass before attack", passed);
        Check(state.ShouldReserve(plan, "1000", 10, 2, 10, true) && !state.ShouldReserve(plan, "1000", 10, 2, 10, false),
            "copies stay protected during forced use and are released for the extra natural pass", passed);
        state.MarkNaturalInventory(2, 10);
        Check(state.ShouldSkipInventory(plan, 2, 10) && !state.NeedsFinalInventory(plan, 2, 10),
            "extra natural inventory runs once after the complete queue", passed);
        Check(!state.ShouldSkipInventory(plan, 2, 11), "consumed one-shot queue leaves the next turn unchanged", passed);
        plan.useInventoryAfterReservations = false;
        Check(state.ShouldSkipInventory(plan, 2, 10) && !state.NeedsFinalInventory(plan, 2, 10),
            "skip option suppresses the extra pass even after every attempt", passed);
        b.repeat = true;
        Check(state.HasPending(plan, 2, 11) && state.TryBegin(b, 2, 11, AIQAPhase.InventoryReady),
            "repeat scheduling is independent for each row", passed);
        var unrelated = new AIQAReservationPlan { useInventoryAfterReservations = false, reservations = new List<AIQAUseReservation>
        {
            new() { id = "observe", enabled = true, itemId = "1000", mode = AIQAUseMode.Observe },
            new() { id = "future", enabled = true, itemId = "1002", mode = AIQAUseMode.ForceUse, turnIndex = 20 },
            new() { id = "disabled", enabled = false, itemId = "2001", mode = AIQAUseMode.ForceUse }
        }};
        Check(!state.ShouldSkipInventory(unrelated, 2, 10) && !state.NeedsFinalInventory(unrelated, 2, 10) &&
            !state.ShouldSkipInventory(new AIQAReservationPlan(), 2, 10),
            "empty, observe-only, disabled or future queues preserve normal inventory", passed);
        var copy = JsonUtility.FromJson<AIQAReservationPlan>(JsonUtility.ToJson(plan));
        Check(copy.reservations.Count == 3 && copy.reservations[1].id == "b" && copy.reservations[1].repeat &&
            !copy.useInventoryAfterReservations, "queue order and post-inventory option survive session JSON", passed);
        var legacy = AIQAReservationPlan.FromLegacy(new AIQAUseReservation { itemId = "4009", useCount = 2 });
        Check(legacy.reservations.Count == 1 && legacy.reservations[0].UseCount == 2 && legacy.useInventoryAfterReservations,
            "single-reservation settings migrate into a queue", passed);
    }

    private static void CheckSequences(List<string> passed)
    {
        var rule = new AIQAUseReservation { useCount = 2, uniqueId = 10, subsequentUniqueIds = new List<int> { 11 } };
        Check(rule.GetUseUid(0) == 10 && rule.GetUseUid(1) == 11 && rule.ReservesUid(11) && !rule.ReservesUid(12),
            "sequence preserves exact UIDs and withholds every selected copy", passed);
        var copy = JsonUtility.FromJson<AIQAUseReservation>(JsonUtility.ToJson(rule));
        Check(copy.UseCount == 2 && copy.GetUseUid(1) == 11, "sequence round trips through session JSON", passed);
        Check(JsonUtility.FromJson<AIQAUseReservation>("{\"uniqueId\":10}").UseCount == 1,
            "older reservations still use one item", passed);
        var requested = new List<int>();
        var attempts = new List<int>();
        Func<int, int, UniTask<AIQAUseResult>> success = (i, uid) =>
        {
            requested.Add(uid);
            return UniTask.FromResult(new AIQAUseResult { confirmed = true, uniqueId = uid > 0 ? uid : 100 + i });
        };
        int count = AIQAActionControl.ExecuteUsesAsync(rule, success, () => true,
            (i, r) => attempts.Add(i), CancellationToken.None).GetAwaiter().GetResult();
        Check(count == 2 && requested.Count == 2 && requested[0] == 10 && requested[1] == 11 && attempts.Count == 2,
            "two uses execute in order and both are logged", passed);
        var firstCompletion = new UniTaskCompletionSource<AIQAUseResult>();
        int sequentialCalls = 0;
        var pending = AIQAActionControl.ExecuteUsesAsync(rule, (i, uid) =>
        {
            sequentialCalls++;
            return i == 0 ? firstCompletion.Task : UniTask.FromResult(new AIQAUseResult { confirmed = true, uniqueId = uid });
        }, () => true, (i, r) => {}, CancellationToken.None);
        Check(sequentialCalls == 1 && !pending.GetAwaiter().IsCompleted,
            "second use waits until the first use is confirmed", passed);
        firstCompletion.TrySetResult(new AIQAUseResult { confirmed = true, uniqueId = 10 });
        Check(pending.GetAwaiter().GetResult() == 2 && sequentialCalls == 2,
            "confirmation releases the next use", passed);
        rule.uniqueId = 0; rule.subsequentUniqueIds.Clear();
        requested.Clear();
        count = AIQAActionControl.ExecuteUsesAsync(rule, success, () => true, (i, r) => {}, CancellationToken.None).GetAwaiter().GetResult();
        Check(count == 2 && requested[0] == 0 && requested[1] == 0 && rule.ReservesUid(999),
            "automatic selection reselects a remaining copy each time", passed);
        foreach (bool uncertain in new[] { false, true })
        {
            int calls = 0;
            rule.useCount = 3;
            count = AIQAActionControl.ExecuteUsesAsync(rule, (i, uid) =>
            {
                calls++;
                return UniTask.FromResult(i == 0 ? new AIQAUseResult { confirmed = true, uniqueId = 10 } :
                    uncertain ? new AIQAUseResult { message = "timeout" } : AIQAUseResult.Rejected("기력/보유 부족"));
            }, () => true, (i, r) => {}, CancellationToken.None).GetAwaiter().GetResult();
            Check(count == 1 && calls == 2, uncertain ? "uncertain use stops later requests" : "rejected use stops later requests", passed);
        }
        rule.useCount = 2;
        requested.Clear();
        count = AIQAActionControl.ExecuteUsesAsync(rule, success, () => requested.Count == 0,
            (i, r) => {}, CancellationToken.None).GetAwaiter().GetResult();
        Check(count == 1 && requested.Count == 1, "cancellation or turn change between uses stops requests", passed);
        rule.uniqueId = 10; rule.subsequentUniqueIds.Add(10);
        requested.Clear();
        count = AIQAActionControl.ExecuteUsesAsync(rule, success, () => true,
            (i, r) => {}, CancellationToken.None).GetAwaiter().GetResult();
        Check(count == 1 && requested.Count == 1, "consumed explicit UID cannot be requested again", passed);
        bool cancelled = false;
        using (var source = new CancellationTokenSource())
        {
            source.Cancel();
            try { AIQAActionControl.ExecuteUsesAsync(rule, success, () => true, (i, r) => {}, source.Token).GetAwaiter().GetResult(); }
            catch (OperationCanceledException) { cancelled = true; }
        }
        Check(cancelled && requested.Count == 1, "cancelled token prevents the first use", passed);
        Check(rule.UseCount == 2 && new AIQAUseReservation { useCount = 0 }.UseCount == 1 &&
            new AIQAUseReservation { useCount = 999 }.UseCount == 20, "sequence count is bounded", passed);
    }

    private static void Check(bool condition, string name, List<string> passed)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed.Add(name);
    }
}
