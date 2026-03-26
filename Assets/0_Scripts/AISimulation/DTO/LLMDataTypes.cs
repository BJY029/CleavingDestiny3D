using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


//요청 클래스
[System.Serializable]
public class LLMRequest
{
    //사용 모델명: Gemma3:12b
    public string model = "gemma:12b";
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
}

[System.Serializable]
public class PlayerStatusDTO
{
    public float villageHP;
    public float barrier;
    public int energy;
    public bool hasDebuff;
}

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