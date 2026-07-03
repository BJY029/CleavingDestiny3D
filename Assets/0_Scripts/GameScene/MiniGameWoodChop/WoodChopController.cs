using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using EzySlice;
using System.Collections;

public class WoodChopController : MonoBehaviourPunCallbacks, IMinigameInteractable
{
    public static WoodChopController instance;

    public WoodChopUIController woodChopUIController;

    [Header("Wood")]
    [SerializeField] private GameObject woodPrefab;
    [SerializeField] private Transform woodSpawnPoint;
    [SerializeField] private Transform woodParent;
    [SerializeField] private Material crossSectionMaterial;

    [Header("Axe Auto Move")]
    [SerializeField] private Transform axeBladePoint;
    [SerializeField] private Transform axeRoot;
    [SerializeField] private Transform axeLeftPoint;
    [SerializeField] private Transform axeRightPoint;
    [SerializeField] private Transform axeUpPoint;
    [SerializeField] private Transform axeDownPoint;

    [SerializeField] private float axeMoveSpeed = 0.8f;
    [SerializeField] private float axeStrikeDownTime = 0.12f;
    [SerializeField] private float axeStrikeUpTime = 0.18f;
    [SerializeField] private float afterStrikeDelay = 0.15f;
    [SerializeField] private float freezeBeforeStrikeDelay = 0.06f;
    [SerializeField] private float axeFreezeSmoothTime = 0.04f;


    private bool isResolvingStrike;


    [Header("Slice Physics")]
    [SerializeField] private float discardForce = 2.5f;
    [SerializeField] private float discardTorque = 4f;
    [SerializeField] private float discardDestroyDelay = 2f;


    [Header("Rule Settings")]
    [SerializeField] private float edgeMargin = 0.02f;
    [SerializeField] private float minChoppableWidth = 0.06f;
    [SerializeField] private float turnTimeLimit = 3f;

    [Header("Wait UI Time")]
    [SerializeField] private float waitDuraion = 2f;

    private GameObject currentWood;

    public bool isPlaying { get; private set; }

    private int playerAActorNumber;
    private int playerBActorNumber;

    private int energyBet;
    private int turnCnt;

    private double turnStartTime;

    private int syncedCurrentPlayerIndex;

    private bool isWaitingMasterResult;
    private bool isAxeFrozenByNetwork;
    private Coroutine strikeCoroutine;

    private int CurrentTurnActorNumber
    {
        get
        {
            return syncedCurrentPlayerIndex == 0 ? playerAActorNumber : playerBActorNumber;
        }
    }

