using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AxeSkinCatalogSO", menuName = "Scriptable Objects/AxeSkinCatalogSO")]
public class AxeSkinCatalogSO : ScriptableObject
{
    [SerializeField] private List<AxeSkinSO> skins = new();
    public IReadOnlyList<AxeSkinSO> Skins => skins;

    public bool TryGetSkin(string skinId, out AxeSkinSO result)
    {
        result = null;

        if (string.IsNullOrEmpty(skinId)) return false;

        foreach (AxeSkinSO skin in skins)
        {
            if (skin == null) return false;

            if (string.Equals(skin.SkinId, skinId, StringComparison.Ordinal))
            {
                result = skin;
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnVaildate()
    {
        HashSet<string> ids = new();

        foreach (AxeSkinSO skin in skins)
        {
            if (skin == null) continue;

            if (!ids.Add(skin.SkinId))
                Debug.LogError($"[AxeSkinCatalog] 중복 SkinId 발견 : {skin.SkinId}", this);
        }
    }
#endif
}
