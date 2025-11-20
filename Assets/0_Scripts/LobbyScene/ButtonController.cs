using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

		SetButtonText(MatchmakingBtn, "UI_PVP");
		SetButtonText(PlayWithAIBtn, "UI_PVE");
		SetButtonText(ExitBtn, "UI_EXIT");
        if(LoadingPanel != null)
            LoadingPanel.transform.localScale = Vector3.zero;
	}	

    void SetButtonText(Button btn, string textID)
    {
        Text text = btn.GetComponentInChildren<Text>();
        if(text != null)
        {
            text.text = LocalizationManager.Instance.GetText(textID);
            return;
        }
    }
}
