using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
public enum ActionPhase
{
    ItemSelect = 0,
    ItemUse = 1,
    TreeAttack = 2,
    NightUpgrade = 3,
};

public enum AIState
{
    Aggressive,
    Balanced,
    Cunning,
    Defensive,
    Default,
}

public class PromptBuilder
{
    //기본 프롬프트(규칙 설명 및 기본 지식 설정)
    public string BuildDynamicSystemPrompt(SimGameState state, int myPlayerNum, ActionPhase phase)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("[Rules]");
        sb.AppendLine("승리 조건: 상대방이 나무의 체력을 0 이하로 만드는 최종 타격 하거나, 상대방의 마을 체력이 0 이하가 되는 경우");
        sb.AppendLine("- 낮 페이즈에는 내 상황에 맞는 아이템을 선택하고 사용하여 상대방과의 심리전에서 우위를 가져간다.");
        sb.AppendLine("- 낮 페이즈에 나무에게 데미지를 넣으면 상대방에게 턴이 넘어가고, 해당 데미지들이 모여 일부가 마을의 방어력이 된다.");
        sb.AppendLine("- 밤 페이즈에는 나무의 독성 데미지가 뿜어져 나오며, 마을이 피해를 입는다. 피해를 입은 후 마을의 각 요소를 업그레이드 하여 정비한다.");
        sb.AppendLine("- 기력은 하루에 한번 일정량 주어지며, 아이템을 사용하기 위해 기력을 소모한다.");
        sb.AppendLine("- 아이템은 각각 희귀도가 존재하며, Common-Hero-Rare-Legendary 순으로 등급이 높아진다. 등급이 높을수록 강하고, 등작 확률이 적다.");
        sb.AppendLine();

        sb.AppendLine("[Init State]");
        sb.AppendLine("나무 최대 체력 : 30000");
        sb.AppendLine("마을 최대 체력 : 5000");
        sb.AppendLine();


        sb.AppendLine("[System]");
        sb.AppendLine("너는 턴제 생존 게임의 완벽한 AI 플레이어야.");
        sb.AppendLine($"{GetPromptBasedOnState(state, myPlayerNum)}");
        sb.AppendLine("목표1: 상대방이 나무의 체력을 0 이하로 만드는 최종 타격을 하도록 유도한다.");
        sb.AppendLine("목표2: 내 마을의 체력을 나무 독성 데미지로부터 방어한다.");


        string actionContext = "";
        switch (phase)
        {
            case ActionPhase.ItemSelect:
                actionContext = GetItemSelectPrompt(state, myPlayerNum);
                break;
            case ActionPhase.ItemUse:
                actionContext = GetItemUsePrompt(state, myPlayerNum);
                break;
            case ActionPhase.TreeAttack:
                actionContext = GetTreeHitPrompt(state, myPlayerNum);
                break;
            case ActionPhase.NightUpgrade:
            default:
                break;
        }
        sb.AppendLine(actionContext);

