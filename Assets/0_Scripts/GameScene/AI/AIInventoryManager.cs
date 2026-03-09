using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System;

public class AIInventoryManager : AILogicModule
{
    [HideInInspector]
    public WorldInventory AIInv { private get; set; }

    [Header("AI 아이템 사용 설정을 위한 점수표")]
    public AIItemScoreTableSO scoreTable;

    [Header("스냅샷 생성을 위한 so")]
    public PlayerSetting playerSetting;
    public RoomSetting roomSetting;

    [Tooltip("아이템을 사용하기 위한 최소 점수 조건")]
    public float usageThreshold = 50f;

    public float useDelay = 1.2f;


    public async UniTask ProcessInventoryAsync()
    {
        Debug.Log($"[AI {brain.MyActorNum}] 인벤토리 아이템 사용 판단 시작");

        bool canUseMore = true;
        //무한 루프 방지용 장치
        int safetyLoopCount = 0;
        int safetyLoopCountMax = playerSetting.inventoryCapacity;

        while (canUseMore && safetyLoopCount < safetyLoopCountMax)
        {
            safetyLoopCount++;

            await UniTask.Delay(System.TimeSpan.FromSeconds(useDelay), cancellationToken: this.GetCancellationTokenOnDestroy());

            //  1. 스냅샷 생성
            AIContext context = brain.GetCurAIStat(brain.MyActorNum);

            //  2. 기력 예산 편성
            int energyBudeget = CalcEnergyBudget(context);

            //  3. AI 인벤토리 가져오기
            string InvStr = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(brain.MyActorNum));
            int capacity = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(brain.MyActorNum));
            var InvSlots = ItemInfoSerializer.Decode(InvStr, capacity);

            ItemSO bestItemToUse = null;
            float highestScore = usageThreshold;

            //인벤토리를 돌아본다.
            foreach (var slot in InvSlots)
            {
                //Debug.Log($"{slot.itemID}");
                ItemSO item = ItemDB.Instance.Get(slot.itemID);
                if (item == null) continue;


                //필터링 : 기력 부족으로 아예 사용 불가하면 패스
                if (item.itemCost > context.curEnergy) continue;
                //필터링 : 위기 사항이 아닌데 이번 예산 기력을 넘어가면 pass, 기력 사용량이 0이면 통과
                if (item.itemCost > energyBudeget && item.itemCost > 0) continue;
                //필터링 : 제약 조건(턴 당 1회 등)에 걸리면 패스
                if (!ItemHandlingSystem.instance.CheckItemAvaiable(brain.MyActorNum, item.itemId)) continue;


                //  4. 아이템 점수 계산(아이템 선택 로직과 동일한 알고리즘)
                // 예외 상황에 대한 처리 필요(희생 아이템인데 보유 아이템이 1개뿐인 경우 등)
                float score = EvaluateUtilityCurves(item, context);
                score += EvaluateGimmicks(item, context);

                Debug.Log($"이름 : {item.displayName_ID}, 점수 : {score}");

                if (score > highestScore)
                {
                    highestScore = score;
                    bestItemToUse = item;
                }
            }

