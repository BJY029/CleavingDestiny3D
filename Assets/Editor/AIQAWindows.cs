using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

internal static class AIQAEditorItems
{
    private static List<ItemSO> items;
    private static readonly Dictionary<string, string> names = new();
    public static List<ItemSO> Items => items ??= Load();

    public static void Reload() { items = null; names.Clear(); }

    private static List<ItemSO> Load()
    {
        string path = Path.Combine(Application.dataPath, "Localization/Item_CSV.csv");
        if (File.Exists(path))
        {
            foreach (string line in File.ReadLines(path))
            {
                var fields = CsvFields(line);
                if (fields.Count > 1 && fields[0].StartsWith("NAME_", StringComparison.Ordinal)) names[fields[0]] = fields[1];
            }
        }
        return AssetDatabase.FindAssets("t:ItemSO").Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ItemSO>).Where(i => i != null)
            .OrderBy(i => i.itemId, StringComparer.Ordinal).ToList();
    }

    private static List<string> CsvFields(string line)
    {
        var result = new List<string>();
        var value = new StringBuilder();
        bool quoted = false;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"') { value.Append('"'); i++; }
                else quoted = !quoted;
            }
            else if (c == ',' && !quoted) { result.Add(value.ToString()); value.Clear(); }
            else value.Append(c);
        }
        result.Add(value.ToString());
        return result;
    }

    public static string Label(string id, string nameKey = null)
    {
        var item = Items.FirstOrDefault(i => i.itemId == id);
        string key = nameKey ?? item?.displayName_ID;
        string name = key != null && names.TryGetValue(key, out string translated) ? translated : key ?? "알 수 없는 아이템";
        return $"{id} / {name}";
    }

    public static string Pick(string label, string id, string search)
    {
        var available = Items.Where(i => string.IsNullOrEmpty(search) ||
            Label(i.itemId).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        var current = Items.FirstOrDefault(i => i.itemId == id);
        if (current != null && !available.Contains(current)) available.Insert(0, current);
        string[] labels = new[] { "(아이템 선택)" }.Concat(available.Select(i => $"{Label(i.itemId)} / 기력 {i.itemCost}")).ToArray();
        int index = available.FindIndex(i => i.itemId == id) + 1;
        int next = EditorGUILayout.Popup(label, index, labels);
        return next > 0 ? available[next - 1].itemId : null;
    }
}

public sealed class AIQAItemOfferWindow : EditorWindow
{
    private AIQAOfferRule human;
    private AIQAOfferRule ai;
    private string search = "";
    private Vector2 scroll;

    [MenuItem("Tools/AI QA/ItemOffer 지정")]
    public static void Open() => GetWindow<AIQAItemOfferWindow>("ItemOffer 지정");

    private void OnEnable()
    {
        minSize = new Vector2(540, 560);
        human = AIQAOfferOverrides.GetRule(false);
        ai = AIQAOfferOverrides.GetRule(true);
    }

    private void OnInspectorUpdate() => Repaint();

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Editor의 AI 모드 / MasterClient 전용. 이미 제공한 선택지는 변경하지 않고 다음 제안 생성부터 적용합니다.\n예약은 Editor 세션에 저장되며 Play 시작·새 방에서는 1회 예약이 다시 준비됩니다. 빌드에는 포함되지 않습니다.", MessageType.Info);
        if (Application.isPlaying && (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient ||
            GameManager.Instance == null || !GameManager.Instance.isSoloPlay))
            EditorGUILayout.HelpBox("현재 AI 모드의 MasterClient가 아닙니다. 예약 설정만 가능하며 실제 적용은 대기합니다.", MessageType.Warning);

