using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using EzySlice;

public class WoodChopController : MonoBehaviourPunCallbacks, IMinigameInteractable
{
    public static WoodChopController instance;

    [Header("Wood")]
    [SerializeField] private GameObject woodPrefab;
    [SerializeField] private Transform woodSpawnPoint;
    [SerializeField] private Transform woodParent;
    [SerializeField] private Material crossSectionMaterial;

    [Header("Axe Auto Move")]
    [SerializeField] private Transform axeRoot;
    [SerializeField] private Transform axeLeftPoint;
    [SerializeField] private Transform axeRightPoint;
    [SerializeField] private Transform axeUpPoint;
    [SerializeField] private Transform axeDownPoint;

    [SerializeField] private float axeMoveSpeed = 0.8f;
    [SerializeField] private float axeStrikeDownTime = 0.12f;
    [SerializeField] private float axeStrikeUpTime = 0.18f;
    [SerializeField] private float afterStrikeDelay = 0.15f;

    private bool isResolvingStrike;
    private float frozenAxeX01;

    [Header("Slice Physics")]
    [SerializeField] private float discardForce = 2.5f;
    [SerializeField] private float discardTorque = 4f;
    [SerializeField] private float discardDestroyDelay = 2f;

    [Header("Rule Settings")]
    [SerializeField] private float edgeMargin = 0.02f;
    [SerializeField] private float minChoppableWidth = 0.06f;

    [SerializeField] private float turnTimeLimit = 3f;

    private GameObject currentWood;

    private WoodChopDuelRules rules;
    public bool isPlaying { get; private set; }

    private int playerAActorNumber;
    private int playerBActorNumber;

    private int energyBet;
    private int turnCnt;

    private double turnStartTime;

    private float syncedSegmentLeft;
    private float syncedSegmentRight;
    private int syncedCurrentPlayerIndex;

    private int CurrentTurnActorNumber
    {
        get
        {
            return syncedCurrentPlayerIndex == 0 ? playerAActorNumber : playerBActorNumber;
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);


        //규칙 객체 초기화
        rules = new WoodChopDuelRules(
            playerCount: 2,
            edgeMargin: edgeMargin,
            minChoppableWidth: minChoppableWidth
        );
    }

    //미니게임 진행중이면 시간을 지속적으로 계산한다.
    private void Update()
    {
        if (!isPlaying) return;
        if (!isResolvingStrike) UpdateAxeView();

        if (PhotonNetwork.IsMasterClient) Master_CheckTimeout();
    }

    //미니게임 시작 함수
    public void RequestStartDual(Player targetPlayer, int betAmount)
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
        int p1Eng = PhotonPropertyHelper.GetPlayerProp<int>(requestActorNumber, PlayerPropKeys.Energy);
        int p2Eng = PhotonPropertyHelper.GetPlayerProp<int>(targetActorNumber, PlayerPropKeys.Energy);
        if (p1Eng < betAmount || p2Eng < betAmount)
        {
            Debug.LogWarning("배팅 기력 부족");
            return;
        }

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

        rules.Reset(0);

        syncedSegmentLeft = rules.CurrentSegment.left;
        syncedSegmentRight = rules.CurrentSegment.right;
        syncedCurrentPlayerIndex = rules.CurrentPlayerIndex;

        turnStartTime = PhotonNetwork.Time + 0.2d;
        isPlaying = true;

