using UnityEngine;
using Photon.Pun;

public class AITreeAttacker : AILogicModule
{
    [Header("AI 컨트롤 오차 범위")]
    public float jitterMin = -50f;
    public float jitterMax = 50f;

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
        //  1.스탭샷 생성
        GetCurAIStat(brain.MyActorNum);

        //  2.아이템 효가 계산(convertRate, multiplier)
        var result = getItemEffect();

        //만약, 덮어씌워지는 데미지 값이 존재하면, 그냥 계산하지 않고 랜덤 값으로 데미지를 넣는다.
        if (result.dmgOverrideDmg >= 0)
        {
            return UnityEngine.Random.Range(0, 100);
        }

        //  3.목표 타격량 계산
        //현재 웨이브와 최대 웨이브를 통해 남은 타격 횟수를 구한다.
        int curWave = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
        int maxWave = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.MaxWaveCnt);
        int remainingAttacks = Mathf.Max(maxWave - curWave, 1); //0으로 나누기 방지 장치

        //필요한 방어력
        float requiredBarrier = (curTreeToxicDmg - curPlayerBarrier);

        float targetDamage = 0f;
        if (requiredBarrier > 0)
        {
            //부족한 방어력을 남은 타격 횟수로 분배한다.
            float allocatedBarrierForThisTurn = requiredBarrier / remainingAttacks;
            //이번 턴의 할당량으로 달서하기 위한 데미지 계산
            targetDamage = allocatedBarrierForThisTurn / result.BarrierConvRate;
        }

        //목표 타격량에 아이템 효과 적용
        targetDamage /= result.dmgMultiRate;
        //목표 타격량이 현재 타격 가능한 범위를 넘어선 경우 범위 안으로 조정 수행
        float damageRange = curPlayerMaxDmg - curPlayerMinDmg;
        if (targetDamage > curPlayerMaxDmg)
        {   //jitter 적용을 위한 노이즈 추가
            targetDamage = curPlayerMaxDmg - 40f;
        }
        else if (targetDamage < curPlayerMinDmg)
        {
            targetDamage = curPlayerMinDmg + 40f;
        }

        //  4.나무 체력 확인 및 딜레마 로직
        //만약 목표 데미지가 나무 체력보다 많은 경우
        if (targetDamage >= curTreeHP)
        {
            //안전 데미지 계산
            float safeDamage = Mathf.Max(curPlayerMinDmg, curTreeHP - 100f);

            //해당 안전 데미지로부터 얻는 방어력 계산
            float predictedBarrierGained = safeDamage * result.BarrierConvRate;

            //최종 마을이 받을 데미지 계산
            float predictedVillageDamage = curTreeToxicDmg - (curPlayerBarrier + predictedBarrierGained);

            //만약 마을이 버틸 수 없는 경우
            if (predictedVillageDamage >= curPlayerVillageHP)
            {
                //그냥 기존 결정된 데미지로 타격 수행
                Debug.Log($"[AI {brain.MyActorNum}] 마을 체력으로 버틸 수 없음, 풀파워 가격");
            }
            else
            {
                Debug.Log($"[AI {brain.MyActorNum}] 마을 체력 버티기 가능, 나무를 살살 가격");
                //버틸 수 있으면 안전 데미지 적용
                targetDamage = safeDamage;
            }
        }

        Debug.Log($"[AI {brain.MyActorNum}] Plain Target Damage : {targetDamage}");
        //  5. 최종 타격 범위 설정 및 랜덤 jitter, 모듈러 연산
        float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);
        targetDamage += jitter;
        Debug.Log($"[AI {brain.MyActorNum}] Target Damage + Jitter: {targetDamage}");

        //목표 데미지 범위 조정
        if (targetDamage > curPlayerMaxDmg)
        {
            targetDamage = curPlayerMinDmg + ((targetDamage - curPlayerMinDmg) % damageRange);
        }
        else if (targetDamage < curPlayerMinDmg)
        {
            targetDamage = curPlayerMaxDmg - (curPlayerMinDmg - targetDamage);
        }

        Debug.Log($"[AI {brain.MyActorNum}] Final Damage : {targetDamage}");
        //최종 타격 데미지 반환
        return targetDamage;
    }

    //AI 프로퍼티에서 스탯 값을 가져오는 함수
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

    //아이템 효과 계산을 위한 DamagePacket 발행 및 계산 수행
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

