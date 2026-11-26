using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Option;

public class ButtonController : MatchController
{
    public static ButtonController Instance;

    private const string TutorialGuideSeenKey = "HasSeenTutorialGuide";

    //버튼 요소
    [Header("Button Elements")]
    public Button MatchmakingBtn;
    public Button PlayWithAIBtn;
    public Button ShopBtn;
    public Button ExitBtn;
    public Button OptionBtn;

    [Header("Tutorial Guide Panel")]
    [SerializeField] private GameObject tutorialGuidePanel;

    private Button boundStopMatching;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        Debug.Log("[ButtonController] Start() 시작됨.");

        base.Start();

        MatchmakingBtn.onClick.AddListener(FindMatch);
        MatchmakingBtn.onClick.AddListener(PlayButtonClickSound);

        boundStopMatching = StopMatching;
        boundStopMatching.onClick.AddListener(PlayButtonClickSound);
        boundStopMatching.onClick.AddListener(CancelMatch);

        PlayWithAIBtn.onClick.AddListener(PlayButtonClickSound);
        PlayWithAIBtn.onClick.AddListener(OnClickPlayWithAI);

        ShopBtn.onClick.AddListener(PlayButtonClickSound);
        ShopBtn.onClick.AddListener(OnClickShop);

        ExitBtn.onClick.AddListener(PlayButtonClickSound);
        ExitBtn.onClick.AddListener(OnClickExitGame);

        OptionBtn.onClick.AddListener(PlayButtonClickSound);
        OptionBtn.onClick.AddListener(OnClickOption);

        // 시작 시 로비 접속 대기 상태이므로 버튼 임시 비활성화
        SetButtonsInteractable(false);
        Debug.Log("[ButtonController] 리스너 등록 완료 및 버튼 임시 비활성화 처리됨.");
    }

    private void PlayButtonClickSound()
    {
        //     Debug.Log(
        //     $"[Button Sound] PlayButtonClickSound 호출됨\n" +
        //     $"{System.Environment.StackTrace}"
        // );
        AudioManager.Instance.PlaySfx2D("ui_button");
    }

    protected override void OnDestroy()
    {
        if (boundStopMatching != null)
        {
            boundStopMatching.onClick.RemoveListener(PlayButtonClickSound);
            boundStopMatching.onClick.RemoveListener(CancelMatch);
        }
        base.OnDestroy();
    }

    public void SetButtonsInteractable(bool state)
    {
        if (MatchmakingBtn != null) MatchmakingBtn.interactable = state;
        if (PlayWithAIBtn != null) PlayWithAIBtn.interactable = state;
    }

    private void OnClickExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnClickOption()
    {
        OptionManager.Instance.SetOptionMenu(true);
    }

    private void OnClickPlayWithAI()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.isTutorial = false;

        StartSoloplay();
    }

    /// <summary>
    /// Play 패널의 ToggleCanvas.OnOpened 콜백에 연결: 최초 오픈 시 튜토리얼 가이드 패널을 연다.
    /// </summary>
    public void TryOpenTutorialGuide()
    {
        if (tutorialGuidePanel == null) return;
        if (PlayerPrefs.GetInt(TutorialGuideSeenKey, 0) == 1) return;

        tutorialGuidePanel.SetActive(true);
    }

    /// <summary>
    /// 튜토리얼 가이드 패널의 Close 버튼에 연결: 안내 확인 처리 후 패널을 닫는다.
    /// </summary>
    public void OnClickCloseTutorialGuide()
    {
        PlayButtonClickSound();
        MarkTutorialGuideSeen();

        if (tutorialGuidePanel != null)
            tutorialGuidePanel.SetActive(false);
    }

    /// <summary>
    /// 로비의 튜토리얼 버튼 및 튜토리얼 가이드 패널의 Play 버튼에 연결: 튜토리얼 모드로 AI전을 시작한다.
    /// </summary>
    public void OnClickPlayTutorial()
    {
        PlayButtonClickSound();
        MarkTutorialGuideSeen();

        if (tutorialGuidePanel != null)
            tutorialGuidePanel.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.isTutorial = true;

        StartSoloplay();
    }

    private void MarkTutorialGuideSeen()
    {
        if (PlayerPrefs.GetInt(TutorialGuideSeenKey, 0) == 1) return;

        PlayerPrefs.SetInt(TutorialGuideSeenKey, 1);
        PlayerPrefs.Save();
    }

    private void OnClickShop()
    {
        ShopUIManager.Instance.EnterShopUI();
    }
}
