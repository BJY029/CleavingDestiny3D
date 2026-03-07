using UnityEngine;
using Photon.Pun;
using Cysharp.Threading.Tasks;

public struct AIContext
{
    public int curEnergy;
    public int curMaxEnergy;

    public float curTreeHP;
    public float curTreeToxicDmg;
    public float curVillageHP;
    public float curOppVillageHp;
    public float curVillageBarrier;

    public int curInvCap;
    public string curInvStr;
    public string curOppInvStr;

    public int maxWaveCnt;
    public int curWaveCnt;
}

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

    public AIContext GetCurAIStat(int aiNum)
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        AIContext context = new AIContext();

        context.curTreeHP = GetValue<float>(props, RoomPropKeys.TreeHP);
        context.curInvCap = GetValue<int>(props, ItemPropKeys.INV_CAPACITY(aiNum));
        context.curInvStr = GetValue<string>(props, ItemPropKeys.INV(aiNum));
        context.curOppInvStr = GetValue<string>(props, ItemPropKeys.INV(PhotonNetwork.LocalPlayer.ActorNumber));
        context.maxWaveCnt = GetValue<int>(props, RoomPropKeys.MaxWaveCnt);
        context.curWaveCnt = GetValue<int>(props, RoomPropKeys.CurrentWave);
        context.curTreeToxicDmg = GetValue<float>(props, RoomPropKeys.TreeAtkPow);

        string attachedKey = $"_{aiNum}";
        context.curVillageHP = GetValue<float>(props, PlayerPropKeys.VillageHP + attachedKey);
        context.curVillageBarrier = GetValue<float>(props, PlayerPropKeys.VillageBarrier + attachedKey);
        context.curEnergy = GetValue<int>(props, PlayerPropKeys.Energy + attachedKey);
        context.curMaxEnergy = GetValue<int>(props, PlayerPropKeys.MaxEnergy + attachedKey);

        context.curOppVillageHp = PhotonPropertyHelper.GetPlayerProp<float>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageHP);
        return context;
    }

    private T GetValue<T>(ExitGames.Client.Photon.Hashtable prop, string key)
    {
        if (prop.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        return default(T);
    }
}
