using System.Collections.Generic;
using System.Linq;

public class GameEvent
{
    //이벤트 종류(공격 전, 후, N 턴동안 등등)
    public TriggerMask type;
    //이벤트 주체
    public int actorNum;
    //이벤트에 딸려가는 데이터(DamagePacket 등)
    public object payload;
}

//모든 상태이상 객체(StatusInstance)를 저장하고 관리하는 시스템
public class StatusSystem
{
    //현재 활성 상태 이상 리스트
    private readonly List<StatusInstance> _status = new();

    //외부 노출용 (읽기 전용)
    public IEnumerable<StatusInstance> ALL => _status;

    public void Add(StatusInstance newStatus)
    {
        _status.Add(newStatus);
    }

    //특정 owner의 상태 이상을 태그 기준으로 제거
    //제거된 상태 이상 갯수 반환
    public int DispelByTags(int ownerActorNum, TagMask removeTags)
    {
        //현재 상태 이상
        int before = _status.Count;
        //상태 이상 객체의 주체와 태그 정보를 AND 연산해서 0이 아니면(하나라도 겹치면) 삭제한다.
        _status.RemoveAll(s => s.ownerActorNum == ownerActorNum && (s.spec.tags & removeTags) != 0);
        //삭제 후의 상태 이상 갯수를 반환
        return before - _status.Count;
    }

    //턴 종료 시 만료/감소  처리
    public void TickTurnEnd(int ownerActorNum)
    {
        foreach(var st in _status)
        {
            //특정 플레이어의 상태이상 객체 중, 이번 턴만 활성화 된 상태 이상인 경우
            if(st.ownerActorNum == ownerActorNum && st.spec.durationType == DurationType.ThisTurn)
            {
                //남은 turn을 0으로 처리(만료 처리 수행)
                st.remainingTurns = 0;
            }

			//특정 플레이어의 상태이상 객체 중, N Turn 동안 활성화 된 상태 이상인 경우
			if (st.ownerActorNum == ownerActorNum && st.spec.durationType == DurationType.Turns)
            {
                //남은 Turn 정보 1 감소
                st.remainingTurns--;
            }
        }

        //상태 이상 객체 삭제
        //remainingTruns이 0 이하인 상태이상 객체 모두 삭제
        _status.RemoveAll(s => 
        (s.spec.durationType == DurationType.Turns || s.spec.durationType == DurationType.ThisTurn)
        && s.remainingTurns <= 0);
    }
}
