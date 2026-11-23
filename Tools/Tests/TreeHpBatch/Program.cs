int passed = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
    passed++;
}
StatusInstance Termite(int owner, int damage) => new()
{
    ownerActorNum = owner,
    remainingTurns = 10,
    spec = new StatusSpec
    {
        statusId = "DMG_TERMITE", triggers = TriggerMask.OnTurnStart,
        triggerScope = TriggerScope.AnyActor, randMin = damage, randMax = damage
    }
};
var statuses = new StatusSystem();
var resolver = new DamageResolver(new GameEventBus(statuses), statuses);
var logs = new List<string>();
var ctx = new EffectContext(new DeterministicRng(42), logs.Add);

PhotonPropertyHelper.Reset(8364);
resolver.ResolveWhenStartTurn(ctx, 1);
Check(PhotonPropertyHelper.Writes.Count == 0, "no statuses: no HP transmission");

statuses.Statuses.Add(Termite(1, 102));
PhotonPropertyHelper.Reset(8364);
resolver.ResolveWhenStartTurn(ctx, 1);
Check(PhotonPropertyHelper.Writes.SequenceEqual(new[] { 8262f }), "single termite: unchanged damage, one write");

statuses.Statuses.Add(Termite(2, 66));
PhotonPropertyHelper.Reset(8364);
logs.Clear();
resolver.ResolveWhenStartTurn(ctx, 2);
Check(PhotonPropertyHelper.Writes.SequenceEqual(new[] { 8196f }), "two owners: both damages included in one write");
Check(PhotonPropertyHelper.TreeHp == 8364 && PhotonPropertyHelper.Reads == 1,
    "deferred Photon read cache remains unchanged; HP snapshot read once");
Check(logs[0].Contains("TreeHP = 8262") && logs[1].Contains("TreeHP = 8196"), "logs use accumulated HP");

// Simulate the server delivering the prior result, then advance the other actor's turn.
PhotonPropertyHelper.Reset(8196);
resolver.ResolveWhenStartTurn(ctx, 1);
Check(PhotonPropertyHelper.Writes.SequenceEqual(new[] { 8028f }), "next turn uses latest server HP");

statuses.Statuses.Add(Termite(1, 20));
PhotonPropertyHelper.Reset(8364);
resolver.ResolveWhenStartTurn(ctx, 1);
Check(PhotonPropertyHelper.Writes.SequenceEqual(new[] { 8176f }), "three statuses all accumulate");

PhotonPropertyHelper.Reset(100);
resolver.ResolveWhenStartTurn(ctx, 2);
Check(PhotonPropertyHelper.Writes.SequenceEqual(new[] { -88f }), "lethal damage preserves existing HP policy, one final update");

// Throw after the first status has staged HP, as a failing effect/log callback would.
var failingCtx = new EffectContext(new DeterministicRng(42), _ => throw new InvalidOperationException("effect failure"));
PhotonPropertyHelper.Reset(8364);
bool propagated = false;
try { resolver.ResolveWhenStartTurn(failingCtx, 1); }
catch (InvalidOperationException ex) { propagated = ex.Message == "effect failure"; }
Check(propagated && PhotonPropertyHelper.Writes.Count == 0, "failure propagates without publishing partial HP");
PhotonPropertyHelper.TreeHp = 9000;
Check(failingCtx.GetTreeHP() == 9000, "failed batch discards temporary HP");
failingCtx.SetTreeHP(9200);
Check(PhotonPropertyHelper.Writes.SequenceEqual(new[] { 9200f }), "immediate heal outside batch still transmits immediately");

PhotonPropertyHelper.Reset(1000);
Photon.Pun.PhotonNetwork.IsMasterClient = false;
resolver.ResolveWhenStartTurn(ctx, 1);
ctx.SetTreeHP(2000);
Check(PhotonPropertyHelper.Writes.Count == 0, "non-master cannot apply or transmit tree damage");

PhotonPropertyHelper.Reset(1000);
var sim = new SimGameState { treeHP = 600 };
ctx.SetTreeHP(ctx.GetTreeHP(sim) + 200, sim);
Check(sim.treeHP == 800 && PhotonPropertyHelper.Writes.Count == 0 && ctx.GetTreeHP() == 1000,
    "simulation HP is separate and never transmits");

// Reuse the failed context successfully: cancellation must leave no active batch behind.
statuses.Statuses.Clear();
resolver.ResolveWhenStartTurn(failingCtx, 1);
Check(PhotonPropertyHelper.Writes.Count == 0, "context can start another batch after failure");
// Multiplier previews must preserve status lifetimes and skip conversion triggers.
var previewStatuses = new StatusSystem();
var previewResolver = new DamageResolver(new GameEventBus(previewStatuses), previewStatuses);
StatusInstance AttackStatus(string id, float multiplier, TagMask tags = TagMask.None) => new()
{
    ownerActorNum = 1,
    remainingTurns = 2,
    spec = new StatusSpec
    {
        statusId = id, triggers = TriggerMask.OnBeforeAttack,
        triggerScope = TriggerScope.OwnerOnly, multiplier = multiplier,
        tags = tags, consumeOnTrigger = true
    }
};
float PreviewMultiplier() => previewResolver.PreviewAttackMultiplier(
    new DamagePacket { attackerNum = 1, isBasicAttack = true }, ctx);
var sharpen = AttackStatus("DMG_SHARPEN", 1.5f);
var taunt = AttackStatus("GIM_TAUNT", 1.3f, TagMask.Negative);
var curse = new StatusInstance
{
    ownerActorNum = 1, remainingTurns = 2,
    spec = new StatusSpec
    {
        statusId = "GIM_CURSE", triggers = TriggerMask.OnDamageConvert,
        triggerScope = TriggerScope.OwnerOnly, convertRate = -0.15f, tags = TagMask.Negative
    }
};
previewStatuses.Statuses.AddRange(new[] { sharpen, taunt, curse });
for (int i = 0; i < 3; i++)
    Check(Math.Abs(PreviewMultiplier() - 1.95f) < 0.0001f, "preview includes active attack modifiers " + i);
Check(taunt.remainingTurns == 2 && curse.remainingTurns == 2,
    "repeated UI previews consume neither taunt nor curse");
previewStatuses.Statuses.Remove(taunt);
Check(Math.Abs(PreviewMultiplier() - 1.5f) < 0.0001f,
    "after taunt removal preview preserves sharpen");
var actualAttack = new DamagePacket { attackerNum = 1, isBasicAttack = true, baseDamage = 100 };
previewResolver.Resolve(actualAttack, ctx);
Check(actualAttack.finalDamage == 150, "actual attack matches remaining multiplier");
Check(curse.remainingTurns == 0, "real attack still triggers curse normally");
previewStatuses.Statuses.Clear();
Check(PreviewMultiplier() == 1f, "no active modifiers resets preview to one");
previewStatuses.Statuses.Add(AttackStatus("DMG_GREASED", 0f));
Check(PreviewMultiplier() == 0f, "zero damage multiplier remains zero in preview");
Console.WriteLine($"{passed} regression checks passed.");
