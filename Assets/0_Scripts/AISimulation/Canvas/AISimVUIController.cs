using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class DebugInfo
{
    public TMP_Text GoldIncome;
    public TMP_Text DefaultShild;
    public TMP_Text MaxEng;
    public TMP_Text DayEng;
    public TMP_Text MaxPow;
    public TMP_Text MinPow;
}

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
    public DebugInfo infos;
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
        SimVillageState.OnVStatChange += HandleVillageValueChanged;
    }

    private void OnDisable()
    {
        SimVillageState.OnVillageObjChanged -= HandleVillageObjChanged;
        SimVillageState.OnVStatChange -= HandleVillageValueChanged;
    }

    public void ActiveVillageUI()
    {
        VillagePanel.transform.localScale = Vector3.one;
    }

    public void UnActiveVillageUI()
    {
        VillagePanel.transform.localScale = Vector3.zero;
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

    private void HandleVillageValueChanged(int playerNum, VStateType type, int value)
    {
        DebugInfo debugInfo = playerNum == 1 ? p1VUIInfo.infos : p1VUIInfo.infos;

        switch (type)
        {
            case VStateType.VIncomeGold:
                debugInfo.GoldIncome.text = value.ToString();
                break;
            case VStateType.VBarrier:
                debugInfo.DefaultShild.text = value.ToString();
                break;
            case VStateType.MaxEnergy:
                debugInfo.MaxEng.text = value.ToString();
                break;
            case VStateType.DayEnergy:
                debugInfo.DayEng.text = value.ToString();
                break;
            case VStateType.MaxHitDamage:
                debugInfo.MaxPow.text = value.ToString();
                break;
            case VStateType.MinHitDamage:
                debugInfo.MinPow.text = value.ToString();
                break;
        }
    }
}
