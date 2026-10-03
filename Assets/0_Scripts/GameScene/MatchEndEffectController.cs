using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using Unity.VisualScripting;
using Photon.Pun;

public class MatchEndEffectController : MonoBehaviourPun
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

        int actNum = PhotonNetwork.LocalPlayer.ActorNumber;
        GameObject PlayerObj = PlayerManager.Instance.LocalPlayerObj;
        Vector3 des = PlayerManager.Instance.hitPos[actNum - 1];
        Quaternion rot = PlayerManager.Instance.spawnRot[actNum - 1];
        //플레이어의 PlayerController 컴포넌트
        PlayerController pc = PlayerObj.GetComponent<PlayerController>();

        //플레이어 순간이동
        if (PlayerObj != null)
        {
            TeleportPlayer(PlayerObj, des, rot);
            pc?.ResetCameraToForward();
        }

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

    private void TeleportPlayer(GameObject player, Vector3 destination, Quaternion rotation)
    {
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = destination;
            player.transform.rotation = rotation;
            cc.enabled = true;
        }
    }
}

