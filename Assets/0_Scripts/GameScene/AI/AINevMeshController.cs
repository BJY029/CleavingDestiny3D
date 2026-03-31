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
    public float rotationSpeed = 10f;

    //AI 움직임 처리용 NavMeshAgent
    public NavMeshAgent agent;

    //플레이어 순서 기준 실제 위치 데이터 값 저장용
    private Vector3 myInvPos;
    private Vector3 myInvEntryPos;
    public Vector3 myHitPos { get; private set; }
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

    private Vector3 GetLookAtPosFromCommand(LocationCommand cmd)
    {
        switch (cmd)
        {
            case LocationCommand.MY_INV: return myInvPos * 2;
            case LocationCommand.MY_INV_ENTRY: return myInvEntryPos * 2;
            case LocationCommand.MY_HIT: return myHitPos / 2;
            case LocationCommand.OPP_INV: return oppInvPos * 2;
            case LocationCommand.OPP_INV_ENTRY: return oppInvEntryPos * 2;
            case LocationCommand.OPP_HIT: return oppHitPos / 2;
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

        Vector3 targetRotPos = GetLookAtPosFromCommand(command);

        await LookAtTargetAsync(targetRotPos, token);
    }

    //특정 방향으로 AI를 부드럽게 회전시키는 함수
    public async UniTask LookAtTargetAsync(Vector3 targetPosition, CancellationToken token)
    {
        //agent 자동 회전 끄기
        agent.updateRotation = false;

        //목표 방향 구하기
        Vector3 direction = (targetPosition - transform.position).normalized;

        //y축은 0으로 고정(위아래 회전 방지)
        direction.y = 0;

        //이미 목표 회전 값이면 실행 안함
        if (direction == Vector3.zero) return;

        //목표 회전 값 구하기
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        //회전 수행(각도 차이가 1 이하가 될 때 까지)
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            //보간을 통한 부드로운 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            //다음 프레임 까지 대기
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        //최종 회전값으로 고정
        transform.rotation = targetRotation;
        //agent 자동 회전 켜기
        agent.updateRotation = true;
    }

    //특정 방향으로 AI 플레이어를 스냅 회전 시키는 함수
    public void SnapToTarget(LocationCommand cmd)
    {
        Vector3 targetPosition = GetLookAtPosFromCommand(cmd);
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
