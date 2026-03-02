using UnityEngine;
using Cysharp.Threading.Tasks;

//각 AI 모듈이 상속받을 기본 클래스
public abstract class AILogicModule : MonoBehaviour
{
    protected AIBrain brain;

    public virtual void Initialize(AIBrain brain)
    {
        this.brain = brain;
    }
}

//AI 중심 컨트롤러
public class AIBrain : MonoBehaviour
{
    //AI 고유 번호
    public int MyActorNum { get; set; }

    //아이템 선택 모듈
    public AIItemSelector ItemSelector { get; private set; }
    //아이템 사용 모듈
    public AIInventoryManager InventoryManager { get; private set; }
    //마을 업그레이드 모듈
    public AIVillageUpgrader VillageUpgrader { get; private set; }
    //나무 타격 데미지 결정 모듈
    public AITreeAttacker TreeAttacker { get; private set; }

    //각 모듈 찾아서 연결 후 초기화
    public void InitializeBrain(int actorNum)
    {
        MyActorNum = actorNum;

        ItemSelector = GetComponent<AIItemSelector>();
        ItemSelector?.Initialize(this);

        InventoryManager = GetComponent<AIInventoryManager>();
        InventoryManager?.Initialize(this);

        VillageUpgrader = GetComponent<AIVillageUpgrader>();
        VillageUpgrader?.Initialize(this);

        TreeAttacker = GetComponent<AITreeAttacker>();
        TreeAttacker?.Initialize(this);

    }
}
