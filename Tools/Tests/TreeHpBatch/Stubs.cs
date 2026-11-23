// Infrastructure only: production effect, trigger dispatch and damage resolver are linked.
namespace UnityEngine
{
    public class ScriptableObject { }
    public class Sprite { }
    public class CreateAssetMenuAttribute : Attribute
    {
        public string fileName;
        public string menuName;
    }
    public static class Application { public static bool isPlaying; }
    public static class Mathf
    {
        public static int RoundToInt(float value) => (int)MathF.Round(value);
        public static float Clamp01(float value) => Math.Clamp(value, 0, 1);
        public static int Max(int a, int b) => Math.Max(a, b);
    }
}
namespace Photon.Pun
{
    public static class PhotonNetwork { public static bool IsMasterClient = true; }
}
namespace Photon.Realtime { }
namespace ExitGames.Client.Photon { }

public class AtlasManager
{
    public static AtlasManager instance;
    public UnityEngine.Sprite GetItemSprite(string id) => null;
}
public class StatusSystem
{
    public readonly List<StatusInstance> Statuses = new();
    public IEnumerable<StatusInstance> ALL => Statuses;
}
public class GameEvent
{
    public TriggerMask type;
    public int actorNum;
    public object payload;
}
public static class PhotonPropertyHelper
{
    // Writes deliberately do NOT update the read cache until the test delivers them.
    public static float TreeHp;
    public static int Reads;
    public static readonly List<float> Writes = new();
    public static T GetRoomProp<T>(string key)
    {
        if (key == RoomPropKeys.TreeHP) { Reads++; return (T)(object)TreeHp; }
        return default;
    }
    public static void SetRoomProp(string key, object value)
    {
        if (key == RoomPropKeys.TreeHP) Writes.Add((float)value);
    }
    public static T GetPlayerProp<T>(int actor, string key) => default;
    public static void SetPlayerProp(int actor, string key, object value) { }
    public static void Reset(float hp)
    {
        TreeHp = hp;
        Reads = 0;
        Writes.Clear();
        Photon.Pun.PhotonNetwork.IsMasterClient = true;
    }
}
public static class RoomPropKeys { public const string TreeHP = "TreeHP"; }
public static class PlayerPropKeys
{
    public const string VillageHP = "VillageHP", VillageBarrier = "VillageBarrier",
        Energy = "Energy", MaxEnergy = "MaxEnergy", BarrierConversionRate = "BarrierConversionRate", CarryOverEnergy = "CarryOverEnergy";
}
public static class ItemPropKeys
{
    public static string LOCKPICK(int actor) => $"LOCKPICK_{actor}";
    public static string LOCKCNT(int actor) => $"LOCKCNT_{actor}";
    public static string HIDEDMG(int actor) => $"HIDEDMG_{actor}";
}
public class SimGameState
{
    public float treeHP, p1VillHP, p2VillHP, p1VillBarrier, p2VillBarrier, p1VillBarConRate, p2VillBarConRate;
    public int p1Energy, p2Energy, p1LockpickCnt, p2LockpickCnt, p1LockCnt, p2LockCnt;
    public void SetPlayerDmgMultRate(int actor, float value) { }
    public void SetPlayerBarConRate(int actor, float value) { }
}
