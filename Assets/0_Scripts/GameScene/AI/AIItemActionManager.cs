using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using Photon.Pun;

public class AIItemActionManager : AILogicModule
{
    [Header("AI 락픽 피지컬 세팅")]
    public float baseLockPassRate = 0.8f;
    public float baseLockDuration = 1.0f;
    [SerializeField, Range(0.1f, 1f)] private float reinforcedLockPassRateMultiplier = 0.5f;

    [Tooltip("아이템을 사용하기 위한 최소 점수 조건")]
    public float usageThreshold = 50f;

    public async UniTask<bool> ProcessLockpickItem(CancellationToken token)
    {
        //AI 스탯 불러오기
        AIContext context = brain.GetCurAIStat(brain.MyActorNum);
        //락픽 사용 가능한 상태인지 확인
        if (!CheckLockpickPossible(context, out string blockReason))
        {
#if UNITY_EDITOR
            var blocked = AIQADiagnostics.Begin(brain.MyActorNum, "훔치기", context, threshold: usageThreshold);
            AIQADiagnostics.Publish(blocked, summary: blockReason);
#endif
            return false;
        }

        //가져올만한 아이템 계산
        ItemSO item = GetBestItemInOppInv(context);
        //만약 가져올만한 아이템이 없다면 락픽 사용하지 않음
        if (item == null) return false;

        //상대방 인벤토리 입구로 이동
        await brain.aINevMeshController.MoveToLocationAsync(LocationCommand.OPP_INV_ENTRY, token);

        //상대방 잠금 수에 비례한 락픽 해제 시간 적용
        await UniTask.Delay(System.TimeSpan.FromSeconds(baseLockDuration * context.OppLockCnt), cancellationToken: token);

        //락픽에 실패한 경우
        if (!TryLockPick(context))
        {
            //자신 자리로 되돌아 가기
            await brain.aINevMeshController.MoveToLocationAsync(LocationCommand.MY_HIT, token);
            return false;
        }

        //싱대방 인벤토리 안으로 들어가기
        await brain.aINevMeshController.MoveToLocationAsync(LocationCommand.OPP_INV, token);

        //TODO: 해당되는 아이템 가져오기
        await StealItemUsageAsync(item, token);

        //자리 돌아가기
        await brain.aINevMeshController.MoveToLocationAsync(LocationCommand.MY_HIT, token);
        return true;
    }


    //락픽 사용이 가능한지 판단하는 함수
    private bool CheckLockpickPossible(AIContext context, out string reason)
    {
        reason = null;
        //AI 플레이어 락픽 개수 카운트
        int lockpickCnt = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.LOCKPICK(brain.MyActorNum));
        if (lockpickCnt <= 0) { reason = "락픽 없음"; return false; }

        //각 플레이어 인벤토리 내 아이템 개수 기반 판단
        int AIItemInvMaxCnt = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(brain.MyActorNum));
        int AIItemsCnt = brain.GetPlayerItemCnt(brain.MyActorNum);
        int OppItemsCnt = brain.GetPlayerItemCnt(PhotonNetwork.LocalPlayer.ActorNumber);

        if (AIItemsCnt == -1)
        {
            reason = "AI 인벤토리 개수 조회 실패";
            Debug.LogError("AI Inv Cnt Error");
            return false;
        }
        if (AIItemsCnt >= AIItemInvMaxCnt) { reason = "AI 인벤토리 만석"; return false; }
        if (OppItemsCnt <= 0) { reason = "상대 아이템 없음"; return false; }

        return true;
    }



    //락픽 게임 시도
    private bool TryLockPick(AIContext context)
    {
        //AI 플레이어 인벤토리 객체 찾아오기
        if (PlayerManager.Instance.PlayersInv.TryGetValue((PhotonNetwork.LocalPlayer.ActorNumber), out var inv))
        {
            //인벤토리 방어 스크립트에서(InventoryBarrier.cs) 상호작용 호출
            //즉 게임 시작 처리
            //내부적으로 멤버 변수들만 초기화함
            inv.InvBarrier.OnInteract(gameObject.GetComponent<AIController>());
        }
        int oppActNum = PhotonNetwork.LocalPlayer.ActorNumber;
        //상대방 인벤토리 락 수 
        int extraLocks = context.OppLockCnt;
        //상대방 락수에 기반한 락픽 성공 확률 계산
        float finalSuccessProbability = Mathf.Pow(baseLockPassRate, extraLocks);

        bool isReinforced = PhotonPropertyHelper.GetRoomProp<bool>(ItemPropKeys.REINFORCED_LOCK(oppActNum));
        if (isReinforced)
        {
            finalSuccessProbability *= reinforcedLockPassRateMultiplier;
            PhotonPropertyHelper.SetRoomProp(ItemPropKeys.REINFORCED_LOCK(oppActNum), false);
            Debug.Log("[AI] 상대 강화 잠금장치 적용 -> 락픽 성공률 감소 및 소모");
        }
        //0.0~1.0 사이의 난수 뽑기
        float roll = UnityEngine.Random.value;

        if (roll <= finalSuccessProbability)
        {
            //성공 했음을 알림
            LockpickController.instance.IsAISuccess(true);
            Debug.Log("[AI 락픽 성공!]");
            return true;
        }
        else
        {
            LockpickController.instance.IsAISuccess(false);
            Debug.Log("[AI 락픽 실패]");
            return false;
        }
    }

    private ItemSO GetBestItemInOppInv(AIContext context)
    {
        int OppActNum = PhotonNetwork.LocalPlayer.ActorNumber;

        string OppInvStr = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(OppActNum));
        int OppInvCap = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(OppActNum));
        var InvSlots = ItemInfoSerializer.Decode(OppInvStr, OppInvCap);

        ItemSO bestItem = null;
        float highestScore = usageThreshold;
