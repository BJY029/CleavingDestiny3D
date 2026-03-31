using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BalanceSimulator : MonoBehaviour
{
    public PlayerSetting playerSetting;
    public RoomSetting roomSetting;

    private OllamaAPIClient apiClient = new OllamaAPIClient();
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
}
