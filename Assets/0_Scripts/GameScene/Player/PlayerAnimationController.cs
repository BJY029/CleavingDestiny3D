using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator; //플레이어 애니메이터
    [SerializeField] private int hitAnimCount = 4;//hit 모션 개수
    [SerializeField] private bool avoidRepeat = true; //기존 피격 동작 반복 재생 여부
    //[SerializeField] private bool forceRestartEvenIfHitting = false; //키 중복 입력 받을건지 여부

    //기존 피격 중복 재생 방지용
    private int lastIndex = -1;

    //애니메이션 이름 해시화
    private static readonly int HashHit = Animator.StringToHash("Hit");
    private static readonly int HashHitIndex = Animator.StringToHash("HitIndex");

    private string GetHitStateName(int idx) => $"Hit{idx}";

    //Hit 애니메이션 재생 함수
    //별도의 index 값을 받을 경우, 해당 인덱스에 해당하는 애니메이션 재생(상대방 애니메이션 동기화용)
    //아니면 랜덤으로 정해서 애니메이션 재생(자기 자신 재생용)
    public int PlayHit(int index = -1)
    {
        //오류
        if (hitAnimCount <= 0) return -999;

        int idx;

        //임의의 index 값을 받지 않은 경우
        if (index == -1)
        {
            //랜덤으로 정해서 재생
            idx = Random.Range(0, hitAnimCount);

            //만약 기존에 재생한 애니메이션이 재생되길 원치 않으면
            if (avoidRepeat)
            {
                while (idx == lastIndex)
                {
                    idx = Random.Range(0, hitAnimCount);
                }
            }
        }
        //임의의 index 값 받은 경우 
        else idx = index;
        lastIndex = idx;

        //애니메이션 재생
        animator.SetInteger(HashHitIndex, idx);
        animator.ResetTrigger(HashHit);
        animator.SetTrigger(HashHit);

        return idx;
    }
}
