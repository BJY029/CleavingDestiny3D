using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public enum VFXType : byte
{
    PowerUP,
    PowerDown,
    PlayerShield,
    PlayerEngUp,

}

public class ItemVFXController : MonoBehaviourPun
{
    public static ItemVFXController Instance;

    private readonly Dictionary<(VFXType, int), EffectInstance> activeEffects = new();

    private void Awake()
    {
        if (Instance == null || Instance == this) Instance = this;
        else Destroy(gameObject);
    }

    public void Master_PlayItemVFX(ItemSO item, int actorNumber)
    {
        if (item == null)
        {
            Debug.LogWarning("[ItemVFXController]Item is NULL");
            return;
        }

        bool isAITurn = GameManager.Instance.isSoloPlay && actorNumber == PlayerManager.Instance.AIActNum;
        if (!isAITurn)
        {
            switch (item.type)
            {
                case ItemType.Damage:
                    photonView.RPC(nameof(Client_PlayItemVFX), RpcTarget.All, VFXType.PowerUP, actorNumber);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (item.type)
            {
                case ItemType.Damage:
                    AIMode_PlayItemVFX(VFXType.PowerUP, actorNumber);
                    break;
                default:
                    break;
            }
        }
    }

    public void Any_StopItemVFX(VFXType vfxType, int actorNum)
    {
        bool isAIMode = GameManager.Instance.isSoloPlay;

        if (!isAIMode)
        {
            photonView.RPC(nameof(Client_StopItemVFX), RpcTarget.All, vfxType, actorNum);
        }
        else
        {
            StopItemVFX(vfxType, actorNum);
        }
    }

    [PunRPC]
    private void Client_PlayItemVFX(VFXType vfxType, int actorNum)
    {
        if (!PlayerObjectRegistry.TryGet(actorNum, out PlayerController pc))
        {
            Debug.LogWarning($"[ItemVFXController] PlayerController not found. ActorNumber={actorNum}");
            return;
        }

        if (pc.EffectPoints == null)
        {
            Debug.LogWarning($"[ItemVFXController] EffectPoints not found. ActorNumber={actorNum}");
            return;
        }

        switch (vfxType)
        {
            case VFXType.PowerUP:
                if (pc.EffectPoints.Axe == null)
                {
                    Debug.LogWarning($"[ItemVFXController] Axe EffectPoint not found. ActorNumber={actorNum}");
                    return;
                }

                PlayAndRegister(vfxType, actorNum, "PowerUp", pc.EffectPoints.Axe);
                break;
            default:
                break;
        }
    }

    private void AIMode_PlayItemVFX(VFXType vfxType, int aiNum)
    {
        if (!PlayerObjectRegistry.TryGet(aiNum, out AIController ac))
        {
            Debug.LogWarning($"[ItemVFXController] AIController not found. ActorNumber={aiNum}");
            return;
        }

        if (ac.EffectPoints == null)
        {
            Debug.LogWarning($"[ItemVFXController] EffectPoints not found. ActorNumber={aiNum}");
            return;
        }

        switch (vfxType)
        {
            case VFXType.PowerUP:
                if (ac.EffectPoints.Axe == null)
                {
                    Debug.LogWarning($"[ItemVFXController] Axe EffectPoint not found. ActorNumber={aiNum}");
                    return;
                }

                PlayAndRegister(vfxType, aiNum, "PowerUp", ac.EffectPoints.Axe);
                break;
            default:
                break;
        }
    }

    [PunRPC]
    private void Client_StopItemVFX(VFXType vfxType, int actorNum)
    {
        StopItemVFX(vfxType, actorNum);
    }

    private void PlayAndRegister(VFXType vfxType, int actorNum, string effectId, Transform target)
    {
        (VFXType, int) key = (vfxType, actorNum);

        if (activeEffects.TryGetValue(key, out EffectInstance currentEffect))
        {
            if (currentEffect != null) currentEffect.Stop();
            activeEffects.Remove(key);
        }

        EffectInstance instance = GameVFXManager.Instance.Play(effectId, target);

        if (instance != null) activeEffects[key] = instance;
    }

    private void StopItemVFX(VFXType vfxType, int actorNum)
    {
        Debug.LogWarning("Stopping Effect");
        (VFXType, int) key = (vfxType, actorNum);

        if (!activeEffects.TryGetValue(key, out EffectInstance instance))
        {
            Debug.LogWarning($"[ItemVFXController] There is no {vfxType.ToString()} in DIC");
            return;
        }

        if (instance != null) instance.Stop();

        activeEffects.Remove(key);
    }
}
