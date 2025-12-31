using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations;

public interface IAnimNotify
{
    void OnAnimStateExit(int stateKey);
}
public class NotifyOnAnimExit : StateMachineBehaviour
{
	[Tooltip("어떤 애니메이션의 종료인지 구분하기 위한 키(1 = HitEnd, ...)")]
	public int stateKey = 1;

    private IAnimNotify cached;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		cached ??= animator.GetComponent<IAnimNotify>();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		var pv = animator.GetComponent<PhotonView>();
		if (pv == null) return;
		if (!pv.IsMine) return;
		cached?.OnAnimStateExit(stateKey);
	}
}
