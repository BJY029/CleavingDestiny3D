// Pure production arbitration tests; no Unity, Photon, accounts, or saves.
int passed = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAIL: " + name);
    Console.WriteLine("PASS " + name);
    passed++;
}
AttackCommand Command(int turn = 10, int actor = 1, int nonce = 1, bool automatic = false) =>
    new() { turnIndex = turn, attackerNum = actor, clientNonce = nonce, baseDamage = 900, isBasicAttack = true, automatic = automatic };
foreach (double beforeDeadline in new[] { 0.5, 0.1, 0.001 })
{
    var state = new TurnAttackState(); state.Begin(10, 1);
    var command = Command();
    Check(state.TryCommit(command, 20 - beforeDeadline, 20, true), "accept before deadline " + beforeDeadline);
    Check(!state.TryExpire(), "committed strike excludes timeout");
    Check(state.TryApply(command), "accepted strike applies even after deadline");
    Check(!state.TryApply(command), "duplicate impact cannot apply twice");
    Check(!state.NeedsFallback(100), "applied strike excludes watchdog");
}
foreach (double now in new[] { 20d, 20.1d })
{
    var state = new TurnAttackState(); state.Begin(10, 1);
    Check(!state.TryCommit(Command(), now, 20, true), "reject at/after deadline " + now);
    Check(state.TryExpire() && !state.TryExpire(), "timeout claimed once");
    Check(!state.TryCommit(Command(), 19, 20, true), "expired turn excludes manual strike");
    var automatic = Command(automatic: true);
    Check(state.TryCommit(automatic, now + 2, 20, false), "timeout permits automatic strike after deadline");
    Check(state.TryApply(automatic) && !state.TryApply(automatic), "automatic strike applies once");
}
{
    var state = new TurnAttackState(); state.Begin(10, 1);
    Check(!state.TryCommit(Command(turn: 9), 19, 20, true), "stale turn rejected");
    Check(!state.TryCommit(Command(actor: 2), 19, 20, true), "wrong actor rejected");
    Check(!state.TryCommit(Command(automatic: true), 19, 20, true), "cannot forge timeout attack before expiry");
    Check(!state.TryCommit(Command(), 19, 20, false), "paused/inactive timer rejects manual commit");
    var command = Command();
    Check(state.TryCommit(command, 19, 20, true), "manual commit");
    Check(state.TryCommit(command, 22, 20, false), "same request commit is idempotent");
    Check(!state.TryCommit(Command(nonce: 2), 19, 20, true), "second request cannot replace commit");
    Check(!state.NeedsFallback(21.999) && state.NeedsFallback(22), "watchdog starts at three seconds");
    Check(!state.TryApply(Command(nonce: 2)), "wrong impact request rejected");
    state.Begin(10, 1);
    Check(state.Committed && state.Command.baseDamage == 900, "same turn synchronization preserves accepted damage");
    state.Begin(11, 2);
    Check(!state.Committed && !state.Expired && !state.Applied, "next turn resets state");
    Check(!state.TryApply(command), "late impact cannot affect next turn");
}
// Both event orders at the boundary must select exactly one winner.
for (int i = 0; i < 1000; i++)
{
    var state = new TurnAttackState(); state.Begin(i, 1);
    var manual = Command(turn: i);
    if ((i & 1) == 0)
    {
        if (!state.TryCommit(manual, 19.9, 20, true) || state.TryExpire()) throw new Exception("manual race");
    }
    else
    {
        if (!state.TryExpire() || state.TryCommit(manual, 20, 20, true)) throw new Exception("timeout race");
        manual.automatic = true;
        if (!state.TryCommit(manual, 22, 20, false)) throw new Exception("automatic race");
    }
    if (!state.TryApply(manual) || state.TryApply(manual)) throw new Exception("duplicate race");
}
Check(true, "1000 alternating boundary races select one damage application");
Console.WriteLine($"{passed} checks passed.");
