using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameThemeCatalogSO", menuName = "Scriptable Objects/GameThemeCatalogSO")]
public class GameThemeCatalogSO : ScriptableObject
{
    [Serializable]
    private class ThemeEntry
    {
        [SerializeField]
        private GameThemeSO theme;

        [Min(0)]
        [SerializeField]
        private int weight = 1;

        public GameThemeSO Theme => theme;
        public int Weight => weight;
    }

    [SerializeField] private List<ThemeEntry> themes = new();

    public int Count => themes.Count;

    public GameThemeSO GetRandomTheme()
    {
        if (themes == null || themes.Count == 0)
        {
            Debug.LogError("[GameThemeCatalogSO] 등록된 테마 없음", this);
            return null;
        }

        int totalWeight = 0;

        foreach (ThemeEntry entry in themes)
        {
            if (entry == null || entry.Theme == null || entry.Weight <= 0) continue;

            totalWeight += entry.Weight;
        }

        if (totalWeight <= 0) return null;

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int accumlatedWeight = 0;

        foreach (ThemeEntry entry in themes)
        {
            if (entry == null || entry.Theme == null || entry.Weight <= 0) continue;
            accumlatedWeight += entry.Weight;

            if (randomValue < accumlatedWeight) return entry.Theme;
        }

        Debug.LogError("failed to select theme as random");
        return null;
    }

    public bool TryGetTheme(string themeId, out GameThemeSO result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(themeId)) return false;

        foreach (ThemeEntry entry in themes)
        {
            if (entry == null || entry.Theme == null) continue;

            if (string.Equals(entry.Theme.ThemeId, themeId, StringComparison.Ordinal))
            {
                result = entry.Theme;
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HashSet<string> ids = new();

        foreach (ThemeEntry entry in themes)
        {
            if (entry == null || entry.Theme == null) continue;
            string themeId = entry.Theme.ThemeId;

            if (string.IsNullOrWhiteSpace(themeId)) continue;

            if (!ids.Add(themeId))
            {
                Debug.LogError($"중복된 Theme ID 존재 : {themeId}", this);
            }

            if (entry.Weight <= 0)
            {
                Debug.LogError($"{themeId}의 가중치가 0 이하이므로, 랜덤 선택에서 제외됩니다.", this);
            }
        }
    }
#endif
}
