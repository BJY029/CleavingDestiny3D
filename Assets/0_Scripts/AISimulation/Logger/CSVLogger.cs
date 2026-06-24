using UnityEngine;
using System;
using System.IO;
using System.Text;

public class CSVLogger
{
    private string filePath;
    private StreamWriter writer;

    public void InitLog()
    {
        string folderPath = Application.dataPath + "/SimulationLogs";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePath = folderPath + $"/BalanceLog_{timeStamp}.csv";

        Encoding utf8BOM = new UTF8Encoding(true);
        writer = new StreamWriter(filePath, false, utf8BOM);

        string header = "GameCount,Winner,TotalTurns,EndDay,EndWave," +
                        "P1_AcquiredItems,P2_AcquiredItems," +
                        "P1_UsedItems,P2_UsedItems," +
                        "P1_EndInventory,P2_EndInventory," +
                        "P1_Upgrades,P2_Upgrades";
        writer.WriteLine(header);
    }

    public void LogGameResult(int gameCnt, int winner, int totalTurns, SimGameState state)
    {
        if (writer == null) return;

        //획득한 아이템들
        string p1Acquired = string.Join(";", state.p1SelectedItems);
        string p2Acquired = string.Join(";", state.p2SelectedItems);
        //실제로 사용한 아이템들
        string p1Used = string.Join(";", state.p1UsedItems);
        string p2Used = string.Join(";", state.p2UsedItems);
        //게임 종료 시점에 사용되지 않고 남아있는 아이템들
        string p1Remain = string.Join(";", state.p1Inventory);
        string p2Remain = string.Join(";", state.p2Inventory);
        //업그레이드 내역
        string p1Upgrades = string.Join(";", state.p1SelectedUpgrades);
        string p2Upgrades = string.Join(";", state.p2SelectedUpgrades);

        string line = string.Format("{0},{1},{2},{3},{4},\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\"",
             gameCnt, winner, totalTurns, state.day, state.wave,
             p1Acquired, p2Acquired,
             p1Used, p2Used,
             p1Remain, p2Remain,
             p1Upgrades, p2Upgrades
         );

        writer.WriteLine(line);
    }

    public void CloseLog()
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
            writer = null;
            Debug.Log($"[로깅 완료] 시뮬레이션 결과가 다음 경로에 저장되었습니다:\n{filePath}");
        }
    }
}
