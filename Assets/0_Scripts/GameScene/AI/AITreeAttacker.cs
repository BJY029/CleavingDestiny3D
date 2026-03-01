using UnityEngine;
using Photon.Pun;

public class AITreeAttacker : AILogicModule
{
    [Header("AI 컨트롤 오차 범위")]
    public float jitterMin = -100f;
    public float jitterMax = 100f;

    private float curTreeHP;
    private int curPlayerMaxDmg;
    private int curPlayerMinDmg;
    private float curPlayerVillageHP;
    private float curPlayerBarrier;
    private float curTreeToxicDmg;

    /// <summary>
    /// Return best damage ratio when TryHit method called
    /// </summary>
    /// <returns>damage ratio</returns>
    public float SelectDamage()
    {
        //1.스탭샷 생성
        GetCurAIStat(brain.MyActorNum);

        //2.아이템 효가 계산(convertRate, multiplier)
        var result = getItemEffect();

        //만약, 덮어씌워지는 데미지 값이 존재하면, 그냥 계산하지 않고 랜덤 값으로 데미지를 넣는다.
        if (result.dmgOverrideDmg >= 0)
        {
            return UnityEngine.Random.Range(0, 100);
        }

        //3.목표 타격량 계산
        float requiredBarrier = (curTreeToxicDmg - curPlayerBarrier) / 3.0f;

        float targetDamage = 0f;
        if (requiredBarrier > 0)
        {
            targetDamage = requiredBarrier / result.BarrierConvRate;
        }

        targetDamage /= result.dmgMultiRate;
        float damageRange = curPlayerMaxDmg - curPlayerMinDmg;
        if (targetDamage > curPlayerMaxDmg)
        {
            targetDamage = curPlayerMaxDmg - 70f;
        }
        else if (targetDamage < curPlayerMinDmg)
        {
            targetDamage = curPlayerMinDmg + 70f;
        }

        //4.나무 체력 확인 및 딜레마 로직
        if (targetDamage >= curTreeHP)
        {
            float safeDamage = Mathf.Max(curPlayerMinDmg, curTreeHP - 100f);

            float predictedBarrierGained = safeDamage * result.BarrierConvRate;

            float predictedVillageDamage = curTreeToxicDmg - (curPlayerBarrier + predictedBarrierGained);

            if (predictedVillageDamage >= curPlayerVillageHP)
            {
                Debug.Log($"[AI {brain.MyActorNum}] 마을 체력으로 버틸 수 없음, 풀파워 가격");
                targetDamage = curPlayerMaxDmg;
            }
            else
            {
                Debug.Log($"[AI {brain.MyActorNum}] 마을 체력 버티기 가능, 나무를 살살 가격");
                targetDamage = safeDamage;
            }
        }

        Debug.Log($"[AI {brain.MyActorNum}] Plain Target Damage : {targetDamage}");
        //5. 최종 타격 범위 설정 및 랜덤 jitter, 모듈러 연산
        float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);
        targetDamage += jitter;
        Debug.Log($"[AI {brain.MyActorNum}] Target Damage + Jitter: {targetDamage}");

        if (targetDamage > curPlayerMaxDmg)
        {
            targetDamage = curPlayerMinDmg + ((targetDamage - curPlayerMinDmg) % damageRange);
        }
        else if (targetDamage < curPlayerMinDmg)
        {
            targetDamage = curPlayerMaxDmg - (curPlayerMinDmg - targetDamage);
        }

        Debug.Log($"[AI {brain.MyActorNum}] Final Damage : {targetDamage}");
        return targetDamage;
    }

    private void GetCurAIStat(int aiNum)
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        curTreeHP = GetValue<float>(props, RoomPropKeys.TreeHP);
        curTreeToxicDmg = GetValue<float>(props, RoomPropKeys.TreeAtkPow);

        string attachedKey = $"_{aiNum}";
        curPlayerMaxDmg = GetValue<int>(props, PlayerPropKeys.MaxAtkPow + attachedKey);
        curPlayerMinDmg = GetValue<int>(props, PlayerPropKeys.MinAtkPow + attachedKey);
        curPlayerVillageHP = GetValue<float>(props, PlayerPropKeys.VillageHP + attachedKey);
        curPlayerBarrier = GetValue<float>(props, PlayerPropKeys.VillageBarrier + attachedKey);
    }

    private T GetValue<T>(ExitGames.Client.Photon.Hashtable props, string key)
    {
        if (props.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        return default(T);
    }

    private (float dmgMultiRate, float dmgOverrideDmg, float BarrierConvRate) getItemEffect()
    {
        //컨텍스트 생성
        var ctx = new EffectContext(ItemHandlingSystem.instance._rng, Debug.Log);

        //데미지 객체 생성
        var dmg = new DamagePacket
        {
            attackerNum = brain.MyActorNum,
            isBasicAttack = true,
            baseDamage = 1
        };

        //최종 데미지 계산(아이템도 함께 반영하여 계산)
        return ItemHandlingSystem.instance._damageResolver.ResolveRatio(dmg, ctx);
    }
}

