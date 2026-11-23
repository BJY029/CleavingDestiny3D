using System;

[Serializable]
public struct AttackCommand
{
    public int attackerNum;
    public int baseDamage;
    public bool isBasicAttack;
    public int clientNonce;
    public int turnIndex;
    public bool automatic;
}

/// <summary>Master-owned, deterministic arbitration between a strike and turn expiry.</summary>
public sealed class TurnAttackState
{
    public int TurnIndex { get; private set; } = -1;
    public int Actor { get; private set; }
    public bool Expired { get; private set; }
    public bool Committed { get; private set; }
    public bool Applied { get; private set; }
    public AttackCommand Command { get; private set; }
    public double CommittedAt { get; private set; }

    public void Begin(int turnIndex, int actor)
    {
        if (TurnIndex == turnIndex && Actor == actor) return;
        TurnIndex = turnIndex;
        Actor = actor;
        Expired = Committed = Applied = false;
        Command = default;
    }

    public bool TryExpire()
    {
        if (Committed || Expired) return false;
        Expired = true;
        return true;
    }

    public bool Matches(AttackCommand command) => Committed && command.turnIndex == TurnIndex
        && command.attackerNum == Actor && command.clientNonce == Command.clientNonce;

    public bool TryCommit(AttackCommand command, double now, double deadline, bool timerActive)
    {
        if (command.turnIndex != TurnIndex || command.attackerNum != Actor || command.baseDamage < 0) return false;
        if (Committed) return Matches(command);
        if (command.automatic ? !Expired : (Expired || !timerActive || now >= deadline)) return false;
        Committed = true;
        Command = command;
        CommittedAt = now;
        return true;
    }

    public bool TryApply(AttackCommand command)
    {
        if (!Matches(command) || Applied) return false;
        Applied = true; // Claim before running effects, which can invoke synchronous Photon callbacks.
        return true;
    }

    public bool NeedsFallback(double now) => Committed && !Applied && now - CommittedAt >= 3d;
}
