using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AIQACombatChecks
{
    public static List<string> Run()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Run outside Play Mode.");
        var passed = new List<string>();
        var randomBefore = JsonUtility.ToJson(UnityEngine.Random.state);
        var plain = Project();
        Check(Near(plain.damageMultiplier, 1) && Near(plain.effectiveConversionRate, .3f), "base damage and conversion", passed);
        var sharpen = State("1000");
        Check(Near(Project(sharpen).damageMultiplier, 1.5f), "Sharpen multiplier uses actual item data", passed);
        Check(Near(Project(sharpen, State("1000")).damageMultiplier, 2.25f), "duplicate modifiers multiply", passed);
        Check(Near(Project(State("1000"), State("1001"), State("1002"), State("1003")).damageMultiplier, 1.2f),
            "mixed positive and negative modifiers", passed);
        var curse = State("4005");
        var silverCurse = Project(State("2001"), curse);
        Check(Near(silverCurse.effectiveConversionRate, .85f), "Silver override is followed by curse delta", passed);
        Check(Near(AIQACombatSnapshot.Project(new[] { curse }, 1, .1f).effectiveConversionRate, 0), "conversion lower clamp", passed);
        var otherOwner = State("1000"); otherOwner.ownerActorNum = 2;
        Check(Near(Project(otherOwner).damageMultiplier, 1), "other owner does not modify this attack", passed);

        var blind = State("1006");
        var blindSharpen = Project(blind, sharpen);
        Check(blindSharpen.hasRandomOverride && blindSharpen.ExampleDamage(1000) == 30 &&
            blindSharpen.ExampleDamage(1000, true) == 3000, "random override displays both outcomes without a roll", passed);
        var greasedBlind = Project(State("1004"), blind);
        Check(greasedBlind.ExampleDamage(1000) == 0 && greasedBlind.ExampleDamage(1000, true) == 0,
            "zero multiplier survives random override", passed);

        var spy = State("4004");
        var defense = AIQACombatSnapshot.Project(new[] { spy }, 1, .3f, 1000, 400, 1, 1500);
        Check(Near(defense.pendingBarrierMultiplier, .7f) && Near(defense.barrier, 1000) &&
            Near(defense.expectedVillageDamageNow, 100) && Near(defense.expectedVillageDamageAfterPending, 400),
            "pending Spy affects barrier only and is not applied to current defense", passed);
        Check(Near(Project(spy, State("4004")).pendingBarrierMultiplier, .49f), "pending Spy stacking", passed);
        var sacrifice = AIQACombatSnapshot.Project(Array.Empty<StatusInstance>(), 1, .3f, 100, 200, .5f, 1500);
        Check(Near(sacrifice.expectedVillageDamageNow, 450), "incoming multiplier precedes barrier and armor subtraction", passed);

        var taunt = State("4003");
        string before = JsonUtility.ToJson(curse.spec) + ":" + curse.remainingTurns + ":" + taunt.remainingTurns;
        for (int i = 0; i < 20; i++) Project(curse, taunt, blind);
        Check(before == JsonUtility.ToJson(curse.spec) + ":" + curse.remainingTurns + ":" + taunt.remainingTurns,
            "repeated display does not consume curse, taunt or mutate specs", passed);
        Check(randomBefore == JsonUtility.ToJson(UnityEngine.Random.state), "read-only projection does not consume Unity random", passed);
        sharpen.remainingTurns = 0;
        var pending = Project(sharpen);
        Check(Near(pending.damageMultiplier, 1.5f) && !string.IsNullOrEmpty(pending.note),
            "zero-turn registered entries match current bus behavior and are labelled", passed);

        foreach (var ids in new[]
        {
            new[] { "1000", "1002" }, new[] { "1000", "1001", "1002", "1003" },
            new[] { "2001", "4005" }, new[] { "1004", "1006" }, new[] { "1000", "4003", "2001", "4005" }
        })
        {
            // Compare to the real status event handlers on isolated copies, not the live singleton/RNG.
            var states = ids.Select(State).ToArray();
            var preview = Project(states);
            var system = new StatusSystem();
            foreach (var st in states) system.Add(st);
            var bus = new GameEventBus(system);
            var packet = new DamagePacket { attackerNum = 1, isBasicAttack = true, baseDamage = 1000 };
            var ctx = new EffectContext(new DeterministicRng(123), null);
            bus.publish(new GameEvent { actorNum = 1, type = TriggerMask.OnBeforeAttack, payload = packet }, ctx);
            bus.publish(new GameEvent { actorNum = 1, type = TriggerMask.OnDamageConvert, payload = packet }, ctx);
            float rate = Mathf.Clamp01((packet.convertRateOverride > 0 ? packet.convertRateOverride : .3f) + packet.convertRateDelta);
            Check(Near(preview.damageMultiplier, packet.multiplier) && Near(preview.effectiveConversionRate, rate),
                "projection matches real handlers: " + string.Join("+", ids), passed);
        }
        var copy = JsonUtility.FromJson<AIQACombatSnapshot>(JsonUtility.ToJson(silverCurse));
        Check(Near(copy.effectiveConversionRate, .85f) && copy.contributions.Count == 2,
            "snapshot export preserves rates and source list", passed);
        return passed;
    }

    private static AIQACombatSnapshot Project(params StatusInstance[] states) => AIQACombatSnapshot.Project(states, 1, .3f);

    private static StatusInstance State(string id)
    {
        var item = AssetDatabase.FindAssets("t:ItemSO").Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ItemSO>).First(i => i != null && i.itemId == id);
        var spec = item.effects.First(e => e.effectType == ItemEffect.AddStatus).statusSpce;
        return new StatusInstance
        {
            spec = JsonUtility.FromJson<StatusSpec>(JsonUtility.ToJson(spec)),
            ownerActorNum = 1, sourceActorNum = 2, remainingTurns = 2
        };
    }

    private static bool Near(float a, float b) => Mathf.Abs(a - b) < .0001f;
    private static void Check(bool condition, string name, List<string> passed)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed.Add(name);
    }
}
