using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    //게임 종료 UI 출력
    public void SetGameEndedCanvas(MatchResultType state, MatchResultReason reason)
    {
        //기존 다른 캔버스 크기 0으로 설정(안보이게 설정)
        // Canvases.transform.localScale = Vector3.zero;
        Canvases.SetActive(false);
        //승패 UI 캔버스켜기
        GameEndedPanel.SetActive(true);
        //관련 정보 설정
        // WinLoseText.text = "You " + state;
        // ReasonText.text = "Reason : " + reason;

        WinLoseText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, $"UI_Result_{state}");
        ReasonText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, state == MatchResultType.Draw ? "UI_ResultReason_Draw" : $"UI_ResultReason_{state}_{reason}");

        // 마우스 커서 활성화
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
