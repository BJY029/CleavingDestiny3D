using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;
using Photon.Pun;

public class AIItemActionManager : AILogicModule
{
    [Header("AI 락픽 피지컬 세팅")]
    public float baseLockPassRate = 0.8f;
    public float baseLockDuration = 1.0f;

    public async UniTask ProcessLockpickItem(CancellationToken token)
    {
        AIContext context = brain.GetCurAIStat(brain.MyActorNum);

        if (context.LockpickCnt <= 0) return;

        await brain.aINevMeshController.MoveToLocationAsync(LocationCommand.OPP_INV_ENTRY, token);

        //상대방 잠금 수에 비례한 락픽 해제 시간 적용
        await UniTask.Delay(System.TimeSpan.FromSeconds(baseLockDuration * context.OppLockCnt), cancellationToken: token);

        //락픽에 실패한 경우
        if (!TryLockPick(context))
        {
            //자신 자리로 되돌아 가기
            await brain.aINevMeshController.MoveToLocationAsync(LocationCommand.MY_HIT, token);
            return;
        }

        //싱대방 인벤토리 안으로 들어가기
        await brain.aINevMeshController.MoveToLocationAsync(LocationCommand.OPP_INV, token);

        //가져올만한 아이템 계산
        ItemSO item = GetBestItemInOppInv(context);

        //TODO: 해당되는 아이템 가져오기
        await StealItemUsageAsync(item, token);

        //자리 돌아가기
        await brain.aINevMeshController.MoveToLocationAsync(LocationCommand.MY_HIT, token);
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
        //상대방 인벤토리 락 수 
        int extraLocks = context.OppLockCnt;
        //상대방 락수에 기반한 락픽 성공 확률 계산
        float finalSuccessProbability = Mathf.Pow(baseLockPassRate, extraLocks);
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
        float highestScore = -999f;
        //인벤토리를 돌아본다.
        foreach (var slot in InvSlots)
        {
            //Debug.Log($"{slot.itemID}");
            ItemSO item = ItemDB.Instance.Get(slot.itemID);
            if (item == null) continue;
            //필터링 : 제약 조건(턴 당 1회 등)에 걸리면 패스
            if (!ItemHandlingSystem.instance.CheckItemAvaiable(brain.MyActorNum, item.itemId)) continue;


            //  4. 아이템 점수 계산(아이템 선택 로직과 동일한 알고리즘)
            // 예외 상황에 대한 처리 필요(희생 아이템인데 보유 아이템이 1개뿐인 경우 등)
            float score = brain.InventoryManager.EvaluateUtilityCurves(item, context);
            score += brain.InventoryManager.EvaluateGimmicks(item, context);

            Debug.Log($"이름 : {item.displayName_ID}, 점수 : {score}");

            if (score > highestScore)
            {
                highestScore = score;
                bestItem = item;
            }
        }

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
