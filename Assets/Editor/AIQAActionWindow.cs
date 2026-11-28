using Photon.Pun;
using UnityEditor;
using UnityEngine;

public sealed class AIQAActionWindow : EditorWindow
{
    private static readonly string[] PhaseLabels =
    {
        "중단점 없음", "제안 선택 전", "첫 훔치기 전", "자기 인벤토리 도착 후",
        "두 번째 훔치기 전", "두 번째 훔치기 시도 직후", "추가 인벤토리 도착 후 (도난 성공 시)", "나무 공격 직전"
    };
    private AIQAActionSettings settings;
    private AIQAReservationPlan plan;
    private Vector2 scroll;
    private string search = "";

    [MenuItem("Tools/AI QA/AI 행동 제어 · 사용 예약")]
    public static void Open() => GetWindow<AIQAActionWindow>("AI 행동 · 사용 예약");

    private void OnEnable()
    {
        minSize = new Vector2(650, 650);
        settings = AIQAActionControl.GetSettings();
        plan = AIQAActionControl.GetPlan();
    }

    private void OnInspectorUpdate() => Repaint();

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Editor의 AI 모드 / MasterClient 전용. AI가 지정 단계에 도달하면 대기합니다. Unity 전체 Pause 기능과는 다릅니다.\n아이템은 미리 보유해야 하며 자동 지급하지 않습니다. 화면 변경 후 적용·예약 버튼을 누르세요.", MessageType.Info);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.LabelField("1. AI 단계 제어", EditorStyles.boldLabel);
        settings.enabled = EditorGUILayout.Toggle("QA 제어 활성화", settings.enabled);
        settings.pauseEveryPhase = EditorGUILayout.Toggle("모든 단계에서 대기", settings.pauseEveryPhase);
        using (new EditorGUI.DisabledScope(settings.pauseEveryPhase))
            settings.pauseAt = (AIQAPhase)EditorGUILayout.Popup("대기할 단계", (int)settings.pauseAt, PhaseLabels);
        settings.freezeTimer = EditorGUILayout.Toggle("대기 중 턴 타이머 정지", settings.freezeTimer);
        if (GUILayout.Button("중단점 설정 적용")) AIQAActionControl.SaveSettings(settings);

