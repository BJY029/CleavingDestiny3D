using System.Linq;

//이벤트 발생기
//이벤트 발생시, 현재 활성 상태 이상들을 priority 순으로 실행하여 DamagePacket 등을 수정한다.
public class GameEventBus
{
	private readonly StatusSystem _statusSystem;

	public GameEventBus(StatusSystem statusSystem)
	{
		_statusSystem = statusSystem;
	}

	//이벤트 발행
	public void publish(GameEvent e, EffectContext ctx, SimGameState state = null)
	{
		//priority 순으로 상태 이상 정렬
		foreach (var st in _statusSystem.ALL.OrderBy(s => s.spec.priority))
		{
			//해당 이벤트 발생 시점이 이벤트의 발생 시점에 반응하는지 체크
			if ((st.spec.triggers & e.type) == 0) continue;

			//자신의 스코프에 해당되는 것만 샐힝
			if (st.spec.triggerScope == TriggerScope.OwnerOnly && st.ownerActorNum != e.actorNum) continue;

			//상태 이상 동작 실행
			StatusBehaviours.Execute(st, e, ctx, state);
		}
	}
}
