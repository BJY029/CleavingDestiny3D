using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatusBarUI : MonoBehaviour
{
    private static ItemStatusBarUI instance;

    private void Awake()
    {
        if (instance == null) instance = null;
        else Destroy(gameObject);
    }

    [Header("Owenr Act Num")]
    public int ownerActorNum;

    [Header("Item Infos")]
    public Transform iconParent;
    public StatusIconView iconPrefab;

    [Header("Binded Icons")]
    public Sprite BindImg;

    private void OnEnable()
    {
        StatusUIModel.instance.StatusOnChanged += Rebuild;
        Rebuild();
    }

    private void OnDisable()
    {
        StatusUIModel.instance.StatusOnChanged -= Rebuild;
    }


    private void Rebuild()
    {
        for (int i = iconParent.childCount - 1; i >= 0; i--)
            Destroy(iconParent.GetChild(i).gameObject);

        List<ItemStatusInfo> list = StatusUIModel.instance.GetAllForOwner(ownerActorNum);

        foreach (var st in list)
        {
            bool isMine = (PhotonNetwork.LocalPlayer.ActorNumber == ownerActorNum);
            bool shouldMask = (!isMine && st.isHiddenToEnemy);

            var view = Instantiate(iconPrefab, iconParent);

            if (shouldMask)
            {
                view.BindMasked(BindImg, st.remainingTurns, st.stackCount);
            }
            else
            {
                var meta = ItemDB.Instance.Get(st.itemId);
                view.Bind(meta.Icon, st.remainingTurns, st.stackCount);
            }
        }
    }
}

