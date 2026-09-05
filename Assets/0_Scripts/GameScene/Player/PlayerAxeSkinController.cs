using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerAxeSkinController : MonoBehaviourPunCallbacks
{
    private const string DefaultSkinId = "axe_basic";

    [SerializeField] private AxeSkinCatalogSO axeSkinCatalog;
    [SerializeField] private Transform axeVisualRoot;

    private GameObject currentAxeVisual;
    private string currentSkinId;

    private void Start()
    {
        if (photonView.IsMine)
        {
            PlayerProfile.OnAxeSkinChanged += HandelLocalAxeSkinChanged;

            ApplyLocalSkin();
            PublishLocalSkin();
        }
        else
        {
            ApplyOwnerSkin();
        }
    }

    private void OnDestroy()
    {
        if (photonView != null && photonView.IsMine)
        {
            PlayerProfile.OnAxeSkinChanged -= HandelLocalAxeSkinChanged;
        }
    }

    private void HandelLocalAxeSkinChanged()
    {
        if (!photonView.IsMine) return;

        ApplyLocalSkin();
        PublishLocalSkin();
    }

    public void ApplyLocalSkin()
    {
        ApplySkin(GetLocalSkinId());
    }

    private void PublishLocalSkin()
    {
        if (!photonView.IsMine) return;

        string skinId = GetLocalSkinId();

        PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.AxeSkinId, skinId);
    }

    private void ApplyOwnerSkin()
    {
        Player owner = photonView.Owner;

        if (owner == null)
        {
            ApplySkin(DefaultSkinId);
            return;
        }

        if (!owner.CustomProperties.TryGetValue(PlayerPropKeys.AxeSkinId, out object value))
        {
            ApplySkin(DefaultSkinId);
            return;
        }

        if (value is not string skinId || string.IsNullOrWhiteSpace(skinId))
        {
            ApplySkin(DefaultSkinId);
            return;
        }

        ApplySkin(skinId);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer != photonView.Owner) return;

        if (!changedProps.ContainsKey(PlayerPropKeys.AxeSkinId)) return;

        ApplyOwnerSkin();
    }

    public void ApplySkin(string skinId)
    {
        if (axeSkinCatalog == null)
        {
            Debug.LogError("[PlayerAxeSkinController] AxeSkinCatalog 없음", this);
            return;
        }

        if (axeVisualRoot == null)
        {
            Debug.LogError("[PlayerAxeSkinController] AxeVisualRoot 없음", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(skinId)) skinId = DefaultSkinId;

        if (currentSkinId == skinId && currentAxeVisual != null) return;

        if (!axeSkinCatalog.TryGetSkin(skinId, out AxeSkinSO skin))
        {
            Debug.LogWarning($"[PlayerAxeSkinController] 존재하지 않는 SkinId : {skinId}", this);

            if (!axeSkinCatalog.TryGetSkin(DefaultSkinId, out skin))
            {
                Debug.LogError("[PlayerAxeSkinController] 기본 도끼가 Catalog에 없습니다.", this);
                return;
            }

            skinId = DefaultSkinId;
        }

        if (skin.AxePrefab == null)
        {
            Debug.LogError($"[PlayerAxeSkinController] {skin.SkinId} Prefab 없음", skin);
            return;
        }

        ReplaceVisual(skin.AxePrefab);

        currentSkinId = skinId;
    }

    private void ReplaceVisual(GameObject axePrefab)
    {
        if (currentAxeVisual != null) Destroy(currentAxeVisual);

        currentAxeVisual = Instantiate(axePrefab, axeVisualRoot);

        currentAxeVisual.transform.localPosition = Vector3.zero;
        currentAxeVisual.transform.localRotation = Quaternion.identity;
        currentAxeVisual.transform.localScale = Vector3.one;
    }

    private string GetLocalSkinId()
    {
        string skinId = PlayerProfile.EquippedAxeSkinId;

#if UNITY_EDITOR
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            skinId = "axe_ruby";
        else
            skinId = "axe_silver";
#endif

        if (string.IsNullOrWhiteSpace(skinId))
            skinId = DefaultSkinId;

        return skinId;
    }
}
