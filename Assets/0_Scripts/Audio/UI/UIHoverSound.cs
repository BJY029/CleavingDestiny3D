using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private string audioId = "UI_Hover";
    [SerializeField] private float cooldown = 0.1f;

    private float _lastPlayTime = -999f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Time.unscaledTime - _lastPlayTime < cooldown) return;

        _lastPlayTime = Time.unscaledTime;
        AudioManager.Instance.PlaySfx2D(audioId);
    }
}