            if (bestItemToUse != null)
            {
                Debug.Log($"[AI {brain.MyActorNum}] '{bestItemToUse.displayName_ID}' 사용! (점수 : {highestScore:F1}, 아이템 기력: {bestItemToUse.itemCost}");
                //구현 예정
                await ExecuteItemUsageAsync(bestItemToUse);
            }
            else
            {
                //만족하는 아이템이 없으면 그만 찾는다.
                Debug.Log($"[AI {brain.MyActorNum}] 더 이상 쓸만한 아이템이 없거나 기력을 아낍니다. 턴 종료. (가장 높은 점수 : {highestScore:F1})");
                canUseMore = false;
            }
        }
    }

    //현재 기력 기준 기력 에산 계산
    private int CalcEnergyBudget(AIContext ctx)
    {
        int remainTurns = roomSetting.maxWave - ctx.curWaveCnt;

        if (IsEmergency(ctx)) return ctx.curEnergy;

        return Mathf.CeilToInt((float)ctx.curEnergy / remainTurns);
    }

    //현재 위기상황 혹은 킬각 인지 판단
    private bool IsEmergency(AIContext ctx)
    {
        float villageHPRatio = ctx.curVillageHP / playerSetting.villageHP;
        float oppVillageHPRatio = ctx.curOppVillageHp / playerSetting.villageHP;
        float treeHPRatio = ctx.curTreeHP / roomSetting.treeHP;
        return (villageHPRatio <= scoreTable.EmgVillageHPRatio
            || treeHPRatio <= scoreTable.EmgTreeHpRatio
            || oppVillageHPRatio <= scoreTable.EmgVillageHPRatio);
    }

    //곡선 평가 로직을 통해 각 상황에 맞게 아이템에 점수 부여
    private float EvaluateUtilityCurves(ItemSO item, AIContext ctx)
    {
        float curveScore = 0f;
        float villageHPRatio = ctx.curVillageHP / playerSetting.villageHP;
        float treeHPRatio = ctx.curTreeHP / roomSetting.treeHP;

        if (item.type == ItemType.Heal)
        {
            if (item.target == ItemTarget.SelfVillage)
            {
                float deficit = 1.0f - villageHPRatio;
                curveScore += scoreTable.defVillageMaxScore * Mathf.Pow(deficit, 2f);
            }
            else if (item.target == ItemTarget.Tree)
            {
                float deficit = 1.0f - treeHPRatio;
                curveScore += scoreTable.healTreeMaxScore * Mathf.Pow(deficit, 2f);
            }
            else
            {
                //기력 회복주 아이템
                if (item.itemId == "3002")
                {
                    int avg = GetInvItemCostAvg(ctx.curInvStr, ctx.curInvCap);
                    if (avg < ctx.curEnergy + 3)
                        curveScore += scoreTable.healEnergyScore;
                }
                //TODO : 기력 이월 아이템 제작 시 해당 코드 또한 별도 적용
                //if(item.itemId == "3003")
            }
        }
        else if (item.type == ItemType.Defence && item.target == ItemTarget.SelfVillage)
        {
            float deficit_1 = 1.0f - villageHPRatio;
            float barrier_toxic_Ratio = ctx.curVillageBarrier / ctx.curTreeToxicDmg;
            float deficit_2 = 1.0f - barrier_toxic_Ratio;
            float deficit = (deficit_1 + deficit_2) / 2.0f;

            curveScore += scoreTable.defVillageMaxScore * Mathf.Pow(deficit, 2f);
        }
        else if (item.type == ItemType.Damage && item.target == ItemTarget.Tree)
        {
            curveScore += scoreTable.dmgTreeMaxScore * Mathf.Pow(treeHPRatio, 2f);
        }

        return curveScore;
    }


    private float EvaluateGimmicks(ItemSO item, AIContext ctx)
    {
        float gimmickScore = 0f;
        float villageHPRatio = ctx.curVillageHP / playerSetting.villageHP;
        float oppVillageHPRatio = ctx.curOppVillageHp / playerSetting.villageHP;
        float treeHPRatio = ctx.curTreeHP / roomSetting.treeHP;

        if (item.type != ItemType.Gimmick) return gimmickScore;

        if (HasTag(item, TagMask.Positive) && ItemHandlingSystem.instance.HasDebuff(brain.MyActorNum))
        {
            gimmickScore += scoreTable.purifyBonus;
        }
        else if (HasTag(item, TagMask.Curse) || HasTag(item, TagMask.Negative))
        {
            if (item.target == ItemTarget.OpponentVillage)
            {
                float deficit = 1.0f - oppVillageHPRatio;
                gimmickScore += scoreTable.killCatchBonus * deficit;
            }
            else if (item.target == ItemTarget.OpponentTree)
            {
                float deficit = 1.0f - treeHPRatio;
                gimmickScore += scoreTable.killCatchBonus * Mathf.Pow(deficit, 2f);
            }
        }
        //TODO : 기력 도박 아이템 점수 부여 로직 구성
        //else if(item.itemId == "4006")
        else
        {
            //각 값이 50% 이상인 경우 유효한 점수 부여
            //50% 이하이면 점수 부여 안함
            float villageComfort = Mathf.Clamp01((villageHPRatio - 0.5f) * 2f);
            float treeComfort = Mathf.Clamp01((treeHPRatio - 0.5f) * 2f);

            float totalComfort = villageComfort * treeComfort;

            float totalScore = scoreTable.gimmicBonus * totalComfort;

            gimmickScore += totalScore;
        }

        return gimmickScore;
    }

    private int GetInvItemCostAvg(string Inv, int Cap)
    {
        var inv = ItemInfoSerializer.Decode(Inv, Cap);
        return ItemInfoSerializer.GetItemCostAvg(inv);
    }

    private bool HasTag(ItemSO item, TagMask targetTag)
    {
        foreach (var effect in item.effects)
        {
            if (effect.effectType == ItemEffect.AddStatus && effect.statusSpce != null)
            {
                if (effect.statusSpce.tags.HasFlag(targetTag)) return true;
            }
        }
        return false;
    }

    private async UniTask ExecuteItemUsageAsync(ItemSO item)
    {
        //ai 인벤토리에서의 아이템 소모 및 효과 적용
        AIInv.InteractSlotByAI(gameObject.GetComponent<AIController>(), item);
        await UniTask.Delay(1000, cancellationToken: this.GetCancellationTokenOnDestroy());
    }
}
