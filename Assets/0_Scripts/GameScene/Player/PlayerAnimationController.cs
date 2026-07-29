using UnityEngine;
using Photon.Pun;
using PrimeTween;
using Cysharp.Threading.Tasks;
using System;
using Random = UnityEngine.Random;

public class PlayerAnimationController : MonoBehaviourPun, IAnimNotify
{
    [Header("Animation Settings")]
    [SerializeField] private Animator animator; // 플레이어 애니메이터
    [SerializeField] private FirstPersonTweenAnimator firstPersonTweenAnimator; // 1인칭 도끼/아이템 트윈 애니메이터

    [Header("Axe Sway Settings")]
    [SerializeField] private Transform axeSwayTransform; // 도끼 트랜스폼
    [SerializeField] float rotSwayMultiplier = 1.0f;
    [SerializeField] float rotMaxSway = 15f;
    [SerializeField] float rotSmoothStep = 4f;
    [SerializeField] float posSwayMultiplier = 0.005f;
    [SerializeField] float posMaxSway = 0.05f; // 너무 많이 벗어나지 않게 제한
    [SerializeField] float posSmoothStep = 6f;
    private Quaternion initialRotation;
    private Vector3 initialPosition;

    [Header("Camera Shake Settings")]
    [SerializeField] Camera playerCamera; // 플레이어 카메라
    [SerializeField] private float shakeStrength = 0.5f;
    [SerializeField] private float shakeDuration = 0.4f;
    [SerializeField] private int shakeFrequency = 10;

    [Header("AI Sound Settings")]
    [SerializeField] private float aiHitSoundDelay = 0.4f; // AI 타격음 지연 시간 (초)

    [Header("Tween Animation Parts")]
    [SerializeField] Transform axeTransform; // 도끼 트랜스폼
    [SerializeField] Transform itemTransform; // 아이템 트랜스폼
    [SerializeField] MeshRenderer itemMeshRenderer; // 아이템 메시 렌더러
    int axeOriginalLayer;

    [Header("Item Use Animation Settings")]
    [SerializeField] Pose itemUsePoint; // 아이템이 이동할 위치

    [Header("IK Settings")]
    [SerializeField] private Transform leftHandTarget; // 도끼 자루의 왼손 타겟 위치
    [SerializeField] private Vector3 leftHandRotOffset = new Vector3(0f, 180f, 0f); // 손바닥/손등 뒤집힘 보정 회전 오프셋 (기본 180도)
    [SerializeField] private float leftHandWeight = 1.0f; // IK 적용 무게 (0~1)
    [SerializeField] private bool enableIK = true; // IK 사용 여부

    // 파라미터 해시화
    public static readonly int HashSpeedX = Animator.StringToHash("Speed_X");
    public static readonly int HashSpeedZ = Animator.StringToHash("Speed_Z");
    public static readonly int HashHitReady = Animator.StringToHash("HitReady");
    public static readonly int HashHit = Animator.StringToHash("Hit");
    int firstPersonLayer;

    private PlayerController playerController;
    private AIController aiController;

    private IAnimNotify animNotify;
    [SerializeField] bool isAI = false;

    MaterialPropertyBlock itemMpb;

    // [Header("Effect")]
    // [SerializeField] ParticleSystem hitEffectObject;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();

        if (TryGetComponent(out playerController))
        {
            animNotify = playerController;
        }
        else if (TryGetComponent(out aiController))
        {
            animNotify = aiController;
            isAI = true;
        }

        if (itemTransform != null)
        {
            itemMpb = new MaterialPropertyBlock();
            itemTransform.gameObject.SetActive(false); // 아이템 트랜스폼 초기 비활성화
        }

        if (axeTransform != null)
        {
            firstPersonLayer = LayerMask.NameToLayer("FirstPersonItem"); // "FirstPerson" 레이어 인덱스 저장
            axeOriginalLayer = axeTransform.gameObject.layer; // 도끼의 원래 레이어 저장
        }

        if (axeSwayTransform != null)
        {
            initialRotation = axeSwayTransform.localRotation; // 도끼의 초기 회전값 저장
            initialPosition = axeSwayTransform.localPosition; // 도끼의 초기 위치값 저장
        }

        Transform cameraAnimationPivot = null;
        if (playerCamera != null)
        {
            cameraAnimationPivot = playerCamera.transform.parent;
            if (cameraAnimationPivot != null && !cameraAnimationPivot.name.Contains("Pivot"))
            {
                cameraAnimationPivot = transform.Find("CameraHolder/CameraAnimationPivot");
            }
        }

