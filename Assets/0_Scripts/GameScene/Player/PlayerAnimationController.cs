using UnityEngine;
using Photon.Pun;
using PrimeTween;

public class PlayerAnimationController : MonoBehaviourPun, IAnimNotify
{
    [SerializeField] private Animator animator; // 플레이어 애니메이터
    [SerializeField] Animator firstPersonAnimator; // 1인칭 도끼 애니메이터
    [SerializeField] private int hitAnimCount = 4; // hit 모션 개수
    [SerializeField] private bool avoidRepeat = true; // 동일한 모션 연속 재생 방지

    private bool isSwinging = false;

    // 이전 모션 중복 재생 방지용 변수
    private int lastIndex = -1;

    // 애니메이션 파라미터 해시화
    private static readonly int HashHit = Animator.StringToHash("Hit");
    private static readonly int HashHitIndex = Animator.StringToHash("HitIndex");
    private static readonly int HashSpeedX = Animator.StringToHash("Speed_X");
    private static readonly int HashSpeedZ = Animator.StringToHash("Speed_Z");
    private static readonly int HashIsRun = Animator.StringToHash("IsRun");

    private IAnimNotify animNotify;
    bool isAI = false;

    Sequence currentSequence; // 현재 진행 중인 도끼 휘두르기 시퀀스

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
        if (isSwinging) return;

        // 도끼 휘두르기
        if (!isAI && photonView.IsMine)
        {
            firstPersonAnimator.SetTrigger(HashHit);
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
}
