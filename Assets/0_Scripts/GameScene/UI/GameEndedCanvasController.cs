using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndedCanvasController : MonoBehaviour
{
    public static GameEndedCanvasController instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this);
            return;
        }
        GameEndedPanel.transform.localScale = Vector3.one;
        GameEndedPanel.SetActive(false);
    }

    [Header("Other Canvases")]
    public GameObject Canvases;

    [Header("UIs")]
    public GameObject GameEndedPanel;
    public TextMeshProUGUI WinLoseText;
    public TextMeshProUGUI ReasonText;

    //게임 종료 UI 출력
    public void SetGameEndedCanvas(string state, string reason)
    {
        //기존 다른 캔버스 크기 0으로 설정(안보이게 설정)
        Canvases.transform.localScale = Vector3.zero;
        //승패 UI 캔버스켜기
        GameEndedPanel.SetActive(true);
        //관련 정보 설정
        WinLoseText.text = "You " + state;
        ReasonText.text = "Reason : " + reason;

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
