using System.Collections.Generic;
using System.Diagnostics;
using Photon.Realtime;

public static class PlayerObjectRegistry
{
    private static readonly Dictionary<int, PlayerController> players = new();
    private static int aiID;
    private static AIController aiController;

    public static void Register(PlayerController pc)
    {
        if (pc == null || pc.photonView == null || pc.photonView.Owner == null) return;

        players[pc.photonView.OwnerActorNr] = pc;
    }

    public static void Register(AIController ac)
    {
        if (ac == null) return;

        aiID = ac.PlayerActNum;
        aiController = ac;
    }

    public static void Unregister(PlayerController pc)
    {
        if (pc == null || pc.photonView == null) return;

        int actorNumber = pc.photonView.OwnerActorNr;

        if (players.TryGetValue(actorNumber, out PlayerController registered) && registered == pc)
        {
            players.Remove(actorNumber);
        }
    }

    public static void Unregister(AIController ac)
    {
        if (ac == null) return;
        if (ac.PlayerActNum != aiID) return;

        aiID = -1;
        aiController = null;
    }

    public static PlayerController Get(int actorNum)
    {
        players.TryGetValue(actorNum, out PlayerController pc);
        return pc;
    }

    public static AIController Get_AI(int aiNum)
    {
        if (aiNum != aiID) return null;
        return aiController;
    }

    public static bool TryGet(int actorNum, out PlayerController pc)
    {
        return players.TryGetValue(actorNum, out pc);
    }

    public static bool TryGet(int aiNum, out AIController ac)
    {
        ac = null;

        if (aiNum != aiID) return false;

        ac = aiController;
        return true;
    }

    public static void Clear()
    {
        players.Clear();
    }
}
