
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class TownUIManager : MonoBehaviour
{

    StringBuilder currentGoldText = new StringBuilder();
    public TextMeshProUGUI goldText;

    void Start()
    {
        TownManager.Instance.OnGoldChanged.AddListener(UpdateGoldText);
        UpdateGoldText(TownManager.Instance.GetMyGold());
    }

    private void UpdateGoldText(int gold)
    {
        currentGoldText.Clear();
        currentGoldText.Append("Gold: ");
        currentGoldText.Append(gold);
        goldText.text = currentGoldText.ToString();
    }

    public void OnClickUpgradeHouseButton()
    {
        TownManager.Instance.TryUpgradeFacility(VillageUpgradeIndex.House);
    }

    public void OnClickAddGoldButton()
    {
        TownManager.Instance.AddGold(100);
    }


}