using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

//보다 더 쉽게 Photon의 Customproperty를 설정하고 가져올 수 있게 해주는 코드
public class PhotonPropertyHelper
{
    //방 CustomProperty 설정
    public static void SetRoomProp(string key, object value)
    {
        var ht = new Hashtable { { key, value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
    }

    //방의 CustomProperty를 Key를 통해 접근 후 가져오기
    public static T GetRoomProp<T>(string key, T defaultValue = default)
    {
        var room = PhotonNetwork.CurrentRoom;
        if (room == null || room.CustomProperties == null) return defaultValue;
        if (!room.CustomProperties.ContainsKey(key)) return defaultValue;

        return (T)room.CustomProperties[key];
    }

    //플레이어 CustomProperty 설정
    // public static void SetPlayerProp(Player player, string key, object value)
    // {
    //     var ht = new Hashtable { { key, value } };
    //     player.SetCustomProperties(ht);
    // }

    //actor 번호를 기반으로 플레이어 프로퍼티 세팅(ai 겸용사용)
    public static void SetPlayerProp(int actorNumber, string key, object value)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

        if (player != null)
        {
            var ht = new Hashtable { { key, value } };
            player.SetCustomProperties(ht);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient && GameManager.Instance.isSoloPlay)
            {
                string aiKey = $"{key}_{actorNumber}";
                var ht = new Hashtable { { aiKey, value } };
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            }
        }
    }

    // //플레이어의 CustomProperty를 Key를 통해 접근 후 가져오기
    // public static T GetPlayerProp<T>(Player player, string key, T defaultValue = default)
    // {
    //     var props = player.CustomProperties;
    //     if (props == null || !props.ContainsKey(key)) return defaultValue;
    //     return (T)props[key];
    // }

    //actor 번호를 기반으로 플레이어 프로퍼티 가져오기(ai 겸용사용)
    public static T GetPlayerProp<T>(int actorNumber, string key, T defaultValue = default)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        if (player != null)
        {
            var props = player.CustomProperties;
            if (props == null || !props.ContainsKey(key)) return defaultValue;
            return (T)props[key];
        }
        else
        {
            if (!GameManager.Instance.isSoloPlay) return defaultValue;
            string aiKey = $"{key}_{actorNumber}";
            var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (roomProps != null && roomProps.ContainsKey(aiKey))
                return (T)roomProps[aiKey];
        }
        return default;
    }
}
