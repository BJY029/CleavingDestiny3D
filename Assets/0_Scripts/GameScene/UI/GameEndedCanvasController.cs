using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameEndedCanvasController : MonoBehaviour
{
    private void Awake()
    {
        GameEndedPanel.SetActive(false);
    }

    [Header("Other Canvases")]
    public GameObject Canvases;

    [Header("UIs")]
    public GameObject GameEndedPanel;
    public TextMeshProUGUI WinLoseText;
    public TextMeshProUGUI ReasonText;

    [Header("Stats UI")]
    public TextMeshProUGUI StatsText; // 유니티 에디터에서 전용 텍스트 컴포넌트 할당용 (헤더 또는 전체 출력용)

    [Header("Stats UI (1:1 Left/Right)")]
    public TextMeshProUGUI LeftPlayerStatsText;  // 로컬 플레이어 (나)
    public TextMeshProUGUI RightPlayerStatsText; // 상대 플레이어 (적 또는 AI)

    //게임 종료 UI 출력
    public void SetGameEndedCanvas(MatchResultType state, MatchResultReason reason)
    {
        //기존 다른 캔버스 크기 0으로 설정(안보이게 설정)
        // Canvases.transform.localScale = Vector3.zero;
        Canvases.SetActive(false);
        //승패 UI 캔버스켜기
        GameEndedPanel.SetActive(true);

        string reasonStr = LocalizationManager.Instance.GetText(CSV_Type.UI, state == MatchResultType.Draw ? "UI_ResultReason_Draw" : $"UI_ResultReason_{state}_{reason}");
        WinLoseText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, $"UI_Result_{state}");
        ReasonText.text = reasonStr;

        // 진행 시간 및 라운드 헤더 빌드
        string matchHeader = BuildMatchHeaderString();

        // 1:1 경기 통계 조립 (나 vs 상대)
        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;
        string leftStats = "";
        string rightStats = "";

        if (PlayerManager.Instance != null && PlayerManager.Instance.Players != null)
        {
            foreach (var kvp in PlayerManager.Instance.Players)
            {
                if (kvp.Key == localActor)
                {
                    leftStats = GetPlayerStatsString(kvp.Key, kvp.Value);
                }
                else
                {
                    rightStats = GetPlayerStatsString(kvp.Key, kvp.Value);
                }
            }
        }

        if (LeftPlayerStatsText != null) LeftPlayerStatsText.text = leftStats;
        if (RightPlayerStatsText != null) RightPlayerStatsText.text = rightStats;
        if (StatsText != null) StatsText.text = matchHeader;

        // 플레이어 입력 비활성화 (움직임 및 카메라 회전 방지)
        if (KeyInteractManager.Instance != null)
        {
            KeyInteractManager.Instance.SetPlayerActionsEnabled(false);
        }

        // 마우스 커서 활성화
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private string BuildMatchHeaderString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // 1. 경과 시간 계산
        double startTime = PhotonPropertyHelper.GetRoomProp<double>(RoomPropKeys.MatchStartTime, -1);
        if (startTime > 0)
        {
            double elapsed = PhotonNetwork.Time - startTime;
            int minutes = (int)(elapsed / 60);
            int seconds = (int)(elapsed % 60);
            sb.Append($"진행 시간: {minutes:00}:{seconds:00}");
        }
        else
        {
            sb.Append("진행 시간: 알 수 없음");
        }

        // 2. 경과 라운드 (일차)
        int currentDay = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentDay, 1);
        sb.Append($"  |  진행 라운드: {currentDay}일차");

        return sb.ToString();
    }

    private string GetPlayerStatsString(int actorNum, RuntimePlayerInfo info)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        string displayName = info.playerName;
        if (info.isAI)
        {
            displayName += " (AI)";
        }
        else if (actorNum == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            displayName += " (나)";
        }

        int dmgDealt = PhotonPropertyHelper.GetPlayerProp<int>(actorNum, PlayerPropKeys.CumulativeDamage, 0);
        float dmgReceived = PhotonPropertyHelper.GetPlayerProp<float>(actorNum, PlayerPropKeys.CumulativeDamageReceived, 0f);
        int itemsUsed = PhotonPropertyHelper.GetPlayerProp<int>(actorNum, PlayerPropKeys.ItemsUsedCount, 0);
        int goldSpent = PhotonPropertyHelper.GetPlayerProp<int>(actorNum, PlayerPropKeys.CumulativeGoldSpent, 0);

        sb.AppendLine($"<b>{displayName}</b>");
        sb.AppendLine($"  • 가한 피해: {dmgDealt:N0}");
        sb.AppendLine($"  • 받은 피해: {dmgReceived:F0}");
        sb.AppendLine($"  • 사용한 아이템: {itemsUsed}개");
        sb.AppendLine($"  • 사용한 골드: {goldSpent:N0}G");

        return sb.ToString();
    }

    //나가기 버튼 클릭 시 호출될 함수
    public void OnClickExitButton()
    {
        if (GameExitHandler.Instance != null)
        {
            GameExitHandler.Instance.RequestLeaveGame();
        }
    }
}
