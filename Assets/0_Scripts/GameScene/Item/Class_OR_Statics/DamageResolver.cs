using UnityEngine;

//데미지 계산 객체
//여러 아이템이 존재해도 데미지 계산은 해당 객체 한 곳에서만 진행
public class DamageResolver
{
    //GameEveneBus를 통해서 OnBeforeAttack/OnDamageConvert 형 아이템에 반응하여 DamagePacket 수정
    private readonly GameEventBus _bus;
    private readonly StatusSystem _status;

    //생성자
    public DamageResolver(GameEventBus bus, StatusSystem status)
    {
        _bus = bus;
        _status = status;
    }

    public void Resolve(DamagePacket dmg, EffectContext ctx, SimGameState state = null)
    {
        //공격 직전 트리거 발행
        _bus.publish(new GameEvent
        {
            type = TriggerMask.OnBeforeAttack,
            actorNum = dmg.attackerNum,
            payload = dmg
        }, ctx, state);

        //데미지 계산
        //덮어쓰는 데미지가 있으면, 해당 데미지 사용, 아니면 기본 데미지 사용
        int calcDmg = (dmg.overrideDamage >= 0) ? dmg.overrideDamage : dmg.baseDamage;
        //배수 적용
        calcDmg = Mathf.RoundToInt(calcDmg * dmg.multiplier);
        if (state != null) state.SetPlayerDmgMultRate(dmg.attackerNum, dmg.multiplier);

        //공격력->방어력 전환 단계 트리거 발행
        _bus.publish(new GameEvent
        {
            type = TriggerMask.OnDamageConvert,
            actorNum = dmg.attackerNum,
            payload = dmg
        }, ctx, state);

        float baseRate;
        if (state == null)
            baseRate = ctx.GetBarrierConversionRate(dmg.attackerNum);
        else
            baseRate = ctx.GetBarrierConversionRate(dmg.attackerNum, state);
        float rate = (dmg.convertRateOverride >= 0) ? dmg.convertRateOverride : baseRate + dmg.convertRateDelta;
        rate = Mathf.Clamp01(rate);
        if (state != null) state.SetPlayerBarConRate(dmg.attackerNum, rate);

        //최종 데미지 확정
        dmg.finalDamage = calcDmg;
        dmg.convertedToBarrier = calcDmg * rate;

        //공격 직후 트리거 발행(로그, 카운터 등)
        _bus.publish(new GameEvent
        {
            type = TriggerMask.OnAfterAttack,
            actorNum = dmg.attackerNum,
            payload = dmg
        }, ctx, state);
    }

    public (float dmgMultiRate, float dmgOverrideDmg, float BarrierConvRate) ResolveRatio(DamagePacket dmg, EffectContext ctx)
    {
        //공격 직전 트리거 발행
        _bus.publish(new GameEvent
        {
            type = TriggerMask.OnBeforeAttack,
            actorNum = dmg.attackerNum,
            payload = dmg
        }, ctx);

        //공격력->방어력 전환 단계 트리거 발행
        _bus.publish(new GameEvent
        {
            type = TriggerMask.OnDamageConvert,
            actorNum = dmg.attackerNum,
            payload = dmg
        }, ctx);

        float baseRate = ctx.GetBarrierConversionRate(dmg.attackerNum);
        float rate = (dmg.convertRateOverride >= 0) ? dmg.convertRateOverride : baseRate + dmg.convertRateDelta;
        rate = Mathf.Clamp01(rate);

        return (dmg.multiplier, dmg.overrideDamage, rate);
    }

    public void ResolveWhenStartTurn(EffectContext ctx, int currentActNum)
    {
        _bus.publish(new GameEvent
        {
            type = TriggerMask.OnTurnStart,
            actorNum = currentActNum,
            payload = null
        }, ctx);
    }

    public void ResolveWhenVillageStart(EffectContext ctx)
    {
        _bus.publish(new GameEvent
        {
            type = TriggerMask.OnVillageStart,
            actorNum = -1,
            payload = null
        }, ctx
        );
    }

}