        if (firstPersonTweenAnimator != null)
        {
            firstPersonTweenAnimator.Initialize(axeTransform, itemTransform, itemMeshRenderer, itemUsePoint, itemMpb, cameraAnimationPivot);
        }
    }

    // 이동 상태 업데이트 (컨트롤러에서 매 프레임 호출)
    public void UpdateMoveVisuals(float moveX, float moveZ, float deltaTime)
    {
        animator.SetFloat(HashSpeedX, moveX, 0.1f, deltaTime);
        animator.SetFloat(HashSpeedZ, moveZ, 0.1f, deltaTime);

        if (!isAI && photonView.IsMine && firstPersonTweenAnimator != null)
        {
            float speed = new Vector2(moveX, moveZ).magnitude;
            if (speed < 0.1f)
            {
                firstPersonTweenAnimator.SetMovementState(FirstPersonTweenAnimator.MovementState.Idle);
            }
            else if (speed > 0.7f)
            {
                firstPersonTweenAnimator.SetMovementState(FirstPersonTweenAnimator.MovementState.Run);
            }
            else
            {
                firstPersonTweenAnimator.SetMovementState(FirstPersonTweenAnimator.MovementState.Walk);
            }
        }
    }

    public void UpdateCamera(Vector2 lookDelta)
    {
        if (isAI || !photonView.IsMine) return;

        float mouseX_Rot = Mathf.Clamp(lookDelta.x * rotSwayMultiplier, -rotMaxSway, rotMaxSway);
        float mouseY_Rot = Mathf.Clamp(lookDelta.y * rotSwayMultiplier, -rotMaxSway, rotMaxSway);

        Quaternion targetRotation = Quaternion.Euler(mouseY_Rot, -mouseX_Rot, 0f) * initialRotation;

        float currentRotSmooth = lookDelta.magnitude > 0.1f ? rotSmoothStep * 1.5f : rotSmoothStep;
        axeSwayTransform.localRotation = Quaternion.Slerp(axeSwayTransform.localRotation, targetRotation, currentRotSmooth * Time.deltaTime);

        // 마우스를 움직인 반대 방향(-lookDelta)으로 도끼의 목표 위치를 잡습니다.
        float mouseX_Pos = Mathf.Clamp(lookDelta.x * posSwayMultiplier, -posMaxSway, posMaxSway);
        float mouseY_Pos = Mathf.Clamp(lookDelta.y * posSwayMultiplier, -posMaxSway, posMaxSway);

        Vector3 targetPosition = new Vector3(-mouseX_Pos, -mouseY_Pos, 0f) + initialPosition;

        axeSwayTransform.localPosition = Vector3.Lerp(axeSwayTransform.localPosition, targetPosition, posSmoothStep * Time.deltaTime);
    }

    [Header("First Person Animation Settings")]
    [SerializeField] private bool useTweenAnimator = false; // true: Tween 애니메이션 사용, false: 3인칭 Animator 공유 사용

    // Hit 애니메이션 실행
    public void PlayHitAnimation()
    {
        // 도끼 휘두르기
        if (!isAI && photonView.IsMine && useTweenAnimator && firstPersonTweenAnimator != null && axeTransform != null)
        {
            axeTransform.gameObject.layer = firstPersonLayer; // 도끼를 1인칭 레이어로 변경하여 잘 보이도록 설정

            firstPersonTweenAnimator.PlayHitAnimation(
                onImpactEvent: FirstPersonAxeHit,
                onCompleteCallback: () =>
                {
                    axeTransform.gameObject.layer = axeOriginalLayer; // 도끼 레이어 기본으로 되돌림
                }
            );
        }

        // 전체(타인 화면 포함 및 로컬): 애니메이션 트리거
        photonView.RPC(nameof(RPC_PlayHit), RpcTarget.All);
    }

    // 준비 자세 애니메이션 실행 (F Key Down 시점에 호출)
    public void PlayReadyAnimation()
    {
        if (!isAI && photonView.IsMine && useTweenAnimator && firstPersonTweenAnimator != null && axeTransform != null)
        {
            axeTransform.gameObject.layer = firstPersonLayer;
            firstPersonTweenAnimator.PlayReadyAnimation();
        }

        // 전체(타인 화면 포함): HitReady = true
        photonView.RPC(nameof(RPC_SetHitReady), RpcTarget.All, true);
    }

    // 준비 자세 애니메이션 취소 (취소 키 입력 시 호출)
    public void CancelReadyAnimation()
    {
        if (!isAI && photonView.IsMine && useTweenAnimator && firstPersonTweenAnimator != null && axeTransform != null)
        {
            firstPersonTweenAnimator.CancelReadyAnimation();
            axeTransform.gameObject.layer = axeOriginalLayer; // 도끼 레이어 복원
        }

        // 전체(타인 화면 포함): HitReady = false
        photonView.RPC(nameof(RPC_SetHitReady), RpcTarget.All, false);
    }

    // 타격 애니메이션 (F Key Up 또는 2번째 F Key Down 시점에 호출)
    public void PlayStrikeAnimation()
    {
        if (!isAI && photonView.IsMine && useTweenAnimator && firstPersonTweenAnimator != null && axeTransform != null)
        {
            firstPersonTweenAnimator.PlayStrikeAnimation(
                onImpactEvent: FirstPersonAxeHit,
                onCompleteCallback: () =>
                {
                    axeTransform.gameObject.layer = axeOriginalLayer; // 도끼 레이어 복원
                }
            );
        }

        // 전체(타인 화면 포함): 애니메이션 트리거
        photonView.RPC(nameof(RPC_PlayHit), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_SetHitReady(bool isReady)
    {
        if (animator == null) return;
        animator.SetBool(HashHitReady, isReady);
    }

    [PunRPC]
    private void RPC_PlayHit()
    {
        if (animator == null) return;
        animator.SetBool(HashHitReady, false); // 타격 시 준비 상태 해제
        animator.ResetTrigger(HashHit);
        animator.SetTrigger(HashHit);

        // AI인 경우, FBX 애니메이션 이벤트를 쓸 수 없으므로 지연 후 소리 재생
        if (isAI)
        {
            PlayAIImpactAsync().Forget();
        }
    }

    private async UniTaskVoid PlayAIImpactAsync()
    {
        try
        {
            // 지정된 시간만큼 대기 (애니메이션 휘두르는 시간에 맞춤)
            await UniTask.Delay(TimeSpan.FromSeconds(aiHitSoundDelay), cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException) { return; }

        PlayLocalHitSound();

        aiController?.RequestAttackAtImpact();
    }

    // 애니메이션 이벤트로부터 호출되는 함수
    public void OnAnimStateExit(int stateKey)
    {
        // IAnimNotify에 종료 사실을 알림
        animNotify?.OnAnimStateExit(stateKey);
    }

    public void FirstPersonAxeHit()
    {
        // 1인칭 타격 이펙트 재생 (카메라 쉐이크 및 이펙트는 로컬에서만)
        if (photonView.IsMine)
        {
            // hitEffectObject.Play();
            VFXManager.Instance.PlayPredefinedEffect(VFXManager.VFXIndex.Hit_Tree, axeTransform.position);

            DamageTextManager.instance?.CacheLocalHitPoint(axeTransform.position);

            playerController?.RequestAttackAtImpact();
        }

        Tween.ShakeLocalPosition(playerCamera.transform,
            strength: new Vector3(shakeStrength, shakeStrength, 0f),
            duration: shakeDuration,
            frequency: shakeFrequency);

        // 플레이어인 경우에만 1인칭 애니메이션 이벤트에서 소리를 트리거함
        if (!isAI && photonView.IsMine)
        {
            photonView.RPC(nameof(RPC_PlayHitSound), RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_PlayHitSound()
    {
        PlayLocalHitSound();
    }

    private void PlayLocalHitSound()
    {
        if (TreeStatus.Instance != null)
        {
            AudioManager.Instance.PlaySfx3D("hit_wood", TreeStatus.Instance.transform.position);
        }
    }

    public void UseItemAnimation(Transform itemSlotTransform, ItemClass currentItemClass, Texture itemTexture, Action onComplete)
    {
        if (!isAI && photonView.IsMine && firstPersonTweenAnimator != null)
        {
            firstPersonTweenAnimator.PlayUseItemAnimation(itemSlotTransform, currentItemClass, itemTexture, onComplete);
        }
        else
        {
            // AI이거나 타인의 화면이면 즉시 콜백을 처리하여 동기화
            onComplete?.Invoke();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (enableIK && leftHandTarget != null && leftHandWeight > 0f)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);

            Quaternion targetRotation = leftHandTarget.rotation * Quaternion.Euler(leftHandRotOffset);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targetRotation);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
        }
    }
}
