using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator; // 플레이어 애니메이터
    [SerializeField] private int hitAnimCount = 4; // hit 모션 개수
    [SerializeField] private bool avoidRepeat = true; // 동일한 모션 연속 재생 방지
    //[SerializeField] private bool forceRestartEvenIfHitting = false; // 타격 중복 입력시 강제 재시작

    // 이전 모션 중복 재생 방지용 변수
    private int lastIndex = -1;

    // 애니메이션 파라미터 해시화
    private static readonly int HashHit = Animator.StringToHash("Hit");
    private static readonly int HashHitIndex = Animator.StringToHash("HitIndex");

    private string GetHitStateName(int idx) => $"Hit{idx}";

    // Hit 애니메이션 재생 함수
    // 전달된 index 값이 있는 경우, 해당 인덱스에 해당하는 애니메이션 재생 (다른 플레이어와 동기화용)
    // 아니면 랜덤으로 선택해서 애니메이션 재생 (자기 자신 재생용)
    public int PlayHit(int index = -1)
    {
        // 예외 처리
        if (hitAnimCount <= 0) return -999;

        int idx;

        // 전달된 index 값이 없을 경우 (랜덤 재생)
        if (index == -1)
        {
            // 랜덤으로 선택해서 재생
            idx = Random.Range(0, hitAnimCount);

            // 이전 모션과 동일한 애니메이션이 선택되지 않도록 처리
            if (avoidRepeat)
            {
                while (idx == lastIndex)
                {
                    idx = Random.Range(0, hitAnimCount);
                }
            }
        }
        // 전달된 index 값이 있을 경우
        else idx = index;


        lastIndex = idx;

        // 애니메이션 재생
        animator.SetInteger(HashHitIndex, idx);
        animator.ResetTrigger(HashHit);
        animator.SetTrigger(HashHit);

        return idx;
    }
}
