using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class ComponentUIInfo
{
    public VillageType villageType;
    public TMP_Text CurLevel;
    public TMP_Text CurDesc;
    public TMP_Text NextDesc;
    public TMP_Text UpgradeGold;
}

[Serializable]
public class VillageUIInfo
{
    public TMP_Text CurGold;
    public List<ComponentUIInfo> VillageComponents;
}

public class AISimVUIController : MonoBehaviour
{
    public static AISimVUIController instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public GameObject VillagePanel;
    public VillageUIInfo p1VUIInfo;
    public VillageUIInfo p2VUIInfo;

    private void OnEnable()
    {
        SimVillageState.OnVillageObjChanged += HandleVillageObjChanged;
    }

    private void OnDisable()
    {
        SimVillageState.OnVillageObjChanged -= HandleVillageObjChanged;
    }

    private void HandleVillageObjChanged(int playerNum, VillageObjInfo changedInfo)
    {
        VillageUIInfo VUIInfo = playerNum == 1 ? p1VUIInfo : p2VUIInfo;

        ComponentUIInfo targetUI = VUIInfo.VillageComponents.Find(
        component => component.villageType == changedInfo._levelData.VillageType);

        if (targetUI != null)
        {
            if (targetUI.CurLevel != null)
                targetUI.CurLevel.text = $"Lv.{changedInfo.currentLevel}";

            if (targetUI.CurDesc != null)
                targetUI.CurDesc.text = changedInfo.curLevelDesc;

            if (targetUI.NextDesc != null)
                targetUI.NextDesc.text = changedInfo.nextLevelDesc;

            if (targetUI.UpgradeGold != null)
                targetUI.UpgradeGold.text = changedInfo.upgradeGold.ToString();
        }
        else
        {
            Debug.LogWarning($"[{changedInfo._levelData.VillageType}] 타입의 UI 컴포넌트를 찾을 수 없습니다.");
        }
    }
}
