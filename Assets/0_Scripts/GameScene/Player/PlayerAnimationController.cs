using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int hitAnimCount = 4;
    [SerializeField] private bool avoidRepeat = true; //기존 피격 동작 반복 재생 여부
    [SerializeField] private bool forceRestartEvenIfHitting = false; //키 중복 입력 받을건지 여부

    private int lastIndex = -1;

    private static readonly int HashHit = Animator.StringToHash("Hit");
    private static readonly int HashHitIndex = Animator.StringToHash("HitIndex");

    private string GetHitStateName(int idx) => $"Hit{idx}";

    public int PlayHit(int index = -1)
    {
        if (hitAnimCount <= 0) return -999;

        int idx;

		if (index != -1)
        {
            idx = Random.Range(0, hitAnimCount);

            if (avoidRepeat)
            {
                while (idx == lastIndex)
                {
                    idx = Random.Range(0, hitAnimCount);
                }
            }
        }
        else idx = index;
		lastIndex = idx;

		animator.SetInteger(HashHitIndex, idx);
        animator.ResetTrigger(HashHit);
        animator.SetTrigger(HashHit);

        return idx;
    }
}
