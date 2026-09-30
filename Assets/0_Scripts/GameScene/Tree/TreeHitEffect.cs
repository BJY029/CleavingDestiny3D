using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class TreeHitEffect : MonoBehaviour
{
    public static TreeHitEffect instance;

    [SerializeField] private Transform shakePivot;
    [SerializeField] private ParticleSystem leafParticles;

    [Header("Tree Shake")]
    [SerializeField] private float shakeAngle = 3f;
    [SerializeField] private float shakeDuration = 0.45f;
    [SerializeField] private float shakeCycle = 2f;

    [Header("leafs")]
    [SerializeField] private int leafCount = 12;

    private Quaternion originalRotation;
    private Vector3 localShakeAxis;
    private float elapsed;
    private bool isShaking;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (shakePivot != null)
        {
            originalRotation = shakePivot.localRotation;
        }
    }

    public void PlayHit(Vector3 hitDirection)
    {
        if (leafParticles != null)
            leafParticles.Emit(leafCount);

        if (shakePivot == null) return;

        Transform pivotParent = shakePivot.parent;

        Vector3 worldUp = pivotParent != null ? pivotParent.TransformDirection(Vector3.up) : Vector3.up;

        Vector3 direction = Vector3.ProjectOnPlane(hitDirection, worldUp);

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector3.ProjectOnPlane(transform.forward, worldUp);

        if (direction.sqrMagnitude < 0.0001f) return;

        Vector3 worldAxis = Vector3.Cross(worldUp, direction.normalized);

        localShakeAxis = pivotParent != null
            ? pivotParent.InverseTransformDirection(worldAxis).normalized
            : worldAxis.normalized;

        elapsed = 0f;
        isShaking = true;
    }

    private void Update()
    {
        if (!isShaking || shakePivot == null) return;

        elapsed += Time.deltaTime;

        float progress = Mathf.Clamp01(elapsed / Mathf.Max(shakeDuration, 0.01f));
        float decay = (1f - progress) * (1f - progress);
        float angle = Mathf.Sin(progress * Mathf.PI * 2f * shakeCycle) * shakeAngle * decay;

        shakePivot.localRotation = Quaternion.AngleAxis(angle, localShakeAxis) * originalRotation;

        if (progress >= 1f)
        {
            shakePivot.localRotation = originalRotation;
            isShaking = false;
        }
    }

    private void OnDisable()
    {
        isShaking = false;

        if (shakePivot != null) shakePivot.localRotation = originalRotation;
    }
}
