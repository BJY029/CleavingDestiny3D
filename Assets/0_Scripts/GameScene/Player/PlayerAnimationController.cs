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
    [SerializeField] Animator firstPersonAnimator; // 1인칭 도끼 애니메이터
    [SerializeField] private int hitAnimCount = 4; // hit 모션 개수
    [SerializeField] private bool avoidRepeat = true; // 동일한 모션 연속 재생 방지

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

        firstPersonLayer = LayerMask.NameToLayer("FirstPersonItem"); // "FirstPerson" 레이어 인덱스 저장
        axeOriginalLayer = axeTransform.gameObject.layer; // 도끼의 원래 레이어 저장
    }

    // 이동 상태 업데이트 (컨트롤러에서 매 프레임 호출)
    public void UpdateMoveVisuals(float moveX, float moveZ, float deltaTime)
    {
        if (animator == null) return;

        animator.SetFloat(HashSpeedX, moveX, 0.1f, deltaTime);
        animator.SetFloat(HashSpeedZ, moveZ, 0.1f, deltaTime);

        if (!isAI && photonView.IsMine)
        {
            firstPersonAnimator.SetBool(HashIsRun, moveZ > 0.7f);
        }
    }

    // 타격 애니메이션 실행
    public void PlayHitAnimation()
    {
        // 도끼 휘두르기
        if (!isAI && photonView.IsMine)
        {
            axeTransform.gameObject.layer = firstPersonLayer; // 도끼를 1인칭 레이어로 변경하여 항상 보이도록 설정
            firstPersonAnimator.SetTrigger(HashHit);

            Tween.Delay(2f).OnComplete(this, (ctrl) =>
            {
                ctrl.axeTransform.gameObject.layer = ctrl.axeOriginalLayer; // 도끼 레이어를 기본으로 되돌림
            });
        }

        // 전체(타인 화면 포함): 애니메이터 동기화
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
        float intensity = 2f; // 발광 강도
        Color glowColor = Color.white * intensity;

        itemMeshRenderer.GetPropertyBlock(itemMpb);

        if (itemTexture != null)
        {
            // 아이템이 있을 때 텍스쳐 적용 + 색상을 "하얀색(불투명)"으로 변경
            itemMpb.SetTexture("_BaseMap", itemTexture);
            itemMpb.SetColor("_BaseColor", Color.white);
        }
        // 최종 적용
        itemMeshRenderer.SetPropertyBlock(itemMpb);

        // 이전 연출로 인해 0이 된 스케일을 원래 크기(1)로 초기화
        itemTransform.localScale = Vector3.one;
        itemTransform.gameObject.SetActive(true);

        // 아이템 위치 세팅
        itemTransform.SetPositionAndRotation(itemSlotTransform.position, itemSlotTransform.rotation);

        Vector3 axeOriginalPos = axeTransform.localPosition;
        // 애니메이션 시퀀스
        Sequence seq = Sequence.Create()
            // 1. 화면 가운데로 날아가며 회전하기
            .Group(Tween.LocalPosition(itemTransform, itemUsePoint.position, 0.5f, Ease.InOutQuad))
            .Group(Tween.LocalRotation(itemTransform, itemUsePoint.rotation, 0.5f, Ease.InOutQuad))

            // 도끼는 화면 아래로 스르륵 내려놓기
            .Group(Tween.LocalPosition(axeTransform, axeOriginalPos + new Vector3(0, -0.5f, 0), 0.5f, Ease.InOutQuad))

            // 2. 크기 강조 (1.5배로 커지며 등장)
            .Chain(Tween.Scale(itemTransform, Vector3.one * 1.5f, 0.5f, Ease.OutBack))

            // 3. 다시 크기 감소하며 사라짐 (기가 모이듯 쪼그라듦)
            .Chain(Tween.Scale(itemTransform, Vector3.zero, 0.5f, Ease.InBack))

            // 4. 모든 애니메이션이 끝난(크기가 0이 된) 순간 실행
            .OnComplete(() =>
            {
                // 아이템이 완전히 사라지는 타이밍에 파티클 생성
                VFXManager.Instance.PlayItemExplosion(itemTransform.position, currentItemClass);

                // TODO: 사운드 재생

                // 아이템 비활성화
                itemTransform.gameObject.SetActive(false);

                // 도끼 원래 위치로 복귀
                Tween.LocalPosition(axeTransform, axeOriginalPos, 0.3f, Ease.OutQuad);

                onComplete?.Invoke();
            });
    }
}