    private int OpponentTurnActorNumber
    {
        get
        {
            return syncedCurrentPlayerIndex == 0 ? playerBActorNumber : playerAActorNumber;
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    //미니게임 진행중이면 시간을 지속적으로 계산한다.
    private void Update()
    {
        if (!isPlaying) return;
        if (!isResolvingStrike && !isWaitingMasterResult && !isAxeFrozenByNetwork)
            UpdateAxeView();

        if (PhotonNetwork.IsMasterClient) Master_CheckTimeout();
    }

    public void RequestStartDual(Player requestPlayer, Player targetPlayer, int betAmount)
    {
        photonView.RPC(nameof(RPC_RequestStartDual), requestPlayer, targetPlayer, betAmount);
    }


    //미니게임 시작 함수
    [PunRPC]
    public void RPC_RequestStartDual(Player targetPlayer, int betAmount)
    {
        if (targetPlayer == null) return;
        if (!PhotonNetwork.InRoom) return;

        //요청자와 타겟 플레이어 정보 받아오기
        int requestActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int targetActorNumber = targetPlayer.ActorNumber;

        //Master에게 게임 시작 요청
        photonView.RPC(nameof(RPC_RequestStartDuel), RpcTarget.MasterClient, requestActorNumber, targetActorNumber, betAmount);
    }

    //Master가 게임 시작 처리
    [PunRPC]
    private void RPC_RequestStartDuel(int requestActorNumber, int targetActorNumber, int betAmount, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (isPlaying) return;
        //정보가 불일치하면 처리 안함
        if (info.Sender.ActorNumber != requestActorNumber) return;
        if (requestActorNumber == targetActorNumber) return;
        //플레이어가 유효한 상태가 아니라면 처리 안함
        if (!IsPlayerInRoom(requestActorNumber) || !IsPlayerInRoom(targetActorNumber)) return;

        //TODO : 기력 보유 여부 검증
        // int p1Eng = PhotonPropertyHelper.GetPlayerProp<int>(requestActorNumber, PlayerPropKeys.Energy);
        // int p2Eng = PhotonPropertyHelper.GetPlayerProp<int>(targetActorNumber, PlayerPropKeys.Energy);
        // if (p1Eng < betAmount || p2Eng < betAmount)
        // {
        //     Debug.LogWarning("배팅 기력 부족");
        //     return;
        // }

        //Master에게 정보 전달
        Master_StartDuel(requestActorNumber, targetActorNumber, betAmount);
    }

    //Master가 게임 시작 정보 초기화 및 각 클라이언트에게 전파
    private void Master_StartDuel(int actorA, int actorB, int betAmount)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        playerAActorNumber = actorA;
        playerBActorNumber = actorB;
        energyBet = betAmount;

        turnCnt = 0;

        syncedCurrentPlayerIndex = 0;

        turnStartTime = PhotonNetwork.Time + 0.2d;
        isPlaying = true;

        woodChopUIController.Master_SetCurTurnUI(actorA, turnStartTime, turnTimeLimit);
        woodChopUIController.Master_SetOpTurnUI(actorB);

        //Master에서 초기화된 값들 클라이언트들에게 전파
        photonView.RPC(nameof(RPC_SyncStartDuel), RpcTarget.All,
        playerAActorNumber, playerBActorNumber, energyBet, syncedCurrentPlayerIndex,
        turnStartTime, turnCnt);
    }

    //각 클라이언트가 정보를 받아서 미니 게임 정보를 초기화한다.
    [PunRPC]
    private void RPC_SyncStartDuel(int actorA, int actorB, int betAmount,
    int currentPlayerIndex, double startTime, int syncedTurnCount)
    {
        woodChopUIController.HideCanvasForMiniGame();
        CameraSwitchManager.Instance.Player_to_LogMiniGame();

        playerAActorNumber = actorA;
        playerBActorNumber = actorB;
        energyBet = betAmount;

        syncedCurrentPlayerIndex = currentPlayerIndex;

        turnStartTime = startTime;
        turnCnt = syncedTurnCount;

        ResetAxeRuntimeState();
        isPlaying = true;

        SpawnLocalWood();
        UpdateAxeView();

        Debug.Log($"나무 쪼개기 시작: Player {actorA} vs Player {actorB}");
    }

    public void OnInteract(PlayerController pc)
    {
        RequestChopByInteract();
    }

    public void RequestChopByInteract()
    {
        if (!isPlaying) return;
        if (currentWood == null) return;
        if (isResolvingStrike) return;
        if (isWaitingMasterResult) return;

        int localActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if (localActorNumber != CurrentTurnActorNumber)
        {
            Debug.Log("현재 내 턴이 아닙니다.");
            return;
        }

        double pressTime = PhotonNetwork.Time;
        float predictedCutX01 = GetAxeX01(pressTime);

        isWaitingMasterResult = true;

        FreezeAxeAt(predictedCutX01);

        photonView.RPC(
            nameof(RPC_RequestChopByInteract),
            RpcTarget.MasterClient,
            localActorNumber,
            pressTime
        );
    }

    private void FreezeAxeAt(float axeX01)
    {
        if (axeRoot == null) return;
        if (axeLeftPoint == null || axeRightPoint == null) return;
        if (axeUpPoint == null) return;

        axeRoot.position = GetAxeUpPosition(axeX01);
    }

    //Master가 인터렉트를 처리하는 함수
    [PunRPC]
    private void RPC_RequestChopByInteract(int requestActorNumber, double pressTime, PhotonMessageInfo info)
    {
        //예외 처리
        if (!PhotonNetwork.IsMasterClient) return;
        if (!isPlaying) return;
        if (isResolvingStrike) return;
        if (info.Sender.ActorNumber != requestActorNumber) return;

        int currentPlayerIndex = syncedCurrentPlayerIndex;
        int expectedActorNumber = GetActorNumberByPlayerIndex(currentPlayerIndex);

        if (requestActorNumber != expectedActorNumber)
        {
            Debug.LogWarning(
                $"[WoodChop] 턴 불일치 / request={requestActorNumber}, expected={expectedActorNumber}, currentPlayerIndex={currentPlayerIndex}"
            );
            return;
        }

        if (pressTime < turnStartTime)
        {
            return;
        }

        if (pressTime > turnStartTime + turnTimeLimit)
        {
            Master_EndDuel(requestActorNumber, "시간 초과 입력");
            return;
        }

        double now = PhotonNetwork.Time;

        if (pressTime > now + 0.2d)
        {
            return;
        }

        float cutX01 = GetAxeX01(pressTime);

        Master_ResolveChop(currentPlayerIndex, requestActorNumber, cutX01);
    }

    private void Master_ResolveChop(int currentPlayerIndex, int requesterActorNumber, float cutX01)
    {
        WoodSegment currentWoodSegment = CalculateWoodSegmentOnAxeRail(currentWood);

        if (currentWoodSegment.width <= minChoppableWidth)
        {
            Master_EndDuel(requesterActorNumber, "더 이상 자를 수 없음");
            return;
        }

        bool isOutside =
            cutX01 <= currentWoodSegment.left + edgeMargin ||
            cutX01 >= currentWoodSegment.right - edgeMargin;

        if (isOutside)
        {
            Master_EndDuel(requesterActorNumber, "나무 바깥을 찍음");
            return;
        }

        int nextPlayerIndex = 1 - currentPlayerIndex;

        double nextTurnStartTime =
            PhotonNetwork.Time
            + axeFreezeSmoothTime
            + freezeBeforeStrikeDelay
            + axeStrikeDownTime
            + axeStrikeUpTime
            + afterStrikeDelay;

        int nextTurnCnt = turnCnt + 1;

        isResolvingStrike = true;

        photonView.RPC(
            nameof(RPC_ApplySuccessfulStrike),
            RpcTarget.All,
            cutX01,
            nextPlayerIndex,
            nextTurnStartTime,
            nextTurnCnt
        );
    }

    [PunRPC]
    private void RPC_ApplySuccessfulStrike(
        float cutX01,
        int nextPlayerIndex,
        double nextTurnStartTime,
        int syncedTurnCount)
    {
        isWaitingMasterResult = false;


        if (strikeCoroutine != null) StopCoroutine(strikeCoroutine);

        strikeCoroutine = StartCoroutine(Co_PlayAxeStrikeAndSlice(
            cutX01,
            nextPlayerIndex,
            nextTurnStartTime,
            syncedTurnCount
        ));
    }

    private System.Collections.IEnumerator Co_PlayAxeStrikeAndSlice(
    float cutX01,
    int nextPlayerIndex,
    double nextTurnStartTime,
    int syncedTurnCount)
    {
        isResolvingStrike = true;
        isAxeFrozenByNetwork = true;

        yield return Co_SmoothFreezeAxe(cutX01);
        yield return new WaitForSeconds(freezeBeforeStrikeDelay);

        Vector3 upPosition = GetAxeUpPosition(cutX01);
        Vector3 downPosition = GetAxeDownPosition(cutX01);

        float t = 0f;

        while (t < axeStrikeDownTime)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / axeStrikeDownTime);
            axeRoot.position = Vector3.Lerp(upPosition, downPosition, ratio);
            yield return null;
        }