        using (new EditorGUILayout.HorizontalScope())
        {
            search = EditorGUILayout.TextField("아이템 이름 / ID 검색", search);
            if (GUILayout.Button("목록 새로고침", GUILayout.Width(110))) AIQAEditorItems.Reload();
        }
        scroll = EditorGUILayout.BeginScrollView(scroll);
        DrawRule(false, "사람 플레이어", human);
        DrawRule(true, "AI 플레이어", ai);
        EditorGUILayout.Space();
        DrawLiveOffer();
        EditorGUILayout.LabelField("최근 지정 기록 (최대 50개)", EditorStyles.boldLabel);
        for (int i = AIQAOfferOverrides.Events.Count - 1; i >= 0; i--)
            EditorGUILayout.LabelField(AIQAOfferOverrides.Events[i], EditorStyles.wordWrappedLabel);
        EditorGUILayout.EndScrollView();
    }

    private void DrawRule(bool isAI, string title, AIQAOfferRule rule)
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            rule.enabled = EditorGUILayout.Toggle("지정 활성화", rule.enabled);
            rule.repeat = EditorGUILayout.Toggle("반복 적용", rule.repeat);
            bool specificTurn = rule.turnIndex >= 0;
            specificTurn = EditorGUILayout.Toggle("특정 TurnIndex만", specificTurn);
            rule.turnIndex = specificTurn ? Mathf.Max(0, EditorGUILayout.IntField("TurnIndex", Mathf.Max(0, rule.turnIndex))) : -1;
            rule.force = EditorGUILayout.Toggle("QA 강제 지정", rule.force);
            if (rule.force)
                EditorGUILayout.HelpBox("신약·도박 등 등장 제외와 등급 가중치를 무시합니다. ItemDB 등록, 서로 다른 3개, 인벤토리 용량 검사는 유지됩니다.", MessageType.Warning);
            if (rule.itemIds == null || rule.itemIds.Length != 3) rule.itemIds = new string[3];
            for (int i = 0; i < 3; i++) rule.itemIds[i] = AIQAEditorItems.Pick($"선택지 {i + 1}", rule.itemIds[i], search);

            var normalItems = AIQAEditorItems.Items.Where(i => i.itemId != "3001" && i.itemId != "4006" && i.itemId != "5000").ToList();
            bool valid = rule.TryResolve(AIQAEditorItems.Items, normalItems, out _, out string reason);
            if (rule.enabled && !valid) EditorGUILayout.HelpBox(reason, MessageType.Warning);
            EditorGUILayout.LabelField("예약 상태", AIQAOfferOverrides.GetStatus(isAI), EditorStyles.wordWrappedLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(rule.enabled && !valid))
                {
                    if (GUILayout.Button("설정 저장 / 다시 예약")) AIQAOfferOverrides.SetRule(isAI, rule);
                }
                if (GUILayout.Button("지정 해제"))
                {
                    rule.enabled = false;
                    AIQAOfferOverrides.SetRule(isAI, rule);
                }
            }
            EditorGUILayout.LabelField("화면의 변경사항은 ‘설정 저장 / 다시 예약’을 눌러야 반영됩니다.", EditorStyles.miniLabel);
        }
    }

    private static void DrawLiveOffer()
    {
        if (!Application.isPlaying || !PhotonNetwork.InRoom || PlayerManager.Instance == null) return;
        EditorGUILayout.LabelField("현재 제공된 선택지 (읽기 전용)", EditorStyles.boldLabel);
        foreach (int actor in PlayerManager.Instance.Players.Keys)
        {
            string offer = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.OFFER(actor));
            EditorGUILayout.LabelField($"Actor {actor}", string.IsNullOrEmpty(offer) ? "없음 / 선택 완료" : offer);
        }
    }
}

public sealed class AIQADecisionMonitorWindow : EditorWindow
{
    [Serializable] private sealed class Export { public List<AIQADecision> decisions; }
    private readonly string[] phaseLabels = { "전체", "제안 선택", "인벤토리 사용", "훔치기", "아이템 사용 후", "전투 수치 반영", "실제 공격 결과" };
    private Vector2 historyScroll;
    private Vector2 detailScroll;
    private int phase;
    private int actorFilter;
    private int selectedSequence = -1;
    private bool followLatest = true;
    private bool showLiveCombat = true;
    private bool showEffectSources;

    [MenuItem("Tools/AI QA/AI 판단 모니터")]
    public static void Open() => GetWindow<AIQADecisionMonitorWindow>("AI 판단 모니터");

    private void OnEnable() { minSize = new Vector2(920, 480); }
    private void OnInspectorUpdate() => Repaint();

