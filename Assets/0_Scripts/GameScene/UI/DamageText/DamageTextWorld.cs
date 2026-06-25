using TMPro;
using UnityEngine;
using PrimeTween;

public class DamageTextWorld : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float moveUpDistance = 0.8f;
    [SerializeField] private float duration = 0.65f;

    [SerializeField] private Vector3 targetScale = Vector3.one * 0.002f;

    private Camera targetCamera;

    public void Initialize(Camera camera, int damage)
    {
        targetCamera = camera;
        Play(damage);
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        transform.LookAt(
            transform.position + targetCamera.transform.rotation * Vector3.forward,
            targetCamera.transform.rotation * Vector3.up
        );
    }

    private void Play(int damage)
    {
        damageText.text = damage.ToString();
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.zero;

        Vector3 endPos = transform.position + Vector3.up * moveUpDistance;

        Sequence.Create()
            .Group(Tween.Scale(transform, targetScale, 0.15f, Ease.OutBack))
            .Group(Tween.Position(transform, endPos, duration, Ease.OutCubic))
            .Insert(0.15f, Tween.Custom(
                1f,
                0f,
                duration - 0.15f,
                value => canvasGroup.alpha = value
            ))
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }
}
