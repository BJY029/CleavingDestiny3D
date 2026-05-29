using UnityEngine;
using PrimeTween;
using System;

public class FirstPersonTweenAnimator : MonoBehaviour
{
    public enum MovementState { Idle, Walk, Run }

    // PlayerAnimationController로부터 전달받을 참조 변수들
    private Transform axeTransform;
    private Transform itemTransform;
    private MeshRenderer itemMeshRenderer;
    private Pose itemUsePoint;
    private MaterialPropertyBlock itemMpb;
    private Transform cameraAnimationPivot; // 카메라 회전 연출용 피벗

    // 기본 위치/회전 상태 저장용
    private Vector3 axeDefaultPos;
    private Quaternion axeDefaultRot;

    // Tween 제어용 시퀀스
    private Sequence bobbingSequence;
    private Sequence actionSequence;

    private MovementState currentMovementState = MovementState.Idle;
    private bool isPerformingAction = false;
    private bool isInitialized = false;

    public void Initialize(Transform axe, Transform item, MeshRenderer itemMesh, Pose usePoint, MaterialPropertyBlock mpb, Transform cameraPivot)
    {
        axeTransform = axe;
        itemTransform = item;
        itemMeshRenderer = itemMesh;
        itemUsePoint = usePoint;
        itemMpb = mpb;
        cameraAnimationPivot = cameraPivot;

        if (axeTransform != null)
        {
            axeDefaultPos = axeTransform.localPosition;
            axeDefaultRot = axeTransform.localRotation;
        }

        isInitialized = true;

        // 초기화 완료 후 첫 Bobbing 루프 구동
        PlayBobbing();
    }

    /// <summary>
    /// 플레이어의 이동 상태에 맞춰 밥빙(Bobbing) 상태를 갱신합니다.
    /// </summary>
    public void SetMovementState(MovementState state)
    {
        if (currentMovementState == state) return;
        currentMovementState = state;

        if (isInitialized && !isPerformingAction)
        {
            PlayBobbing();
        }
    }

