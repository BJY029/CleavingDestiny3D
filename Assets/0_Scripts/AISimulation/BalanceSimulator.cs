using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BalanceSimulator : MonoBehaviour
{
    public PlayerSetting playerSetting;
    public RoomSetting roomSetting;

    private OllamaAPIClient apiClient = new OllamaAPIClient();
    private PromptBuilder promptBulider = new PromptBuilder();
    private string csvPath;

    private int gameCount = 10;

    private void Start()
    {
        csvPath = Application.dataPath + "/BalanceResult.csv";
        File.WriteAllText(csvPath, "GameNum,Winner,TotalTurns,P1_Items,P2_Items\n");

    }

    private async UniTask RunMassiveSimulation()
    {
        for (int cnt = 1; cnt <= gameCount; cnt++)
        {
            SimGameState state = new SimGameState(playerSetting, roomSetting);
            bool isGameOver = false;
            int winner = 0;
            int turnCount = 0;

            while (!isGameOver && turnCount < 50)
            {
                turnCount++;


            }


        }
    }

    private async UniTask RunPlayerTurn(SimGameState state, int playerNum)
    {
        // --- 1. 아이템 선택 페이즈 ---
        string selectPrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.ItemSelect);
        // (API 호출 시 유저의 상태 정보 JSON을 함께 넘겨준다고 가정)
        //string selectJsonAns = await apiClient.AskNextMove(selectPrompt, GetStateJson(state, playerNum));
        //executor.ExecutePhaseAction(selectJsonAns, ActionPhase.ItemSelect, state, playerNum);

        // --- 2. 아이템 사용 페이즈 ---
        string usePrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.ItemUse);
        // string useJsonAns = await apiClient.AskNextMove(usePrompt, GetStateJson(state, playerNum));
        //executor.ExecutePhaseAction(useJsonAns, ActionPhase.ItemUse, state, playerNum);

        // --- 3. 나무 타격 페이즈 ---
        string hitPrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.TreeAttack);
        //string hitJsonAns = await apiClient.AskNextMove(hitPrompt, GetStateJson(state, playerNum));
        //executor.ExecutePhaseAction(hitJsonAns, ActionPhase.TreeAttack, state, playerNum);
    }
}
