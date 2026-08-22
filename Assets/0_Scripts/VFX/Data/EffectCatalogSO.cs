using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectCatalogSO", menuName = "Scriptable Objects/EffectCatalogSO")]
public class EffectCatalogSO : ScriptableObject
{
    [SerializeField] private List<EffectDataSO> effects = new();

    public bool TryGetEffect(string effectId, out EffectDataSO result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(effectId))
        {
            return false;
        }

        foreach (EffectDataSO effect in effects)
        {
            if (effect == null)
            {
                continue;
            }

            if (string.Equals(effect.EffectId, effectId, StringComparison.Ordinal))
            {
                result = effect;
                return true;
            }
        }

        return false;
    }
}
