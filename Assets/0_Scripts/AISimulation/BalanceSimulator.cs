using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class BalanceSimulator : MonoBehaviour
{
    public PlayerSetting playerSetting;
    public RoomSetting roomSetting;

    private OllamaAPIClient apiClient = new OllamaAPIClient();
    private PromptBuilder promptBulider = new PromptBuilder();
    private LLMActionExecutor executor = new LLMActionExecutor();
    private string csvPath;

    public Button myButton;

    private int gameCount = 1;
    private int winner;

    private void Start()
    {
        csvPath = Application.dataPath + "/BalanceResult.csv";
        string header = "GameNum,Winner,TotalTurns,P1_RemainHP,P2_RemainHP,Tree_RemainHP,P1_UsedItems,P2_UsedItems\n";
        File.WriteAllText(csvPath, header, Encoding.UTF8);
        myButton.onClick.AddListener(() => RunMassiveSimulation().Forget());
    }


    private async UniTask RunMassiveSimulation()
    {
        CancellationToken cancellToken = this.GetCancellationTokenOnDestroy();

        for (int cnt = 1; cnt <= gameCount; cnt++)
        {
            SimGameState state = new SimGameState(playerSetting, roomSetting);
            int turnCount = 0;

            while (!IsGameOver(state) && turnCount < 50)
            {
                turnCount++;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 1; j <= 2; j++)
                    {
                        state.curTurnPlayerNum = j;
                        Debug.Log($"Day : {state.day}, Wave : {state.wave}, Turn : {state.turn}");

                        await RunPlayerTurn(state, j, cancellToken);

                        if (IsGameOver(state)) break;
                        turnCount++;
                        state.totalTurnCount = turnCount;
                        state.turn++;
                    }
                    state.turn = 0;
                    state.wave++;
                    if (IsGameOver(state)) break;
                }
                state.wave = 0;
                //임시 효과로 단순히 각 마을에 나무 독성 데미지를 적용한다.
                //추후 업그레이드 관련 로직도 구현해야 한다.
                state.ApplyToxicToVillage();
                state.day++;
            }

            LogToCSV(cnt, winner, turnCount, state);
            Debug.Log($"[시뮬레이션] {cnt}판 완료. 승자: P{winner}");
        }
    }

    private bool IsGameOver(SimGameState state)
    {
        if (state.treeHP <= 0f)
        {
            if (state.turn == 1) winner = 2;
            else winner = 1;
            return true;
        }

        if (state.p1VillHP <= 0f || state.p2VillHP <= 0f)
        {
            if (state.p1VillHP <= 0f && state.p2VillHP <= 0f)
                winner = -1;//draw
            else if (state.p1VillHP <= 0f) winner = 2;
            else winner = 1;
            return true;
        }

        return false;
    }


    private async UniTask RunPlayerTurn(SimGameState state, int playerNum, CancellationToken token)
    {
        try
        {
            // --- 1. 아이템 선택 페이즈 ---
            string selectPrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.ItemSelect);
            Debug.Log($"<color=green>[ItemSelectPhase]\n{selectPrompt}</color>");
            // (API 호출 시 유저의 상태 정보 JSON을 함께 넘겨준다고 가정)
            string selectJsonAns = await apiClient.AskNextMove(selectPrompt, GetStateJson(state, playerNum), token);
            executor.ExecutePhaseAction(selectJsonAns, ActionPhase.ItemSelect, state, playerNum);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[Abort Simulation] Unity Editor Has Been Suspended");
            return;
        }
        catch (Exception e)
        {
            Debug.LogError($"[P{playerNum} Item Selection Error] ERROR : {e.Message} \nStackTrace : {e.StackTrace}");
        }

        try
        {
            // --- 2. 아이템 사용 페이즈 ---
            string usePrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.ItemUse);
            Debug.Log($"<color=yellow>[ItemUsePhase]\n{usePrompt}</color>");
            string useJsonAns = await apiClient.AskNextMove(usePrompt, GetStateJson(state, playerNum), token);
            executor.ExecutePhaseAction(useJsonAns, ActionPhase.ItemUse, state, playerNum);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[Abort Simulation] Unity Editor Has Been Suspended");
            return;
        }
        catch (Exception e)
        {
            Debug.LogError($"[P{playerNum} Item Use Error] ERROR : {e.Message} \nStackTrace : {e.StackTrace}");
        }

        try
        {
            // --- 3. 나무 타격 페이즈 ---
            string hitPrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.TreeAttack);
            Debug.Log($"<color=orange>[HitTreePhase]\n{hitPrompt}</color>");
            string hitJsonAns = await apiClient.AskNextMove(hitPrompt, GetStateJson(state, playerNum), token);
            executor.ExecutePhaseAction(hitJsonAns, ActionPhase.TreeAttack, state, playerNum);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[Abort Simulation] Unity Editor Has Been Suspended");
            return;
        }
        catch (Exception e)
        {
            Debug.LogError($"[P{playerNum} Tree Damage Calc Error] ERROR : {e.Message} \nStackTrace : {e.StackTrace}");
        }

    }

    public string GetStateJson(SimGameState state, int playerNum)
    {
        LLMGameStateDTO playerStateDTO = state.GetStateForPlayer(playerNum);
        string jsonString = JsonConvert.SerializeObject(playerStateDTO, Formatting.Indented);
        return jsonString;
    }

    private void LogToCSV(int gameNum, int winner, int turns, SimGameState state)
    {
        // 1. 리스트에 담긴 아이템 ID들을 "2002|3002|1001" 형태로 묶어줌 
        // (쉼표(',')를 쓰면 CSV 셀이 넘어가 버리므로 반드시 파이프('|') 기호 등을 써야 합니다)
        string p1ItemsStr = string.Join("|", state.p1Inventory);
        string p2ItemsStr = string.Join("|", state.p2Inventory);

        // 아무 아이템도 안 썼을 경우 보기 좋게 "None" 처리
        if (string.IsNullOrEmpty(p1ItemsStr)) p1ItemsStr = "None";
        if (string.IsNullOrEmpty(p2ItemsStr)) p2ItemsStr = "None";

        // 2. CSV에 기록할 한 줄(Row) 문자열 만들기
        string rowData = string.Format("{0},Player{1},{2},{3},{4},{5},{6},{7}\n",
            gameNum,               // 몇 번째 게임인지
            winner,                // 승자 (1 or 2)
            turns,                 // 총 소요 턴 수
            state.p1VillHP,            // P1 남은 체력
            state.p2VillHP,            // P2 남은 체력
            state.treeHP,          // 남은 세계수 체력
            p1ItemsStr,            // P1이 사용한 아이템들
            p2ItemsStr             // P2가 사용한 아이템들
        );

        // 3. 파일의 맨 아랫줄에 덧붙여 쓰기 (Append)
        // 매번 파일을 새로 쓰지 않고 덧붙이므로 메모리 관리와 속도 면에서 안전합니다.
        File.AppendAllText(csvPath, rowData, Encoding.UTF8);
    }
}
