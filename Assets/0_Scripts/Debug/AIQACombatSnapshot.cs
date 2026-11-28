#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

[Serializable]
public sealed class AIQACombatContribution
{
    public string statusId;
    public int owner;
    public int source;
    public int remainingTurns;
    public string description;
}

// Read-only projection of the registered basic-attack effects. No event dispatch or RNG calls.
[Serializable]
public sealed class AIQACombatSnapshot
{
    public bool available;
    public string note;
    public int actor;
    public int turnIndex;
    public int wave;
    public float time;
    public float damageMultiplier = 1;
    public bool hasRandomOverride;
    public int overrideMin;
    public int overrideMax;
    public float baseConversionRate;
    public float conversionOverride = -1;
    public float conversionDelta;
    public float effectiveConversionRate;
    public float barrier;
    public float armor;
    public float incomingTreeMultiplier;
    public float treeAttack;
    public float pendingBarrierMultiplier = 1;
    public float expectedVillageDamageNow;
    public float expectedVillageDamageAfterPending;
    public List<AIQACombatContribution> contributions = new();

    public static AIQACombatSnapshot Capture(int actor)
    {
        if (!Application.isPlaying || !PhotonNetwork.InRoom || actor <= 0 ||
            GameManager.Instance == null || ItemHandlingSystem.instance == null)
            return new AIQACombatSnapshot { note = "경기 시작 후 전투 효과를 표시합니다." };
        var statuses = ItemHandlingSystem.instance.GetQAStatuses();
        if (statuses == null) return new AIQACombatSnapshot { note = "상태 시스템 초기화 대기" };
        float baseTree = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeAtkPow);
        int day = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentDay);
        var setting = GameManager.Instance.roomDefaultSetting;
        // TreeStatus.getTreeAtkPow formula, without modifying that component's cached values.
        float nightAttack = setting == null ? baseTree : Mathf.Min(baseTree +
            setting.poisonDamageIncreasePerDay * Mathf.Max(0, day - 1), setting.maxPoisonDamage);
        var result = Project(statuses, actor,
            PhotonPropertyHelper.GetPlayerProp<float>(actor, PlayerPropKeys.BarrierConversionRate),
            PhotonPropertyHelper.GetPlayerProp<float>(actor, PlayerPropKeys.VillageBarrier),
            PhotonPropertyHelper.GetPlayerProp<float>(actor, PlayerPropKeys.BarrierArmor),
            PhotonPropertyHelper.GetPlayerProp<float>(actor, PlayerPropKeys.TreeAtkMulti), nightAttack);
        result.turnIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
        result.wave = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
        result.time = Time.realtimeSinceStartup;
        return result;
    }

    public static AIQACombatSnapshot Project(IEnumerable<StatusInstance> statuses, int actor, float baseRate,
        float barrier = 0, float armor = 0, float incomingMultiplier = 1, float treeAttack = 0)
    {
        var result = new AIQACombatSnapshot
        {
            available = true, actor = actor, baseConversionRate = baseRate, barrier = barrier,
            armor = armor, incomingTreeMultiplier = incomingMultiplier, treeAttack = treeAttack
        };
        foreach (var st in (statuses ?? Enumerable.Empty<StatusInstance>()).Where(s => s?.spec != null).OrderBy(s => s.spec.priority))
        {
            var spec = st.spec;
            bool affectsAttack = spec.triggerScope == TriggerScope.AnyActor || st.ownerActorNum == actor;
            bool before = affectsAttack && (spec.triggers & TriggerMask.OnBeforeAttack) != 0;
            bool conversion = affectsAttack && (spec.triggers & TriggerMask.OnDamageConvert) != 0;
            string description = null;
            if (before)
            {
                switch (spec.statusId)
                {
                    case "DMG_SHARPEN": case "DMG_DULL": case "DMG_DOUBLE": case "DMG_RUSTY":
                        result.damageMultiplier *= spec.multiplier;
                        description = $"공격 피해 ×{spec.multiplier:0.###}";
                        break;
                    case "GIM_TAUNT":
                        if (st.ownerActorNum == actor)
                        {
                            result.damageMultiplier *= spec.multiplier;
                            description = $"도발 피해 ×{spec.multiplier:0.###}";
                        }
                        break;
                    case "DMG_GREASED":
                        result.damageMultiplier *= 0;
                        description = "기본 공격 피해 ×0";
                        break;
                    case "DMG_BLIND":
                        result.hasRandomOverride = true;
                        result.overrideMin = spec.randMin;
                        result.overrideMax = spec.randMax;
                        description = $"공격 원본 {spec.randMin} 또는 {spec.randMax} (난수 미추첨)";
                        break;
                    default:
                        description = "공격 미리보기 미지원 효과 — 실측 대조 필요";
                        break;
                }
            }
            if (conversion)
            {
                if (spec.statusId == "DEF_SILVER")
                {
                    result.conversionOverride = spec.convertRate;
                    description = $"전환율 덮어쓰기 {spec.convertRate:P1}";
                }
                else if (spec.statusId == "GIM_CURSE" && st.ownerActorNum == actor)
                {
                    result.conversionDelta += spec.convertRate;
                    description = $"전환율 보정 {spec.convertRate * 100:+0.#;-0.#;0} %p";
                }
                else if (description == null) description = "전환 미리보기 미지원 효과 — 실측 대조 필요";
            }
            if (st.ownerActorNum == actor && spec.statusId == "GIM_SPY" &&
                spec.triggerScope == TriggerScope.AnyActor && (spec.triggers & TriggerMask.OnVillageStart) != 0)
            {
                result.pendingBarrierMultiplier *= spec.multiplier;
                description = $"마을 진입 시 방어막 ×{spec.multiplier:0.###} (아직 미적용)";
            }
            if (description == null && st.ownerActorNum == actor) description = "기본 공격 배수 외 상태";
            if (description == null) continue;
            result.contributions.Add(new AIQACombatContribution
            {
                statusId = spec.statusId, owner = st.ownerActorNum, source = st.sourceActorNum,
                remainingTurns = st.remainingTurns, description = description
            });
        }
        // Match GameEventBus, which retains zero-turn statuses until cleanup.
        if (result.contributions.Any(c => c.remainingTurns <= 0))
            result.note = "잔여0 이하의 삭제 대기 상태 포함 — 현재 등록 목록 기준";
        float startRate = result.conversionOverride > 0 ? result.conversionOverride : baseRate;
        result.effectiveConversionRate = Mathf.Clamp01(startRate + result.conversionDelta);
        result.expectedVillageDamageNow = Mathf.Max(0, treeAttack * incomingMultiplier - barrier - armor);
        result.expectedVillageDamageAfterPending = Mathf.Max(0,
            treeAttack * incomingMultiplier - barrier * result.pendingBarrierMultiplier - armor);
        return result;
    }

    public int ExampleDamage(int originalDamage, bool secondRandomOutcome = false) => Mathf.RoundToInt(
        (hasRandomOverride ? (secondRandomOutcome ? overrideMax : overrideMin) : originalDamage) * damageMultiplier);
}

[Serializable]
public sealed class AIQAResolvedAttack
{
    public int originalDamage;
    public int overrideDamage;
    public float multiplier;
    public float baseConversionRate;
    public float conversionOverride;
    public float conversionDelta;
    public float conversionRate;
    public int finalDamage;
    public float convertedBarrier;
    public bool basicAttack;
}
#endif
