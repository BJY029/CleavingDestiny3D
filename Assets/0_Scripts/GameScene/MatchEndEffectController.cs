using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;

public class MatchEndEffectController : MonoBehaviour
{
    public static MatchEndEffectController instance;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.8f;

    [Header("Tree")]
    [SerializeField] private Transform standingTreeRoot;
    private GameObject standingTree;
    [SerializeField] private GameObject fallenTree;
    [SerializeField] private ParticleSystem treeDust;
    [SerializeField] private float treeBlackScreenDuration = 2.5f;
    [SerializeField] private float treeRevealDuration = 2f;

    [Header("Village")]
    [SerializeField] private UnityEvent<int> onVillageDestroyed;
    [SerializeField] private UnityEvent onBothVillagesDestroyed;
    [SerializeField] private float villageEffectDuration = 3f;

    private bool hasStarted;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else instance = this;

        if (fallenTree != null) fallenTree.SetActive(false);
        if (treeDust != null) treeDust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void Play(int loserActorNum, MatchResultReason reason, Action onCompleted)
    {
        if (hasStarted) return;

        hasStarted = true;
        PlayAsync(loserActorNum, reason, onCompleted).Forget();
    }

    private async UniTask PlayAsync(int loserActorNum, MatchResultReason reason, Action OnCompleted)
    {
        switch (reason)
        {
            case MatchResultReason.TreeDestroyed:
                await PlayTreeDestroyedAsync();
                break;
            case MatchResultReason.VillageDestroyed:
                onVillageDestroyed?.Invoke(loserActorNum);
                await UniTask.Delay(TimeSpan.FromSeconds(villageEffectDuration));
                break;
            case MatchResultReason.Draw:
                //현재 무승부 조건: 두 마을이 함께 파괴됨.
                onBothVillagesDestroyed?.Invoke();
                await UniTask.Delay(TimeSpan.FromSeconds(villageEffectDuration));
                break;
        }

        OnCompleted?.Invoke();
    }

    private async UniTask PlayTreeDestroyedAsync()
    {
        SetActivedTree();
        FadeCanvas fade = FadeCanvas.Instance;

        if (fade != null) await fade.FadeInAsync(fadeDuration);

        AudioManager.Instance.PlaySfx2D("TreeFallSound");

        await UniTask.Delay(TimeSpan.FromSeconds(treeBlackScreenDuration));

        if (standingTree != null) standingTree.SetActive(false);
        if (fallenTree != null) fallenTree.SetActive(true);

        if (treeDust != null)
        {
            treeDust.gameObject.SetActive(true);
            treeDust.Play();
        }

        if (fade != null) await fade.FadeOutAsync(fadeDuration);

        await UniTask.Delay(TimeSpan.FromSeconds(treeRevealDuration));
    }

    private void SetActivedTree()
    {
        foreach (Transform child in standingTreeRoot)
        {
            if (child.gameObject.activeSelf)
            {
                standingTree = child.gameObject;
                break;
            }
        }
    }
}

