using UnityEngine;
using TMPro;
using System;

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
    }

    private void OnDisable()
    {
        SimGameState.OnStatChange -= UpdateStatUI;
        SimGameState.OnTreeChange -= UpdateTreeUI;
    }

    [Header("P1 UIs")]
    public PlayerUIGruop p1UI;

    [Header("P2 UIs")]
    public PlayerUIGruop p2UI;

    [Header("Tree UIs")]
    public TMP_Text TreeHp;
    public TMP_Text TreeToxic;


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
}
