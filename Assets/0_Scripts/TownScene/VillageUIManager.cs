
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class VillageUIManager : MonoBehaviour
{

    StringBuilder currentGoldText = new StringBuilder();
    public TextMeshProUGUI goldText;

    void Start()
    {
        VillageManager.Instance.OnGoldChanged.AddListener(UpdateGoldText);
        UpdateGoldText(VillageManager.Instance.GetMyGold());
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
        VillageManager.Instance.TryUpgradeLevel(VillageUpgradeIndex.House);
    }

    public void OnClickAddGoldButton()
    {
        VillageManager.Instance.AddGold(100);
    }


}