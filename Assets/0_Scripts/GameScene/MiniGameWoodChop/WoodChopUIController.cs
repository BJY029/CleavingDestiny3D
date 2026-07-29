using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class WoodChopUIController : MonoBehaviourPunCallbacks
{
    [Header("UI Infos")]
    public GameObject TurnInfoPanel;
    public TextMeshProUGUI TurnValue;
    public TextMeshProUGUI Desc;
    public TextMeshProUGUI GameResult;
    public Slider TimeSlider;

    [Header("Hide Canvas During Minigame")]
    [SerializeField] private GameObject[] canvasToHideDuriongMiniGame;
    private bool[] canvasOriginalActivateStates;
    private bool canvasStatesSaved = false;

    private enum TurnUIType
    {
        None, MyTurn, OpTurn
    }

    private TurnUIType curTurnUIType = TurnUIType.None;

    private bool isUITimerRunning = false;
    private double uiTimerStartTime = -1d;
    private float uiTimerDuration = 0f;

    private Player GetPlayer(int actorNumber)
    {
        if (!PhotonNetwork.InRoom) return null;
        return PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
    }

    private void Start()
    {
        TurnInfoPanel.SetActive(false);
    }

    private void Update()
    {
        UpdateTimerSlider();
    }

    public void Master_SetCurTurnUI(int actNum, double startTime, float duration)
    {
        Player player = GetPlayer(actNum);

        if (player == null) return;

        photonView.RPC(nameof(InitUI), player, (int)TurnUIType.MyTurn, startTime, duration);
    }

    public void Master_SetOpTurnUI(int actNum)
    {
        Player player = GetPlayer(actNum);

        if (player == null) return;

        photonView.RPC(nameof(InitUI), player, (int)TurnUIType.OpTurn, 0d, 0f);
    }

    private void UpdateTimerSlider()
    {
        if (!isUITimerRunning) return;

        float elapsed = Mathf.Max(0f, (float)(PhotonNetwork.Time - uiTimerStartTime));
        float remaining = Mathf.Clamp(uiTimerDuration - elapsed, 0f, uiTimerDuration);

        TimeSlider.maxValue = uiTimerDuration;
        TimeSlider.value = remaining;
        if (remaining <= 0f) StopLocalUITimer();
    }

    [PunRPC]
    private void InitUI(int UIType, double startTime, float duration)
    {
        TurnUIType uiType = (TurnUIType)UIType;
        GameResult.text = "";
        if (uiType == TurnUIType.MyTurn)
        {
            curTurnUIType = uiType;
            uiTimerStartTime = startTime;
            uiTimerDuration = duration;
            isUITimerRunning = true;

            TimeSlider.maxValue = uiTimerDuration;
            TimeSlider.value = uiTimerDuration;

            TurnValue.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_LogWood_MyTurn);
            Desc.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_LogWood_Interact);
            TimeSlider.gameObject.SetActive(true);
        }
        else if (uiType == TurnUIType.OpTurn)
        {
            TurnValue.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_LogWood_OpTurn);
            Desc.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_LogWood_Wait);
            TimeSlider.gameObject.SetActive(false);
        }
        TurnInfoPanel.SetActive(true);
    }

    public void SetWinLoseUI(bool isWin, int energy, float villageDmg = 0f)
    {
        if (isWin)
        {
            TurnValue.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_LogWood_Result_Win_Title);
            Desc.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_LogWood_Result_Win_Desc);
            GameResult.text = "ENERGY + " + energy;
            GameResult.color = Color.lightBlue;
        }
        else
        {
            TurnValue.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_LogWood_Result_Lose_Title);
            Desc.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_LogWood_Result_Lose_Desc);
            string addText = villageDmg > 0 ? " VillageHP - " + villageDmg : "";
            GameResult.text = "ENERGY - " + energy + addText;
            GameResult.color = Color.red;
        }

        TimeSlider.gameObject.SetActive(false);
        TurnInfoPanel.SetActive(true);
    }

    public void UIOff()
    {
        StopLocalUITimer();
        TurnInfoPanel.SetActive(false);
    }

    private void StopLocalUITimer()
    {
        isUITimerRunning = false;
        curTurnUIType = TurnUIType.None;
        uiTimerStartTime = -1d;
        uiTimerDuration = 0f;
        TimeSlider.value = 0f;
    }

    public void HideCanvasForMiniGame()
    {
        if (canvasToHideDuriongMiniGame == null || canvasToHideDuriongMiniGame.Length == 0) return;

        canvasOriginalActivateStates = new bool[canvasToHideDuriongMiniGame.Length];

        for (int i = 0; i < canvasToHideDuriongMiniGame.Length; i++)
        {
            if (canvasToHideDuriongMiniGame[i] == null) continue;

            canvasOriginalActivateStates[i] = canvasToHideDuriongMiniGame[i].activeSelf;
            canvasToHideDuriongMiniGame[i].SetActive(false);
        }

        canvasStatesSaved = true;
    }

    public void RestoreCanvasAfterMiniGame()
    {
        if (!canvasStatesSaved) return;
        if (canvasToHideDuriongMiniGame == null || canvasToHideDuriongMiniGame.Length == 0) return;

        for (int i = 0; i < canvasToHideDuriongMiniGame.Length; i++)
        {
            if (canvasToHideDuriongMiniGame[i] == null) continue;
            if (i >= canvasOriginalActivateStates.Length) continue;

            canvasToHideDuriongMiniGame[i].SetActive(canvasOriginalActivateStates[i]);
        }
        canvasStatesSaved = false;
    }
}
