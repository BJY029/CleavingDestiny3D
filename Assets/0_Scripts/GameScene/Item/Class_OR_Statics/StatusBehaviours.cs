using Photon.Pun;
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
            case "DMG_SHARPEN":
                //공격 전에만 반응
                if (e.type != TriggerMask.OnBeforeAttack || dmg == null) return;
                //평타만 적용 하거나, 기본 평타 관련 데미지 객체가 아닌 경우, 실행 안함
                if(st.spec.basicOnly && !dmg.isBasicAttack) return;
                //배수 누적
                dmg.multiplier *= st.spec.multiplier;

                //디버그 로그로 임시 실행
                ctx.Log?.Invoke($"[Status] DMG_SHARPEN x{st.spec.multiplier}");
                return;
            //날 무디기 아이템
            case "DMG_DULL":
                if (e.type != TriggerMask.OnBeforeAttack || dmg == null) return;
                if (st.spec.basicOnly && !dmg.isBasicAttack) return;
                dmg.multiplier *= st.spec.multiplier;

                ctx.Log?.Invoke($"[Status] DMG_DULL x{st.spec.multiplier}");
                return;
            //연속 베기 아이템(2배)
            case "DMG_DOUBLE":
				if (e.type != TriggerMask.OnBeforeAttack || dmg == null) return;
				if (st.spec.basicOnly && !dmg.isBasicAttack) return;
				dmg.multiplier *= st.spec.multiplier;

				ctx.Log?.Invoke($"[Status] DMG_DOUBLE x{st.spec.multiplier}");
				return;
            //날 무디기 아이템
            case "DMG_RUSTY":
				if (e.type != TriggerMask.OnBeforeAttack || dmg == null) return;
				if (st.spec.basicOnly && !dmg.isBasicAttack) return;
				dmg.multiplier *= st.spec.multiplier;

				ctx.Log?.Invoke($"[Status] DMG_RUSTY x{st.spec.multiplier}");
				return;
            //기름 바르기 아이템
			case "DMG_GREASED":
                if(e.type != TriggerMask.OnBeforeAttack || dmg == null) return;
                //평타 관련 데미지 객체가 아닌 경우 무시
                if(!dmg.isBasicAttack) return;

                //데미지 고정 덮어쓰기
                dmg.overrideDamage = 0;

                //디버그 로그로 임시 실행
                ctx.Log?.Invoke($"[Status] DMG_GREASED overriedDamage = 0");
                return;
            //흰개미 아이템
            case "DMG_TERMITE":
                if (e.type != TriggerMask.OnTurnStart) return;

                if (!PhotonNetwork.IsMasterClient) return;

                int terDmg = ctx.Rng.Range(st.spec.randMin, st.spec.randMax + 1);

                float hp = ctx.GetTreeHP();
                hp -= terDmg;
                ctx.SetTreeHP_MasterOnly(hp);

				ctx.Log?.Invoke($"[Status] DMG_TERMITE -{terDmg}(TreeHP = {hp})");
				return;
            //눈 가리고 때리기 아이템
            case "DMG_BLIND":
                if(e.type != TriggerMask.OnBeforeAttack) return;

                if (!dmg.isBasicAttack) return;

                int pick = ctx.Rng.Range(0, 2);

                int value = (pick == 0) ? st.spec.randMin : st.spec.randMax;

                dmg.overrideDamage = value;

				ctx.Log?.Invoke($"[Status] DMG_BLIND overriedDamage = {value}");
				return;

            case "DEF_SILVER":
                if (e.type != TriggerMask.OnDamageConvert) return;
                if (!dmg.isBasicAttack) return;

                dmg.convertRateOverride = st.spec.convertRate;

				ctx.Log?.Invoke($"[Status] DEF_SILVER overriedBarrierConvertRate = {dmg.convertRateOverride}");
				return;

            case "GIM_TAUNT":
                if(dmg.attackerNum != st.ownerActorNum) return;
				//공격 전에만 반응
				if (e.type != TriggerMask.OnBeforeAttack || dmg == null) return;
				//평타만 적용 하거나, 기본 평타 관련 데미지 객체가 아닌 경우, 실행 안함
				if (st.spec.basicOnly && !dmg.isBasicAttack) return;
				//배수 누적
				dmg.multiplier *= st.spec.multiplier;

                if(st.spec.consumeOnTrigger) st.remainingTurns = 0;

				//디버그 로그로 임시 실행
				ctx.Log?.Invoke($"[Status] GIM_TAUNT x{st.spec.multiplier}");
				return;

		}
    }
}
