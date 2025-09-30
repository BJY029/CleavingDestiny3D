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
        
        if(LoadingPanel != null)
            LoadingPanel.transform.localScale = Vector3.zero;
	}	
}
