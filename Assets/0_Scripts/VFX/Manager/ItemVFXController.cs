using Photon.Pun;
using UnityEngine;

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

        bool isAIMode = GameManager.Instance.isSoloPlay;
        if (!isAIMode)
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
                GameVFXManager.Instance.Play("PowerUp", pc.EffectPoints.Axe);
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
                GameVFXManager.Instance.Play("PowerUp", ac.EffectPoints.Axe);
                break;
            default:
                break;
        }
    }
}
