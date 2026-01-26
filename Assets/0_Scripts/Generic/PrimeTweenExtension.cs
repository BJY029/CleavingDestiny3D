
using PrimeTween;
using Unity.Cinemachine;

public static class PrimeTweenExtension
{
    // 만들고 안써서 일단 유지
    public static Tween TweenOrthoSize(this CinemachineCamera vcam, float endValue, float duration, Ease ease = Ease.Default)
    {
        // Tween.Custom(시작값, 끝값, 시간, 갱신콜백)
        return Tween.Custom(
            vcam.Lens.OrthographicSize,
            endValue,
            duration,
            val => vcam.Lens.OrthographicSize = val,
            ease
        );
    }
}