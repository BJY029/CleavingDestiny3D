using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SimGameState
{
    public int p1VillHP, p1VillBarrier, p1Energy;
    public int p2VillHP, p2VillBarrier, p2Energy;
    public float treeHP;
    public float treeToxicDmg;

}

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

        }
    }
}
