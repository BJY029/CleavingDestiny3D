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
    [SerializeField] private Transform poisonTransfer;
    [SerializeField] private ParticleSystem poisonHead;
    [SerializeField] private ParticleSystem poisonTrail;
    [SerializeField] private float poisonTraveDuration = 2.5f;
    [SerializeField] private float poisonArcHeight = 5f;
    [SerializeField] private float villagePoisonDuration = 7f;

    [Header("Village each")]
    [SerializeField] private ParticleSystem p1PoisonCloud;
    [SerializeField] private ParticleSystem p2PoisonCloud;
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform p1VilageTransform;
    [SerializeField] private Transform p2VilageTransform;

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

        Transform targetTransform = actNum == 0 ? p1VilageTransform : p2VilageTransform;
        ParticleSystem targetParticle = actNum == 0 ? p1PoisonCloud : p2PoisonCloud;
        await PlayPoisonTransferAsync(startTransform.position, targetTransform.position, targetParticle);
    }

    private async UniTask PlayPoisonTransferAsync(Vector3 startPosition, Vector3 targetPosition, ParticleSystem villageCloud)
    {
        poisonHead.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        poisonTrail.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        poisonTransfer.position = startPosition;
        poisonTransfer.gameObject.SetActive(true);

        poisonHead.Play(true);
        poisonTrail.Play(true);

        AudioManager audioManager = AudioManager.Instance;
        AudioSource movingSound = null;
        try
        {
            if (audioManager != null)
            {
                movingSound = audioManager.PlayAmbient3D("Poison_Travel", startPosition, 15f, 200f);

                if (movingSound != null)
                {
                    movingSound.rolloffMode = AudioRolloffMode.Linear;

                    movingSound.dopplerLevel = 0f;
                }
            }
            float elapsed = 0f;
            float duration = Mathf.Max(0.01f, poisonTraveDuration);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);

                float height = 4f * t * (1f - t) * poisonArcHeight;

                poisonTransfer.position = position + Vector3.up * height;

                if (movingSound != null) movingSound.transform.position = position;

                await UniTask.Yield(PlayerLoopTiming.Update, destroyCancellationToken);
            }

            poisonTransfer.position = targetPosition;
        }
        finally
        {
            if (audioManager != null && movingSound != null)
                audioManager.StopAmbient3D(movingSound);

            if (poisonHead != null)
                poisonHead.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (poisonTrail != null)
                poisonTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (villageCloud != null)
        {
            villageCloud.gameObject.SetActive(true);
            villageCloud.Play(true);
            AudioManager.Instance.PlaySfx3D("CrowdScream", targetPosition, 40f, 500f, AudioRolloffMode.Linear);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(villagePoisonDuration));
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


