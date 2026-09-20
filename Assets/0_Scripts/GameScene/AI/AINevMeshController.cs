using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class AINevMeshController : AILogicModule
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float angularSpeed = 360f;
    [SerializeField] private float rotationSpeed = 10f;

    //위치 데이터 스크립터블 오브젝트
    public WayPointSO wayPointData;

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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;


        InitWayPoints();
    }

    private void InitWayPoints()
    {
        bool isMaster = PhotonNetwork.LocalPlayer.ActorNumber == 1;

        (myInvPos, oppInvPos) = isMaster ? (wayPointData.Inv_2, wayPointData.Inv_1) : (wayPointData.Inv_1, wayPointData.Inv_2);
        (myInvEntryPos, oppInvEntryPos) = isMaster ? (wayPointData.Inv_Entry_2, wayPointData.Inv_Entry_1) : (wayPointData.Inv_Entry_1, wayPointData.Inv_Entry_2);
        (myHitPos, oppHitPos) = isMaster ? (wayPointData.Hit_2, wayPointData.Hit_1) : (wayPointData.Hit_1, wayPointData.Hit_2);
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

    public async UniTask<bool> MoveToPosition(Vector3 destination, CancellationToken token, float stoppingDistance = 1.5f)
    {
        if (agent == null || !agent.isOnNavMesh) return false;

        if (!NavMesh.SamplePosition(destination, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"[AINavMesh] NavMesh 위치를 찾을 수 없음 : {destination}");
            return false;
        }

        NavMeshPath path = new();

        if (!agent.CalculatePath(navHit.position, path)) return false;

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning($"[AINavMesh] 도달 불가능한 위치 : {destination}");
            return false;
        }

        float originalStoppingDistance = agent.stoppingDistance;

        try
        {
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(navHit.position);

            await UniTask.WaitWhile(
                () => agent.pathPending, cancellationToken: token
            );

            await UniTask.WaitUntil(
                () =>
                    !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
                    (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.01f),
                cancellationToken: token
            );

            agent.velocity = Vector3.zero;

            return true;
        }
        catch (OperationCanceledException)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }

            return false;
        }
        finally
        {
            if (agent != null)
            {
                agent.stoppingDistance = originalStoppingDistance;
            }
        }
    }

    //특정 방향으로 AI를 부드럽게 회전시키는 함수
    public async UniTask LookAtTargetAsync(Vector3 targetPosition, CancellationToken token)
    {
        //agent 자동 회전 끄기
        agent.updateRotation = false;

        try
        {
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
        }
        finally
        {
            if (agent != null)
            {
                //agent 자동 회전 켜기
                agent.updateRotation = true;
            }
        }

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