        var gate = AIQAActionControl.Held;
        EditorGUILayout.HelpBox(gate == null ? AIQAActionControl.Status :
            $"대기: Actor {gate.actor} / Turn {gate.turn} / {PhaseLabels[(int)gate.phase]}\n" +
            (gate.ownsTimerPause ? "QA가 턴 타이머를 정지 중" : "QA가 정지한 타이머 없음"), MessageType.None);
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(gate == null || AIQAActionControl.Busy))
            {
                if (GUILayout.Button("현재 단계 진행")) AIQAActionControl.Release();
                if (GUILayout.Button("다음 단계까지 진행")) AIQAActionControl.Step();
            }
            if (GUILayout.Button("모든 예약 · 대기 해제"))
            {
                AIQAActionControl.DisableAll();
                settings = AIQAActionControl.GetSettings();
                plan = AIQAActionControl.GetPlan();
            }
        }
        if (AIQAActionControl.Busy)
            EditorGUILayout.HelpBox("사용 요청 처리 중입니다. 해제해도 이미 보낸 요청·소모는 자동 취소하거나 환불하지 않습니다.", MessageType.Info);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("2. 아이템 사용 예약", EditorStyles.boldLabel);
        bool valid = true;
        int remove = -1, moveFrom = -1, moveTo = -1;
        using (new EditorGUI.DisabledScope(AIQAActionControl.Busy))
        {
            plan.useInventoryAfterReservations = EditorGUILayout.Popup("모든 예약 처리 후", plan.useInventoryAfterReservations ? 0 : 1,
                new[] { "추가 인벤토리 사용 진행", "추가 인벤토리 사용 건너뛰기" }) == 0;
            EditorGUILayout.HelpBox("이번 턴의 강제 사용 예약이 모두 처리될 때까지 자연 인벤토리 사용을 보류합니다. 처리 후 위 옵션에 따라 1회 진행하거나 건너뜁니다.\n같은 단계는 목록 순서대로, 서로 다른 단계는 AI 진행 순서대로 실행합니다. 관찰 예약만 있거나 해당 턴의 예약이 없으면 기존 행동을 유지합니다.", MessageType.Info);
            search = EditorGUILayout.TextField("이름 / ID 검색", search);
            for (int i = 0; i < plan.reservations.Count; i++)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField($"예약 #{i + 1}", EditorStyles.boldLabel);
                    valid &= DrawReservation(plan.reservations[i]);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledScope(i == 0))
                            if (GUILayout.Button("위로")) { moveFrom = i; moveTo = i - 1; }
                        using (new EditorGUI.DisabledScope(i == plan.reservations.Count - 1))
                            if (GUILayout.Button("아래로")) { moveFrom = i; moveTo = i + 1; }
                        if (GUILayout.Button("예약 삭제")) remove = i;
                    }
                }
            }
            if (remove >= 0) plan.reservations.RemoveAt(remove);
            else if (moveFrom >= 0)
            {
                var row = plan.reservations[moveFrom];
                plan.reservations.RemoveAt(moveFrom);
                plan.reservations.Insert(moveTo, row);
            }
            if (GUILayout.Button("아이템 예약 추가"))
                plan.reservations.Add(new AIQAUseReservation { id = System.Guid.NewGuid().ToString("N"), enabled = true, mode = AIQAUseMode.ForceUse });
            using (new EditorGUI.DisabledScope(!valid))
                if (GUILayout.Button("전체 예약 저장 / 다시 예약"))
                {
                    AIQAActionControl.SaveSettings(settings);
                    AIQAActionControl.ArmPlan(plan);
                }
        }
        EditorGUILayout.LabelField("변경 후 전체 예약을 저장하세요. 저장하면 모든 단발 예약을 다시 준비합니다. 각 예약은 단계·턴당 1회 실행합니다.", EditorStyles.wordWrappedMiniLabel);

        EditorGUILayout.Space();
        DrawInventory(gate);
        if (Application.isPlaying && PhotonNetwork.InRoom && PlayerManager.Instance != null)
        {
            EditorGUILayout.LabelField("현재 전투 효과 (읽기 전용)", EditorStyles.boldLabel);
            AIQACombatGUI.DrawLive(gate?.actor ?? PlayerManager.Instance.AIActNum, false);
        }
        EditorGUILayout.LabelField("실행 결과 (최근 100개)", EditorStyles.boldLabel);
        for (int i = AIQAActionControl.History.Count - 1; i >= 0; i--)
            EditorGUILayout.LabelField(AIQAActionControl.History[i], EditorStyles.wordWrappedLabel);
        EditorGUILayout.EndScrollView();
    }

    private bool DrawReservation(AIQAUseReservation reservation)
    {
        reservation.enabled = EditorGUILayout.Toggle("사용 예약 활성화", reservation.enabled);
        reservation.phase = (AIQAPhase)EditorGUILayout.Popup("예약 시점", (int)reservation.phase, PhaseLabels);
        reservation.mode = (AIQAUseMode)EditorGUILayout.Popup("시험 방식", (int)reservation.mode,
            new[] { "자연 판단 관찰 (강제 사용 안 함)", "강제 사용 (권한 검증 유지)" });
        reservation.itemId = AIQAEditorItems.Pick("대상 아이템", reservation.itemId, search);
        reservation.uniqueId = Mathf.Max(0, EditorGUILayout.IntField("사용 UID (0=첫 보유품)", reservation.uniqueId));
        if (reservation.mode == AIQAUseMode.ForceUse)
        {
            reservation.useCount = Mathf.Clamp(EditorGUILayout.IntField("연속 사용 횟수", reservation.UseCount), 1, 20);
            if (reservation.subsequentUniqueIds == null) reservation.subsequentUniqueIds = new System.Collections.Generic.List<int>();
            for (int i = 1; i < reservation.UseCount; i++)
            {
                while (reservation.subsequentUniqueIds.Count < i) reservation.subsequentUniqueIds.Add(0);
                reservation.subsequentUniqueIds[i - 1] = Mathf.Max(0, EditorGUILayout.IntField($"{i + 1}회차 UID (0=자동)", reservation.subsequentUniqueIds[i - 1]));
            }
            if (reservation.UseCount > 1)
                EditorGUILayout.HelpBox("각 회차의 사용 완료·소모를 확인한 뒤 다음 보유품을 사용합니다. 같은 아이템을 횟수만큼 미리 준비하세요. 거절·확인 불가·턴 변경 시 나머지는 중단합니다. 매 턴 반복과는 별도입니다.", MessageType.Info);
        }
        if (reservation.itemId == "2002")
        {
            reservation.sacrificeUid = Mathf.Max(0, EditorGUILayout.IntField("제물 UID (0=자동 선택)", reservation.sacrificeUid));
            if (reservation.mode == AIQAUseMode.ForceUse && reservation.UseCount > 1)
                EditorGUILayout.HelpBox("제물 UID는 매 회차 동일하게 적용됩니다. 연속 사용은 0(자동)을 권장합니다. 자동 선택은 후속 사용용 예약품을 제외합니다.", MessageType.Info);
        }
        bool specificTurn = EditorGUILayout.Toggle("특정 TurnIndex만", reservation.turnIndex >= 0);
        reservation.turnIndex = specificTurn
            ? Mathf.Max(0, EditorGUILayout.IntField("TurnIndex", Mathf.Max(0, reservation.turnIndex))) : -1;
        reservation.repeat = EditorGUILayout.Toggle("매 턴 반복", reservation.repeat);
        reservation.pauseOnReach = EditorGUILayout.Toggle("예약 시점에서 먼저 대기", reservation.pauseOnReach);
        if (reservation.mode == AIQAUseMode.ForceUse)
            EditorGUILayout.HelpBox("AI의 점수·기력 예산 판단을 우회합니다. 실제 기력·자기 턴·사용 제한·보유 검증은 유지됩니다.\n예약 아이템은 해당 턴의 자연 사용·자동 제물 선택에서 보관됩니다. 강제 사용 결과를 자연 판단 통과로 기록하지 마세요.", MessageType.Info);
        else
            EditorGUILayout.HelpBox("아이템 사용을 강제하거나 보관하지 않습니다. 이 시점부터 자연 행동과 판단 모니터를 확인하세요.", MessageType.Info);
        bool valid = !reservation.enabled || (reservation.phase != AIQAPhase.None && !string.IsNullOrEmpty(reservation.itemId));
        if (!valid) EditorGUILayout.HelpBox("예약 시점과 아이템을 선택하세요.", MessageType.Warning);
        if (reservation.enabled && !settings.enabled)
            EditorGUILayout.HelpBox("실행하려면 QA 제어 활성화를 켜고 적용하세요.", MessageType.Warning);
        return valid;
    }

    private static void DrawInventory(AIQAActionGate gate)
    {
        if (!Application.isPlaying || !PhotonNetwork.InRoom || PlayerManager.Instance == null) return;
        int actor = gate?.actor ?? PlayerManager.Instance.AIActNum;
        if (actor <= 0) return;
        EditorGUILayout.LabelField($"AI 보유품 / Actor {actor} (읽기 전용)", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"기력 {PhotonPropertyHelper.GetPlayerProp<int>(actor, PlayerPropKeys.Energy)} / 최대 {PhotonPropertyHelper.GetPlayerProp<int>(actor, PlayerPropKeys.MaxEnergy)}");
        int cap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(actor));
        var slots = ItemInfoSerializer.Decode(PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(actor)), cap);
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].uniqueId <= 0) continue;
            EditorGUILayout.LabelField($"슬롯 {i} / UID {slots[i].uniqueId} / {AIQAEditorItems.Label(slots[i].itemID)}", EditorStyles.wordWrappedLabel);
        }
    }
}
