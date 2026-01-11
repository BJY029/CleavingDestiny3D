using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MatchController
{
    //버튼 요소
    [Header("Button Eelements")]
    public Button MatchmakingBtn;
    public Button PlayWithAIBtn;
    public Button ExitBtn;

	private void Start()
	{
        MatchmakingBtn.GetComponent<Button>().onClick.AddListener(FindMatch);
        StopMatching.GetComponent<Button>().onClick.AddListener(CancelMatch);
        ExitBtn.onClick.AddListener(OnClickExitGame);

        SetButtonText(MatchmakingBtn, UI_CSV.UI_PVP);
		SetButtonText(PlayWithAIBtn, UI_CSV.UI_PVE);
		SetButtonText(ExitBtn, UI_CSV.UI_EXIT);
        if(LoadingPanel != null)
            LoadingPanel.transform.localScale = Vector3.zero;
	}	

    void SetButtonText(Button btn, string textID)
    {
        TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
        if(text != null)
        {
            text.text = LocalizationManager.Instance.GetText(CSV_Type.UI, textID);
            return;
        }
    }

    void OnClickExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