    private void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            AIQADiagnostics.CaptureEnabled = GUILayout.Toggle(AIQADiagnostics.CaptureEnabled, "기록 수집", EditorStyles.toolbarButton, GUILayout.Width(80));
            followLatest = GUILayout.Toggle(followLatest, "최신 따라가기", EditorStyles.toolbarButton, GUILayout.Width(100));
            phase = EditorGUILayout.Popup(phase, phaseLabels, GUILayout.Width(125));
            GUILayout.Label("Actor (0=전체)", GUILayout.Width(90));
            actorFilter = EditorGUILayout.IntField(actorFilter, GUILayout.Width(45));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("JSON 내보내기", EditorStyles.toolbarButton)) ExportHistory();
            if (GUILayout.Button("기록 비우기", EditorStyles.toolbarButton)) { AIQADiagnostics.Clear(); selectedSequence = -1; }
        }
        EditorGUILayout.HelpBox("‘기록 수집’을 켜면 판단·아이템 사용 후 상태·실제 공격 결과를 최근 200개까지 보관합니다.\n전투 효과 표시는 읽기 전용이며 재평가·난수 추첨·상태 소모를 호출하지 않습니다. ‘최종 선택’은 계획입니다.", MessageType.Info);
        var records = AIQADiagnostics.History.Where(r => (phase == 0 || r.phase == phaseLabels[phase]) &&
            (actorFilter == 0 || r.actor == actorFilter)).ToList();
        if (followLatest && records.Count > 0) selectedSequence = records[records.Count - 1].sequence;
        AIQADecision selected = records.FirstOrDefault(r => r.sequence == selectedSequence);
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(285)))
            {
                EditorGUILayout.LabelField($"판단 이력 ({records.Count})", EditorStyles.boldLabel);
                historyScroll = EditorGUILayout.BeginScrollView(historyScroll);
                for (int i = records.Count - 1; i >= 0; i--)
                {
                    var record = records[i];
                    string label = $"#{record.sequence}  Actor {record.actor} / Turn {record.turnIndex}\n{record.phase} / Wave {record.wave}";
                    bool active = record.sequence == selectedSequence;
                    if (GUILayout.Toggle(active, label, "Button", GUILayout.Height(43)) && !active)
                    {
                        selectedSequence = record.sequence;
                        followLatest = false;
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            using (new EditorGUILayout.VerticalScope())
            {
                detailScroll = EditorGUILayout.BeginScrollView(detailScroll);
                showLiveCombat = EditorGUILayout.Foldout(showLiveCombat, "현재 전투 효과 (실시간)", true);
                showEffectSources = EditorGUILayout.ToggleLeft("효과 출처·중첩 상세 표시", showEffectSources);
                if (showLiveCombat)
                {
                    int liveActor = actorFilter > 0 ? actorFilter : PlayerManager.Instance != null ? PlayerManager.Instance.AIActNum : 0;
                    AIQACombatGUI.DrawLive(liveActor, showEffectSources);
                }
                if (selected != null) DrawDecision(selected, showEffectSources);
                else EditorGUILayout.LabelField("판단 기록을 선택하세요. 필터 변경·이력 삭제 후에는 ‘최신 따라가기’를 사용할 수 있습니다.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.EndScrollView();
            }
        }
    }

    private static void DrawDecision(AIQADecision record, bool showSources)
    {
        EditorGUILayout.LabelField($"#{record.sequence} {record.phase} / Actor {record.actor}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Turn {record.turnIndex} / Wave {record.wave} / Play +{record.time:F2}초");
        if (record.hasContext)
        {
            var c = record.context;
            EditorGUILayout.LabelField($"기력 {c.curEnergy} / 최대 {c.curMaxEnergy} / 예산 {(record.budget < 0 ? "미계산" : record.budget.ToString())}");
            EditorGUILayout.LabelField($"나무 HP {c.curTreeHP:F1} / 공격력 {c.curTreeToxicDmg:F1}");
            EditorGUILayout.LabelField($"내 마을 HP {c.curVillageHP:F1} / 상대 HP {c.curOppVillageHp:F1} / 내 방어막 {c.curVillageBarrier:F1}");
            EditorGUILayout.LabelField($"인벤토리 용량 {c.curInvCap} / 락픽 {c.LockpickCnt} / 상대 잠금 {c.OppLockCnt}");
        }
        if (record.hasThreshold) EditorGUILayout.LabelField($"선택 기준: 점수 > {record.threshold:F2} (동점 불가)");
        EditorGUILayout.HelpBox(record.summary ?? "", MessageType.None);
        AIQACombatGUI.DrawResolved(record.resolvedAttack);
        if (record.combat != null && record.combat.available)
        {
            EditorGUILayout.LabelField("기록 시점 전투 효과 (현재 값과 별도)", EditorStyles.boldLabel);
            AIQACombatGUI.Draw(record.combat, showSources);
        }
        for (int i = 0; i < record.candidates.Count; i++)
        {
            var row = record.candidates[i];
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField((i == record.selectedIndex ? "▶ " : "") + AIQAEditorItems.Label(row.itemId, row.nameKey), EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"UID {(row.uniqueId > 0 ? row.uniqueId.ToString() : "—")} / 비용 {row.cost} / 점수 {(row.scored ? row.score.ToString("F3") : "미평가")}");
                if (!string.IsNullOrEmpty(row.details)) EditorGUILayout.LabelField(row.details, EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField(row.reason ?? "", EditorStyles.wordWrappedLabel);
            }
        }
        if (record.hasContext && record.candidates.Count == 0) EditorGUILayout.LabelField("평가 후보 없음 / 단계 생략");
    }

    private static void ExportHistory()
    {
        string path = EditorUtility.SaveFilePanel("AI 판단 기록 저장", "", "AI_QA_Decisions.json", "json");
        if (string.IsNullOrEmpty(path)) return;
        try
        {
            File.WriteAllText(path, JsonUtility.ToJson(new Export { decisions = new List<AIQADecision>(AIQADiagnostics.History) }, true), new UTF8Encoding(false));
        }
        catch (Exception ex) { Debug.LogException(ex); }
    }
}
