using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSkinCatalogSO", menuName = "Scriptable Objects/CharacterSkinCatalogSO")]
public class CharacterSkinCatalogSO : ScriptableObject
{
    [SerializeField] private CharacterSkinSO defaultSkin;
    public CharacterSkinSO DefaultSkin => defaultSkin;

    [SerializeField] private List<CharacterSkinSO> skins = new();
    public IReadOnlyList<CharacterSkinSO> Skins => skins;

    public bool TryGetSkin(string skinId, out CharacterSkinSO result)
    {
        result = null;

        if (string.IsNullOrEmpty(skinId)) return false;
        if (defaultSkin != null && defaultSkin.SkinId == skinId) { result = defaultSkin; return true; }

        foreach (CharacterSkinSO skin in skins)
        {
            if (skin == null) continue;

            if (string.Equals(skin.SkinId, skinId, StringComparison.Ordinal))
            {
                result = skin;
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (defaultSkin == null || string.IsNullOrWhiteSpace(defaultSkin.SkinId))
            Debug.LogError("[CharacterSkinCatalog] 기본 캐릭터를 지정하세요.", this);
        HashSet<string> ids = new();

        foreach (CharacterSkinSO skin in skins)
        {
            if (skin == null) continue;

            if (!ids.Add(skin.SkinId))
                Debug.LogError($"[CharacterSkinCatalog] 중복 SkinId 발견 : {skin.SkinId}", this);
        }
    }
#endif
}
