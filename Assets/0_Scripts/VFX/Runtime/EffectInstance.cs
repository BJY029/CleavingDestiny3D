using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EffectInstance : MonoBehaviour
{
    private GameVFXManager owner;
    private EffectDataSO effectData;
    private ParticleSystem[] particleSystems;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void Initialize(GameVFXManager manager, EffectDataSO data)
    {
        owner = manager;
        effectData = data;
    }

    public async UniTaskVoid Play()
    {
        gameObject.SetActive(true);

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Clear(true);
            particleSystem.Play(true);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(effectData.Duration), cancellationToken: this.GetCancellationTokenOnDestroy());

        owner.Return(effectData.EffectId, this);
    }
}
