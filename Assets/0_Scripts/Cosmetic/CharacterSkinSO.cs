using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSkinSO", menuName = "Scriptable Objects/CharacterSkinSO")]
public class CharacterSkinSO : ScriptableObject
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
    [SerializeField] private GameObject characterPrefab;
    [Tooltip("Visual-only prefab used by the lobby equipment display.")]
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField, HideInInspector] private string photonPrefabPath;

    public string SkinId => skinId;
    public string DisplayName => displayName;
    public int Price => price;
    public Sprite Icon => icon;
    public string Description => description;
    public GameObject CharacterPrefab => characterPrefab;
    public GameObject LobbyPrefab => lobbyPrefab;
    public string PhotonPrefabPath => photonPrefabPath;


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (price < 0) price = 0;
        photonPrefabPath = "";
        if (characterPrefab != null)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(characterPrefab);
            const string marker = "/Resources/";
            int index = path.LastIndexOf(marker, System.StringComparison.Ordinal);
            if (index >= 0 && path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase))
                photonPrefabPath = path.Substring(index + marker.Length, path.Length - index - marker.Length - ".prefab".Length);
            else Debug.LogError($"[{name}] CharacterPrefab must be a prefab inside a Resources folder.", this);
        }
        if (string.IsNullOrEmpty(skinId))
            Debug.LogError($"[{name}] SKinId가 비어있음.", this);
    }
#endif
}
