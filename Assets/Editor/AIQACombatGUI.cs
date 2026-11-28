using Photon.Pun;
using UnityEditor;
using UnityEngine;

internal static class AIQACombatGUI
{
    public static void DrawLive(int actor, bool showSources)
    {
        var snapshot = AIQACombatSnapshot.Capture(actor);
        Draw(snapshot, showSources);
    }

    public static void Draw(AIQACombatSnapshot s, bool showSources)
    {
        if (s == null || !s.available)
        {
            EditorGUILayout.LabelField(s?.note ?? "전투 효과 기록 없음", EditorStyles.wordWrappedLabel);
            return;
        }
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField($"Actor {s.actor} / 기본 공격 기준", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"공격 피해 배수  ×{s.damageMultiplier:0.###}");
            EditorGUILayout.LabelField($"피해 → 방어막 전환율  {s.effectiveConversionRate:P1}");
            string start = s.conversionOverride > 0 ? $"덮어쓰기 {s.conversionOverride:P1}" : $"기본 {s.baseConversionRate:P1}";
            EditorGUILayout.LabelField($"  {start} + 보정 {s.conversionDelta * 100:+0.#;-0.#;0} %p → 0~100% 제한", EditorStyles.miniLabel);
            if (s.hasRandomOverride)
                EditorGUILayout.LabelField($"공격 원본 덮어쓰기  {s.overrideMin} 또는 {s.overrideMax} (난수 미추첨)", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField($"현재 방어막 {s.barrier:0.###} + 기본 방어력 {s.armor:0.###} = {s.barrier + s.armor:0.###}");
            EditorGUILayout.LabelField($"마을 피격 배수  ×{s.incomingTreeMultiplier:0.###}  (나무 공격에 적용)");
            EditorGUILayout.LabelField($"현재 나무 공격 {s.treeAttack:0.###} 기준 예상 마을 피해  {s.expectedVillageDamageNow:0.###}");
            if (!Mathf.Approximately(s.pendingBarrierMultiplier, 1))
            {
                EditorGUILayout.LabelField($"마을 진입 대기 효과  방어막 ×{s.pendingBarrierMultiplier:0.###} (기본 방어력 제외)", EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField($"  적용 후 방어막 {s.barrier * s.pendingBarrierMultiplier:0.###} / 예상 마을 피해 {s.expectedVillageDamageAfterPending:0.###}", EditorStyles.miniLabel);
            }
            EditorGUILayout.LabelField("현재 상태가 유지된다는 가정의 미리보기입니다. 최종 공격·밤 피해 실측과 구분하세요.", EditorStyles.wordWrappedMiniLabel);
            if (!string.IsNullOrEmpty(s.note)) EditorGUILayout.LabelField(s.note, EditorStyles.wordWrappedMiniLabel);
            if (showSources)
            {
                foreach (var row in s.contributions)
                    EditorGUILayout.LabelField($"• {row.statusId}: {row.description} / 출처 {row.source} / 잔여 {row.remainingTurns}", EditorStyles.wordWrappedLabel);
                if (s.contributions.Count == 0) EditorGUILayout.LabelField("등록된 효과 없음", EditorStyles.miniLabel);
            }
        }
    }

    public static void DrawResolved(AIQAResolvedAttack attack)
    {
        if (attack == null) return;
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("실제 공격 확정값", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"원본 {attack.originalDamage} / 덮어쓰기 {(attack.overrideDamage >= 0 ? attack.overrideDamage.ToString() : "없음")} / 피해 배수 ×{attack.multiplier:0.###}", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField($"기본 전환율 {attack.baseConversionRate:P1} / 덮어쓰기 {(attack.conversionOverride > 0 ? attack.conversionOverride.ToString("P1") : "없음")} / 보정 {attack.conversionDelta * 100:+0.#;-0.#;0} %p", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField($"최종 전환율 {attack.conversionRate:P1} / 피해 {attack.finalDamage} / 방어막 획득 {attack.convertedBarrier:0.###}");
        }
    }
}
