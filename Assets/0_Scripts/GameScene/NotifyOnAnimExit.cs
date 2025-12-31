using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations;

//콜백 함수 역할을 할 인터페이스
public interface IAnimNotify
{
    void OnAnimStateExit(int stateKey);
}
public class NotifyOnAnimExit : StateMachineBehaviour
{
	[Tooltip("어떤 애니메이션의 종료인지 구분하기 위한 키(1 = HitEnd, ...)")]
	public int stateKey = 1;

	//특정 플레이어의 IAnimNotify 객체
    private IAnimNotify cached;

	//모션 state에 enter 되면 호출될 함수
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//해당 애니메이터를 보유한 플레이어로부터 IAnimNotify 객체 가져오기(PlayerController)
		cached ??= animator.GetComponent<IAnimNotify>();
	}

	//모션이 종료되면 호출될 함수
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//해당 애니메이터를 보유한 플레이어의 PhotonView 얻어오기
		var pv = animator.GetComponent<PhotonView>();
		//만약 PhotonView가 없거나(에러) 내 PhotonView가 아니면 콜백 함수 실행 안함
		if (pv == null) return;
		if (!pv.IsMine) return;
		//IAnimNotify 객체의 OnAnimStateExit 함수 실행(콜백 함수)
		cached?.OnAnimStateExit(stateKey);
	}
}
