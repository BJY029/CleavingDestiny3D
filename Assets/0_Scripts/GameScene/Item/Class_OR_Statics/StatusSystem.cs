using System.Collections.Generic;
using Photon.Pun;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using System.Diagnostics;
using System;

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
        foreach (var st in _status)
        {
            //특정 플레이어의 상태이상 객체 중, 이번 턴만 활성화 된 상태 이상인 경우
            if (st.ownerActorNum == ownerActorNum && st.spec.durationType == DurationType.ThisTurn)
            {
                //남은 turn을 0으로 처리(만료 처리 수행)
                st.remainingTurns = 0;
                StatusSyncHub.instance?.Master_BroadcastRemove(ownerActorNum, st.spec.statusId);
            }

            //특정 플레이어의 상태이상 객체 중, N Turn 동안 활성화 된 상태 이상인 경우
            if (st.ownerActorNum == ownerActorNum && (st.spec.durationType == DurationType.Turns))
            {
                //남은 Turn 정보 1 감소
                st.remainingTurns--;
                if (StatusUIModel.instance != null && StatusUIModel.instance.GetStatusInfoInstance(ownerActorNum, st.spec.statusId, out var IS))
                {
                    IS.remainingTurns = st.remainingTurns;
                    StatusSyncHub.instance?.Master_BroadcastUpdate(IS);
                }
                else
                {
                    Console.WriteLine("Item Status Remove Error; No Status Info founded");
                }
            }
        }

        //상태 이상 객체 삭제
        //remainingTruns이 0 이하인 상태이상 객체 모두 삭제
        //_status.RemoveAll(s => 
        //(s.spec.durationType == DurationType.Turns || s.spec.durationType == DurationType.ThisTurn)
        //&& s.remainingTurns <= 0);
        //_status.RemoveAll(s => s.remainingTurns <= 0);
        RemoveRemainingTurns_Zero();
    }

    public void ProcessOnApplyItem(StatusInstance st, EffectContext ctx)
    {

    }

    public void RemoveRemainingTurns_Zero()
    {
        List<StatusInstance> toRemove = new List<StatusInstance>();

        for (int i = 0; i < _status.Count; i++)
        {
            StatusInstance st = _status[i];

            if (st.remainingTurns <= 0)
                toRemove.Add(st);
        }

        if (PhotonNetwork.IsMasterClient && StatusSyncHub.instance != null)
        {
            for (int i = 0; i < toRemove.Count; i++)
            {
                StatusInstance st = toRemove[i];
                StatusSyncHub.instance?.Master_BroadcastRemove(st.ownerActorNum, st.spec.statusId);
            }
        }

        _status.RemoveAll(s => s.remainingTurns <= 0);
    }

    //적용 기간을 기반으로 해당되는 모든 상태이상 객체를 삭제하는 함수
    public void RemoveAllByDuration(DurationType duration)
    {
        List<StatusInstance> toRemove = new List<StatusInstance>();

        for (int i = 0; i < _status.Count; i++)
        {
            StatusInstance st = _status[i];

            if (st.spec.durationType == duration)
                toRemove.Add(st);
        }

        if (PhotonNetwork.IsMasterClient && StatusSyncHub.instance != null)
        {
            for (int i = 0; i < toRemove.Count; i++)
            {
                StatusInstance st = toRemove[i];
                StatusSyncHub.instance?.Master_BroadcastRemove(st.ownerActorNum, st.spec.statusId);
            }
        }

        _status.RemoveAll(st => st.spec.durationType == duration);
    }
}
