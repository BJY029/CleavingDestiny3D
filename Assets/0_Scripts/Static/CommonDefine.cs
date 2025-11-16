using UnityEngine;

public class CommonDefine
{
    //Scene name
    public const string LOBBYSCENE = "LobbyScene";
    public const string GAMESCENE = "GameScene";
}

public enum PLAYER{
   P1, P2, P3, P4, NONE,
}

public class PlayerInfo
{
    public int playerNum;
    public string playerName;
    public GameObject playerObject;
    public PLAYER playerSeq;
}

public class RuntimePlayer
{
    public int actorNumber;
    public string playerName;
    public int turnIdx;
    public bool isMyTurn;
}