    /// <summary>
    /// Weapon Bobbing 재생 (Idle, Walk, Run)
    /// </summary>
    private void PlayBobbing()
    {
        if (!isInitialized || axeTransform == null) return;
        if (bobbingSequence.isAlive) bobbingSequence.Stop();
        if (isPerformingAction) return;

        // axe의 위치가 이상한 곳에 있을 경우 기본 위치로 Tween
        if (Vector3.Distance(axeTransform.localPosition, axeDefaultPos) > 0.01f || Quaternion.Angle(axeTransform.localRotation, axeDefaultRot) > 1f)
        {
            Tween.LocalPosition(axeTransform, axeDefaultPos, 0.1f, Ease.OutQuad);
            Tween.LocalRotation(axeTransform, axeDefaultRot, 0.1f, Ease.OutQuad);
        }
        // axeTransform.SetLocalPositionAndRotation(axeDefaultPos, axeDefaultRot);

        bobbingSequence = Sequence.Create(-1);

        switch (currentMovementState)
        {
            case MovementState.Idle:
                // ==========================================
                // [Idle Bobbing - 숨쉬기 & 미세 좌우 흔들림]
                // ==========================================
                bobbingSequence
                    .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0.004f, 0.008f, 0f), 1.2f, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.3f, 0.3f, 0.2f), 1.2f, Ease.InOutSine))
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.004f, -0.008f, 0f), 2.4f, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(-0.3f, -0.3f, -0.2f), 2.4f, Ease.InOutSine))
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos, 1.2f, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot, 1.2f, Ease.InOutSine));
                break;

            case MovementState.Walk:
                // ==========================================
                // [Walk Bobbing - 걷기]
                // ==========================================
                float walkDuration = 0.38f;

                bobbingSequence
                    .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.008f, -0.005f, 0.002f), walkDuration, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.6f, -0.8f, -1f), walkDuration, Ease.InOutSine))
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.004f, 0.006f, 0f), walkDuration, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.2f, -0.2f, 0f), walkDuration, Ease.InOutSine))
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0.008f, -0.005f, 0.002f), walkDuration, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.6f, 0.8f, 1f), walkDuration, Ease.InOutSine))
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos, walkDuration, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot, walkDuration, Ease.InOutSine));
                break;

            case MovementState.Run:
                // ==========================================
                // [Run Bobbing - 달리기]
                // ==========================================
                float runDuration = 0.25f;

                bobbingSequence
                    .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.025f, -0.016f, 0.01f), runDuration, Ease.InOutQuad))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(2.0f, -2.5f, -4f), runDuration, Ease.InOutQuad))
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.012f, 0.018f, 0f), runDuration, Ease.InOutQuad))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.8f, -0.8f, 0f), runDuration, Ease.InOutQuad))
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0.025f, -0.016f, 0.01f), runDuration, Ease.InOutQuad))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(2.0f, 2.5f, 4f), runDuration, Ease.InOutQuad))
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos, runDuration, Ease.InOutQuad))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot, runDuration, Ease.InOutQuad));
                break;
        }
    }

    /// <summary>
    /// [타격 1단계: 준비 페이즈] 도끼를 오른쪽 어깨 위로 깊게 치켜들며 가로 날로 조준 상태를 형성합니다.
    /// </summary>
    public void PlayReadyAnimation()
    {
        if (!isInitialized) return;
        isPerformingAction = true;

        if (bobbingSequence.isAlive) bobbingSequence.Stop();
        if (actionSequence.isAlive) actionSequence.Stop();

        Vector3 readyPos = new Vector3(0.65f, 0.95f, 0.35f);
        Quaternion readyRot = Quaternion.Euler(-15f, 75f, -80f);
        Quaternion camReadyRot = Quaternion.Euler(0f, 60f, 0f);

        actionSequence = Sequence.Create()
            .Group(Tween.LocalRotation(axeTransform, readyRot, 0.4f, Ease.OutQuad))
            .Group(Tween.LocalPosition(axeTransform, readyPos, 0.4f, Ease.OutQuad))
            .Group(Tween.LocalRotation(cameraAnimationPivot, camReadyRot, 0.4f, Ease.OutQuad));
    }

    /// <summary>
    /// [타격 2&3단계: 타격 및 복귀 페이즈] 플레이어 몸체 정면(X = 0, Z = +0.7m)에 위치한 나무를 향해 날카롭게 수평 횡베기를 날립니다.
    /// </summary>
    public void PlayStrikeAnimation(Action onImpactEvent, Action onCompleteCallback)
    {
        if (!isInitialized) return;
        isPerformingAction = true;

        if (actionSequence.isAlive) actionSequence.Stop();

        // Y좌표 1.0f로 상향 조절 및 회전 각도를 기획에 맞춘 절대각으로 대입
        Vector3 strikePos = new Vector3(-0.1f, 1.0f, 0.7f);
        Quaternion strikeRot = Quaternion.Euler(15.54f, -85.617f, -70.569f);

        actionSequence = Sequence.Create()
            // ----------------------------------------------------
            // [2단계] 타격 페이즈 (Strike) - 정면 수평 횡베기 강타 (0.13초)
            // ----------------------------------------------------
            .Group(Tween.LocalPosition(axeTransform, strikePos, 0.13f, Ease.InQuad))
            .Group(Tween.LocalRotation(axeTransform, strikeRot, 0.13f, Ease.InQuad))
            .Group(Tween.LocalRotation(cameraAnimationPivot, Quaternion.identity, 0.13f, Ease.InQuad))

            // 타격 시점 싱크 콜백 (이펙트, 화면 흔들림, 사운드 가동)
            .Group(Sequence.Create()
                .ChainDelay(0.10f)
                .ChainCallback(() => onImpactEvent?.Invoke())
            )

            // ----------------------------------------------------
            // [3단계] 복귀 페이즈 (Recovery) - 타격 후 딜레이 및 복구
            // ----------------------------------------------------
            .Chain(Tween.Delay(0.2f))
            .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos, 0.5f, Ease.OutQuad))
            .Group(Tween.LocalRotation(axeTransform, axeDefaultRot, 0.5f, Ease.OutQuad))
            .Group(Tween.LocalRotation(cameraAnimationPivot, Quaternion.identity, 0.5f, Ease.OutQuad))
            .OnComplete(() =>
            {
                isPerformingAction = false;
                onCompleteCallback?.Invoke();
                PlayBobbing();
            });
    }

    /// <summary>
    /// 1인칭 Hit 애니메이션 전체를 순차 재생합니다.
    /// </summary>
    public void PlayHitAnimation(Action onImpactEvent, Action onCompleteCallback)
    {
        if (!isInitialized || isPerformingAction) return;
        isPerformingAction = true;

        if (bobbingSequence.isAlive) bobbingSequence.Stop();
        if (actionSequence.isAlive) actionSequence.Stop();

        PlayReadyAnimation();

        Sequence.Create()
            .ChainDelay(0.4f) // 준비 동작 완수까지 대기
            .ChainDelay(0.6f) // 준비 자세에서 타격까지의 지연 (풀타임)
            .ChainCallback(() => PlayStrikeAnimation(onImpactEvent, onCompleteCallback));
    }

    /// <summary>
    /// [아이템 사용 애니메이션] 도끼를 집어넣고 아이템을 가운데로 띄워 소모하는 프리미엄 연출
    /// </summary>
    public void PlayUseItemAnimation(Transform itemSlotTransform, ItemClass currentItemClass, Texture itemTexture, Action onCompleteCallback)
    {
        if (!isInitialized || isPerformingAction || itemTransform == null) return;
        isPerformingAction = true;

        if (bobbingSequence.isAlive) bobbingSequence.Stop();
        if (actionSequence.isAlive) actionSequence.Stop();

        // Material Property Block 셋업
        if (itemMeshRenderer != null && itemMpb != null)
        {
            itemMeshRenderer.GetPropertyBlock(itemMpb);
            if (itemTexture != null)
            {
                itemMpb.SetTexture("_BaseMap", itemTexture);
                itemMpb.SetColor("_BaseColor", Color.white);
            }
            itemMeshRenderer.SetPropertyBlock(itemMpb);
        }

        // 아이템 초기 위치 & 스케일 세팅
        itemTransform.localScale = Vector3.one;
        itemTransform.gameObject.SetActive(true);
        itemTransform.SetPositionAndRotation(itemSlotTransform.position, itemSlotTransform.rotation);

        actionSequence = Sequence.Create()
            .Group(Tween.LocalPosition(itemTransform, itemUsePoint.position, 0.5f, Ease.InOutQuad))
            .Group(Tween.LocalRotation(itemTransform, itemUsePoint.rotation, 0.5f, Ease.InOutQuad))
            .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0, -0.6f, -0.2f), 0.5f, Ease.InOutQuad))
            .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(20f, 0, 0), 0.5f, Ease.InOutQuad))

            .Chain(Tween.Scale(itemTransform, Vector3.one * 1.5f, 0.4f, Ease.OutBack))
            .Group(Tween.ShakeLocalRotation(itemTransform, new Vector3(0, 15f, 0), 0.4f))
            .Chain(Tween.Scale(itemTransform, Vector3.zero, 0.4f, Ease.InBack))
            .OnComplete(() =>
            {
                VFXManager.Instance.PlayItemExplosion(itemTransform.position, currentItemClass);
                itemTransform.gameObject.SetActive(false);

                Tween.LocalPosition(axeTransform, axeDefaultPos, 0.35f, Ease.OutBack);
                Tween.LocalRotation(axeTransform, axeDefaultRot, 0.35f, Ease.OutBack)
                    .OnComplete(() =>
                    {
                        isPerformingAction = false;
                        onCompleteCallback?.Invoke();
                        PlayBobbing();
                    });
            });
    }

    private void OnDestroy()
    {
        if (bobbingSequence.isAlive) bobbingSequence.Stop();
        if (actionSequence.isAlive) actionSequence.Stop();
    }
}