#if UNITY_EDITOR
        var qaDecision = AIQADiagnostics.Begin(brain.MyActorNum, "훔치기", context, threshold: usageThreshold);
        int qaSelected = -1;
#endif
        //인벤토리를 돌아본다.
        foreach (var slot in InvSlots)
        {
            //Debug.Log($"{slot.itemID}");
            ItemSO item = ItemDB.Instance.Get(slot.itemID);
            if (item == null) continue;
#if UNITY_EDITOR
            var qaRow = qaDecision?.Add(item, slot.uniqueId);
#endif

            //필터링 : 기력 부족으로 아예 사용 불가하면 패스
            if (item.itemCost > context.curEnergy)
            {
#if UNITY_EDITOR
                if (qaRow != null) qaRow.reason = $"기력 부족: 비용 {item.itemCost} > 기력 {context.curEnergy}";
#endif
                continue;
            }
            //필터링 : 위기 사항이 아닌데 이번 예산 기력을 넘어가면 pass, 기력 사용량이 0이면 통과
            int budget = brain.InventoryManager.CalcEnergyBudget(context);
#if UNITY_EDITOR
            if (qaDecision != null) qaDecision.budget = budget;
#endif
            if (item.itemCost > budget && item.itemCost > 0)
            {
#if UNITY_EDITOR
                if (qaRow != null) qaRow.reason = $"예산 초과: 비용 {item.itemCost} > 예산 {budget}";
#endif
                continue;
            }
            //필터링 : 제약 조건(턴 당 1회 등)에 걸리면 패스
            string usageBlock = ItemHandlingSystem.instance.GetItemUsageBlockReason(brain.MyActorNum, item.itemId);
            if (usageBlock != null)
            {
#if UNITY_EDITOR
                if (qaRow != null) qaRow.reason = "사용 제한: " + usageBlock;
#endif
                continue;
            }


            // 아이템 점수 계산(아이템 선택 로직과 동일한 알고리즘)
            // 예외 상황에 대한 처리 필요(희생 아이템인데 보유 아이템이 1개뿐인 경우 등)
            float utility = brain.InventoryManager.EvaluateUtilityCurves(item, context);
            float gimmick = brain.InventoryManager.EvaluateGimmicks(item, context);
            float score = utility + gimmick;
#if UNITY_EDITOR
            if (qaDecision != null) qaDecision.Score(qaRow, score, $"효용 {utility:F2} / 기믹 {gimmick:F2} (훔치기에는 정화 추가 평가 없음)");
#endif

            //if (item.itemId == "3003") score += 10000;

            Debug.Log($"이름 : {item.displayName_ID}, 점수 : {score}");

            if (score > highestScore)
            {
                highestScore = score;
                bestItem = item;
#if UNITY_EDITOR
                if (qaDecision != null) qaSelected = qaDecision.candidates.Count - 1;
#endif
            }
        }

#if UNITY_EDITOR
        AIQADiagnostics.Publish(qaDecision, qaSelected, bestItem == null ? "훔칠 후보 없음" : "훔칠 대상 선택 — 락픽 성공·이전 결과와 별도");
#endif
        return bestItem;
    }

    private async UniTask StealItemUsageAsync(ItemSO item, CancellationToken token)
    {
        //ai 인벤토리에서의 아이템 소모 및 효과 적용
        if (PlayerManager.Instance.PlayersInv.TryGetValue((PhotonNetwork.LocalPlayer.ActorNumber), out var inv))
        {
            //inv.InvBarrier.OnInteract(gameObject.GetComponent<AIController>())
            inv.InteractSlotForStealByAI(gameObject.GetComponent<AIController>(), item);
            await UniTask.Delay(1000, cancellationToken: token);
        }
    }
}
