using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;

//���� �� ���� Photon�� Customproperty�� �����ϰ� ������ �� �ְ� ���ִ� �ڵ�
public class PhotonPropertyHelper
{
    private static T ConvertValue<T>(object value, T defaultValue = default)
    {
        if (value == null) return defaultValue;
        if (value is T typedValue) return typedValue;

        try
        {
            Type targetType = typeof(T);

            if (targetType == typeof(int))
            {
                // float→int 변환 시 반올림 적용
                if (value is float floatValue)
                    return (T)(object)UnityEngine.Mathf.RoundToInt(floatValue);
                return (T)(object)Convert.ToInt32(value);
            }
            if (targetType == typeof(float))
                return (T)(object)Convert.ToSingle(value);
            if (targetType == typeof(double))
                return (T)(object)Convert.ToDouble(value);
            if (targetType == typeof(long))
                return (T)(object)Convert.ToInt64(value);
            if (targetType == typeof(bool))
                return (T)(object)Convert.ToBoolean(value);

            if (targetType.IsEnum)
                return (T)Enum.ToObject(targetType, Convert.ToInt32(value));

            return (T)Convert.ChangeType(value, targetType);
        }
        catch
        {
            return defaultValue;
        }
    }

    // Hashtable에서 안전하게 값을 추출하는 공개 메서드
    public static T GetHashtableValue<T>(Hashtable props, string key, T defaultValue = default)
    {
        if (props == null || !props.TryGetValue(key, out object value))
            return defaultValue;
        return ConvertValue(value, defaultValue);
    }

    //�� CustomProperty ����
    public static void SetRoomProp(string key, object value)
    {
        var ht = new Hashtable { { key, value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
    }

    //���� CustomProperty�� Key�� ���� ���� �� ��������
    public static T GetRoomProp<T>(string key, T defaultValue = default)
    {
        var room = PhotonNetwork.CurrentRoom;
        if (room == null || room.CustomProperties == null) return defaultValue;
        if (!room.CustomProperties.ContainsKey(key)) return defaultValue;

        return ConvertValue(room.CustomProperties[key], defaultValue);
    }

    //�÷��̾� CustomProperty ����
    // public static void SetPlayerProp(Player player, string key, object value)
    // {
    //     var ht = new Hashtable { { key, value } };
    //     player.SetCustomProperties(ht);
    // }

    //actor ��ȣ�� ������� �÷��̾� ������Ƽ ����(ai �����)
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

    // //�÷��̾��� CustomProperty�� Key�� ���� ���� �� ��������
    // public static T GetPlayerProp<T>(Player player, string key, T defaultValue = default)
    // {
    //     var props = player.CustomProperties;
    //     if (props == null || !props.ContainsKey(key)) return defaultValue;
    //     return (T)props[key];
    // }

    //actor ��ȣ�� ������� �÷��̾� ������Ƽ ��������(ai �����)
    public static T GetPlayerProp<T>(int actorNumber, string key, T defaultValue = default)
    {
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        if (player != null)
        {
            var props = player.CustomProperties;
            if (props == null || !props.ContainsKey(key)) return defaultValue;
            return ConvertValue(props[key], defaultValue);
        }
        else
        {
            if (!GameManager.Instance.isSoloPlay) return defaultValue;
            string aiKey = $"{key}_{actorNumber}";
            var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (roomProps != null && roomProps.ContainsKey(aiKey))
                return ConvertValue(roomProps[aiKey], defaultValue);
        }
        return defaultValue;
    }
}
