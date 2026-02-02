using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class FadeCanvas : MonoBehaviour
{
    private static FadeCanvas _instance;
    public static FadeCanvas Instance => _instance;

    [SerializeField] Image fadeImage;
    private Tween _currentFadeTween; // 현재 진행 중인 트윈

    /// <summary>
    /// Returns true if the canvas is fully faded in.
    /// </summary>
    public bool IsFaded => fadeImage.color.a >= 1f;
    /// <summary>
    /// Returns true if the canvas is fully clear.
    /// </summary>
    public bool IsClear => fadeImage.color.a <= 0f;

    /// <summary>
    /// Returns true if a fade operation is currently in progress.
    /// </summary>
    public bool IsFading { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
            SetFade(false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the fade state instantly.
    /// </summary>
    public void SetFade(bool isFaded)
    {
        if (_currentFadeTween.isAlive) _currentFadeTween.Stop();
        IsFading = false;

        float alpha = isFaded ? 1f : 0f;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, alpha);
        fadeImage.gameObject.SetActive(isFaded);
    }

    /// <summary>
    /// Fade in (Fire-and-forget).
    /// </summary>
    public void FadeIn(float duration, Action onComplete = null)
    {
        FadeInAsync(duration, onComplete).Forget();
    }

    /// <summary>
    /// Fade out (Fire-and-forget).
    /// </summary>
    public void FadeOut(float duration, Action onComplete = null)
    {
        FadeOutAsync(duration, onComplete).Forget();
    }

    /// <summary>
    /// 페이드 인 (화면 어두워짐). UniTask를 반환합니다.
    /// 취소 토큰(CancellationToken)을 지원합니다.
    /// </summary>
    public async UniTask FadeInAsync(float duration, Action onComplete = null, float delay = 0f, float endDelay = 0f, CancellationToken ct = default)
    {
        if (delay > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);
        }

        if (_currentFadeTween.isAlive) _currentFadeTween.Stop();

        IsFading = true;
        fadeImage.gameObject.SetActive(true);

        _currentFadeTween = Tween.Alpha(fadeImage, 1f, duration);
        await _currentFadeTween;

        if (endDelay > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(endDelay), cancellationToken: ct);
        }

        onComplete?.Invoke();
        IsFading = false;
    }

    /// <summary>
    /// 페이드 아웃 (화면 밝아짐). UniTask를 반환합니다.
    /// </summary>
    public async UniTask FadeOutAsync(float duration, Action onComplete = null, float delay = 0f, float endDelay = 0f, CancellationToken ct = default)
    {
        if (delay > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);
        }

        if (_currentFadeTween.isAlive) _currentFadeTween.Stop();

        IsFading = true;

        _currentFadeTween = Tween.Alpha(fadeImage, 0f, duration);
        await _currentFadeTween;

        if (endDelay > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(endDelay), cancellationToken: ct);
        }

        fadeImage.gameObject.SetActive(false);
        onComplete?.Invoke();
        IsFading = false;
    }
}
