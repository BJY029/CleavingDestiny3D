using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Isolated checks: do not start a match or alter scene assets, room properties or random state.
public static class AIQAToolChecks
{
    public static List<string> Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Run outside Play Mode.");
        var passed = new List<string>();
        var created = new List<ItemSO>();
        bool capture = AIQADiagnostics.CaptureEnabled;
        UnityEngine.Random.State random = UnityEngine.Random.state;
        try
        {
            foreach (string id in new[] { "1000", "1002", "3000", "3001" })
            {
                var item = ScriptableObject.CreateInstance<ItemSO>();
                item.itemId = id;
                created.Add(item);
            }
            var normal = created.GetRange(0, 3);
            var rule = new AIQAOfferRule { enabled = true, itemIds = new[] { "1000", "1002", "3000" } };
            Check(rule.TryResolve(created, normal, out string offer, out _) && offer == "1000|1002|3000",
                "three valid items preserve offer order", passed);
            Check(rule.IsDue(12, false) && !rule.IsDue(12, true), "one-shot is consumed only once", passed);
            rule.repeat = true;
            Check(rule.IsDue(12, true), "repeat ignores one-shot consumed flag", passed);
            rule.turnIndex = 15;
            Check(!rule.IsDue(14, false) && rule.IsDue(15, false) && !rule.IsDue(16, false),
                "exact TurnIndex reservation", passed);
            rule.enabled = false;
            Check(!rule.IsDue(15, false), "disabled reservation stays inactive", passed);
            rule.enabled = true;
            rule.itemIds[2] = "3001";
            Check(!rule.TryResolve(created, normal, out _, out _), "normal mode rejects excluded item", passed);
            rule.force = true;
            Check(rule.TryResolve(created, normal, out _, out _), "force mode permits registered excluded item", passed);
            rule.itemIds[2] = "1000";
            Check(!rule.TryResolve(created, normal, out _, out _), "force mode still rejects duplicates", passed);
            rule.itemIds[2] = "unknown";
            Check(!rule.TryResolve(created, normal, out _, out _), "force mode rejects unknown IDs", passed);
            rule.itemIds = new[] { "1000", "1002" };
            Check(!rule.TryResolve(created, normal, out _, out _), "incomplete offer is rejected", passed);

            var decision = new AIQADecision { hasThreshold = true, threshold = 30 };
            var below = decision.Add(created[0], 11);
            var first = decision.Add(created[1], 12);
            var tied = decision.Add(created[1], 13);
            var blocked = decision.Add(created[2], 14);
            decision.Score(below, 30);
            decision.Score(first, 64);
            decision.Score(tied, 64);
            blocked.reason = "예산 초과";
            decision.Complete(1);
            Check(below.reason.Contains("임계값 이하") && first.reason.Contains("최종 선택") &&
                tied.reason.Contains("동점") && blocked.reason == "예산 초과",
                "threshold, tied UID rows and rejection reasons remain distinct", passed);
            Check(!blocked.scored, "filtered candidates are not evaluated by monitor", passed);

            decision.context = new AIContext { curEnergy = 8, curTreeHP = 12345, curInvStr = "captured" };
            var copy = JsonUtility.FromJson<AIQADecision>(JsonUtility.ToJson(decision));
            Check(copy.context.curEnergy == 8 && copy.context.curTreeHP == 12345 &&
                copy.context.curInvStr == "captured" && copy.candidates[2].uniqueId == 13,
                "JSON preserves context and candidate UIDs", passed);

            AIQADiagnostics.CaptureEnabled = false;
            Check(AIQADiagnostics.Begin(2, "test", default) == null, "disabled monitor does not allocate a decision", passed);
            Check(JsonUtility.ToJson(UnityEngine.Random.state) == JsonUtility.ToJson(random),
                "offer resolution and diagnostic formatting do not consume Unity random", passed);
        }
        finally
        {
            AIQADiagnostics.CaptureEnabled = capture;
            UnityEngine.Random.state = random;
            foreach (var item in created) UnityEngine.Object.DestroyImmediate(item);
        }
        return passed;
    }

    private static void Check(bool condition, string name, List<string> passed)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed.Add(name);
    }
}
