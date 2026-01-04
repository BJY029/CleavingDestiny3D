using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

//보다 더 쉽게 Photon의 Customproperty를 설정하고 가져올 수 있게 해주는 코드
public class PhotonPropertyHelper
{
    //방 CustomProperty 설정
    public static void SetRoomProp(string key, object value)
    {
        var ht = new Hashtable { {key, value} };
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
    }

    //방의 CustomProperty를 Key를 통해 접근 후 가져오기
    public static T GetRoomProp<T>(string key, T defaultValue = default)
    {
        var room = PhotonNetwork.CurrentRoom;
        if(room == null || room.CustomProperties == null) return defaultValue;
        if(!room.CustomProperties.ContainsKey(key)) return defaultValue;

        return (T)room.CustomProperties[key];
    }

    //플레이어 CustomProperty 설정
    public static void SetPlayerProp(Player player, string key, object value)
    {
        var ht = new Hashtable { {  key, value} };
        player.SetCustomProperties(ht);
    }

    //플레이어의 CustomProperty를 Key를 통해 접근 후 가져오기
    public static T GetPlayerProp<T>(Player player, string key, T defaultValue = default)
    {
        var props = player.CustomProperties;
		if(props == null || !props.ContainsKey(key)) return defaultValue;
        return (T)props[key];
	}
}
