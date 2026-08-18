using System.Collections.Generic;
using UnityEngine;

public class GameVFXManager : MonoBehaviour
{
    public static GameVFXManager Instance { get; private set; }

    [SerializeField] private EffectCatalogSO effectCatalog;

    private readonly Dictionary<string, Queue<EffectInstance>> pools = new();

    private void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 특정 Position에서 한번 재생하고 끝나는 경우 사용
    /// </summary>
    /// <param name="effectId">effect id</param>
    /// <param name="position">effect position</param>
    public EffectInstance Play(string effectId, Vector3 position, float? colorValue = null)
    {
        if (!effectCatalog.TryGetEffect(effectId, out EffectDataSO effectData))
        {
            Debug.LogWarning($"[GameVFXManager] Effect not found : {effectId}");
            return null;
        }

        EffectInstance instance = Get(effectData);

        instance.transform.SetParent(null);
        instance.transform.position = position + effectData.PositionOffset;
        instance.transform.rotation = Quaternion.Euler(effectData.RotationOffset);

        if (colorValue.HasValue && effectData.UseValueColor)
        {
            instance.SetColor(effectData.GetColor(colorValue.Value));
        }
        else
        {
            instance.ResetColor();
        }

        instance.Play();

        return instance;
    }

    /// <summary>
    /// 특정 Transform 자식에 이펙트가 생성되어 따라다니도록 할 때 사용
    /// </summary>
    /// <param name="effectId">effect id</param>
    /// <param name="target">Play target transform</param>
    public EffectInstance Play(string effectId, Transform target, float scaleMultiplier = 1f, float? colorValue = null)
    {
        if (target == null) return null;

        if (!effectCatalog.TryGetEffect(effectId, out EffectDataSO effectData))
        {
            Debug.LogWarning($"[GameVFXManager] Effect not found : {effectId}");
            return null;
        }

        EffectInstance instance = Get(effectData);

        if (effectData.FollowTarget)
        {
            instance.transform.SetParent(target);
            instance.transform.localPosition = effectData.PositionOffset;
            instance.transform.localRotation = Quaternion.Euler(effectData.RotationOffset);
        }
        else
        {
            instance.transform.SetParent(null);
            instance.transform.position = target.position + effectData.PositionOffset;
            instance.transform.rotation = Quaternion.Euler(effectData.RotationOffset);
        }

        instance.transform.localScale = effectData.Scale * scaleMultiplier;

        if (colorValue.HasValue && effectData.UseValueColor)
        {
            Color color = effectData.GetColor(colorValue.Value);
            instance.SetColor(color);
        }
        else
        {
            instance.ResetColor();
        }

        instance.Play();

        return instance;
    }

    private EffectInstance Get(EffectDataSO effectData)
    {
        if (!pools.TryGetValue(effectData.EffectId, out Queue<EffectInstance> pool))
        {
            pool = new Queue<EffectInstance>();
            pools.Add(effectData.EffectId, pool);
        }

        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        GameObject effectObj = Instantiate(effectData.Prefab, transform);

        EffectInstance instance = effectObj.GetComponent<EffectInstance>();

        if (instance == null)
        {
            instance = effectObj.AddComponent<EffectInstance>();
        }

        instance.Initialize(this, effectData);

        effectObj.SetActive(false);

        return instance;
    }

    public void Return(string effectId, EffectInstance instance)
    {
        instance.transform.SetParent(transform);
        instance.gameObject.SetActive(false);

        if (!pools.TryGetValue(effectId, out Queue<EffectInstance> pool))
        {
            pool = new Queue<EffectInstance>();
            pools.Add(effectId, pool);
        }

        pool.Enqueue(instance);
    }
}
