using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHandlingInSim : MonoBehaviour
{
    public static ItemHandlingInSim Instance;
    private StatusSystem _statusSystem;
    private GameEventBus _gameEvenetBus;
    private DamageResolver _damageResolver;
    private DeterministicRng _rng;

    private Dictionary<int, List<string>> UsedTurnItem;
    private Dictionary<int, List<string>> UsedDayItem;
    private Dictionary<int, List<string>> UsedGameItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _statusSystem = new StatusSystem();
        _gameEvenetBus = new GameEventBus(_statusSystem);
        _damageResolver = new DamageResolver(_gameEvenetBus, _statusSystem);

        UsedTurnItem = new Dictionary<int, List<string>>();
        UsedDayItem = new Dictionary<int, List<string>>();
        UsedGameItem = new Dictionary<int, List<string>>();

        //Turn 시드 값 설정 및 적용
        //Turn이 변경될 때마다 재설정 된다.
        InitRandomSystem();
    }

    public void InitRandomSystem()
    {
        int seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        Debug.Log($"[InitRandomSystem] Generated Turn Seed: {seed}");
        _rng = new DeterministicRng(seed);
    }

    /// <summary>
    /// Manage and control items that can only be used once in a day in a list
    /// </summary>
    /// <param name="itemId"></param>
    public void AddUsedDayItem(int actorNum, string itemId)
    {
        if (!UsedDayItem.ContainsKey(actorNum))
            UsedDayItem.Add(actorNum, new List<string>());
        UsedDayItem[actorNum].Add(itemId);
    }

    /// <summary>
    /// When the day is reset, it is reset.
    /// </summary>
    public void ClearUsedDayItem()
    {
        foreach (var I in UsedDayItem)
        {
            I.Value.Clear();
        }
    }

    /// <summary>
    /// Manage and control items that can only be used once in a turn in a list
    /// </summary>
    /// <param name="itemId"></param>
    public void AddUsedTurnItem(int actorNum, string itemId)
    {
        if (!UsedTurnItem.ContainsKey(actorNum))
            UsedTurnItem.Add(actorNum, new List<string>());

        UsedTurnItem[actorNum].Add(itemId);
    }

    /// <summary>
    /// When the turn is reset, it is reset.
    /// </summary>
    public void ClearUsedTurnItem()
    {
        foreach (var I in UsedTurnItem)
        {
            I.Value.Clear();
        }
    }

    public void AddUsedGameItem(int actorNum, string itemId)
    {
        if (!UsedGameItem.ContainsKey(actorNum))
            UsedGameItem.Add(actorNum, new List<string>());
        UsedGameItem[actorNum].Add(itemId);
    }

    /// <summary>
    /// Check if the item is available (check turn, wave).
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public bool CheckItemAvaiable(int actorNum, string itemId)
    {
        // 1. UsedGameItem에 키가 있는지 확인 후 검사
        if (UsedGameItem.ContainsKey(actorNum) && UsedGameItem[actorNum].Contains(itemId))
            return false;

        // 2. UsedDayItem에 키가 있는지 확인 후 검사
        if (UsedDayItem.ContainsKey(actorNum) && UsedDayItem[actorNum].Contains(itemId))
            return false;

        // 3. UsedTurnItem에 키가 있는지 확인 후 검사
        if (UsedTurnItem.ContainsKey(actorNum) && UsedTurnItem[actorNum].Contains(itemId))
            return false;

        return true;
    }

    public int HasLockPick(int actorNum, SimGameState state)
    {
        var ctx = new EffectContext(_rng, Debug.Log);

        int lockPickCnt = ctx.GetPlayerLockpickCount(actorNum, state);
        return lockPickCnt;
    }

    public void UseLockPick(int actorNum, SimGameState state)
    {
        var ctx = new EffectContext(_rng, Debug.Log);
        ctx.RemovePlayerLockPickCount(actorNum, state);
    }

    public bool HasDebuff(int actNum)
    {
        foreach (var st in _statusSystem.ALL)
        {
            if (st.spec.tags == TagMask.Negative && st.ownerActorNum == actNum)
            {
                return true;
            }
        }
        return false;
    }

    //플레이어가 사용한 아이템을 StatusInstance 객체로 객체화 후, 해당 플레이어의 _statuisSystem 리스트에 삽입한다.
    //후에 턴 변화가 발생할 때, 플레이어의 stasusSystem 내의 아이템들이 적절한 타이밍에 실행된다.
    public void AddItemStatusInstance(int actorNum, ItemSO item, SimGameState state)
    {
        if (item.oncePerTurn) AddUsedTurnItem(actorNum, item.itemId);
        if (item.oncePerDay) AddUsedDayItem(actorNum, item.itemId);
        if (item.oncePerGame) AddUsedGameItem(actorNum, item.itemId);

        //If Sacrifice Item
        if (item.itemId == "2002")
        {
            //**TODO**
            //별도의 처리 필요
        }
        //If Lockpick Item
        if (item.itemId == "4000")
        {
            var ctx = new EffectContext(_rng, Debug.Log);
            ctx.AddPlayerLockPickCount(actorNum, state);
            Debug.Log($"[ItemLockPick] Player{actorNum}'s LockPick added");
            return;
        }
        if (item.itemId == "4001")
        {
            var ctx = new EffectContext(_rng, Debug.Log);
            ctx.AddPlayerLockCount(actorNum, state);
            Debug.Log($"[ItemLockPick] Player{actorNum}'s Lock added");
            return;
        }

        //아이템 적용 대상을 기준으로 분기하여 처리한다.
        switch (item.target)
        {
            //아이템 적용 대상이 자기 자신인 경우
            case ItemTarget.Self:
            case ItemTarget.SelfVillage:
            case ItemTarget.Tree:
                //해당 아이템에 부착된 효과들을 돌면서
                foreach (EffectSpec es in item.effects)
                {
                    //AddStatus 외의 다른 아이템 효과로 정의된 아이템일 경우
                    if (es.effectType != ItemEffect.AddStatus)
                    {
                        ItemProcessImm(actorNum, es, state);

                        continue;
                    }

                    //AddStatus에 정의된 해당 아이템 정보를 가져온다.
                    //이는 추후에 다른 아이템을 처리하기 위해 별개의 코드를 넣어야 할 듯 하다.
                    StatusSpec ss = es.statusSpce;

                    //남은 턴 수를 durationType을 기반으로 초기화하고
                    int remainTurns = getRemainTurns(ss, state);

                    //StatusIntance를 생성한 다음
                    var st = setAndGetStatusInstance(ss, actorNum, actorNum, remainTurns);

                    //플레이어의 상태 관리 시스템 리스트에 삽입
                    _statusSystem.Add(st);

                    //디버깅
                    Debug.Log($"[Item] AddStatus {ss.statusId} to {actorNum}");
                }
                break;

            //아이템 적용 대상이 다른 플레이어인 경우
            case ItemTarget.Opponent:
            case ItemTarget.OpponentVillage:
            case ItemTarget.OpponentTree:
                foreach (EffectSpec es in item.effects)
                {
                    //AddStatus 외의 다른 아이템 효과로 정의된 아이템일 경우
                    if (es.effectType != ItemEffect.AddStatus)
                    {

                        //나를 제외한 다른 모든 플레이어에게 해당 즉시 적용 아이템 효과를 적용한다.
                        if (state.curTurnPlayerNum == 1)
                        {
                            ItemProcessImm(2, es, state);
                        }
                        else if (state.curTurnPlayerNum == 2)
                        {
                            ItemProcessImm(1, es, state);
                        }

                        continue;
                    }

                    //AddStatus에 정의된 해당 아이템 정보를 가져온다.
                    StatusSpec ss = es.statusSpce;

                    //남은 턴 수를 durationType을 기반으로 초기화하고
                    int remainTurns = getRemainTurns(ss, state);

                    //다른 플레이어어정보로 초기화하여 해당 아이템 효과를 삽입한다.
                    if (state.curTurnPlayerNum == 1)
                    {
                        var opst = setAndGetStatusInstance(ss, 2, actorNum, remainTurns);

                        _statusSystem.Add(opst);

                        //디버깅
                        Debug.Log($"[Item] AddStatus {ss.statusId} to {2}");
                    }
                    else if (state.curTurnPlayerNum == 2)
                    {
                        var opst = setAndGetStatusInstance(ss, 1, actorNum, remainTurns);

                        _statusSystem.Add(opst);

                        //디버깅
                        Debug.Log($"[Item] AddStatus {ss.statusId} to {1}");
                    }
                }
                break;


            //아이템 적용 대상이 전체인 경우
            case ItemTarget.Global:
                foreach (EffectSpec es in item.effects)
                {
                    //AddStatus 외의 다른 아이템 효과로 정의된 아이템일 경우
                    if (es.effectType != ItemEffect.AddStatus)
                    {
                        ItemProcessImm(1, es, state);
                        ItemProcessImm(2, es, state);
                        continue;
                    }

                    //AddStatus에 정의된 해당 아이템 정보를 가져온다.
                    StatusSpec ss = es.statusSpce;

                    //남은 턴 수를 durationType을 기반으로 초기화하고
                    int remainTurns = getRemainTurns(ss, state);

                    //각 플레이어 정보로 초기화하여 아이템효과를 삽입한다.

                    var gbst = setAndGetStatusInstance(ss, 1, actorNum, remainTurns);
                    var gbstt = setAndGetStatusInstance(ss, 2, actorNum, remainTurns);

                    _statusSystem.Add(gbst);

                    //디버깅
                    Debug.Log($"[Item] AddStatus {ss.statusId} to {1} and {2}");

                }
                break;
        }
    }

    //실제 아이템 처리 로직
    public void ProcessItemEffect(int actorNum, int baseDamage, bool isBasicAttack, SimGameState state)
    {
        //컨텍스트 생성
        var ctx = new EffectContext(_rng, Debug.Log);

        //데미지 객체 생성
        var dmg = new DamagePacket
        {
            attackerNum = actorNum,
            isBasicAttack = isBasicAttack,
            baseDamage = baseDamage
        };

        //최종 데미지 계산(아이템도 함께 반영하여 계산)
        _damageResolver.Resolve(dmg, ctx, state);


        //각 아이템의 남은 턴 수 계산, 남은 턴수가 모두 지나면 해당 아이템을 _statusSystem 리스트에서 삭제한다.
        _statusSystem.TickTurnEnd(actorNum);


        //나무 데미지 업데이트
        float hp = state.treeHP;
        hp -= dmg.finalDamage;
        state.treeHP = hp;


        //TODO: 게임 종료 검증
        if (state.treeHP <= 0f)
        {
            Debug.Log("Match End By Tree HP 0");
            state.looserPlayerNum = state.curTurnPlayerNum;
            return;
        }

        //계산된 결과를 처리하는 함수 호출
        ApplyItemEffectResult(actorNum, dmg.finalDamage, dmg.convertedToBarrier, hp, state);
    }

    public void ApplyItemEffectResult(int actorNum, int finalDamage, float finalBarrierConverted, float treeHPAfter, SimGameState state)
    {
        float currentTotalDamage = actorNum == 1 ? state.p1TotalHitDmg : state.p2TotalHitDmg;
        float currentBarrier = actorNum == 1 ? state.p1VillBarrier : state.p2VillBarrier;

        //데미지 합계 계산
        currentTotalDamage += finalDamage;
        //Barrier 값 계산
        currentBarrier = currentBarrier + finalBarrierConverted;

        if (actorNum == 1)
        {
            state.p1TotalHitDmg = currentTotalDamage;
            state.p1VillBarrier = currentBarrier;
        }
        else
        {
            state.p2TotalHitDmg = currentTotalDamage;
            state.p2VillBarrier = currentBarrier;
        }
    }

    private StatusInstance setAndGetStatusInstance(StatusSpec ss, int ownerAct, int sourceAct, int remainTurns)
    {
        var st = new StatusInstance
        {
            spec = ss,
            ownerActorNum = ownerAct,
            sourceActorNum = sourceAct,
            remainingTurns = remainTurns
        };
        return st;
    }

    private void ItemProcessImm(int actorNum, EffectSpec es, SimGameState state)
    {
        var ctx = new EffectContext(_rng, Debug.Log);
        //ItemEffect 타입 기준 구분
        switch (es.effectType)
        {
            //나무 체력에 추가 HP를 +/- 하는 아이템이라면
            case ItemEffect.DeltaTreeUp:
                float val = es.floatValue1;
                if (Mathf.Approximately(val, 0f))
                {
                    Debug.LogError("Item Heal Value null Exception");
                    return;
                }
                val += ctx.GetTreeHP(state);
                ctx.SetTreeHP(val, state);

                Debug.Log($"[ItemProcessImm] TreeHP Changed to {val}");
                break;
            //마을 체력에 추가 HP를 +/- 하는 아이템이라면
            case ItemEffect.DeltaVillageHp:
                float delta = es.floatValue1;
                if (Mathf.Approximately(delta, 0f))
                {
                    Debug.LogError("Item Village Heal Value null Exception");
                    return;
                }
                float cur = ctx.GetPlayerVillageHP(actorNum, state);
                //아래 코드는 해당 아이템을 사용하면 게임을 바로 지게 되는 요인을 막기 위한 플레이어를 위한 장치
                //도입 여부는 아직 모름
                //if (delta < 0f && cur + delta <= 0f)
                //{
                //	ctx.Log?.Invoke("[ItemProcessImm] Not enough VillageHP for donation");
                //	return;
                //}
                float next = delta + cur;
                ctx.SetPlayerVIllageHP(actorNum, next, state);

                Debug.Log($"[ItemProcessImm] VillageHP Changed to {next}");
                break;
            //마을 쉴드량에 추가 쉴드를 +/- 하는 아이템이라면
            case ItemEffect.DeltaVillageShield:
                float shield = es.floatValue1;
                if (Mathf.Approximately(shield, 0f))
                {
                    Debug.LogError("Item Shield Value null Exception");
                    return;
                }
                shield += ctx.GetPlayerVillageShield(actorNum, state);

                ctx.SetPlayerVIllageShield(actorNum, shield, state);

                Debug.Log($"[ItemProcessImm] Player{actorNum}'s VillageShield Changed to {shield}");
                break;
            //마을 쉴드량에 특정 값(비율)을 곱하는 아이템이라면
            case ItemEffect.MultVillageShield:
                float mult = es.floatValue1;
                if (Mathf.Approximately(mult, 0f))
                {
                    Debug.LogError("Item Shield Value null Exception");
                    return;
                }
                float nextShield = ctx.GetPlayerVillageShield(actorNum, state) * mult;

                ctx.SetPlayerVIllageShield(actorNum, nextShield, state);

                Debug.Log($"[ItemProcessImm] Player{actorNum}'s VillageShield Changed to {nextShield}");
                break;
            //플레이어 기력에 추가 기력을 +/- 하는 아이템이라면
            case ItemEffect.DeltaPlayerEng:
                int eng = es.intValue1;
                if (eng == 0)
                {
                    Debug.LogError("Item Charge Value null Exception");
                    return;
                }

                eng += ctx.GetPlayerEng(actorNum, state);

                ctx.SetPlayerEng(actorNum, eng, state);

                Debug.Log($"[ItemProcessImm] Player{actorNum}'s Energy Changed to {eng}");
                break;

            case ItemEffect.TransferOpponentShieldPct:
                int targetActNum = (actorNum == 1 ? 2 : 1);
                float VillageShieldPct = es.floatValue1;

                float targetShieldValue = ctx.GetPlayerVillageShield(targetActNum, state);
                float myShieldValue = ctx.GetPlayerVillageShield(actorNum, state);
                float deltaValue = targetShieldValue * VillageShieldPct;

                ctx.SetPlayerVIllageShield(actorNum, myShieldValue + deltaValue, state);
                ctx.SetPlayerVIllageShield(targetActNum, Mathf.Max(0f, targetShieldValue - deltaValue), state);

                Debug.Log($"[ItemProcessImm] Player{actorNum}'s VillageShield Changed from {myShieldValue} to {myShieldValue + deltaValue}");
                Debug.Log($"[ItemProcessImm] Player{targetActNum}'s VillageShield Changed from {targetShieldValue} to {targetShieldValue - deltaValue}");
                break;
        }
    }

    private int getRemainTurns(StatusSpec ss, SimGameState state)
    {
        int remainTurns = 9999;
        switch (ss.durationType)
        {
            //이번 턴에만 사용되는 아이템인 경우
            case DurationType.ThisTurn:
                remainTurns = 1;
                break;
            //다음 턴까지 사용되는 아이템일 경우
            case DurationType.NextTurn:
                remainTurns = 2;
                break;
            //N번의 Turn 동안 활성화되는 아이템일 경우
            case DurationType.Turns:
                remainTurns = ss.durationTurns;
                break;
            //이번 일자 동안 활성화 되는 아이템일 경우
            case DurationType.UntilWaveEnd:
                //현재 wave 값
                int currentWave = state.wave;
                //최대 wave 값
                int MaxWaveCnt = state.roomSetting.maxWave;
                //플레이어 수(Turn 수)
                int PlayerCnt = 2;
                //현재 턴 인덱스
                int currentTurnIdx = state.turn;
                //남은 턴 계산
                //ex) 2번째 wave의 첫번째 턴인 경우 
                //remainTurns = (3 - 1 - 1) * 2 + (2 - 0) = 4
                remainTurns = (MaxWaveCnt - currentWave - 1) * PlayerCnt + (PlayerCnt - currentTurnIdx);
                break;
        }
        return remainTurns;
    }
}
