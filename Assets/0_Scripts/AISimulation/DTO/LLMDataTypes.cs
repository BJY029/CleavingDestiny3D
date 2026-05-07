using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


//요청 클래스
[System.Serializable]
public class LLMRequest
{
    //사용 모델명: Gemma3:12b
    public string model = "gemma3:12b";
    public List<Message> messages = new List<Message>();
    public ResponseFormat response_format = new ResponseFormat { type = "json_object" };
}

[System.Serializable]
public class Message { public string role; public string content; }

[System.Serializable]
public class ResponseFormat { public string type; }

[System.Serializable]
public class LLMGameStateDTO
{
    public int currentWave;
    public float expectedToxicDamage;
    public float treeHP;

    public PlayerStatusDTO myStatus;
    public PlayerStatusDTO oppStatus;
    public List<ItemInfoDTO> myInventory;
    public List<ItemInfoDTO> oppInventory;

    public PlayerVStateDTO myVStatus;
}

//플레이어 상태 정의 DTO
[System.Serializable]
public class PlayerStatusDTO
{
    public int maxTreeHitDmg, minTreeHitDmg;
    public float villageHP;
    public float barrier;
    public int maxEnergy;
    public int energy;
    public bool hasDebuff;
}

//아이템 정보 정의 DTO
[System.Serializable]
public class ItemInfoDTO
{
    public string itemId;
    public string name;
    public int cost;
    [JsonConverter(typeof(StringEnumConverter))]
    public ItemClass rarity;

    [JsonConverter(typeof(StringEnumConverter))]
    public ItemType type;
}

[System.Serializable]
public class PlayerVStateDTO
{
    public int curGold;
    public List<VillageState> villageInfos;
}

[System.Serializable]
public class VillageState
{
    public int curLevel;
    public string curEffect;
    public string nextEffect;
    public int upgradeGold;
}

//
[System.Serializable]
public class LLMDecision
{
    public string reasoning;
    public List<LLMAction> actions;
}

[System.Serializable]
public class LLMAction
{
    //UseItem, HitTree 등
    public string actionType;
    public string itemId;
    public string targetId;
}


//AI 프롬프트 응답 전용 DTO 정의
//프롬프트에서 정의한 반환형 JSON과 동일화 해야 함
//아이템 선택 결정 후 오는 JSON 반환
//실제 테스트시 reasoning 제거하기
public class ItemSelectResopnse
{
    public string reasoning;
    public string selectedItemId;
}

//아이템 사용 결정 후 오는 Json 형식 반환
public class ItemUseResponse
{
    public string reasoning;
    public List<ItemAction> actions;
}

//아이템 정보 정의
public class ItemAction
{
    public string itemId;
    public int itemCost;
    public string itemRarity;
}

//나무 타격 데미지 결정 후 Json 형식 반환
public class TreeHitResponse
{
    public string reasoning;
    public int hitDamage;
}

//상점 업그레이드 부분
public class NightUpgradeResponse
{
    public string reasoning;
    public string upgradeVillageType;
}