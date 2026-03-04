using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public struct AIContext
{
    public int curEnergy;
    public int curMaxEnergy;

    public float curTreeHP;
    public float curTreeToxicDmg;
    public float curVillageHP;
    public float curOppVillageHp;
    public float curVillageBarrier;

    public int curInvCap;
    public string curInvStr;
    public string curOppInvStr;

    public int maxWaveCnt;
    public int curWaveCnt;
}

public class AIItemSelector : AILogicModule
{
    public AIItemScoreTableSO scoreTable;
    public PlayerSetting playerSetting;
    public RoomSetting roomSetting;

    public int AIThinkDelay_ms = 3000;

    public async UniTask ChooseItemAsync(string offerString)
    {
        await UniTask.Delay(AIThinkDelay_ms, cancellationToken: this.GetCancellationTokenOnDestroy());

        //  1. 스냅샷 생성
        AIContext context = GetCurAIStat(brain.MyActorNum);

        ItemSO bestItem = null;
        float highestScore = -9999f;

        Debug.Log(offerString);
        var ItemList = offerString.Split("|");
        if (ItemList == null)
        {
            Debug.LogError("Item Offer is NULL!");
            return;
        }

        foreach (var itemId in ItemList)
        {
            Debug.Log($"{itemId}");
            ItemSO item = ItemDB.Instance.Get(itemId);
            if (item == null)
            {
                Debug.LogError("Item is null");
                continue;
            }

            float score = EvaluateItemWithCurves(item, context);
            Debug.Log($"[AI {brain.MyActorNum}] {item.displayName_ID} 평가 점수(Curve) : {score:F1}");

            if (score > highestScore)
                bestItem = item;
        }

        InsertItemToInv(bestItem, context);
    }

    private void InsertItemToInv(ItemSO item, AIContext ctx)
    {
        var slots = ItemInfoSerializer.Decode(ctx.curInvStr, ctx.curInvCap);

        //다음 아이템 고유 아이디 가져오기
        int nextUid = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.NEXT_UID);
        //플레이어 아이템 인벤토리 첫 칸에 해당 아이템 삽입 시도하기
        if (!ItemInfoSerializer.TryAddFirstEmpty(slots, (nextUid, item.itemId)))
        {
            //실패시
            Debug.LogError("Item Insertion ERROR");
            return;
        }

        //삽입 결과, 변경 결과 프로퍼티로 한번에 업데이트
        var ht = new ExitGames.Client.Photon.Hashtable
        {
            {ItemPropKeys.INV(brain.MyActorNum),ItemInfoSerializer.Encode(slots)},
            {ItemPropKeys.NEXT_UID, nextUid + 1},
            {ItemPropKeys.OFFER(brain.MyActorNum), "" },

        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
    }

    private float EvaluateItemWithCurves(ItemSO item, AIContext ctx)
    {
        float score = 0f;

        score += EvaluateDuplicateItem(item, ctx);

        score += EvaluateConstraints(item, ctx);

        score += GetClassBaseScore(item.itemClass);

        score += EvaluateUtilityCurves(item, ctx);

        score += EvaluateGimmicks(item, ctx);

        return score;
    }

    //중복된 아이템에 따른 패널티 부여
    private float EvaluateDuplicateItem(ItemSO item, AIContext ctx)
    {
        float score = 0f;

        var inv = ItemInfoSerializer.Decode(ctx.curInvStr, ctx.curInvCap);

        int DupCnt = ItemInfoSerializer.FindDuplicatedItem(inv, item.itemId);

        float ratio = Mathf.Min(DupCnt, 3) / 3.0f;

        return score += scoreTable.duplicateItemPenalty * Mathf.Pow(ratio, 2f);
    }

    //아이템 사용 불가능 상황에 패널티 부여
    private float EvaluateConstraints(ItemSO item, AIContext ctx)
    {
        float penalty = 0f;
        if (item.itemCost > ctx.curMaxEnergy) penalty += scoreTable.impossibleCostPenalty;
        else if (item.itemCost > ctx.curEnergy) penalty += scoreTable.lackEnergyPenalty;

        int leftSlotCnt = GetInvLeftSlots(ctx.curInvStr, ctx.curInvCap);
        if (leftSlotCnt == 1 && item.itemCost > ctx.curEnergy)
        {
            penalty += scoreTable.fullInvPenalty;
        }
        return penalty;
    }

    //아이템 희귀도 기반 점수 반환
    private float GetClassBaseScore(ItemClass iClass)
    {
        return iClass switch
        {
            ItemClass.Common => scoreTable.classCommon,
            ItemClass.Hero => scoreTable.classHero,
            ItemClass.Rare => scoreTable.classRare,
            ItemClass.Legendary => scoreTable.classLegendary,
            _ => 0f
        };
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




    //인벤토리 남은 자리 수 계산
    private int GetInvLeftSlots(string Inv, int Cap)
    {
        var inv = ItemInfoSerializer.Decode(Inv, Cap);
        int itemCnt = ItemInfoSerializer.GetItemCntInInv(inv);
        return Cap - itemCnt;
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

    private AIContext GetCurAIStat(int aiNum)
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        AIContext context = new AIContext();

        context.curTreeHP = GetValue<float>(props, RoomPropKeys.TreeHP);
        context.curInvCap = GetValue<int>(props, ItemPropKeys.INV_CAPACITY(aiNum));
        context.curInvStr = GetValue<string>(props, ItemPropKeys.INV(aiNum));
        context.curOppInvStr = GetValue<string>(props, ItemPropKeys.INV(PhotonNetwork.LocalPlayer.ActorNumber));
        context.maxWaveCnt = GetValue<int>(props, RoomPropKeys.MaxWaveCnt);
        context.curWaveCnt = GetValue<int>(props, RoomPropKeys.CurrentWave);
        context.curTreeToxicDmg = GetValue<float>(props, RoomPropKeys.TreeAtkPow);

        string attachedKey = $"_{aiNum}";
        context.curVillageHP = GetValue<float>(props, PlayerPropKeys.VillageHP + attachedKey);
        context.curVillageBarrier = GetValue<float>(props, PlayerPropKeys.VillageBarrier + attachedKey);
        context.curEnergy = GetValue<int>(props, PlayerPropKeys.Energy + attachedKey);
        context.curMaxEnergy = GetValue<int>(props, PlayerPropKeys.MaxEnergy + attachedKey);

        context.curOppVillageHp = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageHP);
        return context;
    }

    private T GetValue<T>(ExitGames.Client.Photon.Hashtable prop, string key)
    {
        if (prop.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        return default(T);
    }

}
