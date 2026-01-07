//런타임 상태 이상 정보 객체
public class StatusInstance
{
    //아이템 상태 정보
    public StatusSpec spec;
    //해당 상태 이상 정보의 주인
    public int ownerActorNum;
    //해당 상태를 주입한 사람
    public int sourceActorNum;
    //해당 상태의 남은 턴
    public int remainingTurns;

    //상태가 만료 되었는지 확인용
    public bool IsExpired
    {
        get
        {
            //N Turns 타입인 경우
            if (spec.durationType == DurationType.Turns)
                return remainingTurns <= 0;
            //그외 타입은 외부에서 처리
            return false;
        }
    }
}
