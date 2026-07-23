using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameThemeCatalogSO", menuName = "Scriptable Objects/GameThemeCatalogSO")]
public class GameThemeCatalogSO : ScriptableObject
{
    [SerializeField] private List<GameThemeSO> themes = new();

    public int Count => themes.Count;

    public GameThemeSO GetRandomTheme()
    {
        if (themes == null || themes.Count == 0)
        {
            Debug.LogError("[GameThemeCatalogSO] 등록된 테마 없음", this);
            return null;
        }

        int index = UnityEngine.Random.Range(0, themes.Count);
        return themes[index];
    }

    public bool TryGetTheme(string themeId, out GameThemeSO result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(themeId)) return false;

        foreach (GameThemeSO theme in themes)
        {
            if (theme == null) continue;

            if (string.Equals(theme.ThemeId, themeId, StringComparison.Ordinal))
            {
                result = theme;
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnVaildate()
    {
        HashSet<string> ids = new();

        foreach (GameThemeSO theme in themes)
        {
            if (theme == null || string.IsNullOrWhiteSpace(theme.ThemeId)) continue;

            if (!ids.Add(theme.ThemeId))
            {
                Debug.LogError($"중복된 Theme ID 존재 : {theme.ThemeId}", this);
            }
        }
    }
#endif
}
