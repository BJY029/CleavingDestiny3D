using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                    //Debug.Log($"[P{playerNum} 선택 이유] {selectData.selectedItemId}:{selectData.reasoning}");
                    PromptBuilder promptBuilder = new PromptBuilder();

                    List<string> offer = promptBuilder.Pick3(state.totalTurnCount, playerNum, state.roomSeed, ItemDB.Instance.GetItemsList(), state.roomSetting, state.playerSetting);
                    if (!offer.Contains(selectData.selectedItemId) && !string.IsNullOrEmpty(selectData.selectedItemId))
                    {
                        Debug.LogWarning($"[환각 방어] P{playerNum}가 오퍼에 없는 아이템({selectData.selectedItemId})을 선택했습니다. 강제로 첫 번째 오퍼를 지급합니다.");

                        //패널티 혹은 기본값 적용: 그냥 오퍼에 나온 1번 아이템을 강제로 줘버립니다.
                        selectData.selectedItemId = offer[0];
                    }

                    if (!string.IsNullOrEmpty(selectData.selectedItemId))
                    {
                        state.TryAddItemToInventory(playerNum, selectData.selectedItemId);
                        state.RecordItemSelection(playerNum, selectData.selectedItemId);
                    }
                    break;
                case ActionPhase.ItemUse:
                    var useData = JsonConvert.DeserializeObject<ItemUseResponse>(cleanJson);
                    foreach (var itemInfo in useData.actions)
                    {
                        Debug.Log($"[P{playerNum} 아이템 사용] {itemInfo.itemId}");
                    }
                    //Debug.Log($"[P{playerNum} 사용 이유] {useData.reasoning}");

                    if (useData.actions != null && useData.actions.Count > 0)
                    {
                        foreach (var act in useData.actions)
                        {
                            //아이템 효과 실제 적용
                            //아이템 인벤토리에서 삭제
                            ItemSO item = ItemDB.Instance.Get(act.itemId);
                            int playerEng = (playerNum == 1) ? state.p1Energy : state.p2Energy;
                            if (item.itemCost > playerEng)
                            {
                                Debug.LogError($"[Item Using Error]Not Enough Energy : p{playerNum}'s Energy={playerEng} < itemCost={item.itemCost}");
                                continue;
                            }
                            ItemHandlingInSim.Instance.AddItemStatusInstance(playerNum, item, state);

                            if (playerNum == 1) state.p1Energy -= item.itemCost;
                            else state.p2Energy -= item.itemCost;

                            state.TryDeleteItemFromInventory(playerNum, act.itemId);
                            state.RecordItemUse(playerNum, act.itemId);
                        }
                    }
                    break;
                case ActionPhase.TreeAttack:
                    var hitData = JsonConvert.DeserializeObject<TreeHitResponse>(cleanJson);
                    //Debug.Log($"[P{playerNum} 타격 데미지/이유] {hitData.hitDamage}/{hitData.reasoning}");

                    ItemHandlingInSim.Instance.ProcessItemEffect(playerNum, hitData.hitDamage, true, state);
                    //state.ApplyTreeDamage(playerNum, hitData.hitDamage);
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[LLM JSON 파싱/실행 에러] Phase : {phase} \nERROR : {e.Message} \nStackTrace : {e.StackTrace} \nRaw Json: {llmJson}");
        }
    }

    public bool ExecuteNightPhaseAction(string llmJson, SimGameState state, int playerNum)
    {
        string cleanJson = CleanUpJson(llmJson);
        try
        {
            var VillData = JsonConvert.DeserializeObject<NightUpgradeResponse>(cleanJson);
            //Debug.Log($"[P{playerNum} 마을 업그레이드 요소/이유] {VillData.upgradeVillageType}/{VillData.reasoning}");

            if (!string.IsNullOrEmpty(VillData.upgradeVillageType))
            {
                if (System.Enum.TryParse(VillData.upgradeVillageType, out VillageType type))
                {
                    state.simVillageState.UpgradeVillageObject(playerNum, type, state);
                    return true;
                }
                else { Debug.LogError("Faild to Parse Enum Type"); return false; }
            }
            else
            {
                Debug.LogError("Village Upgrade Data is NULL");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[LLM JSON 파싱/실행 에러] Phase : {ActionPhase.NightUpgrade.ToString()} \nERROR : {e.Message} \nStackTrace : {e.StackTrace} \nRaw Json: {llmJson}");
            return false;
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
