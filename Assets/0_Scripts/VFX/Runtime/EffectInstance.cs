using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EffectInstance : MonoBehaviour
{
    private GameVFXManager owner;
    private EffectDataSO effectData;
    private ParticleSystem[] particleSystems;
    private ParticleSystem.MinMaxGradient[] defaultColors;

    private bool isPlaying;
    private int playVersion;

    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        defaultColors = new ParticleSystem.MinMaxGradient[particleSystems.Length];

        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem.MainModule main = particleSystems[i].main;
            defaultColors[i] = main.startColor;
        }
    }

    public void Initialize(GameVFXManager manager, EffectDataSO data)
    {
        owner = manager;
        effectData = data;
    }

    public void SetColor(Color color)
    {
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ParticleSystem.MainModule main = particleSystem.main;
            main.startColor = color;
        }
    }

    public void SetColorByValue(float value)
    {
        if (!effectData.UseValueColor)
        {
            return;
        }

        Color color = effectData.GetColor(value);
        SetColor(color);
    }

    public void ResetColor()
    {
        for (int i = 0; i < particleSystems.Length; i++)
        {
            ParticleSystem.MainModule main = particleSystems[i].main;
            main.startColor = defaultColors[i];
        }
    }
    public void Play()
    {
        isPlaying = true;
        playVersion++;

        gameObject.SetActive(true);

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Clear(true);
            particleSystem.Play(true);
        }

        if (effectData.LifetimeType == EffectLifetimeType.Duration)
        {
            AutoStop(playVersion).Forget();
        }
    }

    public async UniTaskVoid AutoStop(int version)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(effectData.Duration), cancellationToken: this.GetCancellationTokenOnDestroy());

        if (!isPlaying || version != playVersion) return;

        Stop();
    }

    public void Stop()
    {
        if (!isPlaying) return;

        isPlaying = false;
        playVersion--;

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        owner.Return(effectData.EffectId, this);
    }
}