        return sb.ToString();
    }

    public string GetPromptBasedOnState(SimGameState state, int myPlayerNum)
    {
        float curVillHp = myPlayerNum == 1 ? state.p1VillHP : state.p2VillHP;
        float curTreeHp = state.treeHP;

        // 각 체력의 비율을 계산 (0.0 ~ 1.0)
        float villRatio = curVillHp / state.playerSetting.villageHP;
        float treeRatio = curTreeHp / state.roomSetting.treeHP;

        // 나무 체력에 좀 더 무게를 둔 가중치 평균
        float weightAvg = (villRatio * 0.4f) + (treeRatio * 0.6f);

        AIState curState = weightAvg switch
        {
            > 0.7f => AIState.Aggressive,
            > 0.45f => AIState.Balanced,
            > 0.25f => AIState.Defensive,
            _ => AIState.Cunning
        };

        return curState switch
        {
            AIState.Aggressive =>
                "너는 저돌적이고 자신감 넘치는 AI 플레이어다. 나무와 마을 체력이 넉넉하니 공격적으로 행동해라. **큰 데미지** 를 입혀 방어력을 확보하는 것이 우선이다.",

            AIState.Balanced =>
                "너는 냉정하고 계산적인 AI 플레이어다. 상대 상태를 분석하여 **공격과 방어의 균형** 을 맞춰라. 효율적인 기력 관리가 핵심이다.",

            AIState.Defensive =>
                "너는 절박한 생존자다. 나무 공격보다 **마을 복구와 방어** 가 1순위다. 버티기 모드로 전환하여 생존 시간을 벌어라.",

            AIState.Cunning =>
                "너는 영악한 전략가다. 나무는 곧 터질 폭탄이다. **최종 타격(막타)** 을 절대 치지 않도록 계산하고, 상대가 치도록 유도해라.",

            _ => "너는 턴제 생존 게임의 플레이어다."
        };
    }

    //아이템 선택 시 사용될 프롬프트
    public string GetItemSelectPrompt(SimGameState state, int myPlayerNum)
    {
        StringBuilder sb = new StringBuilder();

        int curEng = myPlayerNum == 1 ? state.p1Energy : state.p2Energy;
        sb.AppendLine("[situation]");
        sb.AppendLine("-다음과 같이 주어진 3 개의 아이템 중 아이템 1개를 선택해야 함");
        sb.AppendLine(GetMyItemOfferPrompt(state));
        sb.AppendLine("-현재 내가 보유한 아이템은 다음과 같다.");
        sb.AppendLine(GetMyInventoryPrompt(state, myPlayerNum));
        sb.AppendLine($"-현재 나의 사용 가능한 기력량은 {curEng}이다.");
        sb.AppendLine();

        sb.AppendLine("[Action]");
        sb.AppendLine("너는 지금 3개의 아이템 중 하나를 인벤토리에 넣어야 하며, 네 현재 마을 체력과 기력을 고려해서 가장 가성비 좋은 아이템 1개를 선택한다.");
        sb.AppendLine("인벤토리 용량은 8이며, 만약 인벤토리가 꽉 찬 경우 빈 문자열을 반환한다.");
        sb.AppendLine();

        sb.AppendLine("[output format]");
        sb.AppendLine("반드시 아래 JSON 포맷으로만 응답해. reasoning은 최대한 간단하게. 부가 설명 금지.");
        sb.AppendLine("아이템을 선택하거나 사용할 때 selectedItemId 와 itemId 필드에는 반드시 숫자로 된 itemId (예: 4000)를 입력해야함");
        sb.AppendLine("{");
        sb.AppendLine("  \"reasoning\": \"이유\",");
        sb.AppendLine("  \"selectedItemId\": \"\"");
        sb.AppendLine("}");
        return sb.ToString();
    }

    //아이템 사용 시 사용될 프롬프트
    public string GetItemUsePrompt(SimGameState state, int myPlayerNum)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("[situation]");
        sb.AppendLine("-현재 내가 보유한 아이템은 다음과 같다.");
        sb.AppendLine(GetMyInventoryPrompt(state, myPlayerNum));
        sb.AppendLine($"-현재 사용할 수 있는 최대 기력량은 {(myPlayerNum == 1 ? state.p1Energy : state.p2Energy)}이고, 권장 사용 기력량은 {CalcEnergyBudget(state, myPlayerNum)} 이다.");
        sb.AppendLine("-현재 보유한 기력량 보다 아이템 사용 기력량이 큰 경우, 해당 아이템은 사용할 수 없다.");
        sb.AppendLine();

        sb.AppendLine("[Action]");
        sb.AppendLine("네 인벤토리에 있는 아이템을 나무 체력, 너의 마을 체력, 기력 상황등을 골라서 적절하게 사용한다.");
        sb.AppendLine("만약 지금 상황에서 기력을 아끼는 것이 더 유리하거나 굳이 사용할 이유가 없다고 판단되면, 아무 아이템도 사용하지 않는다.");
        sb.AppendLine("아무 아이템도 사용하지 않을 경우 빈 배열로 반환한다.");
        sb.AppendLine();

        sb.AppendLine("[output format]");
        sb.AppendLine("반드시 아래 JSON 포맷으로만 응답해. reasoning은 최대한 간단하게. 부가 설명 금지.");
        sb.AppendLine("{");
        sb.AppendLine("  \"reasoning\": \"이유\",");
        sb.AppendLine("  \"actions\": [ { \"itemId\": \"아이템ID\", \"targetId\": \"제물 바치기 등 타겟이 필요할 경우 타겟 아이템ID\" } ]");
        // 아무 템도 안 쓸 경우 빈 배열 [] 반환
        sb.AppendLine("}");

        return sb.ToString();
    }

    public string GetTreeHitPrompt(SimGameState state, int myPlayerNum)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("[situation]");
        sb.AppendLine("-나무를 일정 데미지로 때려야 한다. 때린 데미지의 일부는 마을 방어력으로 전환된다.");
        sb.AppendLine();

        sb.AppendLine("[Action]");
        sb.AppendLine($"오늘 밤 들어올 독성 데미지와 나무의 남은 체력을 고려해서 {(myPlayerNum == 1 ? state.p1MinHitDmg : state.p2MinHitDmg)}~{(myPlayerNum == 1 ? state.p1MaxHitDmg : state.p2MaxHitDmg)} 사이의 타격 데미지를 정확한 숫자로 결정해.");
        sb.AppendLine();

        sb.AppendLine("[output format]");
        sb.AppendLine("반드시 아래 JSON 포맷으로만 응답해. reasoning은 최대한 간단하게. 부가 설명 금지.");
        sb.AppendLine("{");
        sb.AppendLine(" \"reasoning\": \"이유\",");
        sb.AppendLine(" \"hitDamage\": 500");
        sb.AppendLine("}");
        return sb.ToString();
    }


    //아이템 OFFER 정보 프롬프트
    public string GetMyItemOfferPrompt(SimGameState state)
    {
        StringBuilder sb = new StringBuilder();
        List<string> offer = Pick3(state.turn, state.totalTurnCount, state.roomSeed, ItemDB.Instance.GetItemsList(), state.roomSetting, state.playerSetting);
        foreach (string i in offer)
        {
            ItemSO item = ItemDB.Instance.Get(i);
            sb.AppendLine(i + $": {item.itemId} ({item.displayName_ID}, Cost: {item.itemCost}):{LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID)}");
        }
        return sb.ToString();
    }

    //현재 내 인벤토리 아이템 정보 프롬프트
    public string GetMyInventoryPrompt(SimGameState state, int myPlayerNum)
    {
        //내 인벤토리 가져오기
        List<string> myInv = (myPlayerNum == 1) ? state.p1Inventory : state.p2Inventory;
        if (myInv == null) myInv = new List<string>();
        //중복 아이템 설명은 한 번만 들어가도록 HashSet 설정
        HashSet<string> uniqueItems = new HashSet<string>(myInv);

        StringBuilder sb = new StringBuilder();

        if (uniqueItems.Count == 0)
        {
            sb.AppendLine("- 현재 인벤토리에 아이템이 없습니다.");
        }
        else
        {
            foreach (string itemId in uniqueItems)
            {
                ItemSO item = ItemDB.Instance.Get(itemId);

                if (item == null)
                {
                    Debug.LogWarning($"[PromptBuilder] 인벤토리에 유효하지 않은 아이템 ID가 포함됨 : {itemId}");
                }

                sb.AppendLine($" -{item.itemId} ({item.displayName_ID}, 기력: {item.itemCost}):{LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID)}");
            }
        }

        return sb.ToString();
    }

    //현재 기력 기준 기력 에산 계산
    public int CalcEnergyBudget(SimGameState state, int myPlayerNum)
    {
        int maxWave = state.roomSetting.maxWave;
        int curWave = state.wave;
        int energy = myPlayerNum == 1 ? state.p1Energy : state.p2Energy;

        int remainTurns = maxWave - curWave;

        return Mathf.CeilToInt((float)energy / remainTurns);
    }

    //OFFER 함수
    public List<string> Pick3(int turnIndex, int actor, int roomSeed, List<ItemSO> items, RoomSetting roomSetting, PlayerSetting playerSetting)
    {
        int playerActNum = actor;
        //결정론적 난수 생성
        //turnIndex와 actor가 같으면 재현 가능한 난수 생성기
        var rng = new System.Random(roomSeed ^ (turnIndex * 73856093) ^ (actor * 19349663) ^ 12345);

        //각 플레이어의 아이템 등장 확률을 프로퍼티에서 불러와 딕셔너리에 저장
        Dictionary<ItemClass, float> RarityWeight = new()
        {
            {ItemClass.Common,  playerSetting.commonWeight},
            {ItemClass.Hero,  playerSetting.heroWeight},
            {ItemClass.Rare,  playerSetting.rareWeight},
            {ItemClass.Legendary,  playerSetting.legendaryWeight},
        };

        //각 아이템에 점수(key) 부여 알고리즘
        //점수를 저장할 리스트 선언
        var scored = new List<(double key, string itemId)>(items.Count);
        //각 아이템을 돌면서
        foreach (var it in items)
        {
            //만약 아이템이 빈 경우 제외
            if (string.IsNullOrEmpty(it.itemId)) continue;
            // 등급 가중치 0이면 그 등급은 절대 안 나오게 제외
            if (!RarityWeight.TryGetValue(it.itemClass, out float rw) || rw <= 0f) continue;

            // 아이템 가중치도 0이면 제외
            if (it.itemWeight <= 0f) continue;

            double w = rw * it.itemWeight;

            //핵심!
            //(0,1] 범위로 난수 값 조정
            double u = 1.0 - rng.NextDouble();
            //각 아이템 키 값 계산
            //아이템의 가중치가 클 수록 결과값이 작아진다.
            //결과값이 작을수록 아이템이 상위권에 뽑히게 된다.
            double key = -Math.Log(u) / w;

            //계산한 점수를 저장한다.
            scored.Add((key, it.itemId));

            //알고리즘 설명
            // Key = (-ln(Random)) / (Weight)
            // 달리기 기록 = (트랙의 길이(랜덤) / (선수의 속도(가중치))
            //즉, 일반적으로는 Common item의 가중치가 크지만 운 나쁘게 큰 수의 랜덤 값이 나올 수 있고
            //반대로 Legendary item의 가중치가 작지만, 운 좋게 작은 수의 랜덤 값이 나올 수 있다.
        }


        //3개의 아이템을 고르는 LINQ문
        var picked = scored
            .GroupBy(s => s.itemId)                     //같은 아이템 ID끼리 묶기(중복 아이템 제거)
            .Select(g => g.OrderBy(x => x.key).First()) //같은 ID 아이템 중 가장 점수가 작은 아이템 하나만 남김
            .OrderBy(x => x.key)                        //전체 아이템을 점수 순으로 정렬
            .Take(roomSetting.itemOfferCount)                                    //그 중 상위 3등 까지 뽑기
            .Select(x => x.itemId)                      //해당 3 아이템의 id 가져오기
            .ToList();                                  //리스트로 만들기

        while (picked.Count < roomSetting.itemOfferCount) picked.Add("Error");   //만약 빈 요소가 있으면 에러
        return picked; //뽑힌 아이템 리스트 반환
    }
}
