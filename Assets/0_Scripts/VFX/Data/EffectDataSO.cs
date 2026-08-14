using UnityEngine;

[CreateAssetMenu(fileName = "EffectDataSO", menuName = "Scriptable Objects/EffectDataSO")]
public class EffectDataSO : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private string effectId;

    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Lifetime")]
    [SerializeField] private float duration = 2f;

    [Header("Transform")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    [Header("Follow")]
    [SerializeField] private bool followTarget;

    public string EffectId => effectId;
    public GameObject Prefab => prefab;
    public float Duration => duration;
    public Vector3 PositionOffset => positionOffset;
    public Vector3 RotationOffset => rotationOffset;
    public bool FollowTarget => followTarget;
}
