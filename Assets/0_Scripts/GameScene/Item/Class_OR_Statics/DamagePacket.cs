//데미지 계산용 객체

public class DamagePacket
{
    //해당 데미지 객체 주체
    public int attackerNum;
    //데미지 객체가 평타 관련 객체인지 아닌지 여부
    public bool isBasicAttack;
    //클라이언트가 계산한 데미지 게이지 결과, 원본 데미지
    public int baseDamage;
    //고정 데미지로 덮어씌워지는 경우(눈 가리기 아이템(20 혹은 200)
    //-1인 경우 사용 안함
    public int overrideDamage = -1;
    //데미지에 적용할 배수
    public float multiplier = 1f;
    //데미지 숨김 여부
    public bool hidden;
    //방어력 전환에 사용할지 안할지 여부
    public bool bypassConversion;
    //방어 전환에 사용될 전환률
    public float convertRateOverride = -1f;
    //방어력 전환 누정 값
    public float convertRateDelta = 0f;
    //최종 확정 데미지 정보(마스터가 결정)
    public int finalDamage;
    //최종 방어로 전환되는 양
    public float convertedToBarrier;
}

//나무가 각 마을을 공격할 때 사용할 데미지 계산용 패킷
public class VillageDmgPacket
{
    //타겟 마을 소유 actor 번호
    public int targetActorNum;
    //마을이 원래 받는 데미지
    public int baseDamage;
    //방어 감소 관련 퍼센티지
    public float shieldRemovePct = 0f;
    //방어 감소 관련 고정 감소 값
    public int shieldRemoveFlat = 0;
    //공격 직전 제거된 마을 방어력
    public int removedShield;
    //마을 방어력이 막은 데미지
    public int damageToShield;
    //쉴드로 못 막고 직접적 타격을 입을 양
    public int damageToHP;
}