using UnityEngine;
using Photon.Pun;
using PrimeTween;

public class PlayerAnimationController : MonoBehaviourPun, IAnimNotify
{
    [SerializeField] private Animator animator; // 플레이어 애니메이터
    [SerializeField] private int hitAnimCount = 4; // hit 모션 개수
    [SerializeField] private bool avoidRepeat = true; // 동일한 모션 연속 재생 방지

    [Header("First Person Axe (Procedural)")]
    [SerializeField] private Transform axeTransform;
    private Vector3 axeOriginalPos;
    private Quaternion axeOriginalRot;
    private bool isSwinging = false;

    // 이전 모션 중복 재생 방지용 변수
    private int lastIndex = -1;

    // 애니메이션 파라미터 해시화
    private static readonly int HashHit = Animator.StringToHash("Hit");
    private static readonly int HashHitIndex = Animator.StringToHash("HitIndex");
    private static readonly int HashSpeedX = Animator.StringToHash("Speed_X");
    private static readonly int HashSpeedZ = Animator.StringToHash("Speed_Z");

    private PlayerController playerController;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (TryGetComponent(out PlayerController pc))
        {
            playerController = pc;
        }

        if (axeTransform != null)
        {
            axeOriginalPos = axeTransform.localPosition;
            axeOriginalRot = axeTransform.localRotation;
        }
    }

    // 이동 상태 업데이트 (컨트롤러에서 매 프레임 호출)
    public void UpdateMoveVisuals(float moveX, float moveZ, float deltaTime)
    {
        if (animator == null) return;

        animator.SetFloat(HashSpeedX, moveX, 0.1f, deltaTime);
        animator.SetFloat(HashSpeedZ, moveZ, 0.1f, deltaTime);
    }

    // 타격 애니메이션 실행
    public void PlayHitAnimation()
    {
        if (isSwinging) return;

        // 로컬(내 화면): 도끼 휘두르기 (코드로 직접 제어)
        // AI는 playerController가 없으므로 null 체크 (AI 1인칭이 없으므로)
        if (playerController != null && photonView.IsMine)
        {
            PlayProceduralAxeSwing();
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
    }

    // 임시 도끼 휘드르기 애니메이션
    private void PlayProceduralAxeSwing()
    {
        if (axeTransform == null) return;
        isSwinging = true;

        Sequence.Create()
            // 준비 동작
            .Chain(Tween.LocalPosition(axeTransform, axeOriginalPos + new Vector3(0.1f, 0.2f, -0.2f), duration: 0.1f, ease: Ease.OutQuad))
            .Group(Tween.LocalRotation(axeTransform, axeOriginalRot * Quaternion.Euler(-20, 10, 0), duration: 0.1f))
            // 휘두르기
            .Chain(Tween.LocalPosition(axeTransform, axeOriginalPos + new Vector3(0, -0.4f, 0.3f), duration: 0.15f, ease: Ease.InBack))
            .Group(Tween.LocalRotation(axeTransform, axeOriginalRot * Quaternion.Euler(60, 0, 0), duration: 0.15f))
            // 복귀
            .Chain(Tween.LocalPosition(axeTransform, axeOriginalPos, duration: 0.4f, ease: Ease.OutCubic))
            .Group(Tween.LocalRotation(axeTransform, axeOriginalRot, duration: 0.4f))
            .OnComplete(() => isSwinging = false);
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
        // PlayerController에 종료 사실을 알림
        if (playerController != null)
        {
            playerController.OnAnimStateExit(stateKey);
        }
        else
        {
            if (TryGetComponent(out AIController aiController))
            {
                aiController.OnAnimStateExit(stateKey);
            }
        }
    }
}
