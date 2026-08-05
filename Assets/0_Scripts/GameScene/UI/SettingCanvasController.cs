using System;
using Cysharp.Threading.Tasks;
using Option;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingCanvasController : MonoBehaviour
{
    public static SettingCanvasController instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Background.transform.localScale = Vector3.one;
        Background?.SetActive(false);
    }

    [Header("Panel")]
    public GameObject Background;

    public CanvasGroup warningPanel;

    [Header("Buttons")]
    public Button CloseBtn;

    public Button OptionBtn;
    public Button QuitGameBtn;
    public Button LobbyBtn;

    public Button warningYesBtn;
    public Button warningNoBtn;
    public TextMeshProUGUI warningYesBtnText;

    private int warningStatus = 0; // 0: None, 1: Lobby, 2: Quit

    public bool IsSettingPanelOpened { get; private set; } = false;

    private void Start()
    {
        CloseBtn.onClick.AddListener(ToggleSettingPanel);
        LobbyBtn.onClick.AddListener(() => ShowWarningPanel(1));
        QuitGameBtn.onClick.AddListener(() => ShowWarningPanel(2));
        OptionBtn.onClick.AddListener(OnClickOption);
        
        warningYesBtn.onClick.AddListener(OnWarningYesClick);
        warningNoBtn.onClick.AddListener(OnWarningNoClick);

        warningPanel.gameObject.SetActive(false);
    }
    
    private void ShowWarningPanel(int status)
    {
        warningStatus = status;
        warningPanel.gameObject.SetActive(true);
        warningPanel.interactable = false;

        Tween.ScaleX(warningPanel.transform, 0f, 1f, 0.5f)
            .Group(Tween.Alpha(warningPanel, 0f, 1f, 0.5f))
            .OnComplete(this, (con) => 
            { 
                con.WaitYesButtonActive().Forget();
                con.warningPanel.interactable = true;
            });
    }
    
    private void HideWarningPanel()
    {
        warningStatus = 0;
        warningPanel.interactable = false;
        Tween.ScaleX(warningPanel.transform, 1f, 0f, 0.5f)
            .Group(Tween.Alpha(warningPanel, 1f, 0f, 0.5f))
            .OnComplete(warningPanel, (panel) => panel.gameObject.SetActive(false));
    }

    private async UniTask WaitYesButtonActive()
    {
        int waitTime = 3;
        warningYesBtn.interactable = false;
        string originalText = warningYesBtnText.text;
        string countdownText = originalText + " ({0})";
        while (waitTime > 0 && warningStatus > 0)
        {
            warningYesBtnText.SetText(countdownText, waitTime);            
            await UniTask.WaitForSeconds(1);
            waitTime--;
        }

        warningYesBtnText.SetText(originalText);
        warningYesBtn.interactable = true;
    }

    private void OnWarningNoClick()
    {
        HideWarningPanel();
    }

    private void OnWarningYesClick()
    {
        if (warningStatus == 1)
        {
            GameExitHandler.Instance.RequestLeaveGame();
        }
        else if (warningStatus == 2)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    private void OnClickOption()
    {
        OptionManager.Instance.SetOptionMenu(true);
    }
    

    public void ToggleSettingPanel()
    {
        IsSettingPanelOpened = !IsSettingPanelOpened;
        Background.SetActive(IsSettingPanelOpened);
        if (IsSettingPanelOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (OptionManager.Instance.IsOptionMenuActive())
            {
                OptionManager.Instance.SetOptionMenu(false);
            }
        }
    }

    public void CloseSettingPanel()
    {
        IsSettingPanelOpened = false;
        Background.SetActive(false);
    }
}
