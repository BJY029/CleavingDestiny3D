using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatusBarUI : MonoBehaviour
{
    void Awake()
    {
        ownerActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
    }
    private int ownerActorNum;

    [Header("Item Infos")]
    public Transform iconParent;
    public GameObject iconPrefab;

    [Header("Binded Icons")]
    public Sprite BindImg;

    private void OnEnable()
    {
        if (StatusUIModel.instance != null)
        {
            StatusUIModel.instance.StatusOnChanged += Rebuild;
            Rebuild();
        }
    }

    private void OnDisable()
    {
        if (StatusUIModel.instance != null)
        {
            StatusUIModel.instance.StatusOnChanged -= Rebuild;
        }
    }


    private void Rebuild()
    {
        Debug.Log("Rebuild Activated");
        for (int i = iconParent.childCount - 1; i >= 0; i--)
            Destroy(iconParent.GetChild(i).gameObject);

        List<ItemStatusInfo> list = StatusUIModel.instance.GetAllForOwner(ownerActorNum);
        Debug.Log($"list count : {list.Count}");
        foreach (var st in list)
        {
            bool isMine = (PhotonNetwork.LocalPlayer.ActorNumber == ownerActorNum);
            bool shouldMask = (!isMine && st.isHiddenToEnemy);

            var go = Instantiate(iconPrefab, iconParent);
            StatusIconView view = go.GetComponent<StatusIconView>();

            if (shouldMask)
            {
                view.BindMasked(BindImg, st.stackCount, st.activateTrigger, st.type);
            }
            else
            {
                var meta = ItemDB.Instance.Get(st.itemId);
                view.Bind(meta.Icon, st.stackCount, st.activateTrigger, st.type);
            }
        }
    }
}

