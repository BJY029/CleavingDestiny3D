using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonController : MatchController
{
    //버튼 요소
    [Header("Button Elements")]
    public Button MatchmakingBtn;
    public Button PlayWithAIBtn;
    public Button ExitBtn;

    private void Start()
    {
        MatchmakingBtn.onClick.AddListener(FindMatch);
        StopMatching.onClick.AddListener(CancelMatch);
        PlayWithAIBtn.onClick.AddListener(StartSoloplay);
        ExitBtn.onClick.AddListener(OnClickExitGame);

        if (LoadingPanel != null)
            LoadingPanel.transform.localScale = Vector3.zero;
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
