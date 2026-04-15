using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using System.Collections.Generic;

[Serializable]
public class PlayerUIGruop
{
    public TMP_Text VillageHP;
    public TMP_Text Energy;
    public TMP_Text CurBarrier;
    public TMP_Text TotalDmg;
    public TMP_Text CurBarRate;
    public TMP_Text CurMultRate;

}

public class AISimUIController : MonoBehaviour
{
    public static AISimUIController instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        SimGameState.OnStatChange += UpdateStatUI;
        SimGameState.OnTreeChange += UpdateTreeUI;
        SimGameState.OnItemAdded += AddPlayerInv;
        SimGameState.OnItemRemoved += RemovePlayerInv;
    }

    private void OnDisable()
    {
        SimGameState.OnStatChange -= UpdateStatUI;
        SimGameState.OnTreeChange -= UpdateTreeUI;
        SimGameState.OnItemAdded -= AddPlayerInv;
        SimGameState.OnItemRemoved -= RemovePlayerInv;
    }

    [Header("P1 UIs")]
    public PlayerUIGruop p1UI;

    [Header("P2 UIs")]
    public PlayerUIGruop p2UI;

    [Header("Tree UIs")]
    public TMP_Text TreeHp;
    public TMP_Text TreeToxic;

    [Header("Inventroy UIs")]
    public GameObject ItemInfoPrefab;
    public Transform p1ItemInfoTarget;
    public Transform p2ItemInfoTarget;
    public List<ItemInfo> p1Infos = new List<ItemInfo>();
    public List<ItemInfo> p2Infos = new List<ItemInfo>();

    public void InitStatUI(SimGameState state)
    {
        p1UI.VillageHP.text = state.p1VillHP.ToString();
        p1UI.Energy.text = state.p1Energy.ToString();
        p1UI.CurBarrier.text = state.p1VillBarrier.ToString();
        p1UI.TotalDmg.text = state.p1TotalHitDmg.ToString();
        p1UI.CurBarRate.text = state.p1VillBarConRate.ToString();
        p1UI.CurMultRate.text = state.p1DmgMultRate.ToString();

        p2UI.VillageHP.text = state.p2VillHP.ToString();
        p2UI.Energy.text = state.p2Energy.ToString();
        p2UI.CurBarrier.text = state.p2VillBarrier.ToString();
        p2UI.TotalDmg.text = state.p2TotalHitDmg.ToString();
        p2UI.CurBarRate.text = state.p2VillBarConRate.ToString();
        p2UI.CurMultRate.text = state.p2DmgMultRate.ToString();

        TreeHp.text = state.treeHP.ToString();
        TreeToxic.text = state.treeToxicDmg.ToString();
    }


    private void UpdateStatUI(int playerNum, StatType type, float value)
    {
        PlayerUIGruop targetUI = (playerNum == 1) ? p1UI : p2UI;

        switch (type)
        {
            case StatType.VillageHP: targetUI.VillageHP.text = value.ToString(); break;
            case StatType.Energy: targetUI.Energy.text = value.ToString(); break;
            case StatType.TotalDmg: targetUI.TotalDmg.text = value.ToString(); break;
            case StatType.Barrier: targetUI.CurBarrier.text = value.ToString(); break;
            case StatType.BarConRate: targetUI.CurBarRate.text = value.ToString(); break;
            case StatType.MultRate: targetUI.CurMultRate.text = value.ToString(); break;
            default: break;
        }
    }

    private void UpdateTreeUI(TreeType type, float value)
    {
        switch (type)
        {
            case TreeType.TreeHP: TreeHp.text = value.ToString(); break;
            case TreeType.TreeToxic: TreeToxic.text = value.ToString(); break;
            default: break;
        }
    }

    private void AddPlayerInv(int playerNum, string itemId)
    {
        ItemSO item = ItemDB.Instance.Get(itemId);
        if (playerNum == 1)
        {
            GameObject go = Instantiate(ItemInfoPrefab, p1ItemInfoTarget);
            ItemInfo info = go.GetComponent<ItemInfo>();
            info.InitInfo(item);
            p1Infos.Add(info);
        }
        else
        {
            GameObject go = Instantiate(ItemInfoPrefab, p2ItemInfoTarget);
            ItemInfo info = go.GetComponent<ItemInfo>();
            info.InitInfo(item);
            p2Infos.Add(info);
        }
    }

    private void RemovePlayerInv(int playerNum, string itemId)
    {
        ItemSO item = ItemDB.Instance.Get(itemId);
        if (playerNum == 1)
        {
            foreach (ItemInfo info in p1Infos)
            {
                if (info.ItemId.text.Trim() == itemId)
                {
                    info.PlayItemUsed();
                    p1Infos.Remove(info);
                    return;
                }
            }
        }
        else
        {
            foreach (ItemInfo info in p2Infos)
            {
                if (info.ItemId.text.Trim() == itemId)
                {
                    info.PlayItemUsed();
                    p1Infos.Remove(info);
                    return;
                }
            }

        }

        Debug.LogError($"[Item UI Process ERROR] Can't Find Item In p{playerNum}'s item list");
    }

    public void OnInteractFalse(Button btn)
    {
        btn.interactable = false;
    }
}
