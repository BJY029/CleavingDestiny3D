using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class BettingSystemUIController : MonoBehaviourPunCallbacks
{
    [Header("Backgrond UI")]
    public GameObject BG;

    //요청자 화면에 뜨는 UI
    [Header("Request Panel UIs")]
    public Slider Req_Timer;
    public GameObject Req_Panel;
    public TextMeshProUGUI Req_Title;
    public TextMeshProUGUI Req_Desc1;
    public TextMeshProUGUI Req_Desc2;
    public TextMeshProUGUI Req_BettableEnergy;
    public TMP_InputField Req_EnertyToBet;
    public Button Req_Btn;

    //수신자 화면에 뜨는 UI
    [Header("ReceivePanel")]
    public Slider Rec_Timer;
    public GameObject Rec_Panel;
    public TextMeshProUGUI Rec_Title;
    public TextMeshProUGUI Rec_Desc1;
    public TextMeshProUGUI Rec_Desc2;
    public TextMeshProUGUI Rec_Desc3;
    public TextMeshProUGUI Rec_Desc4;
    public TextMeshProUGUI Rec__WageredAmount;
    public TextMeshProUGUI Rec_EnergyValue;
    public TextMeshProUGUI Rec_VillageHPValue;
    public Button Rec_AcceptBtn;
    public Button Rec_RejectBtn;

    //상대방의 결정을 기다릴 때 뜨는 UI
    [Header("WaitingPanel")]
    public GameObject W_Panel;
    public TextMeshProUGUI W_Title;
    public TextMeshProUGUI W_Desc;
    public TextMeshProUGUI W_EnergyValue;

    private enum BettingUITimerType
    {
        None, Request, Receive
    }

    private BettingUITimerType currentTimerType = BettingUITimerType.None;
    private bool isUITimerRunning = false;
    private double uiTimerStartTime = -1d;
    private float uiTimerDuration = 0f;

    //현재 처리중인 Player 정보 저장
    private int curReqActorNumber = -1;
    private int curTarActorNumber = -1;

    private void Start()
    {
        BG.SetActive(false);
        Req_Panel.SetActive(false);
        Rec_Panel.SetActive(false);
        W_Panel.SetActive(false);
    }

    private void Update()
    {
        UpdateBettingTimerSlider();
    }

    public void Master_StartRequestTimer(int requestActorNumber, double startTime, float duration)
    {
        Player requestPlayer = GetPlayer(requestActorNumber);

        if (requestPlayer == null)
        {
            Debug.LogWarning("[BettingUI] 요청자 플레이어를 찾을 수 없습니다.");
            return;
        }

        photonView.RPC(nameof(RPC_StartBettingUITimer), requestPlayer, (int)BettingUITimerType.Request, startTime, duration);
    }

    public void Master_StartReceiveTimer(int targetActorNumber, double startTime, float duration)
    {
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (targetPlayer == null)
        {
            Debug.LogWarning("[BettingUI] 수신자 플레이어를 찾을 수 없습니다.");
            return;
        }

        photonView.RPC(nameof(RPC_StartBettingUITimer), targetPlayer, (int)BettingUITimerType.Receive, startTime, duration);
    }

    public void Master_StopBettingTimer(int requestActorNumber, int targetActorNumber)
    {
        Player requestPlayer = GetPlayer(requestActorNumber);
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (requestPlayer != null)
        {
            photonView.RPC(nameof(RPC_StopBettingUITimer), requestPlayer);
        }

        if (targetPlayer != null)
        {
            photonView.RPC(nameof(RPC_StopBettingUITimer), targetPlayer);
        }
    }

    private void UpdateBettingTimerSlider()
    {
        if (!isUITimerRunning) return;

        float elapsed = Mathf.Max(0f, (float)(PhotonNetwork.Time - uiTimerStartTime));
        float remaining = Mathf.Clamp(uiTimerDuration - elapsed, 0f, uiTimerDuration);

        Slider targetSlider = null;

        if (currentTimerType == BettingUITimerType.Request) targetSlider = Req_Timer;
        else if (currentTimerType == BettingUITimerType.Receive) targetSlider = Rec_Timer;

        if (targetSlider == null) return;

        targetSlider.maxValue = uiTimerDuration;
        targetSlider.value = remaining;

        if (remaining <= 0f) StopLocalUITimer();
    }

    private void StartLocalUITimer(BettingUITimerType timerType, double startTime, float duration)
    {
        currentTimerType = timerType;
        uiTimerStartTime = startTime;
        uiTimerDuration = Mathf.Max(0.01f, duration);
        isUITimerRunning = true;

        Req_Timer.gameObject.SetActive(timerType == BettingUITimerType.Request);
        Rec_Timer.gameObject.SetActive(timerType == BettingUITimerType.Receive);

        if (timerType == BettingUITimerType.Request)
        {
            Req_Timer.maxValue = uiTimerDuration;
            Req_Timer.value = uiTimerDuration;
        }
        else if (timerType == BettingUITimerType.Receive)
        {
            Rec_Timer.maxValue = uiTimerDuration;
            Rec_Timer.value = uiTimerDuration;
        }
    }

    private void StopLocalUITimer()
    {
        isUITimerRunning = false;
        currentTimerType = BettingUITimerType.None;
        uiTimerStartTime = -1d;
        uiTimerDuration = 0f;

        Req_Timer.value = 0f;
        Rec_Timer.value = 0f;
    }

    private Player GetPlayer(int actorNumber)
    {
        if (!PhotonNetwork.InRoom) return null;
        return PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
    }

    //배팅 게임 시작 시의 UI 처리
    public void Master_StartBetGame(int requestActorNumber, int targetActorNumber, int curEnergy)
    {
        Player requestPlayer = GetPlayer(requestActorNumber);
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (requestPlayer == null || targetPlayer == null)
        {
            Debug.LogWarning("[BettingUI] 플레이어를 찾을 수 없습니다.");
            return;
        }

        photonView.RPC(nameof(Init_SetActive_ReqPanel), requestPlayer, requestActorNumber, targetActorNumber, curEnergy);
        photonView.RPC(nameof(Init_SetActive_WPanel), targetPlayer, false, 0);
    }

    //배팅 게임에서 요청이 발생했을 때 UI 처리
    public void Master_RequestSended(int requestActorNumber, int targetActorNumber, int tarCurEnergy, int wageredEnergy, int gainEnergy, float villageBasicHP)
    {
        Player requestPlayer = GetPlayer(requestActorNumber);
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (requestPlayer == null || targetPlayer == null)
        {
            Debug.LogWarning("[BettingUI] 플레이어를 찾을 수 없습니다.");
            return;
        }

        photonView.RPC(nameof(Init_SetActive_WPanel), requestPlayer, true, gainEnergy);
        photonView.RPC(nameof(Init_SetActive_RecPanel), targetPlayer, requestActorNumber, targetActorNumber, tarCurEnergy, wageredEnergy, gainEnergy, villageBasicHP);
    }

    //UI 닫는 함수
    public void Master_CloseBetPanels(int requestActorNumber, int targetActorNumber, bool isAutoDecline, int declineReward)
    {
        Player requestPlayer = GetPlayer(requestActorNumber);
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (requestPlayer != null)
        {
            photonView.RPC(nameof(RPC_CloseBetPanels), requestPlayer, true, isAutoDecline, declineReward);
        }

        if (targetPlayer != null)
        {
            photonView.RPC(nameof(RPC_CloseBetPanels), targetPlayer, false, isAutoDecline, declineReward);
        }
    }

    //요청 버튼에 연결되는 함수
    public void Req_Btn_OnClicked()
    {
        if (!int.TryParse(Req_EnertyToBet.text, out int betEngValue))
        {
            Debug.LogWarning("[Betting] 배팅 기력 입력값이 올바르지 않습니다.");
            return;
        }

        if (betEngValue <= 0)
        {
            Debug.LogWarning("[Betting] 배팅 기력은 1 이상이어야 합니다.");
            return;
        }

        BettingSystemController.instance.RequestVerification(curReqActorNumber, curTarActorNumber, betEngValue);
    }

    //수락 버튼에 연결되는 함수
    public void Res_AcceptBtn_OnClicked()
    {
        if (!int.TryParse(Rec__WageredAmount.text, out int betEngValue))
        {
            Debug.LogWarning("[Betting] 배팅 기력값이 올바르지 않습니다.");
            return;
        }

        BettingSystemController.instance.StartLogWoodGame(curReqActorNumber, curTarActorNumber, betEngValue);
    }

    //거절 버튼에 연결되는 함수
    public void Res_RejectBtn_OnClicked()
    {
        if (!int.TryParse(Rec__WageredAmount.text, out int betEngValue))
        {
            Debug.LogWarning("[Betting] 배팅 기력값이 올바르지 않습니다.");
            return;
        }

        BettingSystemController.instance.RejectBettingGame(curReqActorNumber, curTarActorNumber, betEngValue, false);
    }

    [PunRPC]
    private void RPC_StartBettingUITimer(int timerType, double startTime, float duration)
    {
        StartLocalUITimer((BettingUITimerType)timerType, startTime, duration);
    }

    [PunRPC]
    private void RPC_StopBettingUITimer()
    {
        StopLocalUITimer();
    }

    [PunRPC]
    private void Init_SetActive_ReqPanel(int reqActorNumber, int tarActorNumber, int curEnergy)
    {
        Req_Title.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_Req_Title);
        Req_Desc1.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_Req_Desc1);
        Req_Desc2.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_Req_Desc2);

        Req_BettableEnergy.text = curEnergy.ToString();

        Req_EnertyToBet.text = "";

        Req_Btn.onClick.RemoveAllListeners();
        Req_Btn.onClick.AddListener(Req_Btn_OnClicked);

        curReqActorNumber = reqActorNumber;
        curTarActorNumber = tarActorNumber;

        Req_Panel.SetActive(true);
        BG.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    [PunRPC]
    private void Init_SetActive_RecPanel(int reqActorNumber, int tarActorNumber, int tarCurEnergy, int wageredEnergy, int gainEnergy, float villageBasicHP)
    {
        W_Panel.SetActive(false);

        Rec_Title.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_Rec_Title);
        Rec_Desc1.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_Rec_Desc1);
        Rec_Desc2.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_Rec_Desc2);
        Rec_Desc3.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_Rec_Desc3);

        if (tarCurEnergy < wageredEnergy)
        {
            Rec_Desc4.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_Rec_Desc4);
            Rec_VillageHPValue.text = ((wageredEnergy - tarCurEnergy) * villageBasicHP).ToString();

            Rec_Desc4.gameObject.SetActive(true);
            Rec_VillageHPValue.gameObject.SetActive(true);
        }
        else
        {
            Rec_Desc4.gameObject.SetActive(false);
            Rec_VillageHPValue.gameObject.SetActive(false);
        }

        Rec__WageredAmount.text = wageredEnergy.ToString();
        Rec_EnergyValue.text = gainEnergy.ToString();

        Rec_AcceptBtn.onClick.RemoveAllListeners();
        Rec_AcceptBtn.onClick.AddListener(Res_AcceptBtn_OnClicked);

        Rec_RejectBtn.onClick.RemoveAllListeners();
        Rec_RejectBtn.onClick.AddListener(Res_RejectBtn_OnClicked);

        curReqActorNumber = reqActorNumber;
        curTarActorNumber = tarActorNumber;

        Rec_Panel.SetActive(true);
        BG.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    [PunRPC]
    private void Init_SetActive_WPanel(bool isRequester, int gainEnergy)
    {
        Req_Panel.SetActive(false);
        Rec_Panel.SetActive(false);

        StopLocalUITimer();

        W_Title.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_W_Title);

        if (isRequester)
        {
            W_Desc.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_W_Desc1);
            W_EnergyValue.text = gainEnergy.ToString();
        }
        else
        {
            W_Desc.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemBet_W_Desc2);
            W_EnergyValue.text = "";
        }

        W_Panel.SetActive(true);
        BG.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [PunRPC]
    private void RPC_CloseBetPanels(bool isRequester, bool isAutoDecline, int declineReward)
    {
        Req_Panel.SetActive(false);
        Rec_Panel.SetActive(false);
        W_Panel.SetActive(false);
        BG.SetActive(false);

        StopLocalUITimer();

        curReqActorNumber = -1;
        curTarActorNumber = -1;

        if (isRequester)
        {
            Debug.Log($"[Betting] 상대가 거절했습니다. 보상 기력: {declineReward}");
        }
        else
        {
            Debug.Log(isAutoDecline ? "[Betting] 기력 부족으로 자동 거절되었습니다." : "[Betting] 결투 요청을 거절했습니다.");
        }
    }
}