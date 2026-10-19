using System;
using System.Collections.Generic;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayWithFriendPanelManager : MonoBehaviourPunCallbacks
{
    // MatchController도 이 룸 속성을 읽어 친구 방의 자동 시작을 막고, 시작 신호를 처리한다.
    public const string FriendRoomProperty = "pwf";
    public const string StartedProperty = "pwfStarted";

    [Header("Menu Elements")]
    [SerializeField] private GameObject playWithFriendPanel;
    
    [Header("PWF Panel Elements")]
    [SerializeField] private GameObject pwfPanel;
    [SerializeField] private TMP_InputField roomCodeInputField;
    
    [Header("Room Elements")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Button startGameButton;    // 상대가 입장한 뒤 호스트만 사용 가능
    [SerializeField] private Button kickPlayerButton;   // 2인 방이므로 강퇴 대상은 상대 한 명
    [SerializeField] private Button leaveRoomButton;    // 시작 신호를 보내면 퇴장 차단
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TextMeshProUGUI playerNameText;    // 상대 닉네임, 미입장 시 "---"
    [SerializeField] private CanvasGroup copyNotificationCanvasGroup;   // 복사·오류 공용 알림
    [SerializeField] private TextMeshProUGUI notificationText;
    
    private Sequence copyNotificationTween;
    private readonly System.Random codeRandom = new System.Random();
    // Photon 요청은 비동기이므로, 응답을 기다리는 동안 중복 요청과 상충하는 입력을 막는다.
    private bool pending;    // 생성·입장·퇴장 요청의 응답 대기
    private bool creating;   // 생성 실패와 참가 실패 콜백을 구분
    private bool joined;     // 이 패널을 통해 친구 방 입장을 완료했는지 여부
    private bool exiting;    // 자발적 퇴장 또는 입장 요청 중 닫기
    private bool starting;   // 게임 시작 이후 시작·강퇴·퇴장 입력 차단
    private bool kicking;    // 상대의 실제 퇴장 콜백을 받을 때까지 시작 차단
    private int createAttempts;
    private string notificationTextID;
    private bool ownsRoom;
    private bool hostLeft;
    private const double KickCooldownSeconds = 10;
    private readonly Dictionary<string, double> kickedUsersUntil = new Dictionary<string, double>();
    private readonly Dictionary<string, double> blockedRoomsUntil = new Dictionary<string, double>();

    public override void OnEnable()
    {
        base.OnEnable();
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshLocalizedText;
        RefreshLocalizedText();
    }

    public override void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshLocalizedText;
        base.OnDisable();
    }

    private void RefreshLocalizedText()
    {
        // 표시 중인 알림과 기본 플레이어 이름도 언어 변경 시 함께 갱신한다.
        if (!string.IsNullOrEmpty(notificationTextID))
            notificationText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, notificationTextID);
        RefreshRoom();
    }

    private void Start()
    {
        playWithFriendPanel.SetActive(false);
        roomPanel.SetActive(false);
        ClearNotification();
    }
    
    private void ShowCopyNotification()
    {
        ShowNotification(UI_CSV.UI_PWF_CodeCopied, 1.5f);
    }

    private void ShowNotification(string textID, float displaySeconds)
    {
        if (copyNotificationTween.isAlive)
        {
            copyNotificationTween.Stop();
        }

        notificationTextID = textID;
        notificationText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, textID);
        copyNotificationCanvasGroup.alpha = 1f;
        copyNotificationTween = Sequence.Create(useUnscaledTime: true)
            .ChainDelay(displaySeconds)
            .Chain(Tween.Alpha(copyNotificationCanvasGroup, 0f, 0.7f, useUnscaledTime: true));
    }
    
    private void SetRoomPanel(bool showRoomPanel)
    {
        pwfPanel.SetActive(!showRoomPanel);
        roomPanel.SetActive(showRoomPanel);
    }

    private void ClearNotification()
    {
        if (copyNotificationTween.isAlive) copyNotificationTween.Stop();
        notificationTextID = null;
        notificationText.text = string.Empty;
        copyNotificationCanvasGroup.alpha = 0f;
    }

    private void ShowError(string textID)
    {
        ShowNotification(textID, 3f);
    }

    private bool CanEnterRoom()
    {
        if (pending || joined) return false;
        ClearNotification();
        if (PhotonNetwork.OfflineMode || !PhotonNetwork.IsConnectedAndReady || PhotonNetwork.InRoom)
        {
            ShowError(UI_CSV.UI_PWF_ConnectionRequired);
            return false;
        }
        exiting = false;
        return true;
    }

    private void CreateRoom()
    {
        createAttempts++;
        // 눈으로 구분하기 어려운 I, O, 0, 1은 초대코드에서 제외한다.
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var code = new char[6];
        for (int i = 0; i < code.Length; i++) code[i] = alphabet[codeRandom.Next(alphabet.Length)];

        if (!PhotonNetwork.CreateRoom(new string(code), new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = false, // 랜덤 매칭에서는 제외하되, 코드를 통한 입장은 허용
            IsOpen = true,
            PublishUserId = true, // 재입장으로 ActorNumber가 바뀌어도 같은 유저인지 확인한다.
            CustomRoomProperties = new Hashtable
            {
                { FriendRoomProperty, true },
                { StartedProperty, false }
            }
        }))
            FailEntry(UI_CSV.UI_PWF_CreateRequestFailed);
    }

    private void FailEntry(string textID)
    {
        pending = false;
        creating = false;
        if (!exiting) ShowError(textID);
    }

    private Player OtherPlayer()
    {
        foreach (var player in PhotonNetwork.PlayerList)
            if (!player.IsLocal) return player;
        return null;
    }

    private bool IsPlayerBlocked(Player player)
    {
        return player != null && !string.IsNullOrEmpty(player.UserId) &&
               kickedUsersUntil.TryGetValue(player.UserId, out double until) &&
               Time.realtimeSinceStartupAsDouble < until;
    }

    private void RefreshRoom()
    {
        if (!joined || !PhotonNetwork.InRoom) return;
        var other = OtherPlayer();
        playerNameText.text = other == null ? "---" :
            string.IsNullOrWhiteSpace(other.NickName) ?
                LocalizationManager.Instance.GetFormatText(CSV_Type.UI, UI_CSV.UI_PWF_PlayerName, other.ActorNumber) : other.NickName;
        // 일시적으로 연결이 끊긴 상대나 강퇴 처리 중인 상대를 두고 시작하지 않는다.
        bool canManage = PhotonNetwork.IsMasterClient && other != null &&
                         !other.IsInactive && !starting && !pending && !kicking && !hostLeft && !IsPlayerBlocked(other);
        startGameButton.interactable = canManage;
        kickPlayerButton.interactable = canManage;
        leaveRoomButton.interactable = !starting && !pending && !hostLeft;
    }

    public override void OnJoinedRoom()
    {
        // 같은 씬에서 일반 매칭·솔로 방의 콜백도 수신하므로 이 패널의 요청만 처리한다.
        if (!pending) return;
        bool createdRoom = creating;
        pending = false;
        creating = false;
        if (exiting)
        {
            // 요청 중 패널을 닫아도 서버 입장은 완료될 수 있으므로, 성공 응답 후 즉시 나간다.
            pending = PhotonNetwork.LeaveRoom();
            return;
        }
        if (!(PhotonNetwork.CurrentRoom.CustomProperties[FriendRoomProperty] is bool friendRoom) || !friendRoom)
        {
            pending = PhotonNetwork.LeaveRoom();
            ShowError(UI_CSV.UI_PWF_NotFriendRoom);
            return;
        }

        joined = true;
        ownsRoom = createdRoom;
        hostLeft = false;
        starting = false;
        kicking = false;
        // PUN 강퇴는 호스트의 전송뿐 아니라 상대 클라이언트의 수신에도 이 설정이 필요하다.
        PhotonNetwork.EnableCloseConnection = true;
        ClearNotification();
        roomCodeText.text = PhotonNetwork.CurrentRoom.Name;
        playWithFriendPanel.SetActive(true);
        SetRoomPanel(true);
        RefreshRoom();
        // 방장 퇴장 직후 입장해 이미 새 마스터가 된 경우에도 방을 이어받지 않는다.
        if (!ownsRoom && PhotonNetwork.IsMasterClient)
        {
            LeaveClosedRoom();
            return;
        }
        if (createdRoom) OnClickCodeText();
        // 입장 전에 설정된 속성은 변경 콜백이 오지 않을 수 있어 현재 값도 확인한다.
        if (PhotonNetwork.CurrentRoom.CustomProperties[StartedProperty] is true)
            OnRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (!creating) return;
        // 같은 코드의 방이 이미 있으면 새 코드로 재시도하되, 무한 재시도는 막는다.
        if (!exiting && returnCode == ErrorCode.GameIdAlreadyExists && createAttempts < 5)
        {
            CreateRoom();
            return;
        }
        FailEntry(UI_CSV.UI_PWF_CreateFailed);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (!pending || creating) return;
        FailEntry(returnCode == ErrorCode.GameFull ? UI_CSV.UI_PWF_RoomFull :
            returnCode == ErrorCode.GameClosed ? UI_CSV.UI_PWF_RoomClosed : UI_CSV.UI_PWF_JoinFailed);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 클라이언트의 입장 제한을 우회해도 호스트가 같은 UserId를 확인해 다시 내보낸다.
        // 자동 재강퇴는 원래의 제한 만료 시각을 연장하지 않는다.
        if (joined && ownsRoom && PhotonNetwork.IsMasterClient && IsPlayerBlocked(newPlayer))
            kicking = PhotonNetwork.CloseConnection(newPlayer);
        RefreshRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        kicking = false;
        RefreshRoom();
    }

    // Photon의 자동 방장 위임을 사용하지 않고, 원래 방장이 나가면 남은 유저도 퇴장한다.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (joined && !ownsRoom && !pending) LeaveClosedRoom();
    }

    private void LeaveClosedRoom()
    {
        hostLeft = true;
        starting = false;
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
        pending = PhotonNetwork.LeaveRoom();
        RefreshRoom();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) => RefreshRoom();

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // 여기서는 UI를 잠그고, 역할 배분과 씬 이동은 MatchController에서 처리한다.
        if (!joined || hostLeft || pending || !(propertiesThatChanged[StartedProperty] is true)) return;
        starting = true;
        RefreshRoom();
    }

    public override void OnLeftRoom()
    {
        if (!joined && !pending) return;
        bool removed = joined && !exiting; // 자발적 퇴장이 아니면 입력 패널로 돌아가 퇴장 안내
        if (removed && !hostLeft && !ownsRoom)
            blockedRoomsUntil[roomCodeText.text] = Time.realtimeSinceStartupAsDouble + KickCooldownSeconds;
        string notice = hostLeft ? UI_CSV.UI_PWF_HostLeft : UI_CSV.UI_PWF_Removed;
        ResetRoom();
        if (removed)
        {
            playWithFriendPanel.SetActive(true);
            ShowError(notice);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (!joined && !pending) return;
        bool showError = !exiting;
        ResetRoom();
        if (showError)
        {
            playWithFriendPanel.SetActive(true);
            ShowError(UI_CSV.UI_PWF_Disconnected);
        }
    }

    private void ResetRoom()
    {
        pending = creating = joined = starting = kicking = false;
        ownsRoom = hostLeft = false;
        kickedUsersUntil.Clear();
        PhotonNetwork.EnableCloseConnection = false;
        roomCodeText.text = string.Empty;
        playerNameText.text = "---";
        startGameButton.interactable = kickPlayerButton.interactable = false;
        leaveRoomButton.interactable = true;
        SetRoomPanel(false);
    }
    
    // --- 인스펙터의 버튼 OnClick에 연결하는 함수 ---
    
    public void OnClickCreateRoomButton()
    {
        if (!CanEnterRoom()) return;
        pending = creating = true;
        createAttempts = 0;
        CreateRoom();
    }
    
    public void OnClickJoinRoomButton()
    {
        if (!CanEnterRoom()) return;
        string code = roomCodeInputField.text.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(code))
        {
            ShowError(UI_CSV.UI_PWF_CodeRequired);
            return;
        }
        // 패널을 닫았다 열어도 이 방의 강퇴 제한은 유지한다. 다른 방은 바로 참가할 수 있다.
        if (blockedRoomsUntil.TryGetValue(code, out double until) && Time.realtimeSinceStartupAsDouble < until)
        {
            ShowError(UI_CSV.UI_PWF_RejoinCooldown);
            return;
        }
        pending = true;
        // JoinOrCreateRoom을 쓰면 오타로 새 방이 생성될 수 있으므로 참가만 요청한다.
        if (!PhotonNetwork.JoinRoom(code))
            FailEntry(UI_CSV.UI_PWF_JoinRequestFailed);
    }
    
    public void OnClickStartGameButton()
    {
        if (!joined || !PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient ||
            pending || starting || kicking || hostLeft || PhotonNetwork.CurrentRoom.PlayerCount != 2 ||
            OtherPlayer() == null || OtherPlayer().IsInactive || IsPlayerBlocked(OtherPlayer())) return;

        starting = true;
        RefreshRoom();
        // 추가 입장을 닫은 뒤, 두 클라이언트가 같은 룸 속성 변경으로 시작 흐름에 진입한다.
        PhotonNetwork.CurrentRoom.IsOpen = false;
        if (!PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { StartedProperty, true } }))
        {
            // 시작 요청을 전송하지 못했다면 방을 다시 열고 호스트가 재시도할 수 있게 한다.
            starting = false;
            PhotonNetwork.CurrentRoom.IsOpen = true;
            RefreshRoom();
        }
    }
    
    public void OnClickKickPlayerButton()
    {
        if (!joined || !PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient ||
            pending || starting || kicking || hostLeft) return;
        var other = OtherPlayer();
        if (other == null) return;
        // 요청 성공만으로 목록에서 지우지 않고 OnPlayerLeftRoom에서 실제 퇴장을 확인한다.
        kicking = PhotonNetwork.CloseConnection(other);
        if (kicking && !string.IsNullOrEmpty(other.UserId))
            kickedUsersUntil[other.UserId] = Time.realtimeSinceStartupAsDouble + KickCooldownSeconds;
        RefreshRoom();
    }
    
    public void OnClickExitButton()
    {
        if (starting) return;
        exiting = true;
        playWithFriendPanel.SetActive(false);
        if (joined && PhotonNetwork.InRoom)
        {
            // 방장이 나가는 동안 새 유저가 들어오지 못하게 먼저 닫는다.
            if (ownsRoom) PhotonNetwork.CurrentRoom.IsOpen = false;
            pending = PhotonNetwork.LeaveRoom();
            if (!pending)
            {
                exiting = false;
                if (ownsRoom) PhotonNetwork.CurrentRoom.IsOpen = true;
                playWithFriendPanel.SetActive(true);
                RefreshRoom();
            }
        }
        else if (!pending) ResetRoom();
    }
    
    public void OnClickPlayWithFriendButton()
    {
        if (pending) return;
        if (PhotonNetwork.InRoom && !joined) return;
        if (joined)
        {
            playWithFriendPanel.SetActive(true);
            SetRoomPanel(true);
            return;
        }
        exiting = false;
        SetRoomPanel(false);

        roomCodeInputField.text = string.Empty;
        ClearNotification();
        
        startGameButton.interactable = false;
        kickPlayerButton.interactable = false;
        
        playWithFriendPanel.SetActive(true);
    }

    public void OnClickCodeText()
    {
        if (!joined || !PhotonNetwork.InRoom) return;
        GUIUtility.systemCopyBuffer = roomCodeText.text;
        ShowCopyNotification();
    }

    private void OnDestroy()
    {
        if (joined) PhotonNetwork.EnableCloseConnection = false;
        if (copyNotificationTween.isAlive) copyNotificationTween.Stop();
    }
}