        //Master에서 초기화된 값들 클라이언트들에게 전파
        photonView.RPC(nameof(RPC_SyncStartDuel), RpcTarget.All,
        playerAActorNumber, playerBActorNumber, energyBet, syncedSegmentLeft, syncedSegmentRight, syncedCurrentPlayerIndex,
        turnStartTime, turnCnt);
    }

    //각 클라이언트가 정보를 받아서 미니 게임 정보를 초기화한다.
    [PunRPC]
    private void RPC_SyncStartDuel(int actorA, int actorB, int betAmount,
    float segmentLeft, float segmentRight, int currentPlayerIndex, double startTime, int syncedTurnCount)
    {
        CameraSwitchManager.Instance.Player_to_LogMiniGame();

        playerAActorNumber = actorA;
        playerBActorNumber = actorB;
        energyBet = betAmount;

        syncedSegmentLeft = segmentLeft;
        syncedSegmentRight = segmentRight;
        syncedCurrentPlayerIndex = currentPlayerIndex;

        turnStartTime = startTime;
        turnCnt = syncedTurnCount;

        isResolvingStrike = false;
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

        int localActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if (localActorNumber != CurrentTurnActorNumber)
        {
            Debug.Log("현재 내 턴이 아닙니다.");
            return;
        }

        double pressTime = PhotonNetwork.Time;

        photonView.RPC(
            nameof(RPC_RequestChopByInteract),
            RpcTarget.MasterClient,
            localActorNumber,
            pressTime
        );
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

        int currentPlayerIndex = rules.CurrentPlayerIndex;
        int expectedActorNumber = GetActorNumberByPlayerIndex(currentPlayerIndex);
        if (requestActorNumber != expectedActorNumber) return;

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
        float oldSegmentLeft = rules.CurrentSegment.left;
        float oldSegmentRight = rules.CurrentSegment.right;

        ChopResolve result = rules.TryChop(currentPlayerIndex, cutX01);

        if (result.type == ChopResolveType.Ignored)
        {
            return;
        }

        if (result.type == ChopResolveType.Failed)
        {
            int loserActorNumber = GetActorNumberByPlayerIndex(result.loserIndex);
            Master_EndDuel(loserActorNumber, "쪼개기 실패");
            return;
        }

        if (result.type == ChopResolveType.Success)
        {
            bool keepRightPiece = Mathf.Abs(result.nextSegment.left - cutX01) < 0.001f;

            isResolvingStrike = true;

            double nextTurnStartTime =
                PhotonNetwork.Time + axeStrikeDownTime + axeStrikeUpTime + afterStrikeDelay;

            int nextTurnCnt = turnCnt + 1;

            photonView.RPC(
                nameof(RPC_ApplySuccessfulStrike),
                RpcTarget.All,
                cutX01,
                keepRightPiece,
                oldSegmentLeft,
                oldSegmentRight,
                result.nextSegment.left,
                result.nextSegment.right,
                rules.CurrentPlayerIndex,
                nextTurnStartTime,
                nextTurnCnt
            );
        }
    }

    [PunRPC]
    private void RPC_ApplySuccessfulStrike(
        float cutX01,
        bool keepRightPiece,
        float oldSegmentLeft,
        float oldSegmentRight,
        float nextSegmentLeft,
        float nextSegmentRight,
        int nextPlayerIndex,
        double nextTurnStartTime,
        int syncedTurnCount)
    {
        StartCoroutine(Co_PlayAxeStrikeAndSlice(
            cutX01,
            keepRightPiece,
            oldSegmentLeft,
            oldSegmentRight,
            nextSegmentLeft,
            nextSegmentRight,
            nextPlayerIndex,
            nextTurnStartTime,
            syncedTurnCount
        ));
    }

    private System.Collections.IEnumerator Co_PlayAxeStrikeAndSlice(
    float cutX01,
    bool keepRightPiece,
    float oldSegmentLeft,
    float oldSegmentRight,
    float nextSegmentLeft,
    float nextSegmentRight,
    int nextPlayerIndex,
    double nextTurnStartTime,
    int syncedTurnCount)
    {
        isResolvingStrike = true;
        frozenAxeX01 = cutX01;

        Vector3 xPosition = Vector3.Lerp(
            axeLeftPoint.position,
            axeRightPoint.position,
            cutX01
        );

        Vector3 upPosition = new Vector3(
            xPosition.x,
            axeUpPoint.position.y,
            xPosition.z
        );

        Vector3 downPosition = new Vector3(
            xPosition.x,
            axeDownPoint.position.y,
            xPosition.z
        );

        float t = 0f;

        while (t < axeStrikeDownTime)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / axeStrikeDownTime);

            axeRoot.position = Vector3.Lerp(upPosition, downPosition, ratio);

            yield return null;
        }

        axeRoot.position = downPosition;

        SliceCurrentWood(cutX01, keepRightPiece, oldSegmentLeft, oldSegmentRight);

        t = 0f;

        while (t < axeStrikeUpTime)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / axeStrikeUpTime);

            axeRoot.position = Vector3.Lerp(downPosition, upPosition, ratio);

            yield return null;
        }

        yield return new WaitForSeconds(afterStrikeDelay);

        syncedSegmentLeft = nextSegmentLeft;
        syncedSegmentRight = nextSegmentRight;
        syncedCurrentPlayerIndex = nextPlayerIndex;

        turnStartTime = nextTurnStartTime;
        turnCnt = syncedTurnCount;

        isResolvingStrike = false;

        Debug.Log($"장작 쪼개기 성공 / 다음 턴 Player {CurrentTurnActorNumber}");
    }

    private void SliceCurrentWood(float globalCutX01, bool keepRightPiece, float oldSegmentLeft, float oldSegmentRight)
    {
        if (currentWood == null) return;

        Bounds localBounds = GetLocalMeshBounds(currentWood);

        float local01 = Mathf.InverseLerp(oldSegmentLeft, oldSegmentRight, globalCutX01);

        float localCutX = Mathf.Lerp(localBounds.min.x, localBounds.max.x, local01);

        Vector3 localPlanePoint = new Vector3(localCutX, localBounds.center.y, localBounds.center.z);

        Vector3 planeWorldPoint = currentWood.transform.TransformPoint(localPlanePoint);

        Vector3 planeWorldNormal = currentWood.transform.right;

        SlicedHull hull = currentWood.Slice(planeWorldPoint, planeWorldNormal, crossSectionMaterial);

        if (hull == null) return;

        GameObject upperHull = hull.CreateUpperHull(currentWood, crossSectionMaterial);
        GameObject lowerHull = hull.CreateLowerHull(currentWood, crossSectionMaterial);

        Debug.Log($"[WoodChop] upperHull={(upperHull != null ? upperHull.name : "null")}");
        Debug.Log($"[WoodChop] lowerHull={(lowerHull != null ? lowerHull.name : "null")}");

        if (upperHull == null || lowerHull == null)
        {
            Debug.LogError("[WoodChop] Hull GameObject 생성 실패");
            return;
        }

        SetupSlicedPiece(upperHull);
        SetupSlicedPiece(lowerHull);

        GameObject keepPiece = SelectKeepPiece(upperHull, lowerHull, planeWorldPoint, planeWorldNormal, keepRightPiece);

        GameObject discardPiece = keepPiece == upperHull ? lowerHull : upperHull;

        Debug.Log($"[WoodChop] 기존 currentWood 제거 예정: {currentWood.name}");
        Destroy(currentWood);

        currentWood = keepPiece;
        currentWood.name = "current_wood";
        currentWood.tag = "WoodLog";

        Debug.Log($"[WoodChop] 유지 조각: {currentWood.name}, 버릴 조각: {discardPiece.name}");
        ThrowAwayDiscardPiece(discardPiece, planeWorldNormal, keepRightPiece);
    }

    private GameObject SelectKeepPiece(GameObject upperHull, GameObject lowerHull, Vector3 planeWorldPoint, Vector3 planeWorldNormal, bool keepRightPiece)
    {
        float upperSide = Vector3.Dot(GetWorldBoundsCenter(upperHull) - planeWorldPoint, planeWorldNormal);
        float lowerSide = Vector3.Dot(GetWorldBoundsCenter(lowerHull) - planeWorldPoint, planeWorldNormal);

        bool upperIsRight = upperSide > lowerSide;

        if (keepRightPiece) return upperIsRight ? upperHull : lowerHull;

        return upperIsRight ? lowerHull : upperHull;
    }

    private void SetupSlicedPiece(GameObject piece)
    {
        if (piece == null) return;

        piece.transform.SetParent(woodParent, true);
        piece.tag = "WoodLog";

        MeshCollider meshCollider = piece.GetComponent<MeshCollider>();

        if (meshCollider == null) meshCollider = piece.AddComponent<MeshCollider>();

        meshCollider.convex = true;

        Rigidbody rb = piece.GetComponent<Rigidbody>();

        if (rb == null) rb = piece.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void ThrowAwayDiscardPiece(GameObject discardPiece, Vector3 planeWorldNormal, bool keptRightPiece)
    {
        if (discardPiece == null)
        {
            Debug.LogError("[WoodChop] discardPiece가 null입니다.");
            return;
        }

        Rigidbody rb = discardPiece.GetComponent<Rigidbody>();


        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            Vector3 forceDirection = keptRightPiece ? -planeWorldNormal : planeWorldNormal;
            Debug.Log($"[WoodChop] 버리는 조각 날림 / discard={discardPiece.name}, dir={forceDirection}, force={discardForce}");
            rb.AddForce(forceDirection * discardForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * discardTorque, ForceMode.Impulse);
        }

        Destroy(discardPiece, discardDestroyDelay);
    }

    //Master가 시간을 계산한다.
    private void Master_CheckTimeout()
    {
        if (isResolvingStrike) return;

        //시작 시간으로부터 현재 시간간의 차이를 구한다.
        double elapsed = PhotonNetwork.Time - turnStartTime;

        //제한 시간 내이면 아무것도 안한다.
        if (elapsed < turnTimeLimit) return;

        //제한 시간을 넘은 경우 실패 처리한다.
        ChopResolve result = rules.FailCurrentPlayer();

        //패배 처리 수행
        int loserActorNumber = GetActorNumberByPlayerIndex(result.loserIndex);

        Master_EndDuel(loserActorNumber, "시간 초과");
    }

    //Master가 미니게임을 끝내는 함수
    private void Master_EndDuel(int loserActorNumber, string reason)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!isPlaying) return;

        isPlaying = false;

        int winnerActorNumber = loserActorNumber == playerAActorNumber ? playerBActorNumber : playerAActorNumber;

        //기력 처리
        int WinnerEng = PhotonPropertyHelper.GetPlayerProp<int>(winnerActorNumber, PlayerPropKeys.Energy);
        int WinnerEngMax = PhotonPropertyHelper.GetPlayerProp<int>(winnerActorNumber, PlayerPropKeys.MaxEnergy);
        int loserEng = PhotonPropertyHelper.GetPlayerProp<int>(loserActorNumber, PlayerPropKeys.Energy);

        PhotonPropertyHelper.SetPlayerProp(winnerActorNumber, PlayerPropKeys.Energy, Mathf.Min(WinnerEng + energyBet, WinnerEngMax));
        PhotonPropertyHelper.SetPlayerProp(loserActorNumber, PlayerPropKeys.Energy, Mathf.Max(loserEng - energyBet, 0));

        //게임 끝내기
        photonView.RPC(nameof(RPC_EndDuel), RpcTarget.All, loserActorNumber, winnerActorNumber, reason);
    }

    [PunRPC]
    private void RPC_EndDuel(int loserActorNumber, int winnerActorNumber, string reason)
    {
        isPlaying = false;

        CameraSwitchManager.Instance.LogMiniGame_to_Player();

        Debug.Log($"나무 쪼개기 종료 / 승리: Player {winnerActorNumber}, 패배: Player {loserActorNumber}, 사유: {reason}");

        // 결과 UI 표시
        // ResultPanel.Show(winnerActorNumber, loserActorNumber, reason);
    }

    private void UpdateAxeView()
    {
        if (axeRoot == null) return;
        if (axeLeftPoint == null || axeRightPoint == null) return;
        if (axeUpPoint == null) return;

        float axeX01 = isResolvingStrike ? frozenAxeX01 : GetAxeX01(PhotonNetwork.Time);

        Vector3 xPosition = Vector3.Lerp(axeLeftPoint.position, axeRightPoint.position, axeX01);

        Vector3 targetPosition = new Vector3(xPosition.x, axeUpPoint.position.y, xPosition.z);

        axeRoot.position = targetPosition;
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


    private Bounds GetLocalMeshBounds(GameObject target)
    {
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();

        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            return meshFilter.sharedMesh.bounds;
        }

        return new Bounds(Vector3.zero, Vector3.one);
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
