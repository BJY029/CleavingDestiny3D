using System.Text;
using Photon.Pun;
using Potan.CoreUtils;
using TMPro;
using UnityEngine;

public enum BattleLogType
{
    Hit,
    Hit_Barrier,
    Item,
    Item_Energy,
    Day_End,
    Day_Start,
    Village_Attack,
    Village_Defense,
    Village_LossHp,
    Village_Upgrade,
    Village_Shop_Purchase,
    Minigame_Start,
    Minigame_Win,
    Minigame_Lose
}

public class BattleLogController : MonoSceneSingleton<BattleLogController>
{
    private const int MaxLogCharacters = 32768;
    private const int TrimmedLogCharacters = 24576;

    private static readonly string[] LocalizationKeys =
    {
        "UI_Log_Hit",
        "UI_Log_Hit_Barrier",
        "UI_Log_Item",
        "UI_Log_Item_Energy",
        "UI_Log_Day_End",
        "UI_Log_Day_Start",
        "UI_Log_Village_Attack",
        "UI_Log_Village_Defend",
        "UI_Log_Village_LossHp",
        "UI_Log_Upgrade",
        "UI_Log_Shop_Purchase",
        "UI_Log_Minigame_Start",
        "UI_Log_Minigame_Win",
        "UI_Log_Minigame_Lose"
    };

    public TextMeshProUGUI battleLogText;

    private readonly StringBuilder battleLogBuilder = new(4096);
    private bool textDirty;

    protected override void OnAwake()
    {
        Clear();
        if (PhotonNetwork.InRoom &&
            PhotonPropertyHelper.GetRoomProp<GamePhaseValue>(RoomPropKeys.GamePhase) == GamePhaseValue.DAY)
            Append(BattleLogType.Day_Start);
    }

    private void LateUpdate()
    {
        if (!textDirty || battleLogText == null) return;
        battleLogText.SetText(battleLogBuilder);
        textDirty = false;
    }

    public static void AddLog(BattleLogType type) => Instance.Append(type);
    public static void AddLog(BattleLogType type, string arg0) => Instance.Append(type, arg0);
    public static void AddLog(BattleLogType type, float arg0) => Instance.Append(type, arg0);
    public static void AddLog(BattleLogType type, string arg0, string arg1) => Instance.Append(type, arg0, arg1);
    public static void AddLog(BattleLogType type, string arg0, float arg1) => Instance.Append(type, arg0, arg1);
    public static void AddHitLog(string playerName, float damage, float barrierDamage = 0f)
    {
        BattleLogController controller = Instance;
        controller.Append(BattleLogType.Hit, playerName, damage);
        if (barrierDamage > 0f) controller.Append(BattleLogType.Hit_Barrier, barrierDamage, true);
    }

    public static void AddHitBarrierLog(float barrierDamage) =>
        Instance.Append(BattleLogType.Hit_Barrier, barrierDamage, true);

    public static void AddItemLog(string playerName, string itemName, float energyCost = -1f, float remainingEnergy = 0f)
    {
        BattleLogController controller = Instance;
        controller.Append(BattleLogType.Item, playerName, itemName);
        if (energyCost >= 0f) controller.Append(BattleLogType.Item_Energy, energyCost, remainingEnergy, true);
    }

    public static void AddVillageAttackLog(float damage, float blockedDamage, float lostHp)
    {
        BattleLogController controller = Instance;
        controller.Append(BattleLogType.Village_Attack, damage);
        if (blockedDamage > 0f) controller.Append(BattleLogType.Village_Defense, blockedDamage, true);
        if (lostHp > 0f) controller.Append(BattleLogType.Village_LossHp, lostHp, true);
    }

    public static void ClearLog() => Instance.Clear();

    private void Clear()
    {
        battleLogBuilder.Clear();
        textDirty = false;
        battleLogText.SetText(string.Empty);
    }

    private void Append(BattleLogType type)
    {
        BeginLog();
        battleLogBuilder.Append(GetTemplate(type));
        EndLog();
    }

