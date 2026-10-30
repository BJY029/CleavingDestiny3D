using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform target;
    [SerializeField] private Image glowImage;
    [SerializeField] private Image shineImage;

    [Header("Scale")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float hoverScale = 1.03f;
    [SerializeField] private float pressedScale = 0.98f;

    [Header("Glow")]
    [SerializeField, Range(0f, 1f)] private float hoverGlowAlpha = 0.9f;

    [Header("Animation")]
    [SerializeField] private float duration = 0.12f;

    [Header("미확인 상태 강조 - 튜토리얼 버튼에서만 활성화")]
    [SerializeField] private bool highlightUntilViewed = false;
    [SerializeField] private string viewedPrefsKey = "Tutorial_Viewed";

    [Header("반복 강조")]
    [SerializeField, Range(0f, 1f)] private float pulseMinAlpha = 0.15f;
    [SerializeField, Range(0f, 1f)] private float pulseMaxAlpha = 0.85f;
    [SerializeField, Min(0.1f)] private float pulsePeriod = 2f;

    private Coroutine animationCoroutine;
    private bool isPointerInside;
    private bool isPressed;
    private bool hasViewed;

    // 호버 알파와 반복 강조 알파를 분리해서 관리
    private float interactionAlpha;
    private float pulseElapsed;

    private void Awake()
    {
        if (target == null)
            target = transform as RectTransform;

        hasViewed = PlayerPrefs.GetInt(viewedPrefsKey, 0) == 1;
    }

    private void OnEnable()
    {
        isPointerInside = false;
        isPressed = false;
        interactionAlpha = 0f;
        pulseElapsed = 0f;

        if (target != null)
            target.localScale = Vector3.one * normalScale;

        ApplyEffect();
    }

    private void Update()
    {
        pulseElapsed += Time.unscaledDeltaTime;
        ApplyEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
        PlayAnimation(isPressed ? pressedScale : hoverScale, hoverGlowAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        PlayAnimation(normalScale, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isPressed = true;
        PlayAnimation(pressedScale, hoverGlowAlpha);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        isPressed = false;

        float nextScale = isPointerInside ? hoverScale : normalScale;
        float nextAlpha = isPointerInside ? hoverGlowAlpha : 0f;

        PlayAnimation(nextScale, nextAlpha);
    }

    // 튜토리얼을 확인했다고 처리할 때 호출
    public void MarkAsViewed()
    {
        if (!highlightUntilViewed)
            return;

        hasViewed = true;

        PlayerPrefs.SetInt(viewedPrefsKey, 1);
        PlayerPrefs.Save();

        ApplyEffect();
    }

    // 테스트용: 저장된 확인 상태 초기화
    [ContextMenu("미확인 상태로 초기화")]
    public void ResetViewed()
    {
        PlayerPrefs.DeleteKey(viewedPrefsKey);
        PlayerPrefs.Save();

        hasViewed = false;
        pulseElapsed = 0f;

        if (Application.isPlaying)
            ApplyEffect();
    }

    private void PlayAnimation(float targetScale, float targetAlpha)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(Animate(targetScale, targetAlpha));
    }

    private IEnumerator Animate(float targetScale, float targetAlpha)
    {
        Vector3 startScale = target.localScale;
        float startAlpha = interactionAlpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - Mathf.Pow(1f - t, 3f);

            target.localScale = Vector3.Lerp(startScale, Vector3.one * targetScale, t);
            interactionAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        target.localScale = Vector3.one * targetScale;
        interactionAlpha = targetAlpha;

        animationCoroutine = null;
    }

    private void ApplyEffect()
    {
        float pulseAlpha = 0f;

        if (highlightUntilViewed && !hasViewed)
        {
            float phase = pulseElapsed / Mathf.Max(0.1f, pulsePeriod);
            float wave = (1f - Mathf.Cos(phase * Mathf.PI * 2f)) * 0.5f;

            pulseAlpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, wave);
        }

        // 글로우는 반복 강조와 호버 중 더 밝은 값을 사용
        SetImageAlpha(glowImage, Mathf.Max(interactionAlpha, pulseAlpha));

        // Shine은 기존처럼 호버·클릭할 때만 표시
        SetImageAlpha(shineImage, interactionAlpha);
    }

    private void OnDisable()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        isPointerInside = false;
        isPressed = false;
        interactionAlpha = 0f;

        SetImageAlpha(glowImage, 0f);
        SetImageAlpha(shineImage, 0f);

        if (target != null)
            target.localScale = Vector3.one * normalScale;
    }

    private static void SetImageAlpha(Image image, float alpha)
    {
        if (image == null)
            return;

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

}
