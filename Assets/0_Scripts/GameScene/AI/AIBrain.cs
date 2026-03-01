using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class AILogicModule : MonoBehaviour
{
    protected AIBrain brain;

    public virtual void Initialize(AIBrain brain)
    {
        this.brain = brain;
    }
}

public class AIBrain : MonoBehaviour
{
    public int MyActorNum { get; set; }

    public AIItemSelector ItemSelector { get; private set; }
    public AIVillageUpgrader VillageUpgrader { get; private set; }
    public AITreeAttacker TreeAttacker { get; private set; }

    public void InitializeBrain(int actorNum)
    {
        MyActorNum = actorNum;

        ItemSelector = GetComponent<AIItemSelector>();
        ItemSelector?.Initialize(this);

        VillageUpgrader = GetComponent<AIVillageUpgrader>();
        VillageUpgrader?.Initialize(this);

        TreeAttacker = GetComponent<AITreeAttacker>();
        TreeAttacker?.Initialize(this);

    }
}
