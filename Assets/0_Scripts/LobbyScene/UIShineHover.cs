using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIShineHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private RectTransform shineImage;
    [SerializeField] private RectTransform maskRect;

    [Header("Movement")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float startPadding = 100f;
    [SerializeField] private float endPadding = 100f;

    [Header("Settings")]
    [SerializeField] private bool replayWhileHovered = false;
    [SerializeField] private float replayDelay = 0.3f;

    private Coroutine shineCoroutine;
    private bool isHovered;

    private void Awake()
    {
        if (maskRect == null)
            maskRect = transform as RectTransform;

        ResetShinePosition();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;

        if (shineCoroutine != null)
            StopCoroutine(shineCoroutine);

        shineCoroutine = StartCoroutine(PlayShine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        if (shineCoroutine != null)
        {
            StopCoroutine(shineCoroutine);
            shineCoroutine = null;
        }

        ResetShinePosition();
    }

    private IEnumerator PlayShine()
    {
        do
        {
            float startX = GetStartX();
            float endX = GetEndX();

            Vector2 position = shineImage.anchoredPosition;
            position.x = startX;
            shineImage.anchoredPosition = position;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                // 부드럽게 시작하고 부드럽게 끝나는 Ease
                t = Mathf.SmoothStep(0f, 1f, t);

                position.x = Mathf.Lerp(startX, endX, t);
                shineImage.anchoredPosition = position;

                yield return null;
            }

            position.x = endX;
            shineImage.anchoredPosition = position;

            if (!replayWhileHovered || !isHovered)
                break;

            yield return new WaitForSecondsRealtime(replayDelay);

        } while (isHovered);

        shineCoroutine = null;
    }

    private float GetStartX()
    {
        float maskHalfWidth = maskRect.rect.width * 0.5f;
        float shineHalfWidth = shineImage.rect.width * 0.5f;

        return -maskHalfWidth - shineHalfWidth - startPadding;
    }

    private float GetEndX()
    {
        float maskHalfWidth = maskRect.rect.width * 0.5f;
        float shineHalfWidth = shineImage.rect.width * 0.5f;

        return maskHalfWidth + shineHalfWidth + endPadding;
    }

    private void ResetShinePosition()
    {
        if (shineImage == null || maskRect == null)
            return;

        Vector2 position = shineImage.anchoredPosition;
        position.x = GetStartX();
        shineImage.anchoredPosition = position;
    }
}
