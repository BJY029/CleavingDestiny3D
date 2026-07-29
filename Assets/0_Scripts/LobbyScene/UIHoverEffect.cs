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
    [SerializeField, Range(0f, 1f)]
    private float hoverGlowAlpha = 0.9f;

    [Header("Animation")]
    [SerializeField] private float duration = 0.12f;

    private Coroutine animationCoroutine;
    private bool isPointerInside;

    private void Awake()
    {
        if (target == null)
            target = transform as RectTransform;

        SetEffectAlpha(0f);
        target.localScale = Vector3.one * normalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
        PlayAnimation(hoverScale, hoverGlowAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        PlayAnimation(normalScale, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayAnimation(pressedScale, hoverGlowAlpha);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float nextScale = isPointerInside ? hoverScale : normalScale;
        float nextGlow = isPointerInside ? hoverGlowAlpha : 0f;

        PlayAnimation(nextScale, nextGlow);
    }

    private void PlayAnimation(float targetScale, float targetAlpha)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(
            Animate(targetScale, targetAlpha)
        );
    }


    private IEnumerator Animate(float targetScale, float targetAlpha)
    {
        Vector3 startScale = target.localScale;
        float startAlpha = glowImage != null ? glowImage.color.a : 0f;
        Vector2 position = shineImage.rectTransform.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - Mathf.Pow(1f - t, 3f);

            target.localScale = Vector3.Lerp(startScale, Vector3.one * targetScale, t);

            SetEffectAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));

            yield return null;
        }

        target.localScale = Vector3.one * targetScale;
        SetEffectAlpha(targetAlpha);

        animationCoroutine = null;
    }

    private void SetEffectAlpha(float alpha)
    {
        SetImageAlpha(glowImage, alpha);
        SetImageAlpha(shineImage, alpha);
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
