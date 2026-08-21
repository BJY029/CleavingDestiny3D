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

    public GameObject[] bonfires;

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

        if (item.itemId == "4002")
        {
            photonView.RPC(nameof(RPC_ActiveBonFireVFX), RpcTarget.All);
            return;
        }

        bool isAITurn = GameManager.Instance.isSoloPlay && actorNumber == PlayerManager.Instance.AIActNum;

        switch (item.type)
        {
            case ItemType.Damage:
                {
                    if (item.target == ItemTarget.Tree || item.target == ItemTarget.OpponentTree)
                    {
                        int targetActNum = actorNumber;
                        if (item.target == ItemTarget.OpponentTree)
                        {
                            targetActNum = PlayerManager.Instance.GetOppActNum(actorNumber);
                            isAITurn = !isAITurn;
                        }

                        float dmgMultiplier = ItemHandlingSystem.instance.GetCurrentDamageMultiplier(targetActNum);

                        if (isAITurn)
                            AIMode_PlayItemVFX(VFXType.PowerUP, targetActNum, dmgMultiplier);
                        else
                            photonView.RPC(nameof(Client_PlayItemVFX), RpcTarget.All, VFXType.PowerUP, targetActNum, dmgMultiplier);
                    }
                    break;
                }
            case ItemType.Defence:
                {
                    if (item.target == ItemTarget.Tree)
                    {
                        float dmgMultiplier = ItemHandlingSystem.instance.GetCurrentDamageMultiplier(actorNumber);

                        if (isAITurn)
                            AIMode_PlayItemVFX(VFXType.PowerUP, actorNumber, dmgMultiplier);
                        else
                            photonView.RPC(nameof(Client_PlayItemVFX), RpcTarget.All, VFXType.PowerUP, actorNumber, dmgMultiplier);
                    }
                    break;
                }

        }
    }

    public void Master_ResetTurnVFX(int actorNum)
    {
        photonView.RPC(nameof(RPC_Master_ResetTurnVFX), RpcTarget.MasterClient, actorNum);
    }

    [PunRPC]
    public void RPC_Master_ResetTurnVFX(int actorNum)
    {
        if (!GameManager.Instance.isSoloPlay && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Master_StopItemVFX(VFXType.PowerUP, actorNum);
    }

    public void Master_StopItemVFX(VFXType vfxType, int actorNum)
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
    private void Client_PlayItemVFX(VFXType vfxType, int actorNum, float colorValue)
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

                PlayOrUpdateVFX(vfxType, actorNum, "PowerUp", pc.EffectPoints.Axe, colorValue);
                break;
            default:
                break;
        }
    }

    private void AIMode_PlayItemVFX(VFXType vfxType, int aiNum, float? colorValue = null)
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

                PlayOrUpdateVFX(vfxType, aiNum, "PowerUp", ac.EffectPoints.Axe, colorValue);
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

    private void PlayOrUpdateVFX(VFXType vfxType, int actorNum, string effectId, Transform target, float? colorValue = null)
    {
        (VFXType, int) key = (vfxType, actorNum);

        if (activeEffects.TryGetValue(key, out EffectInstance currentEffect))
        {
            if (currentEffect != null && colorValue.HasValue)
            {
                currentEffect.SetColorByValue(colorValue.Value);
            }

            return;
        }

        EffectInstance instance = GameVFXManager.Instance.Play(effectId, target, 1f, colorValue);

        if (instance != null)
        {
            activeEffects[key] = instance;
        }
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

    public void StopBonFireVFX()
    {
        photonView.RPC(nameof(RPC_UnActiveBonFireVFX), RpcTarget.All);
    }

    [PunRPC]
    public void RPC_ActiveBonFireVFX()
    {
        foreach (GameObject go in bonfires)
        {
            if (!go.activeSelf)
                go.SetActive(true);
        }
    }

    [PunRPC]
    public void RPC_UnActiveBonFireVFX()
    {
        foreach (GameObject go in bonfires)
        {
            go.SetActive(false);
        }
    }
}
