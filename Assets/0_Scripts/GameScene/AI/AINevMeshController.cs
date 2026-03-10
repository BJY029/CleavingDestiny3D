using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class AINevMeshController : AILogicModule
{
    //위치 데이터 스크립터블 오브젝트
    public WayPointSO wayPointData;

    //AI 움직임 처리용 NavMeshAgent
    public NavMeshAgent agent;

    //플레이어 순서 기준 실제 위치 데이터 값 저장용
    private Vector3 myInvPos;
    private Vector3 myInvEntryPos;
    private Vector3 myHitPos;
    private Vector3 oppInvPos;
    private Vector3 oppInvEntryPos;
    private Vector3 oppHitPos;

    //현재 실행 중인 비동기 이동 작업을 통제(취소)하기 위한 토큰 소스
    private CancellationTokenSource moveCts;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InitWayPoints();
    }

    private void InitWayPoints()
    {
        bool isMaster = PhotonNetwork.LocalPlayer.ActorNumber == 1;

        (myInvPos, oppInvPos) = isMaster ? (wayPointData.Inv_2, wayPointData.Inv_1) : (wayPointData.Inv_1, wayPointData.Inv_2);
        (myInvEntryPos, oppInvEntryPos) = isMaster ? (wayPointData.Inv_Entry_2, wayPointData.Inv_Entry_1) : (wayPointData.Inv_Entry_1, wayPointData.Inv_Entry_2);
        (myHitPos, oppHitPos) = isMaster ? (wayPointData.Hit_2, wayPointData.Hit_1) : (wayPointData.Hit_1, wayPointData.Hit_2);
    }

    public void CommandMoveTo(LocationCommand command)
    {
        if (moveCts != null)
        {
            moveCts.Cancel();
            moveCts.Dispose();
        }

        moveCts = new CancellationTokenSource();

        Vector3 targetPos = GetPositionFromCommand(command);

        ExecuteMoveAsync(targetPos, moveCts.Token).Forget();
    }

    private Vector3 GetPositionFromCommand(LocationCommand cmd)
    {
        switch (cmd)
        {
            case LocationCommand.MY_INV: return myInvPos;
            case LocationCommand.MY_INV_ENTRY: return myInvEntryPos;
            case LocationCommand.MY_HIT: return myHitPos;
            case LocationCommand.OPP_INV: return oppInvPos;
            case LocationCommand.OPP_INV_ENTRY: return oppInvEntryPos;
            case LocationCommand.OPP_HIT: return oppHitPos;
            default: return transform.position;
        }
    }

    private async UniTaskVoid ExecuteMoveAsync(Vector3 destination, CancellationToken token)
    {
        try
        {
            agent.SetDestination(destination);

            await UniTask.WaitWhile(() => agent.pathPending, cancellationToken: token);
            await UniTask.WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance, cancellationToken: token);

            agent.velocity = Vector3.zero;
            Debug.Log($"[{destination}] 목적지 도착 완료");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("이전 이동 명령 취소 됨. 새로운 경로 탐색");
        }
    }

    // 이동 전담 스크립트 
    public async UniTask MoveToLocationAsync(LocationCommand command, CancellationToken token)
    {
        //Enum 명령어를 실제 위치(Vector3)로 변환
        Vector3 targetPos = GetPositionFromCommand(command);

        //목적지 설정
        agent.SetDestination(targetPos);

        //경로 계산 대기
        await UniTask.WaitWhile(() => agent.pathPending, cancellationToken: token);

        //도착할 때까지 대기
        await UniTask.WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance, cancellationToken: token);

        //도착 후 정지
        agent.velocity = Vector3.zero;
    }
}
