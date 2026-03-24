using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class AICanvasController : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TextMeshProUGUI EnergyValue;
    public TextMeshProUGUI VillageHP;
    public TextMeshProUGUI DamageValue;
    public TextMeshProUGUI BarrierValue;
    public TextMeshProUGUI TreeMultValue;

    public int PlayerActNum;

    void Start()
    {
        InitUI();
    }

    public override void OnRoomPropertiesUpdate(Hashtable Changed)
    {
        string attachedKey = $"_{PlayerActNum}";

        bool hasEnergy = Changed.TryGetValue(PlayerPropKeys.Energy + attachedKey, out var eng);
        bool hasMaxEnergy = Changed.TryGetValue(PlayerPropKeys.MaxEnergy + attachedKey, out var Meng);

        if (hasEnergy || hasMaxEnergy)
        {
            int currentEnergy = hasEnergy ? (int)eng : PhotonPropertyHelper.GetPlayerProp<int>(PlayerActNum, PlayerPropKeys.Energy);
            int currentMaxEnergy = hasMaxEnergy ? (int)Meng : PhotonPropertyHelper.GetPlayerProp<int>(PlayerActNum, PlayerPropKeys.MaxEnergy);

            EnergyValue.text = $"{currentEnergy}/{currentMaxEnergy}";
        }

        bool hasBarrier = Changed.TryGetValue(PlayerPropKeys.VillageBarrier + attachedKey, out var bar);
        bool hasArmor = Changed.TryGetValue(PlayerPropKeys.BarrierArmor + attachedKey, out var amo);

        if (hasBarrier || hasArmor)
        {
            float currentBarrier = hasBarrier ? (float)bar : PhotonPropertyHelper.GetPlayerProp<float>(PlayerActNum, PlayerPropKeys.VillageBarrier);
            float currentArmor = hasArmor ? (float)amo : PhotonPropertyHelper.GetPlayerProp<float>(PlayerActNum, PlayerPropKeys.BarrierArmor);

            BarrierValue.text = $"{currentBarrier + currentArmor}";
        }


        if (Changed.TryGetValue(PlayerPropKeys.TotalDamage + attachedKey, out var dmg))
        {
            DamageValue.text = $"{dmg}";
        }

        if (Changed.TryGetValue(PlayerPropKeys.VillageHP + attachedKey, out var hp))
        {
            VillageHP.text = $"{hp}";
        }

        if (Changed.TryGetValue(PlayerPropKeys.TreeAtkMulti + attachedKey, out var mult))
        {
            TreeMultValue.text = $"{mult}";
        }
    }

    public void InitUI()
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        string attachedKey = $"_{PlayerActNum}";

        int curEng = GetValue<int>(props, PlayerPropKeys.Energy + attachedKey);
        int curMaxEng = GetValue<int>(props, PlayerPropKeys.MaxEnergy + attachedKey);
        float curBarrier = GetValue<float>(props, PlayerPropKeys.VillageBarrier + attachedKey);
        float curArmor = GetValue<float>(props, PlayerPropKeys.BarrierArmor + attachedKey);
        float totalDmg = GetValue<float>(props, PlayerPropKeys.TotalDamage + attachedKey);
        float vilHp = GetValue<float>(props, PlayerPropKeys.VillageHP + attachedKey);
        float mulit = GetValue<float>(props, PlayerPropKeys.TreeAtkMulti + attachedKey);

        EnergyValue.text = $"{curEng}/{curMaxEng}";
        BarrierValue.text = $"{curBarrier + curArmor}";
        DamageValue.text = $"{totalDmg}";
        VillageHP.text = $"{vilHp}";
        TreeMultValue.text = $"{mulit}";
    }

    private T GetValue<T>(ExitGames.Client.Photon.Hashtable prop, string key)
    {
        if (prop.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        return default(T);
    }
}
