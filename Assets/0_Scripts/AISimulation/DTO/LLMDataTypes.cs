using System.Collections.Generic;

//요청 클래스
public class LLMRequest
{
    //사용 모델명: Gemma3:12b
    public string model = "gemma:12b";
    public List<Message> messages = new List<Message>();
    public ResponseFormat response_format = new ResponseFormat { type = "json_object" };
}

public class Message { public string role; public string content; }

public class ResponseFormat { public string type; }

public class LLMDecision
{
    public string reasoning;
    public List<LLMAction> actions;
}

public class LLMAction
{
    //UseItem, HitTree 등
    public string actionType;
    public string itemId;
    public string targetId;
}