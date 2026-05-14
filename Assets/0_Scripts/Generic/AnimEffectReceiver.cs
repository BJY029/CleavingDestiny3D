using UnityEngine;
using UnityEngine.Events;

public class AnimEffectReceiver : MonoBehaviour
{
    public UnityEvent onAnimEffect; // 애니메이션 이벤트 수신 시 실행할 유니티 이벤트

    public void ReceiveAnimEffect()
    {
        onAnimEffect?.Invoke(); // 이벤트가 할당되어 있으면 실행
    }
}
