using UnityEngine;
using Newtonsoft.Json;
using System;

public class LLMActionExecutor
{
    public void ExecutePhaseAction(string llmJson, ActionPhase phase, SimGameState state, int playerNum)
    {
        string cleanJson = CleanUpJson(llmJson);

        try
        {
            switch (phase)
            {
                case ActionPhase.ItemSelect:
                    var selectData = JsonConvert.DeserializeObject<ItemSelectResopnse>(cleanJson);
                    Debug.Log($"[P{playerNum} 선택 이유] {selectData.reasoning}");

                    if (!string.IsNullOrEmpty(selectData.selectedItemId))
                    {
                        state.TryAddItemToInventory(playerNum, selectData.selectedItemId);
                    }
                    break;
                case ActionPhase.ItemUse:
                    var useData = JsonConvert.DeserializeObject<ItemUseResponse>(cleanJson);
                    Debug.Log($"[P{playerNum} 사용 이유] {useData.reasoning}");

                    if (useData.actions != null && useData.actions.Count > 0)
                    {
                        foreach (var act in useData.actions)
                        {
                            //아이템 효과 실제 적용
                            //아이템 인벤토리에서 삭제
                            state.TryDeleteItemFromInventroy(playerNum, act.itemId);
                            ItemSO item = ItemDB.Instance.Get(act.itemId);
                            ItemHandlingInSim.Instance.AddItemStatusInstance(playerNum, item, state);
                        }
                    }
                    break;
                case ActionPhase.TreeAttack:
                    var hitData = JsonConvert.DeserializeObject<TreeHitResponse>(cleanJson);
                    Debug.Log($"[P{playerNum} 타격 이유] {hitData.reasoning}");

                    ItemHandlingInSim.Instance.ProcessItemEffect(playerNum, hitData.hitDamage, true, state);
                    //state.ApplyTreeDamage(playerNum, hitData.hitDamage);
                    break;
                case ActionPhase.NightUpgrade:
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[LLM JSON 파싱/실행 에러] Phase : {phase} \nERROR : {e.Message} \nStackTrace : {e.StackTrace} \nRaw Json: {llmJson}");
        }
    }

    //LLM이 붙이는 이상한 것들을 제거하는 사전 에방 함수
    private string CleanUpJson(string raw)
    {
        string cleaned = raw.Trim();
        if (cleaned.StartsWith("```json")) cleaned = cleaned.Substring(7);
        if (cleaned.StartsWith("```")) cleaned = cleaned.Substring(3);
        if (cleaned.EndsWith("```")) cleaned = cleaned.Substring(0, cleaned.Length - 3);
        return cleaned.Trim();
    }
}
