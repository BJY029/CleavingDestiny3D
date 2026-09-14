using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Village.Building;

public class TopbarBuilding : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image highlightImage;
    [Tooltip("호버 시 밝기를 조절할 이미지 (비워두면 현재 오브젝트의 Image 사용)")]
    [SerializeField] private Image hoverImage;
    [SerializeField] private VillageType building;
    public VillageType BuildingType => building;

    [Header("Hover Brightness Settings")]
    [SerializeField] private float normalAlpha = 0f;
    [SerializeField] private float hoverAlpha = 0.2f;
    [SerializeField] private float fadeDuration = 0.12f;

    [Header("Sound Settings")]
    [SerializeField] private string hoverSound = "UI_Hover";
    [SerializeField] private string clickSound = "";
    [SerializeField] private float hoverCooldown = 0.05f;

    private Action<VillageType> onClicked;
    private Tween _fadeTween;
    private float _lastHoverTime = -999f;

    private void Awake()
    {
        if (hoverImage == null)
        {
            hoverImage = GetComponent<Image>();
        }

        ResetAlphaImmediate();
    }

    private void OnDisable()
    {
        _fadeTween.Stop();
        ResetAlphaImmediate();
    }
    
    public void Init(Action<VillageType> newClicked)
    {
        SetHighlight(false);
        onClicked = newClicked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(hoverSound) && Time.unscaledTime - _lastHoverTime >= hoverCooldown)
        {
            _lastHoverTime = Time.unscaledTime;
            AudioManager.Instance?.PlaySfx2D(hoverSound);
        }

        AnimateAlpha(hoverAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateAlpha(normalAlpha);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!string.IsNullOrEmpty(clickSound))
            {
                AudioManager.Instance?.PlaySfx2D(clickSound);
            }

            onClicked?.Invoke(building);
        }
    }
    
    public void SetHighlight(bool highlight)
    {
        if (highlightImage != null)
        {
            highlightImage.enabled = highlight;
        }
    }

    private void AnimateAlpha(float targetAlpha)
    {
        if (hoverImage == null) return;

        _fadeTween.Stop();
        _fadeTween = Tween.Alpha(hoverImage, targetAlpha, fadeDuration, useUnscaledTime: true);
    }

    private void ResetAlphaImmediate()
    {
        if (hoverImage != null)
        {
            Color c = hoverImage.color;
            c.a = normalAlpha;
            hoverImage.color = c;
        }
    }
}
