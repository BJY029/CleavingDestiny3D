using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Option;
using Photon.Pun;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Button GuideBookBtn;

    [Header("GuideBook")]
    public GuideBookUIController guideBookUIController;

    public Button warningYesBtn;
    public Button warningNoBtn;
    public TextMeshProUGUI warningYesBtnText;
    
    [Header("Player Status")]
    public TextMeshProUGUI[] playerNameTexts;
    public RectTransform currentTurnIndicator;

    private int warningStatus = 0; // 0: None, 1: Lobby, 2: Quit

    public bool IsSettingPanelOpened { get; private set; } = false;

    private void Start()
    {
        CloseBtn.onClick.AddListener(ToggleSettingPanel);
        LobbyBtn.onClick.AddListener(() => ShowWarningPanel(1));
        QuitGameBtn.onClick.AddListener(() => ShowWarningPanel(2));
        OptionBtn.onClick.AddListener(OnClickOption);
        GuideBookBtn.onClick.AddListener(OnClickGuideBook);
        
        warningYesBtn.onClick.AddListener(OnWarningYesClick);
        warningNoBtn.onClick.AddListener(OnWarningNoClick);

        warningPanel.gameObject.SetActive(false);

        TurnManager.OnTurnActorChanged += HandleTurnActorChanged;
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnActorChanged -= HandleTurnActorChanged;
    }

    private void HandleTurnActorChanged(int turnActorNumber)
    {
        Debug.Log($"[SettingCanvasController] HandleTurnActorChanged() 호출됨. turnActorNumber: {turnActorNumber}");
        CloseSettingPanel();
        UpdateCurrentTurnIndicator(turnActorNumber);
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

    private void OnClickGuideBook()
    {
        guideBookUIController?.ToggleGuideBook(true);
    }
    

    public void ToggleSettingPanel()
    {
        IsSettingPanelOpened = !IsSettingPanelOpened;
        if (IsSettingPanelOpened)
        {
            OpenSettingPanel();
        }
        else
        {
            CloseSettingPanel();
        }
    }

    public void OpenSettingPanel()
    {
        IsSettingPanelOpened = true;
        Background.SetActive(true);
        BattleLogController.Instance.ScrollToLatest();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PlayerManager.Instance != null && PlayerManager.Instance.Players != null && PlayerManager.Instance.Players.Count > 0)
        {
            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].gameObject.SetActive(false);
            }

            foreach (var rp in PlayerManager.Instance.Players.Values)
            {
                if (rp.turnIdx >= 0 && rp.turnIdx < playerNameTexts.Length)
                {
                    playerNameTexts[rp.turnIdx].gameObject.SetActive(true);
                    playerNameTexts[rp.turnIdx].SetText(rp.playerName);
                }
            }
        }
        else
        {
            var playerList = PhotonNetwork.PlayerList;
            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                if (i < playerList.Length)
                {
                    playerNameTexts[i].gameObject.SetActive(true);
                    if (string.IsNullOrEmpty(playerList[i].NickName))
                    {
                        playerNameTexts[i].SetText("Player {0}", playerList[i].ActorNumber);
                    }
                    else
                    {
                        playerNameTexts[i].SetText(playerList[i].NickName);
                    }

                }
                else
                {
                    playerNameTexts[i].gameObject.SetActive(false);
                }
            }
        }

        UpdateCurrentTurnIndicator();
    }

    public void CloseSettingPanel()
    {
        IsSettingPanelOpened = false;
        guideBookUIController?.ToggleGuideBook(false);
        Background.SetActive(false);

        if (OptionManager.Instance.IsOptionMenuActive())
        {
            OptionManager.Instance.SetOptionMenu(false);
        }

        bool isVillageSceneLoaded = SceneManager.GetSceneByName(CommonDefine.VILLAGESCENE).isLoaded;
        Cursor.lockState = isVillageSceneLoaded ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isVillageSceneLoaded;
    }
    
    void SetCurrentPlayerIndicator(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= playerNameTexts.Length)
        {
            currentTurnIndicator.gameObject.SetActive(false);
            return;
        }

        var posTransform = playerNameTexts[playerIndex].transform as RectTransform;
        currentTurnIndicator.gameObject.SetActive(true);
        if (posTransform != null)
            currentTurnIndicator.anchoredPosition = new Vector2(-75, posTransform.anchoredPosition.y);
    }

    public void UpdateCurrentTurnIndicator()
    {
        int turnActorNumber = GameHelper.getCurrentTurnActorNum();
        UpdateCurrentTurnIndicator(turnActorNumber);
    }

    public void UpdateCurrentTurnIndicator(int turnActorNumber)
    {
        int targetIndex = -1;

        if (PlayerManager.Instance != null && PlayerManager.Instance.Players != null)
        {
            if (PlayerManager.Instance.Players.TryGetValue(turnActorNumber, out var info))
            {
                targetIndex = info.turnIdx;
            }
        }
        else
        {
            var playerList = PhotonNetwork.PlayerList;
            for (int i = 0; i < playerList.Length; i++)
            {
                if (playerList[i].ActorNumber == turnActorNumber)
                {
                    targetIndex = i;
                    break;
                }
            }
        }

        SetCurrentPlayerIndicator(targetIndex);
    }
}
