using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System;

public class FadeCanvas : MonoBehaviour
{
    public Image fadeImage;

    public async Awaitable FadeIn(float duration, Action onComplete = null)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
        await Tween.Alpha(fadeImage, 1f, duration);
        onComplete?.Invoke();
    }

    public async Awaitable FadeOut(float duration, Action onComplete = null)
    {
        await Tween.Alpha(fadeImage, 0f, duration);
        fadeImage.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
