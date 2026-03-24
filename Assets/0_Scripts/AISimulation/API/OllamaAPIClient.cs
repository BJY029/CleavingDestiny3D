using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

/*
public class OllamaAPIClient : MonoBehaviour
{
    private readonly string apiURL = "http://localhost:11434/v1/chat/completion";

    public async UniTask<LLMDecision> AskNextMove(string systemPrompt, string gameStateJson)
    {
        LLMRequest request = new LLMRequest();
        request.messages.Add(new Message { role = "system", content = systemPrompt });
        request.messages.Add(new Message { role = "user", content = gameStateJson });

        string jsonPayload = JsonConvert.SerializeObject(request);

        using (UnityWebRequest www = new UnityWebRequest(apiURL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.SetRequestHeader("Content-Type", "application/json");

            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var rawResponse = JsonConvert.DeserializeObject<dynamic>(www.downloadHandler.text);
                string aiAnswer = rawResponse.choices[0].message.content;
                return JsonConvert.DeserializeObject<LLMDecision>(aiAnswer);
            }
            else
            {
                Debug.LogError($"[LLM API Error] {www.error}");
                return null;
            }
        }
    }
}
*/