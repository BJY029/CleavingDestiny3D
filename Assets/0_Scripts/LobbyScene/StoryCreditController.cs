using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Potan.CoreUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class StoryCreditController : MonoBehaviour
{
    public static StoryCreditController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (!hasStoryPlayed)
            PlayStoryAsync(destroyCancellationToken).Forget();
    }

    [Header("UI")]
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private TMP_Text storyText;

    [Header("Scroll")]
    [SerializeField, Min(1f)] private float scrollSpeed = 80f;
    [SerializeField, Min(0f)] private float padding = 50f;

    [Header("Skip")]
#if ENABLE_INPUT_SYSTEM
    [SerializeField] private Key skipKey = Key.Space;
#else
    [SerializeField] private KeyCode skipKey = KeyCode.Space;
#endif

    [Header("Fade Out")]
    [SerializeField] private CanvasGroup storyCanvasGroup;
    [SerializeField, Min(0f)] private float fadeOutDuration = 1f;

    public event Action onFinished;
    public bool hasStoryPlayed = false;

    private void Start()
    {
        if (!GameManager.Instance.hasStoryPlayed)
            AudioManager.Instance.PlayBgm("Story_BGM");
    }

    private async UniTask PlayStoryAsync(CancellationToken cancellationToken)
    {
        if (GameManager.Instance != null && GameManager.Instance.hasStoryPlayed)
        {
            Debug.LogError("story ended", this);
            return;
        }

        if (storyPanel == null || viewport == null || storyText == null)
        {
            Debug.LogError("Connect Story UIs", this);
            return;
        }

        storyPanel.SetActive(true);

        storyCanvasGroup.alpha = 1f;
        storyCanvasGroup.blocksRaycasts = true;

        // 초기 위치에 있는 텍스트가 먼저 보이지 않도록 숨김.
        storyText.enabled = false;

        // UI 활성화 및 다른 Start 초기화 이후에 크기 계산.
        await UniTask.NextFrame(cancellationToken: cancellationToken);

        Canvas.ForceUpdateCanvases();

        RectTransform textRect = storyText.rectTransform;

        // StoryText는 반드시 Viewport의 바로 아래 자식이어야 함.
        if (textRect.parent != viewport)
        {
            Debug.LogError("StoryText를 Viewport의 바로 아래 자식으로 배치해 주세요.", this);
            return;
        }

        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 1f);

        // 텍스트 박스 상단부터 글자가 배치되도록 설정.
        storyText.alignment = TextAlignmentOptions.Top;
        storyText.overflowMode = TextOverflowModes.Overflow;

        float textHeight = storyText.GetPreferredValues(storyText.text, textRect.rect.width, Mathf.Infinity).y;
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);

        // 중앙 앵커를 기준으로 화면 아래에서 시작해 위로 완전히 빠져나감.
        float startY = -viewport.rect.height * 0.5f - padding;

        textRect.anchoredPosition = new Vector2(0f, startY);

        // 시작 위치 적용 후 표시.
        storyText.enabled = true;
        storyText.ForceMeshUpdate();

        while (true)
        {
            await UniTask.NextFrame(cancellationToken: cancellationToken);

            if (!storyPanel.activeInHierarchy)
                return;

            if (IsSkipPressed())
                break;

            // 런타임에 적용된 현재 텍스트 기준으로 높이 갱신.
            textHeight = storyText.GetPreferredValues(storyText.text, textRect.rect.width, Mathf.Infinity).y;
            textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);

            // 텍스트 상단 위치에서 전체 높이를 빼면 텍스트 하단 위치.
            float bottomY = textRect.anchoredPosition.y - textHeight;
            float viewportTopY = viewport.rect.height * 0.5f;

            // 마지막 줄까지 화면 위로 완전히 빠져나갔을 때 종료.
            if (bottomY >= viewportTopY + padding)
                break;

            float nextY = textRect.anchoredPosition.y + scrollSpeed * Time.unscaledDeltaTime;
            textRect.anchoredPosition = new Vector2(0f, nextY);
        }
        hasStoryPlayed = true;
        GameManager.Instance.hasStoryPlayed = hasStoryPlayed;

        await FadeOutAsync(cancellationToken);

        storyCanvasGroup.blocksRaycasts = false;

        storyPanel.SetActive(false);
        onFinished?.Invoke();
    }

    private bool IsSkipPressed()
    {
        hasStoryPlayed = true;
        GameManager.Instance.hasStoryPlayed = hasStoryPlayed;
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current[skipKey].wasPressedThisFrame;
#else
        return Input.GetKeyDown(skipKey);
#endif
    }

    private async UniTask FadeOutAsync(CancellationToken cancellationToken)
    {
        AudioManager.Instance.StopBgmFadeOut(fadeOutDuration);
        float startAlpha = storyCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            await UniTask.NextFrame(cancellationToken: cancellationToken);

            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeOutDuration);

            storyCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
        }

        storyCanvasGroup.alpha = 0f;
    }
}
