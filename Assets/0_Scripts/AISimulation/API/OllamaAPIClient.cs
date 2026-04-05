using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;


public class OllamaAPIClient
{
    private readonly string apiURL = "http://localhost:11434/v1/chat/completions";

    public async UniTask<string> AskNextMove(string systemPrompt, string gameStateJson)
    {
        LLMRequest request = new LLMRequest();
        request.messages.Add(new Message { role = "system", content = systemPrompt });
        request.messages.Add(new Message { role = "user", content = gameStateJson });

        string jsonPayload = JsonConvert.SerializeObject(request);

        using (UnityWebRequest www = new UnityWebRequest(apiURL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                JObject rawResponse = JObject.Parse(www.downloadHandler.text);
                string aiAnswer = (string)rawResponse["choices"][0]["message"]["content"];
                return aiAnswer;
            }
            else
            {
                Debug.LogError($"[LLM API Error] {www.error}");
                return null;
            }
        }
    }
}
