using UnityEngine;

public enum EffectLifetimeType
{
    Duration,
    Manual
}

[CreateAssetMenu(fileName = "EffectDataSO", menuName = "Scriptable Objects/EffectDataSO")]
public class EffectDataSO : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private string effectId;

    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Lifetime")]
    [SerializeField] private EffectLifetimeType lifetimeType = EffectLifetimeType.Duration;
    [SerializeField] private float duration = 2f;

    [Header("Transform")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private Vector3 scale = Vector3.one;

    [Header("Follow")]
    [SerializeField] private bool followTarget;

    [Header("Value Color")]
    [SerializeField] private bool useValueColor;
    [SerializeField] private float colorMinValue;
    [SerializeField] private float colorMaxValue = 1f;
    [SerializeField] private Gradient colorGradient;

    public string EffectId => effectId;
    public GameObject Prefab => prefab;
    public EffectLifetimeType LifetimeType => lifetimeType;
    public float Duration => duration;
    public Vector3 PositionOffset => positionOffset;
    public Vector3 RotationOffset => rotationOffset;
    public Vector3 Scale => scale;
    public bool FollowTarget => followTarget;
    public bool UseValueColor => useValueColor;

    public Color GetColor(float value)
    {
        if (!useValueColor || colorGradient == null)
        {
            return Color.white;
        }

        float normalized = Mathf.InverseLerp(colorMinValue, colorMaxValue, value);

        return colorGradient.Evaluate(normalized);
    }
}
