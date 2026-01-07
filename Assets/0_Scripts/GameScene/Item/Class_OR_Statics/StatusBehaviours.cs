using UnityEngine;

//아이템 효과 구현(임시)
//리펙토링 필요
public static class StatusBehaviours
{
    public static void Execute(StatusInstance st, GameEvent e, EffectContext ctx)
    {
        var dmg = e.payload as DamagePacket;

        switch (st.spec.statusId)
        {
            //날 갈기 아이템
            case "DMG_MULT":
                //공격 전에만 반응
                if (e.type != TriggerMask.OnBeforeAttack || dmg == null) return;
                //평타만 적용 하거나, 기본 평타 관련 데미지 객체가 아닌 경우, 실행 안함
                if(st.spec.basicOnly && !dmg.isBasicAttack) return;
                //배수 누적
                dmg.multiplier *= st.spec.multiplier;

                //디버그 로그로 임시 실행
                ctx.Log?.Invoke($"[Status] DMG_MULT x{st.spec.multiplier}");
                return;

            case "SET_BASIC_ZERO":
                if(e.type != TriggerMask.OnBeforeAttack || dmg == null) return;
                //평타 관련 데미지 객체가 아닌 경우 무시
                if(!dmg.isBasicAttack) return;

                //데미지 고정 덮어쓰기
                dmg.overrideDamage = 0;

                //디버그 로그로 임시 실행
                ctx.Log?.Invoke($"[Status] SET_BASIC_ZERO overriedDamage = 0");
                return;
        }
    }
}
