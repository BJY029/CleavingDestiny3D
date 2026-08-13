//플레이어 정보 저장 객체
using Photon.Realtime;

public class RuntimePlayerInfo
{
    public int actorNumber;
    public string playerName;
    public int turnIdx;
    public bool isMyTurn = false;
    public bool isAI;   // AI 여부

    public RuntimePlayerInfo(Player info, int turnIndex)
    {
        actorNumber = info.ActorNumber;
        playerName = info.NickName;
        if (string.IsNullOrEmpty(playerName))
        {
            playerName = $"Player_{actorNumber}";
        }
        turnIdx = turnIndex;
        isAI = false;
    }

    public RuntimePlayerInfo(int aiNumber, int turnIndex)
    {
        actorNumber = aiNumber;
        playerName = $"AI_Player_{aiNumber}";
        turnIdx = turnIndex;
        isAI = true;
    }
}