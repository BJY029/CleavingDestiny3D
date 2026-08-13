using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
public class BGMPhotonWatcher : MonoBehaviourPunCallbacks
{
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (!propertiesThatChanged.ContainsKey(RoomPropKeys.TreeHP))
            return;

        if (BgmStateController.Instance == null)
            return;

        BgmStateController.Instance.RefreshMainBgm();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == null || !targetPlayer.IsLocal)
            return;

        if (!changedProps.ContainsKey(PlayerPropKeys.VillageHP))
            return;

        if (BgmStateController.Instance == null)
            return;

        BgmStateController.Instance.RefreshMainBgm();
    }
}
