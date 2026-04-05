using System.IO;
using System.Text;
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

    private void Start()
    {
        csvPath = Application.dataPath + "/BalanceResult.csv";
        string header = "GameNum,Winner,TotalTurns,P1_RemainHP,P2_RemainHP,Tree_RemainHP,P1_UsedItems,P2_UsedItems\n";
        File.WriteAllText(csvPath, header, Encoding.UTF8);
        myButton.onClick.AddListener(() => RunMassiveSimulation().Forget());
    }


    private async UniTask RunMassiveSimulation()
    {
        for (int cnt = 1; cnt <= gameCount; cnt++)
        {
            SimGameState state = new SimGameState(playerSetting, roomSetting);
            bool isGameOver = false;
            int winner = 0;
            int turnCount = 0;

            while (!IsGameOver(state) && turnCount < 50)
            {
                turnCount++;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 1; j <= 2; j++)
                    {
                        turnCount++;
                        state.turn++;
                        Debug.Log($"P{j} turn processing");
                        await RunPlayerTurn(state, j);
                        Debug.Log($"P{j} turn processing End");

                        if (IsGameOver(state)) break;
                    }
                    state.turn = 0;
                    state.wave++;
                    if (IsGameOver(state)) break;
                }
                state.wave = 0;
                //밤 페이즈
                state.day++;
            }

            LogToCSV(cnt, 1, turnCount, state);
            Debug.Log($"[시뮬레이션] {cnt}판 완료. 승자: P{1}");
        }
    }

    private bool IsGameOver(SimGameState state)
    {
        if (state.treeHP <= 0f || state.p1VillHP <= 0f || state.p2VillHP <= 0f) return true;
        return false;
    }

    private async UniTask RunPlayerTurn(SimGameState state, int playerNum)
    {
        // --- 1. 아이템 선택 페이즈 ---
        string selectPrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.ItemSelect);
        Debug.Log(selectPrompt);
        // (API 호출 시 유저의 상태 정보 JSON을 함께 넘겨준다고 가정)
        string selectJsonAns = await apiClient.AskNextMove(selectPrompt, GetStateJson(state, playerNum));
        Debug.Log("success to receive result");
        executor.ExecutePhaseAction(selectJsonAns, ActionPhase.ItemSelect, state, playerNum);

        // --- 2. 아이템 사용 페이즈 ---
        string usePrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.ItemUse);
        string useJsonAns = await apiClient.AskNextMove(usePrompt, GetStateJson(state, playerNum));
        Debug.Log("success to receive result");
        executor.ExecutePhaseAction(useJsonAns, ActionPhase.ItemUse, state, playerNum);

        // --- 3. 나무 타격 페이즈 ---
        string hitPrompt = promptBulider.BuildDynamicSystemPrompt(state, playerNum, ActionPhase.TreeAttack);
        string hitJsonAns = await apiClient.AskNextMove(hitPrompt, GetStateJson(state, playerNum));
        Debug.Log("success to receive result");
        executor.ExecutePhaseAction(hitJsonAns, ActionPhase.TreeAttack, state, playerNum);
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
