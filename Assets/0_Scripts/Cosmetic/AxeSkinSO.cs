using UnityEngine;

[CreateAssetMenu(fileName = "AxeSkinSO", menuName = "Scriptable Objects/AxeSkinSO")]
public class AxeSkinSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string skinId;
    [SerializeField] private string displayName;

    [Header("Shop")]
    [SerializeField] private int price;
    [SerializeField] private Sprite icon;
    [TextArea]
    [SerializeField] private string description;

    [Header("Visual")]
    [SerializeField] private GameObject axePrefab;

    public string SkinId => skinId;
    public string DisplayName => displayName;
    public int Price => price;
    public Sprite Icon => icon;
    public string Description => description;
    public GameObject AxePrefab => axePrefab;


#if UNITY_EDITOR
    private void OnVaildate()
    {
        if (price < 0) price = 0;
        if (string.IsNullOrEmpty(skinId))
            Debug.LogError($"[{name}] SKinId가 비어있음.", this);
    }
#endif
}
