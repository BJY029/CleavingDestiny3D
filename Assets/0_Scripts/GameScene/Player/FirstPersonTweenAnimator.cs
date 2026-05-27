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

    // 기본 위치/회전 상태 저장용
    private Vector3 axeDefaultPos;
    private Quaternion axeDefaultRot;

    // Tween 제어용 시퀀스
    private Sequence bobbingSequence;
    private Sequence actionSequence;

    private MovementState currentMovementState = MovementState.Idle;
    private bool isPerformingAction = false;
    private bool isInitialized = false;

    /// <summary>
    /// PlayerAnimationController에서 에디터 상에 이미 등록된 변수들을 공유받아 초기화합니다.
    /// </summary>
    public void Initialize(Transform axe, Transform item, MeshRenderer itemMesh, Pose usePoint, MaterialPropertyBlock mpb)
    {
        axeTransform = axe;
        itemTransform = item;
        itemMeshRenderer = itemMesh;
        itemUsePoint = usePoint;
        itemMpb = mpb;

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
    /// 절차적 프리미엄 Weapon Bobbing 재생 (Idle, Walk, Run)
    /// </summary>
    private void PlayBobbing()
    {
        if (!isInitialized || axeTransform == null) return;
        if (bobbingSequence.isAlive) bobbingSequence.Stop();
        if (isPerformingAction) return;

        // [끊김 해결] 상태 전환 시점에만 단 한 번 즉시 기본값으로 위치를 세팅하여 
        // 무한 루프 내에서 불필요하게 콜백 스냅(Set)이 중복 호출되며 끊기는 현상을 방지합니다.
        axeTransform.SetLocalPositionAndRotation(axeDefaultPos, axeDefaultRot);

        bobbingSequence = Sequence.Create(-1);

        switch (currentMovementState)
        {
            case MovementState.Idle:
                // ==========================================
                // [Idle Bobbing - 숨쉬기 & 미세 좌우 흔들림]
                // ==========================================
                // [완벽한 대칭 사인 루프 완성]
                // Y축 오실레이션이 0 -> +최대(1.2s) -> -최대(2.4s) -> 0(1.2s)으로 매끄럽게 흐르며 
                // 무한 반복 시 시작점과 끝점이 완벽히 일치하여 끊김이 완전히 사라집니다.
                bobbingSequence
                    // 1. 들숨 연출 (+최대)
                    .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0.004f, 0.008f, 0f), 1.2f, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.3f, 0.3f, 0.2f), 1.2f, Ease.InOutSine))
                    // 2. 날숨 연출 (-최대)
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.004f, -0.008f, 0f), 2.4f, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(-0.3f, -0.3f, -0.2f), 2.4f, Ease.InOutSine))
                    // 3. 기본 상태로 부드러운 복귀 (0)
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos, 1.2f, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot, 1.2f, Ease.InOutSine));
                break;

            case MovementState.Walk:
                // ==========================================
                // [Walk Bobbing - 걷기]
                // ==========================================
                float walkDuration = 0.38f;

                bobbingSequence
                    // 1. 첫 번째 걸음 (왼발)
                    .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.008f, -0.005f, 0.002f), walkDuration, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.6f, -0.8f, -1f), walkDuration, Ease.InOutSine))
                    
                    // 중간 복원
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.004f, 0.006f, 0f), walkDuration, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.2f, -0.2f, 0f), walkDuration, Ease.InOutSine))

                    // 2. 두 번째 걸음 (오른발)
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0.008f, -0.005f, 0.002f), walkDuration, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.6f, 0.8f, 1f), walkDuration, Ease.InOutSine))

                    // 최종 복원 (시작점으로 완벽히 수렴)
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos, walkDuration, Ease.InOutSine))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot, walkDuration, Ease.InOutSine));
                break;

            case MovementState.Run:
                // ==========================================
                // [Run Bobbing - 고품질 무한대 루프 흔들림]
                // ==========================================
                // [달리기 연출 범위/반동 대폭 강화]
                // Y축 및 X축 진폭을 약 1.6배 이상 키워 역동감과 무게감을 강화했습니다.
                float runDuration = 0.25f;

                bobbingSequence
                    // 1. 첫 번째 걸음 (왼발 파워 디딤)
                    .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.025f, -0.016f, 0.01f), runDuration, Ease.InOutQuad))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(2.0f, -2.5f, -4f), runDuration, Ease.InOutQuad))
                    
                    // 중간 반동 및 탄력 복원
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(-0.012f, 0.018f, 0f), runDuration, Ease.InOutQuad))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(0.8f, -0.8f, 0f), runDuration, Ease.InOutQuad))
                    
                    // 2. 두 번째 걸음 (오른발 파워 디딤)
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0.025f, -0.016f, 0.01f), runDuration, Ease.InOutQuad))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(2.0f, 2.5f, 4f), runDuration, Ease.InOutQuad))
                    
                    // 최종 복원 (시작점으로 완벽히 수렴)
                    .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos, runDuration, Ease.InOutQuad))
                    .Group(Tween.LocalRotation(axeTransform, axeDefaultRot, runDuration, Ease.InOutQuad));
                break;
        }
    }

    /// <summary>
    /// [타격 애니메이션] 도끼를 강력하게 휘두르는 시퀀스 실행
    /// </summary>
    public void PlayHitAnimation(Action onImpactEvent, Action onCompleteCallback)
    {
        if (!isInitialized || isPerformingAction) return;
        isPerformingAction = true;

        if (bobbingSequence.isAlive) bobbingSequence.Stop();
        if (actionSequence.isAlive) actionSequence.Stop();

        float speedMultiplier = 0.4f;

        actionSequence = Sequence.Create()
            .Group(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(new Vector3(-55f, -55f, -85f)), 1.167f * speedMultiplier, Ease.OutQuad))
            .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0f, 0.037f, 0f), 1.167f * speedMultiplier, Ease.OutQuad))
            .Chain(Tween.LocalRotation(axeTransform, axeDefaultRot * Quaternion.Euler(new Vector3(-50f, -50f, -80f)), 0.333f * speedMultiplier, Ease.Linear))
            .Group(Tween.LocalPosition(axeTransform, axeDefaultPos + new Vector3(0f, 0.034f, 0f), 0.333f * speedMultiplier, Ease.Linear))
            .Chain(Tween.LocalPosition(axeTransform, new Vector3(0.192f, 1.004f, 0.588f), 0.283f * speedMultiplier, Ease.InQuad))
            .Group(Tween.LocalRotation(axeTransform, Quaternion.Euler(new Vector3(-1.232f, -104.38f, -81.186f)), 0.283f * speedMultiplier, Ease.InQuad))

            .Group(Sequence.Create()
                .ChainDelay(0.2f * speedMultiplier)
                .ChainCallback(() => onImpactEvent?.Invoke())
            )

            .Chain(Tween.Delay(0.484f * speedMultiplier))
            .Chain(Tween.LocalPosition(axeTransform, axeDefaultPos, 0.933f * speedMultiplier, Ease.OutQuad))
            .Group(Tween.LocalRotation(axeTransform, axeDefaultRot, 0.933f * speedMultiplier, Ease.OutQuad))
            .OnComplete(() =>
            {
                isPerformingAction = false;
                onCompleteCallback?.Invoke();
                PlayBobbing();
            });
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
