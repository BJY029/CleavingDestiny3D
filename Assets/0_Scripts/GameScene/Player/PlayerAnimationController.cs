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
    [SerializeField] private int hitAnimCount = 4; // hit 모션 개수
    [SerializeField] private bool avoidRepeat = true; // 동일한 모션 연속 재생 방지

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

    // 이전 모션 중복 재생 방지용 변수
    private int lastIndex = -1;

    // 파라미터 해시화
    public static readonly int HashHit = Animator.StringToHash("Hit");
    public static readonly int HashHitIndex = Animator.StringToHash("HitIndex");
    public static readonly int HashSpeedX = Animator.StringToHash("Speed_X");
    public static readonly int HashSpeedZ = Animator.StringToHash("Speed_Z");
    public static readonly int HashIsRun = Animator.StringToHash("IsRun");
    int firstPersonLayer;

    private IAnimNotify animNotify;
    [SerializeField] bool isAI = false;

    MaterialPropertyBlock itemMpb;

    // [Header("Effect")]
    // [SerializeField] ParticleSystem hitEffectObject;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();

        if (TryGetComponent(out PlayerController playerController))
        {
            animNotify = playerController;
        }
        else if (TryGetComponent(out AIController aiController))
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
        if (animator == null) return;

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

    // Hit 애니메이션 실행
    public void PlayHitAnimation()
    {
        // 도끼 휘두르기
        if (!isAI && photonView.IsMine)
        {
            axeTransform.gameObject.layer = firstPersonLayer; // 도끼를 1인칭 레이어로 변경하여 잘 보이도록 설정

            if (firstPersonTweenAnimator != null)
            {
                firstPersonTweenAnimator.PlayHitAnimation(
                    onImpactEvent: FirstPersonAxeHit,
                    onCompleteCallback: () =>
                    {
                        axeTransform.gameObject.layer = axeOriginalLayer; // 도끼 레이어 기본으로 되돌림
                    }
                );
            }
        }

        // 전체(타인 화면 포함): 애니메이션 트리거
        int idx = GetRandomIndex();
        photonView.RPC(nameof(RPC_PlayHit), RpcTarget.All, idx);
    }

    // 1인칭 준비 자세 애니메이션 실행 (F Key Down 시점에 호출)
    public void PlayReadyAnimation()
    {
        if (!isAI && photonView.IsMine)
        {
            axeTransform.gameObject.layer = firstPersonLayer;

            if (firstPersonTweenAnimator != null)
            {
                firstPersonTweenAnimator.PlayReadyAnimation();
            }
        }
    }

    // 1인칭 준비 자세 애니메이션 취소 (취소 키 입력 시 호출)
    public void CancelReadyAnimation()
    {
        if (!isAI && photonView.IsMine)
        {
            if (firstPersonTweenAnimator != null)
            {
                firstPersonTweenAnimator.CancelReadyAnimation();
            }
            axeTransform.gameObject.layer = axeOriginalLayer; // 도끼 레이어 복원
        }
    }

    // 1인칭 타격 애니메이션 및 3인칭 동기화 RPC 작동 (F Key Up 시점에 호출)
    public void PlayStrikeAnimation()
    {
        if (!isAI && photonView.IsMine)
        {
            if (firstPersonTweenAnimator != null)
            {
                firstPersonTweenAnimator.PlayStrikeAnimation(
                    onImpactEvent: FirstPersonAxeHit,
                    onCompleteCallback: () =>
                    {
                        axeTransform.gameObject.layer = axeOriginalLayer; // 도끼 레이어 복원
                    }
                );
            }
        }

        // 전체(타인 화면 포함): 애니메이션 트리거
        int idx = GetRandomIndex();
        photonView.RPC(nameof(RPC_PlayHit), RpcTarget.All, idx);
    }

    [PunRPC]
    private void RPC_PlayHit(int idx)
    {
        if (animator == null) return;

        lastIndex = idx;
        animator.SetInteger(HashHitIndex, idx);
        animator.ResetTrigger(HashHit);
        animator.SetTrigger(HashHit);

        // AI인 경우, FBX 애니메이션 이벤트를 쓸 수 없으므로 지연 후 소리 재생
        if (isAI)
        {
            PlayAIHitSoundAsync().Forget();
        }
    }

    private async UniTaskVoid PlayAIHitSoundAsync()
    {
        // 지정된 시간만큼 대기 (애니메이션 휘두르는 시간에 맞춤)
        await UniTask.Delay((int)(aiHitSoundDelay * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());

        PlayLocalHitSound();
    }

    private int GetRandomIndex()
    {
        int idx = Random.Range(0, hitAnimCount);
        if (avoidRepeat && hitAnimCount > 1)
        {
            while (idx == lastIndex)
            {
                idx = Random.Range(0, hitAnimCount);
            }
        }
        return idx;
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
        if (TreeStatus.Instance != null && TreeStatus.Instance.treeAudioSource != null)
        {
            TreeStatus.Instance.treeAudioSource.Play();
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
}
