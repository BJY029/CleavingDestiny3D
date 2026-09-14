using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI 버튼 및 인터랙션 요소에 마우스 호버(확대/축소) 피드백과 클릭/호버 사운드를 제공하는 공통 컴포넌트입니다.
/// </summary>
[DisallowMultipleComponent]
public class UIButtonFeedback : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    [Header("Target")]
    [Tooltip("스케일 변화를 적용할 Transform입니다. 비워두면 현재 오브젝트의 Transform을 사용합니다.")]
    [SerializeField] private Transform targetTransform;

    [Header("Scale Feedback")]
    [SerializeField] private bool useScaleEffect = true;
    [Tooltip("마우스 호버 시 배율")]
    [SerializeField] private float hoverScale = 1.06f;
    [Tooltip("마우스 클릭(누르고 있을 때) 배율")]
    [SerializeField] private float pressedScale = 0.96f;
    [Tooltip("애니메이션 시간 (초)")]
    [SerializeField] private float duration = 0.12f;
    [SerializeField] private Ease hoverEase = Ease.OutQuad;
    [SerializeField] private Ease exitEase = Ease.OutQuad;

    [Header("Sound Feedback")]
    [SerializeField] private bool useClickSound = true;
    [SerializeField] private string clickSound = "ui_button";

    [SerializeField] private bool useHoverSound = false;
    [SerializeField] private string hoverSound = "UI_Hover";
    [SerializeField] private float hoverSoundCooldown = 0.05f;

    private Selectable _selectable;
    private Vector3 _originalScale = Vector3.one;
    private Tween _scaleTween;
    private bool _isPointerInside;
    private bool _isPointerDown;
    private float _lastHoverSoundTime = -999f;

    private void Awake()
    {
        if (targetTransform == null)
        {
            targetTransform = transform;
        }

        _originalScale = targetTransform.localScale;
        _selectable = GetComponent<Selectable>();
    }

    private void OnEnable()
    {
        // 팝업 재오픈 시 스케일이 커진 상태로 굳는 문제 방지
        _isPointerInside = false;
        _isPointerDown = false;
        ResetScaleImmediate();
    }

    private void OnDisable()
    {
        _scaleTween.Stop();
        ResetScaleImmediate();
        _isPointerInside = false;
        _isPointerDown = false;
    }

    private bool IsInteractable()
    {
        return _selectable == null || _selectable.interactable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable()) return;

        _isPointerInside = true;

        if (useHoverSound && !string.IsNullOrEmpty(hoverSound))
        {
            if (Time.unscaledTime - _lastHoverSoundTime >= hoverSoundCooldown)
            {
                _lastHoverSoundTime = Time.unscaledTime;
                AudioManager.Instance?.PlaySfx2D(hoverSound);
            }
        }

        if (useScaleEffect && !_isPointerDown)
        {
            AnimateScale(_originalScale * hoverScale, hoverEase);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerInside = false;
        _isPointerDown = false;

        if (useScaleEffect)
        {
            AnimateScale(_originalScale, exitEase);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;

        _isPointerDown = true;

        if (useScaleEffect)
        {
            AnimateScale(_originalScale * pressedScale, Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;

        if (useScaleEffect)
        {
            Vector3 target = _isPointerInside ? (_originalScale * hoverScale) : _originalScale;
            AnimateScale(target, Ease.OutQuad);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable()) return;

        if (useClickSound && !string.IsNullOrEmpty(clickSound))
        {
            AudioManager.Instance?.PlaySfx2D(clickSound);
        }
    }

    private void AnimateScale(Vector3 endScale, Ease ease)
    {
        if (targetTransform == null) return;

        _scaleTween.Stop();
        _scaleTween = Tween.Scale(targetTransform, endScale, duration, ease, useUnscaledTime: true);
    }

    private void ResetScaleImmediate()
    {
        if (targetTransform != null)
        {
            targetTransform.localScale = _originalScale;
        }
    }
}
