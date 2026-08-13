using Potan.CoreUtils;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BgmStateController : MonoSingleton<BgmStateController>
{
    [Header("Main BGM ID")]
    [SerializeField] private string M_defaultBgmId = "Game_Default_BGM";
    [SerializeField] private string M_nightmareBgmId = "Game_Nightmare_BGM";

    [Header("Village BGM ID")]
    [SerializeField] private string V_peacefulBgmId = "Village_Peaceful_BGM";
    [SerializeField] private string V_adversityBgmId = "Village_Adversity_BGM";
    [SerializeField] private string V_nightmareBgmId = "Village_Nightmare_BGM";

    [Header("Main Trigger Threshold")]
    [SerializeField, Range(0f, 1f)] private float villageHpDangerRatio = 0.3f;
    [SerializeField, Range(0f, 1f)] private float treeHpDangerRatio = 0.2f;

    [Header("Village Trigger Threshold")]
    [SerializeField, Range(0f, 1f)] private float villageHpAdversityRatio = 0.7f;
    [SerializeField, Range(0f, 1f)] private float villageHpNightmareRatio = 0.3f;

    [Header("Fade")]
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private float fadeInDuration = 2f;

    private bool _isInVillage;

    public void EnterVillage()
    {
        _isInVillage = true;

        string targetBgmId = GetVillageBgmId();

        AudioManager.Instance.PlayTemporaryBgm(targetBgmId, fadeOutDuration, fadeInDuration);
    }

    public void ExitVillage()
    {
        if (GameStarter.instance == null) return;
        if (GameStarter.instance.CurrentPhase != GameStartPhase.MainGame) return;

        _isInVillage = false;

        string targetBgmId = GetMainBgmId();

        if (AudioManager.Instance.PreviousBgmId == targetBgmId)
        {
            AudioManager.Instance.RestorePreviousBgm(fadeOutDuration, fadeInDuration);
            return;
        }

        AudioManager.Instance.ChangeBgm(targetBgmId, fadeOutDuration, fadeInDuration);
    }

    public void RefreshMainBgm()
    {
        if (_isInVillage) return;
        if (GameStarter.instance == null) return;
        if (GameStarter.instance.CurrentPhase != GameStartPhase.MainGame) return;
        if (!IsHpDataReady()) return;

        string targetBgmId = GetMainBgmId();

        if (AudioManager.Instance.CurrentBgmId == targetBgmId) return;
        AudioManager.Instance.ChangeBgm(targetBgmId, fadeOutDuration, fadeInDuration);
    }


    private string GetMainBgmId()
    {
        float villageHpRatio = GetVillageHpRatio();
        float treeHPRatio = GetTreeHpRatio();

        bool isDanger = villageHpRatio <= villageHpDangerRatio || treeHPRatio <= treeHpDangerRatio;
        return isDanger ? M_nightmareBgmId : M_defaultBgmId;
    }

    private string GetVillageBgmId()
    {
        float villageHpRatio = GetVillageHpRatio();

        if (villageHpRatio > villageHpAdversityRatio) return V_peacefulBgmId;
        else if (villageHpRatio > villageHpNightmareRatio) return V_adversityBgmId;
        else return V_nightmareBgmId;
    }

    private float GetVillageHpRatio()
    {
        int localPlayer = PhotonNetwork.LocalPlayer.ActorNumber;
        float curVillageHP = PhotonPropertyHelper.GetPlayerProp<float>(localPlayer, PlayerPropKeys.VillageHP);
        float defaultVillageHP = GameManager.Instance.playerDefaultSetting.villageHP;

        if (defaultVillageHP != 0) return curVillageHP / defaultVillageHP;
        return 0f;
    }

    private float GetTreeHpRatio()
    {
        float curTreeHP = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeHP);
        float defaultTreeHP = GameManager.Instance.roomDefaultSetting.treeHP;

        if (defaultTreeHP != 0) return curTreeHP / defaultTreeHP;
        return 0f;
    }

    private bool IsHpDataReady()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
            return false;

        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(PlayerPropKeys.VillageHP))
            return false;

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(RoomPropKeys.TreeHP))
            return false;

        if (GameManager.Instance == null)
            return false;

        if (GameManager.Instance.playerDefaultSetting == null)
            return false;

        if (GameManager.Instance.roomDefaultSetting == null)
            return false;

        return true;
    }
}
