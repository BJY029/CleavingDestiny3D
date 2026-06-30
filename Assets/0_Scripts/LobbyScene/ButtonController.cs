using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Option;

public class ButtonController : MatchController
{
    public static ButtonController Instance;

    //버튼 요소
    [Header("Button Elements")]
    public Button MatchmakingBtn;
    public Button PlayWithAIBtn;
    public Button ExitBtn;
    public Button OptionBtn;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        Debug.Log("[ButtonController] Start() 시작됨.");
        
        base.Start();

        MatchmakingBtn.onClick.AddListener(FindMatch);
        StopMatching.onClick.AddListener(CancelMatch);
        PlayWithAIBtn.onClick.AddListener(StartSoloplay);
        ExitBtn.onClick.AddListener(OnClickExitGame);
        OptionBtn.onClick.AddListener(OnClickOption);

        if (LoadingPanel != null)
            LoadingPanel.transform.localScale = Vector3.zero;

        // 시작 시 로비 접속 대기 상태이므로 버튼 임시 비활성화
        SetButtonsInteractable(false);
        Debug.Log("[ButtonController] 리스너 등록 완료 및 버튼 임시 비활성화 처리됨.");
    }

    public void SetButtonsInteractable(bool state)
    {
        if (MatchmakingBtn != null) MatchmakingBtn.interactable = state;
        if (PlayWithAIBtn != null) PlayWithAIBtn.interactable = state;
    }

    void OnClickExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnClickOption()
    {
        OptionManager.Instance.SetOptionMenu(true);
    }
}
