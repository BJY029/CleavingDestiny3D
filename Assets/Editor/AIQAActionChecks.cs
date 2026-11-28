using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        return passed;
    }

    private static void Check(bool condition, string name, List<string> passed)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed.Add(name);
    }
}