    private void Append(BattleLogType type, string arg0)
    {
        BeginLog();
        AppendOne(GetTemplate(type), arg0);
        EndLog();
    }

    private void Append(BattleLogType type, float arg0, bool isDetail = false)
    {
        BeginLog(isDetail);
        AppendOne(GetTemplate(type), arg0);
        EndLog();
    }

    private void Append(BattleLogType type, string arg0, string arg1)
    {
        BeginLog();
        AppendTwo(GetTemplate(type), arg0, arg1);
        EndLog();
    }

    private void Append(BattleLogType type, string arg0, float arg1)
    {
        BeginLog();
        AppendTwo(GetTemplate(type), arg0, arg1);
        EndLog();
    }

    private void Append(BattleLogType type, float arg0, float arg1, bool isDetail = false)
    {
        BeginLog(isDetail);
        AppendTwo(GetTemplate(type), arg0, arg1);
        EndLog();
    }

    private string GetTemplate(BattleLogType type) =>
        LocalizationManager.Instance.GetText(CSV_Type.UI, LocalizationKeys[(int)type]);

    private void BeginLog(bool isDetail = false)
    {
        if (battleLogBuilder.Length == 0) return;
        battleLogBuilder.AppendLine();
        if (!isDetail) battleLogBuilder.AppendLine();
    }

    private void EndLog()
    {
        TrimLogIfNeeded();
        textDirty = true;
    }

    private void AppendOne(string template, string arg0)
    {
        int marker = template.IndexOf("{0}", System.StringComparison.Ordinal);
        if (marker < 0) { battleLogBuilder.Append(template); return; }
        battleLogBuilder.Append(template, 0, marker).Append(arg0)
            .Append(template, marker + 3, template.Length - marker - 3);
    }

    private void AppendOne(string template, float arg0)
    {
        int marker = template.IndexOf("{0}", System.StringComparison.Ordinal);
        if (marker < 0) { battleLogBuilder.Append(template); return; }
        battleLogBuilder.Append(template, 0, marker).Append(arg0)
            .Append(template, marker + 3, template.Length - marker - 3);
    }

    private void AppendTwo(string template, string arg0, string arg1)
    {
        int first = template.IndexOf("{0}", System.StringComparison.Ordinal);
        int second = template.IndexOf("{1}", System.StringComparison.Ordinal);
        if (first < 0 || second < first) { battleLogBuilder.Append(template); return; }
        battleLogBuilder.Append(template, 0, first).Append(arg0)
            .Append(template, first + 3, second - first - 3).Append(arg1)
            .Append(template, second + 3, template.Length - second - 3);
    }

    private void AppendTwo(string template, string arg0, float arg1)
    {
        int first = template.IndexOf("{0}", System.StringComparison.Ordinal);
        int second = template.IndexOf("{1}", System.StringComparison.Ordinal);
        if (first < 0 || second < first) { battleLogBuilder.Append(template); return; }
        battleLogBuilder.Append(template, 0, first).Append(arg0)
            .Append(template, first + 3, second - first - 3).Append(arg1)
            .Append(template, second + 3, template.Length - second - 3);
    }

    private void AppendTwo(string template, float arg0, float arg1)
    {
        int first = template.IndexOf("{0}", System.StringComparison.Ordinal);
        int second = template.IndexOf("{1}", System.StringComparison.Ordinal);
        if (first < 0 || second < first) { battleLogBuilder.Append(template); return; }
        battleLogBuilder.Append(template, 0, first).Append(arg0)
            .Append(template, first + 3, second - first - 3).Append(arg1)
            .Append(template, second + 3, template.Length - second - 3);
    }

    private void TrimLogIfNeeded()
    {
        if (battleLogBuilder.Length <= MaxLogCharacters) return;

        int removeCount = battleLogBuilder.Length - TrimmedLogCharacters;
        while (removeCount < battleLogBuilder.Length && battleLogBuilder[removeCount] != '\n') removeCount++;
        battleLogBuilder.Remove(0, Mathf.Min(removeCount + 1, battleLogBuilder.Length));
    }
}
