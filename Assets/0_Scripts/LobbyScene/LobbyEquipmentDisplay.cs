using UnityEngine;

/// <summary>Displays local equipment using visual-only lobby prefabs.</summary>
public sealed class LobbyEquipmentDisplay : MonoBehaviour
{
    private const string DefaultAxeSkinId = "axe_basic";

    [SerializeField] private CharacterSkinCatalogSO characterCatalog;
    [SerializeField] private AxeSkinCatalogSO axeCatalog;
    [SerializeField] private Transform characterRoot;
    [SerializeField] private Transform axeRoot;

    // Scene previews are replaced on first activation using the saved equipment.
    [SerializeField] private GameObject characterVisual;
    [SerializeField] private GameObject axeVisual;
    private GameObject currentCharacterPrefab;
    private GameObject currentAxePrefab;

    private void OnEnable()
    {
        PlayerProfile.OnCharacterSkinChanged += RefreshCharacter;
        PlayerProfile.OnAxeSkinChanged += RefreshAxe;
        RefreshCharacter();
        RefreshAxe();
    }

    private void OnDisable()
    {
        PlayerProfile.OnCharacterSkinChanged -= RefreshCharacter;
        PlayerProfile.OnAxeSkinChanged -= RefreshAxe;
    }

    private void RefreshCharacter()
    {
        if (characterCatalog == null || characterRoot == null) return;

        characterCatalog.TryGetSkin(PlayerProfile.EquippedCharacterSkinId, out var skin);
        if (skin == null || skin.LobbyPrefab == null) skin = characterCatalog.DefaultSkin;
        if (skin == null || skin.LobbyPrefab == null)
        {
            Debug.LogWarning("[LobbyEquipmentDisplay] Default character lobby prefab is missing.", this);
            return;
        }

        ReplaceVisual(skin.LobbyPrefab, characterRoot, ref characterVisual, ref currentCharacterPrefab);
    }

    private void RefreshAxe()
    {
        if (axeCatalog == null || axeRoot == null) return;

        axeCatalog.TryGetSkin(PlayerProfile.EquippedAxeSkinId, out var skin);
        if (skin == null || skin.LobbyPrefab == null) axeCatalog.TryGetSkin(DefaultAxeSkinId, out skin);
        if (skin == null || skin.LobbyPrefab == null)
        {
            Debug.LogWarning("[LobbyEquipmentDisplay] Default axe lobby prefab is missing.", this);
            return;
        }

        ReplaceVisual(skin.LobbyPrefab, axeRoot, ref axeVisual, ref currentAxePrefab);
    }

    private static void ReplaceVisual(GameObject prefab, Transform parent, ref GameObject visual,
        ref GameObject currentPrefab)
    {
        if (currentPrefab == prefab && visual != null) return;

        if (visual != null)
        {
            visual.SetActive(false);
            Destroy(visual);
        }

        visual = Instantiate(prefab, parent, false);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = Vector3.one;
        currentPrefab = prefab;
    }
}