        axeRoot.position = downPosition;

        SliceCurrentWood(cutX01);

        t = 0f;

        while (t < axeStrikeUpTime)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / axeStrikeUpTime);
            axeRoot.position = Vector3.Lerp(downPosition, upPosition, ratio);
            yield return null;
        }

        yield return new WaitForSeconds(afterStrikeDelay);

        syncedCurrentPlayerIndex = nextPlayerIndex;
        turnStartTime = nextTurnStartTime;
        turnCnt = syncedTurnCount;

        isResolvingStrike = false;
        isAxeFrozenByNetwork = false;

        strikeCoroutine = null;

        Debug.Log($"장작 쪼개기 성공 / 다음 턴 Player {CurrentTurnActorNumber}");

        if (PhotonNetwork.IsMasterClient)
        {
            woodChopUIController.Master_SetCurTurnUI(CurrentTurnActorNumber, turnStartTime, turnTimeLimit);
            woodChopUIController.Master_SetOpTurnUI(OpponentTurnActorNumber);
        }
    }

    private System.Collections.IEnumerator Co_SmoothFreezeAxe(float cutX01)
    {
        if (axeRoot == null)
        {
            yield break;
        }

        Vector3 startPosition = axeRoot.position;

        Vector3 targetPosition = GetAxeUpPosition(cutX01);

        float t = 0f;

        while (t < axeFreezeSmoothTime)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / axeFreezeSmoothTime);

            axeRoot.position = Vector3.Lerp(startPosition, targetPosition, ratio);

            yield return null;
        }

        axeRoot.position = targetPosition;
    }

    private void SliceCurrentWood(float cutX01)
    {
        if (currentWood == null) return;

        Vector3 planeWorldPoint = GetCutPlanePoint(cutX01);
        Vector3 planeWorldNormal = GetCutAxisNormal();

        SlicedHull hull = currentWood.Slice(planeWorldPoint, planeWorldNormal, crossSectionMaterial);

        if (hull == null) return;

        Transform sourceParent = currentWood.transform.parent;
        Vector3 sourceLocalPosition = currentWood.transform.localPosition;
        Quaternion sourceLocalRotation = currentWood.transform.localRotation;
        Vector3 sourceLocalScale = currentWood.transform.localScale;

        GameObject upperHull = hull.CreateUpperHull(currentWood, crossSectionMaterial);
        GameObject lowerHull = hull.CreateLowerHull(currentWood, crossSectionMaterial);

        if (upperHull == null || lowerHull == null)
        {
            Debug.LogError("[WoodChop] Hull GameObject 생성 실패");
            return;
        }

        ApplySourceTransformToHull(upperHull, sourceParent, sourceLocalPosition, sourceLocalRotation, sourceLocalScale);
        ApplySourceTransformToHull(lowerHull, sourceParent, sourceLocalPosition, sourceLocalRotation, sourceLocalScale);

        SetupSlicedPiece(upperHull);
        SetupSlicedPiece(lowerHull);

        GameObject keepPiece = SelectLargerPiece(upperHull, lowerHull, planeWorldNormal);

        GameObject discardPiece = keepPiece == upperHull ? lowerHull : upperHull;

        Destroy(currentWood);

        currentWood = keepPiece;
        currentWood.name = "current_wood";
        currentWood.tag = "WoodLog";

        ThrowAwayDiscardPiece(discardPiece, keepPiece, planeWorldNormal);
    }

    private WoodSegment CalculateWoodSegmentOnAxeRail(GameObject wood)
    {
        if (wood == null)
        {
            return new WoodSegment(0f, 1f);
        }

        Renderer renderer = wood.GetComponent<Renderer>();

        if (renderer == null)
        {
            return new WoodSegment(0f, 1f);
        }

        Vector3 left = axeLeftPoint.position;
        Vector3 right = axeRightPoint.position;

        Vector3 railVector = right - left;
        float railLengthSqr = railVector.sqrMagnitude;

        if (railLengthSqr <= 0.0001f)
        {
            return new WoodSegment(0f, 1f);
        }

        Bounds bounds = renderer.bounds;
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3[] corners =
        {
        center + new Vector3( extents.x,  extents.y,  extents.z),
        center + new Vector3( extents.x,  extents.y, -extents.z),
        center + new Vector3( extents.x, -extents.y,  extents.z),
        center + new Vector3( extents.x, -extents.y, -extents.z),
        center + new Vector3(-extents.x,  extents.y,  extents.z),
        center + new Vector3(-extents.x,  extents.y, -extents.z),
        center + new Vector3(-extents.x, -extents.y,  extents.z),
        center + new Vector3(-extents.x, -extents.y, -extents.z),
    };

        float min01 = float.MaxValue;
        float max01 = float.MinValue;

        for (int i = 0; i < corners.Length; i++)
        {
            float t = Vector3.Dot(corners[i] - left, railVector) / railLengthSqr;

            min01 = Mathf.Min(min01, t);
            max01 = Mathf.Max(max01, t);
        }

        return new WoodSegment(
            Mathf.Clamp01(min01),
            Mathf.Clamp01(max01)
        );
    }

    private GameObject SelectLargerPiece(GameObject upperHull, GameObject lowerHull, Vector3 axis)
    {
        float upperLength = GetProjectedMeshLength(upperHull, axis);
        float lowerLength = GetProjectedMeshLength(lowerHull, axis);

        return upperLength >= lowerLength ? upperHull : lowerHull;
    }

    private float GetProjectedMeshLength(GameObject target, Vector3 axis)
    {
        axis.Normalize();

        MeshFilter meshFilter = target.GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            return GetProjectedRendererLength(target, axis);
        }

        Vector3[] vertices = meshFilter.sharedMesh.vertices;

        if (vertices == null || vertices.Length == 0)
        {
            return 0f;
        }

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPoint = target.transform.TransformPoint(vertices[i]);
            float projected = Vector3.Dot(worldPoint, axis);

            min = Mathf.Min(min, projected);
            max = Mathf.Max(max, projected);
        }

        return max - min;
    }

    private float GetProjectedRendererLength(GameObject target, Vector3 axis)
    {
        Renderer renderer = target.GetComponent<Renderer>();

        if (renderer == null)
        {
            return 0f;
        }

        Bounds bounds = renderer.bounds;

        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3[] corners =
        {
        center + new Vector3( extents.x,  extents.y,  extents.z),
        center + new Vector3( extents.x,  extents.y, -extents.z),
        center + new Vector3( extents.x, -extents.y,  extents.z),
        center + new Vector3( extents.x, -extents.y, -extents.z),
        center + new Vector3(-extents.x,  extents.y,  extents.z),
        center + new Vector3(-extents.x,  extents.y, -extents.z),
        center + new Vector3(-extents.x, -extents.y,  extents.z),
        center + new Vector3(-extents.x, -extents.y, -extents.z),
    };

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < corners.Length; i++)
        {
            float projected = Vector3.Dot(corners[i], axis);

            min = Mathf.Min(min, projected);
            max = Mathf.Max(max, projected);
        }

        return max - min;
    }

    private void ApplySourceTransformToHull(GameObject piece, Transform sourceParent, Vector3 sourceLocalPosition
    , Quaternion sourceLocalRotation, Vector3 sourceLocalScale)
    {
        if (piece == null) return;

        piece.transform.SetParent(sourceParent, false);
        piece.transform.localPosition = sourceLocalPosition;
        piece.transform.localRotation = sourceLocalRotation;
        piece.transform.localScale = sourceLocalScale;
    }

    private void SetupSlicedPiece(GameObject piece)
    {
        if (piece == null) return;

        piece.tag = "WoodLog";

        MeshCollider meshCollider = piece.GetComponent<MeshCollider>();

        if (meshCollider == null) meshCollider = piece.AddComponent<MeshCollider>();

        meshCollider.convex = true;

        Rigidbody rb = piece.GetComponent<Rigidbody>();

        if (rb == null) rb = piece.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void ThrowAwayDiscardPiece(
    GameObject discardPiece,
    GameObject keepPiece,
    Vector3 planeWorldNormal)
    {
        if (discardPiece == null)
        {
            Debug.LogError("[WoodChop] discardPiece가 null입니다.");
            return;
        }

        Rigidbody rb = discardPiece.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = discardPiece.AddComponent<Rigidbody>();
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        // 버리는 조각이 남는 조각 기준 어느 쪽에 있는지 계산
        Vector3 keepCenter = GetWorldBoundsCenter(keepPiece);
        Vector3 discardCenter = GetWorldBoundsCenter(discardPiece);

        float side = Vector3.Dot(
            discardCenter - keepCenter,
            planeWorldNormal
        );

        Vector3 sideDirection = side >= 0f
            ? planeWorldNormal
            : -planeWorldNormal;

        // 옆으로만 날리면 밋밋하니까 살짝 위로 띄움
        Vector3 forceDirection = (sideDirection + Vector3.up * 0.35f).normalized;

        // 버리는 조각은 더 이상 현재 장작 판정에 쓰이지 않도록 태그 제거
        discardPiece.tag = "Untagged";

        rb.AddForce(forceDirection * discardForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * discardTorque, ForceMode.Impulse);

        Destroy(discardPiece, discardDestroyDelay);

        Debug.Log(
            $"[WoodChop] 버리는 조각 날림 / discard={discardPiece.name}, dir={forceDirection}, force={discardForce}"
        );
    }

    //Master가 시간을 계산한다.
    private void Master_CheckTimeout()
    {
        if (isResolvingStrike) return;

        //시작 시간으로부터 현재 시간간의 차이를 구한다.
        double elapsed = PhotonNetwork.Time - turnStartTime;

        //제한 시간 내이면 아무것도 안한다.
        if (elapsed < turnTimeLimit) return;

        //패배 처리 수행
        int loserActorNumber = GetActorNumberByPlayerIndex(syncedCurrentPlayerIndex);

        Master_EndDuel(loserActorNumber, "시간 초과");
    }

    //Master가 미니게임을 끝내는 함수
    private void Master_EndDuel(int loserActorNumber, string reason)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!isPlaying) return;

        isPlaying = false;

        int winnerActorNumber = loserActorNumber == playerAActorNumber ? playerBActorNumber : playerAActorNumber;

        BettingSystemController.instance.Master_SettleBetResult(winnerActorNumber, loserActorNumber, energyBet, reason);

        //게임 끝내기
        //photonView.RPC(nameof(RPC_EndDuel), RpcTarget.All, loserActorNumber, winnerActorNumber);
    }

    public void EndDuel(int loserActorNumber, int winnerActorNumber, int loserReductEnergy, int winnerEarnEnergy, string reason, float villageDamage = 0f)
    {
        photonView.RPC(nameof(RPC_EndDuel), RpcTarget.All, loserActorNumber, winnerActorNumber, loserReductEnergy, winnerEarnEnergy, villageDamage, reason);
    }

    [PunRPC]
    private void RPC_EndDuel(int loserActorNumber, int winnerActorNumber, int loserReductEnergy, int winnerEarnEnergy, float villageDamage, string reason)
    {
        isPlaying = false;
        ResetAxeRuntimeState();

        bool isWin = PhotonNetwork.LocalPlayer.ActorNumber == winnerActorNumber ? true : false;
        if (isWin)
        {
            StartCoroutine(CO_EndDuel(isWin, winnerEarnEnergy, villageDamage));
        }
        else
        {
            StartCoroutine(CO_EndDuel(isWin, loserReductEnergy, villageDamage));
        }


        Debug.Log($"나무 쪼개기 종료 / 승리: Player {winnerActorNumber}, 패배: Player {loserActorNumber}, 사유: {reason}");
    }

    private IEnumerator CO_EndDuel(bool isWin, int energy, float villageHP = 0)
    {
        woodChopUIController.SetWinLoseUI(isWin, energyBet, villageHP);

        yield return new WaitForSeconds(waitDuraion);

        TimeManager.instance.ResumeMainTurnTimerAfterMiniGame();
        CameraSwitchManager.Instance.LogMiniGame_to_Player();
        woodChopUIController.UIOff();
        woodChopUIController.RestoreCanvasAfterMiniGame();
    }

    private void ResetAxeRuntimeState()
    {
        isResolvingStrike = false;
        isWaitingMasterResult = false;
        isAxeFrozenByNetwork = false;

        if (strikeCoroutine != null)
        {
            StopCoroutine(strikeCoroutine);
            strikeCoroutine = null;
        }
    }

    private void UpdateAxeView()
    {
        if (axeRoot == null) return;
        if (axeLeftPoint == null || axeRightPoint == null) return;
        if (axeUpPoint == null) return;

        float axeX01 = GetAxeX01(PhotonNetwork.Time);

        axeRoot.position = GetAxeUpPosition(axeX01);
    }

    private Vector3 GetCutAxisNormal()
    {
        return (axeRightPoint.position - axeLeftPoint.position).normalized;
    }

    private Vector3 GetCutPlanePoint(float cutX01)
    {
        Vector3 railPoint = Vector3.Lerp(axeLeftPoint.position, axeRightPoint.position, cutX01);

        Vector3 axis = GetCutAxisNormal();
        Vector3 woodCenter = GetWorldBoundsCenter(currentWood);

        float distanceOnAxis = Vector3.Dot(railPoint - woodCenter, axis);

        return woodCenter + axis * distanceOnAxis;
    }

    private Vector3 GetAxeRootPositionForBladeTarget(Vector3 bladeTargetWorld)
    {
        if (axeBladePoint == null)
        {
            return bladeTargetWorld;
        }

        Vector3 bladeOffset = axeBladePoint.position - axeRoot.position;

        return bladeTargetWorld - bladeOffset;
    }

    private Vector3 GetAxeUpPosition(float cutX01)
    {
        Vector3 railPoint = Vector3.Lerp(
            axeLeftPoint.position,
            axeRightPoint.position,
            cutX01
        );

        Vector3 bladeTarget = new Vector3(
            railPoint.x,
            axeUpPoint.position.y,
            railPoint.z
        );

        return GetAxeRootPositionForBladeTarget(bladeTarget);
    }

    private Vector3 GetAxeDownPosition(float cutX01)
    {
        Vector3 railPoint = Vector3.Lerp(
            axeLeftPoint.position,
            axeRightPoint.position,
            cutX01
        );

        Vector3 bladeTarget = new Vector3(
            railPoint.x,
            axeDownPoint.position.y,
            railPoint.z
        );

        return GetAxeRootPositionForBladeTarget(bladeTarget);
    }


    private void SpawnLocalWood()
    {
        if (currentWood != null)
        {
            Destroy(currentWood);
        }

        currentWood = Instantiate(
            woodPrefab,
            woodSpawnPoint.position,
            woodSpawnPoint.rotation,
            woodParent
        );

        currentWood.name = "Current_Wood";
        currentWood.tag = "WoodLog";
    }

    private int GetActorNumberByPlayerIndex(int playerIndex)
    {
        if (playerIndex == 0)
        {
            return playerAActorNumber;
        }

        return playerBActorNumber;
    }

    private float GetAxeX01(double networkTime)
    {
        float elapsed = Mathf.Max(0f, (float)(networkTime - turnStartTime));
        float raw = elapsed * axeMoveSpeed;

        float repeated = Mathf.Repeat(raw, 2f);
        if (repeated <= 1f) return repeated;
        return 2f - repeated;
    }


    private Vector3 GetWorldBoundsCenter(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();

        if (renderer != null)
        {
            return renderer.bounds.center;
        }

        return target.transform.position;
    }

    private bool IsPlayerInRoom(int actorNumber)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == actorNumber)
            {
                return true;
            }
        }

        return false;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!isPlaying) return;

        int leftActorNumber = otherPlayer.ActorNumber;

        if (leftActorNumber == playerAActorNumber || leftActorNumber == playerBActorNumber)
        {
            Master_EndDuel(leftActorNumber, "플레이어 이탈");
        }
    }
}
