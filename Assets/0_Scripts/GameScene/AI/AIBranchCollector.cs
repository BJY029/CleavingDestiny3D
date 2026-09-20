using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class AIBranchCollector : AILogicModule
{
    [Header("Branch Collect Settings")]
    [SerializeField] private float searchRadius = 30f;
    [SerializeField] private float collectDistance = 2f;
    [SerializeField] private float navMeshSampleDistance = 2f;
    [SerializeField] private int aiMaxCollectPerOffTurn = 3;

    private CancellationTokenSource collectCts;
    private CancellationTokenSource targetMoveCts;

    private int collectedThisOffTurn;

    private int targetBranchId = -1;
    private bool isMovingToTarget;

    private void OnEnable()
    {
        if (BranchNetworkManager.Instance != null)
        {
            BranchNetworkManager.Instance.OnBranchRemoved += HandleBranchRemoved;
        }
    }

    private void OnDisable()
    {
        if (BranchNetworkManager.Instance != null)
        {
            BranchNetworkManager.Instance.OnBranchRemoved -= HandleBranchRemoved;
        }

        StopCollecting();
    }

    public void StartCollecting()
    {
        StopCollecting();

        collectedThisOffTurn = 0;

        collectCts = new CancellationTokenSource();

        CollectRoutineAsync(collectCts.Token).Forget();
    }

    public void StopCollecting()
    {
        CancelTargetMove();

        if (collectCts != null)
        {
            if (!collectCts.IsCancellationRequested)
                collectCts.Cancel();
            collectCts.Dispose();
            collectCts = null;
        }

        targetBranchId = -1;
    }

    private void HandleBranchRemoved(int branchId)
    {
        if (!isMovingToTarget)
            return;

        if (branchId != targetBranchId)
            return;

        Debug.Log($"[AIBranchCollector] 목표 Branch 제거됨 : {branchId}");

        CancelTargetMove();
    }

    private void CancelTargetMove()
    {
        if (targetMoveCts == null)
            return;

        if (!targetMoveCts.IsCancellationRequested)
        {
            targetMoveCts.Cancel();
        }
    }

    private async UniTask<bool> MoveToBranchAsync(Vector3 branchPosition, CancellationToken collectToken)
    {
        CancellationTokenSource localMoveCts = CancellationTokenSource.CreateLinkedTokenSource(collectToken);

        targetMoveCts = localMoveCts;
        isMovingToTarget = true;

        try
        {
            return await brain.aINevMeshController.MoveToPosition(branchPosition, localMoveCts.Token, collectDistance);
        }
        finally
        {
            isMovingToTarget = false;

            if (ReferenceEquals(targetMoveCts, localMoveCts))
                targetMoveCts = null;

            localMoveCts.Dispose();
        }
    }

    private async UniTaskVoid CollectRoutineAsync(CancellationToken token)
    {
        int curAIMaxCollectPerOffTurn = UnityEngine.Random.Range(1, aiMaxCollectPerOffTurn + 1);
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (!TryFindTargetBranch(out int branchId, out Vector3 branchPosition))
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
                    continue;
                }

                targetBranchId = branchId;

                bool arrived = await MoveToBranchAsync(
                branchPosition,
                token
            );

                if (!arrived)
                {
                    targetBranchId = -1;
                    continue;
                }

                bool collected = TryCollectTargetBranch();

                targetBranchId = -1;

                if (collected)
                {
                    collectedThisOffTurn++;
                    Debug.Log($"[AIBranchCollector] Branch 획득 성공 " + $"({collectedThisOffTurn}/{aiMaxCollectPerOffTurn})");
                }

                if (collectedThisOffTurn >= curAIMaxCollectPerOffTurn) break;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    private bool TryFindTargetBranch(out int resultBranchId, out Vector3 resultPosition)
    {
        resultBranchId = -1;
        resultPosition = Vector3.zero;

        BranchNetworkManager branchManager = BranchNetworkManager.Instance;

        if (branchManager == null) return false;

        Dictionary<int, Vector3> branches = branchManager.GetActiveBranchSnapshot();

        float closestDistance = float.MaxValue;

        foreach (KeyValuePair<int, Vector3> pair in branches)
        {
            int branchId = pair.Key;
            Vector3 branchPosition = pair.Value;

            float directDistance = Vector3.Distance(transform.position, branchPosition);

            if (directDistance > searchRadius) continue;

            if (!TryGetReachablePosition(branchPosition, out Vector3 reachablePosition))
                continue;

            if (directDistance >= closestDistance)
                continue;

            closestDistance = directDistance;

            resultBranchId = branchId;
            resultPosition = reachablePosition;
        }

        return resultBranchId >= 0;
    }

    private bool TryGetReachablePosition(Vector3 branchPosition, out Vector3 reachablePosition)
    {
        reachablePosition = Vector3.zero;

        if (!NavMesh.SamplePosition(branchPosition, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            return false;

        NavMeshPath path = new();

        if (!NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
            return false;

        if (path.status != NavMeshPathStatus.PathComplete)
            return false;

        reachablePosition = hit.position;

        return true;
    }

    private bool TryCollectTargetBranch()
    {
        if (targetBranchId < 0) return false;

        BranchNetworkManager branchNetwork = BranchNetworkManager.Instance;

        if (branchNetwork == null) return false;

        return branchNetwork.TryCollectBranchByAI(targetBranchId, transform.position);
    }
}
