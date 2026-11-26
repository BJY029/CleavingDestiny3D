using System;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Village;
using Village.Building;
using Village.Outside;

/// <summary>
/// GameScene에서 GameManager.Instance.isTutorial == true일 때
/// 1일차 한 사이클(규칙 안내 -> 선공 강제 -> 조작/Tab 아이템 -> 천막 강조 -> 나무 공격 -> 상대턴/나뭇가지 줍기 -> 3턴 안내 -> 밤 마을 -> 완료)을 안내한다.
/// </summary>
public class TutorialGuideController : MonoBehaviourPunCallbacks
{
    public static TutorialGuideController Instance { get; private set; }

    private enum TutorialStep
    {
        None,
        MapIntroduction,
        TurnSelection,
        TurnResult,
        DayNeedItem,
        DayTreeBlockedWarning,
        DayGoToTent,
        DayEnergyGuide,
        DayUseBlockedWarning,
        DayEnergySkipAttack,
        DayCanAttack,
        DayWaitOpponent,
        DayPickBranch,
        DayThreeTurnsNotice,
        NightVillage,
        NightTreeAttack,
        Completed,
        Dismissed
    }

    [Header("Center Modal Popup (규칙 / 조작 / 천막 확인 팝업 공용)")]
    [SerializeField] private GameObject introPopupPanel;
    [SerializeField] private TextMeshProUGUI introTitleText;
    [SerializeField] private TextMeshProUGUI introDescText;
    [SerializeField] private Button introConfirmButton;

    [Header("Banner UI (진행 중 가이드 배너)")]
    [SerializeField] private GameObject guideBannerPanel;
    [SerializeField] private TextMeshProUGUI guideTitleText;
    [SerializeField] private TextMeshProUGUI guideDescText;

    [Header("Complete Popup (1일차 완료 팝업)")]
    [SerializeField] private GameObject completePopupPanel;
    [SerializeField] private TextMeshProUGUI completeTitleText;
    [SerializeField] private TextMeshProUGUI completeDescText;
    [SerializeField] private Button returnToLobbyButton;
    [SerializeField] private Button continuePlayButton;

    [Header("Timing")]
    [SerializeField] private float opponentToBranchDelay = 7.5f;
    [SerializeField] private float branchToThreeTurnsDelay = 7.0f;

    public bool CanAttackTree => !enabled || _treeAttackUnlocked;
    public bool IsModalOpen =>
        (introPopupPanel != null && introPopupPanel.activeSelf) ||
        (completePopupPanel != null && completePopupPanel.activeSelf);

    private TutorialStep _currentStep = TutorialStep.None;
    private Action _pendingModalConfirm;
    private bool _treeAttackUnlocked;
    private bool _hasShownDayIntro;
    private bool _hasAcquiredFirstItem;
    private bool _hasLookedAtTentItem;
    private bool _hasUsedFirstItem;
    private bool _hasShownPostAttackSequence;
    private bool _hasEnteredNightVillage;
    private bool _hasEnteredNightTreeAttack;
    private VillageType? _currentVillageBuilding;
    private bool _isVillageOutside;

    private Coroutine _postAttackRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (introPopupPanel != null) introPopupPanel.SetActive(false);
        if (guideBannerPanel != null) guideBannerPanel.SetActive(false);
        if (completePopupPanel != null) completePopupPanel.SetActive(false);

        if (GameManager.Instance == null || !GameManager.Instance.isTutorial)
        {
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (introConfirmButton != null)
            introConfirmButton.onClick.AddListener(OnClickModalConfirm);

        if (returnToLobbyButton != null)
            returnToLobbyButton.onClick.AddListener(OnClickReturnToLobby);

        if (continuePlayButton != null)
            continuePlayButton.onClick.AddListener(OnClickContinuePlay);

        TurnManager.OnTurnActorChanged += HandleTurnActorChanged;
        ItemOfferCanvasController.OnOfferItemSelected += HandleOfferItemSelected;
        WorldInventorySlot.OnLocalPlayerLookedAtOwnItem += HandleLookedAtOwnItem;
        WorldInventorySlot.OnLocalPlayerUsedItem += HandleUsedOwnItem;
        WorldInventorySlot.OnLocalPlayerItemUseBlocked += HandleItemUseBlocked;
        VillageBuildingManager.OnBuildingChangedGlobal += HandleVillageBuildingChanged;
        VillageOutsideManager.OnOutsideStateChangedGlobal += HandleVillageOutsideChanged;

        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshLocalizedTexts;

        RefreshCompletePopupTexts();
        SyncInitialState();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;

        TurnManager.OnTurnActorChanged -= HandleTurnActorChanged;
        ItemOfferCanvasController.OnOfferItemSelected -= HandleOfferItemSelected;
        WorldInventorySlot.OnLocalPlayerLookedAtOwnItem -= HandleLookedAtOwnItem;
        WorldInventorySlot.OnLocalPlayerUsedItem -= HandleUsedOwnItem;
        WorldInventorySlot.OnLocalPlayerItemUseBlocked -= HandleItemUseBlocked;
        VillageBuildingManager.OnBuildingChangedGlobal -= HandleVillageBuildingChanged;
        VillageOutsideManager.OnOutsideStateChangedGlobal -= HandleVillageOutsideChanged;

        if (Time.timeScale == 0f) Time.timeScale = 1f;

        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= RefreshLocalizedTexts;

        if (introConfirmButton != null)
            introConfirmButton.onClick.RemoveListener(OnClickModalConfirm);
        if (returnToLobbyButton != null)
            returnToLobbyButton.onClick.RemoveListener(OnClickReturnToLobby);
        if (continuePlayButton != null)
            continuePlayButton.onClick.RemoveListener(OnClickContinuePlay);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (_currentStep == TutorialStep.Completed || _currentStep == TutorialStep.Dismissed)
            return;

        if (propertiesThatChanged.ContainsKey(GameStartRoomKeys.Phase))
        {
            EvaluateStartPhase();
        }

        if (propertiesThatChanged.ContainsKey(RoomPropKeys.GamePhase) ||
            propertiesThatChanged.ContainsKey(RoomPropKeys.IsVillageUpgradePhase))
        {
            EvaluateMainGamePhase();
        }
    }

    public void NotifyTreeAttackBlocked()
    {
        if (!enabled || _treeAttackUnlocked) return;

        AudioManager.Instance?.PlaySfx2D("ui_button");

        if (_hasAcquiredFirstItem)
            SetStep(TutorialStep.DayUseBlockedWarning);
        else
            SetStep(TutorialStep.DayTreeBlockedWarning);
    }

    private void SyncInitialState()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) return;

        EvaluateStartPhase();
        EvaluateMainGamePhase();
    }

    private GameStartPhase GetStartPhase()
    {
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameStartRoomKeys.Phase, out object phaseValue))
        {
            return (GameStartPhase)Convert.ToByte(phaseValue);
        }

        return GameStarter.instance != null ? GameStarter.instance.CurrentPhase : GameStartPhase.None;
    }

    private void EvaluateStartPhase()
    {
        switch (GetStartPhase())
        {
            case GameStartPhase.MapIntroduction:
                SetStep(TutorialStep.MapIntroduction);
                break;
            case GameStartPhase.TurnSelection:
                SetStep(TutorialStep.TurnSelection);
                break;
            case GameStartPhase.TurnResult:
                SetStep(TutorialStep.TurnResult);
                break;
            case GameStartPhase.MainGame:
                HandleTurnActorChanged(PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor));
                break;
        }
    }

    private void EvaluateMainGamePhase()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) return;

        GamePhaseValue phase = PhotonPropertyHelper.GetRoomProp<GamePhaseValue>(RoomPropKeys.GamePhase);

        if (phase == GamePhaseValue.END)
        {
            DismissTutorialGuide();
            return;
        }

        if (phase == GamePhaseValue.NIGHT_VILLAGE)
        {
            StopPostAttackSequence();
            _treeAttackUnlocked = true;
            _hasEnteredNightVillage = true;
            _currentVillageBuilding = null;
            _isVillageOutside = false;
            SetStep(TutorialStep.NightVillage);
            return;
        }

        if (phase == GamePhaseValue.NIGHT_TREEATK)
        {
            _hasEnteredNightTreeAttack = true;
            SetStep(TutorialStep.NightTreeAttack);
            return;
        }

        if (phase == GamePhaseValue.DAY && _hasEnteredNightVillage && _hasEnteredNightTreeAttack)
        {
            CompleteTutorial();
        }
    }

    private void HandleTurnActorChanged(int currentTurnActor)
    {
        if (_currentStep == TutorialStep.Completed || _currentStep == TutorialStep.Dismissed)
            return;

        if (GetStartPhase() != GameStartPhase.MainGame)
            return;

        GamePhaseValue phase = PhotonPropertyHelper.GetRoomProp<GamePhaseValue>(RoomPropKeys.GamePhase);
        if (phase == GamePhaseValue.NIGHT_VILLAGE || phase == GamePhaseValue.NIGHT_TREEATK || phase == GamePhaseValue.END)
            return;

        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;

        if (currentTurnActor == localActor)
        {
            if (!_hasShownDayIntro)
            {
                _hasShownDayIntro = true;
                _treeAttackUnlocked = false;
                SetStep(TutorialStep.DayNeedItem);
            }
            else if (_hasShownPostAttackSequence && _currentStep != TutorialStep.DayThreeTurnsNotice)
            {
                StopPostAttackSequence();
                SetStep(TutorialStep.DayThreeTurnsNotice);
            }
        }
        else if (currentTurnActor > 0 && _hasShownDayIntro && !_hasShownPostAttackSequence)
        {
            _hasShownPostAttackSequence = true;
            StartPostAttackSequence();
        }
    }

    private void LateUpdate()
    {
        if (completePopupPanel != null && completePopupPanel.activeSelf)
        {
            if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private void HandleOfferItemSelected(string itemId)
    {
        if (_currentStep == TutorialStep.Completed || _currentStep == TutorialStep.Dismissed)
            return;

        if (_hasAcquiredFirstItem) return;

        _hasAcquiredFirstItem = true;

        if (_hasLookedAtTentItem)
            SetStep(TutorialStep.DayEnergyGuide);
        else
            SetStep(TutorialStep.DayGoToTent);
    }

    private void HandleLookedAtOwnItem()
    {
        if (_currentStep == TutorialStep.Completed || _currentStep == TutorialStep.Dismissed)
            return;
        if (!_hasAcquiredFirstItem || _hasUsedFirstItem || _hasLookedAtTentItem)
            return;

        _hasLookedAtTentItem = true;
        SetStep(TutorialStep.DayEnergyGuide);
    }

    private void HandleItemUseBlocked(string reason)
    {
        if (_currentStep == TutorialStep.Completed || _currentStep == TutorialStep.Dismissed)
            return;
        if (!_hasAcquiredFirstItem || _hasUsedFirstItem)
            return;

        _hasUsedFirstItem = true;
        _treeAttackUnlocked = true;
        SetStep(TutorialStep.DayEnergySkipAttack);
    }

    private void HandleUsedOwnItem()
    {
        if (_currentStep == TutorialStep.Completed || _currentStep == TutorialStep.Dismissed)
            return;
        if (!_hasAcquiredFirstItem || _hasUsedFirstItem)
            return;

        _hasUsedFirstItem = true;
        _treeAttackUnlocked = true;
        SetStep(TutorialStep.DayCanAttack);
    }

    private void HandleVillageBuildingChanged(VillageType? buildingType)
    {
        if (_currentStep != TutorialStep.NightVillage) return;

        _currentVillageBuilding = buildingType;
        ApplyNightVillageBanner();
    }

    private void HandleVillageOutsideChanged(bool isOutside)
    {
        if (_currentStep != TutorialStep.NightVillage) return;

        _isVillageOutside = isOutside;
        if (isOutside) _currentVillageBuilding = null;
        ApplyNightVillageBanner();
    }

    private void ApplyNightVillageBanner()
    {
        if (_isVillageOutside)
        {
            ShowBanner(
                GetLocalized("Stage_NightOutside_Title", "나무의 공격 · 피해 시스템"),
                GetLocalized("Stage_NightOutside_Desc", "밤이 끝나면 나무가 모든 마을을 공격합니다. 공격 피해는 방어막만큼 먼저 막아낸 뒤 남은 피해만 마을 체력을 깎습니다. 정비가 끝나면 준비 완료를 누르세요!")
            );
            return;
        }

        if (_currentVillageBuilding.HasValue)
        {
            switch (_currentVillageBuilding.Value)
            {
                case VillageType.Mine:
                    ShowBanner(
                        GetLocalized("Stage_NightMine_Title", "광산"),
                        GetLocalized("Stage_NightMine_Desc", "매일 아침 얻는 기본 골드 수입을 늘려 이후 강화 자금을 확보합니다.")
                    );
                    return;
                case VillageType.Forge:
                    ShowBanner(
                        GetLocalized("Stage_NightForge_Title", "대장간"),
                        GetLocalized("Stage_NightForge_Desc", "도끼 기본 공격의 최소·최대 피해 범위를 강화합니다.")
                    );
                    return;
                case VillageType.Shop:
                    ShowBanner(
                        GetLocalized("Stage_NightShop_Title", "상점"),
                        GetLocalized("Stage_NightShop_Desc", "골드로 아이템을 구매하거나 목록을 갱신할 수 있습니다. 레벨이 오르면 높은 등급이 등장합니다.")
                    );
                    return;
                case VillageType.Farm:
                    ShowBanner(
                        GetLocalized("Stage_NightFarm_Title", "농장"),
                        GetLocalized("Stage_NightFarm_Desc", "최대 기력과 매일 회복되는 기력을 함께 증가시킵니다.")
                    );
                    return;
                case VillageType.Barrier:
                    ShowBanner(
                        GetLocalized("Stage_NightBarrier_Title", "마법사의 탑"),
                        GetLocalized("Stage_NightBarrier_Desc", "마을 방어력을 높입니다. 방어력은 매 턴 방어막을 제공합니다.")
                    );
                    return;
            }
        }

        ShowBanner(
            GetLocalized("Stage_NightVillage_Title", "밤 - 마을 정비"),
            GetLocalized("Stage_NightVillage_Desc", "골드로 마을 시설(대장간·농장·광산·마법사의 탑·상점)을 강화해 대비하세요. 정비가 끝나면 준비 완료를 누르세요.")
        );
    }

    private void StartPostAttackSequence()
    {
        StopPostAttackSequence();
        _postAttackRoutine = StartCoroutine(PostAttackGuideRoutine());
    }

    private void StopPostAttackSequence()
    {
        if (_postAttackRoutine != null)
        {
            StopCoroutine(_postAttackRoutine);
            _postAttackRoutine = null;
        }
    }

    private IEnumerator PostAttackGuideRoutine()
    {
        SetStep(TutorialStep.DayWaitOpponent);
        yield return new WaitForSeconds(opponentToBranchDelay);

        if (_currentStep == TutorialStep.DayWaitOpponent)
        {
            SetStep(TutorialStep.DayPickBranch);
            yield return new WaitForSeconds(branchToThreeTurnsDelay);

            if (_currentStep == TutorialStep.DayPickBranch)
            {
                SetStep(TutorialStep.DayThreeTurnsNotice);
            }
        }

        _postAttackRoutine = null;
    }

    private void SetStep(TutorialStep step)
    {
        if (_currentStep == step || _currentStep == TutorialStep.Completed || _currentStep == TutorialStep.Dismissed)
            return;

        _currentStep = step;
        ApplyStepUI(step);
    }

    private void ApplyStepUI(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.MapIntroduction:
                ShowCenterModal(
                    GetLocalized("Stage_MapIntro_Title", "규칙 안내"),
                    GetLocalized("Stage_MapIntro_Desc", "중앙의 나무를 공격해 마을 방어력을 쌓고, 밤마다 찾아오는 나무의 공격을 버텨내야 합니다. 단, 나무에 마지막 일격을 가하면 패배합니다!"),
                    () => GameStarter.instance?.CompleteTutorialMapIntroduction()
                );
                break;

            case TutorialStep.TurnSelection:
                HideCenterModal(lockCursorIfMainGame: false);
                ShowBanner(
                    GetLocalized("Stage_TurnSelect_Title", "선공 정하기"),
                    GetLocalized("Stage_TurnSelect_Desc", "더 긴 나뭇가지를 뽑은 플레이어가 먼저 행동합니다. 나뭇가지 하나를 선택하세요.")
                );
                break;

            case TutorialStep.TurnResult:
                HideCenterModal(lockCursorIfMainGame: false);
                ShowBanner(
                    GetLocalized("Stage_TurnResult_Title", "선공 결정"),
                    GetLocalized("Stage_TurnResult_Desc", "이번에는 나뭇가지 결과와 상관없이 먼저 공격하겠습니다.")
                );
                break;

            case TutorialStep.DayNeedItem:
                ShowBanner(
                    GetLocalized("Stage_DayItem_Title", "기본 조작 · 아이템 획득"),
                    GetLocalized("Stage_DayItem_Desc", "WASD로 이동하고 Shift로 달릴 수 있습니다. 먼저 Tab 키를 눌러 아이템을 획득하세요! (획득 전 공격 불가)")
                );
                break;

            case TutorialStep.DayTreeBlockedWarning:
                ShowBanner(
                    GetLocalized("Stage_DayItemBlocked_Title", "아이템을 먼저 획득하세요!"),
                    GetLocalized("Stage_DayItemBlocked_Desc", "아직 나무를 공격할 수 없습니다! Tab 키를 눌러 먼저 아이템을 획득하세요.")
                );
                break;

            case TutorialStep.DayGoToTent:
                ShowBanner(
                    GetLocalized("Stage_DayGoToTent_Title", "빨간색 천막 · 아이템 사용"),
                    GetLocalized("Stage_DayGoToTent_Desc", "빨간색 천막 안의 아이템 앞으로 이동해 사용해 보세요! (아이템 사용 전에는 나무를 공격할 수 없습니다)")
                );
                break;

            case TutorialStep.DayEnergyGuide:
                ShowBanner(
                    GetLocalized("Stage_DayEnergy_Title", "기력 · 아이템 사용"),
                    GetLocalized("Stage_DayEnergy_Desc", "아이템 사용과 나무 공격에는 기력이 소모됩니다. 하단 기력을 확인하고 F키로 아이템을 사용하세요!")
                );
                break;

            case TutorialStep.DayUseBlockedWarning:
                ShowBanner(
                    GetLocalized("Stage_DayUseBlocked_Title", "아이템을 먼저 사용하세요!"),
                    GetLocalized("Stage_DayUseBlocked_Desc", "아직 나무를 공격할 수 없습니다! 빨간색 천막에서 먼저 아이템을 사용하세요.")
                );
                break;

            case TutorialStep.DayEnergySkipAttack:
                ShowBanner(
                    GetLocalized("Stage_DayEnergySkip_Title", "기력 부족 · 나무 공격"),
                    GetLocalized("Stage_DayEnergySkip_Desc", "선택한 아이템은 기력이 부족해 사용할 수 없습니다. 바로 중앙의 나무에 다가가 F키로 공격하세요!")
                );
                break;

            case TutorialStep.DayCanAttack:
                ShowBanner(
                    GetLocalized("Stage_DayAction_Title", "나무 공격"),
                    GetLocalized("Stage_DayAction_Desc", "이제 중앙의 나무에 다가가 F키로 공격하세요! (단, 나무에 마지막 일격을 가하면 패배합니다)")
                );
                break;

            case TutorialStep.DayWaitOpponent:
                ShowBanner(
                    GetLocalized("Stage_DayWait_Title", "상대 턴 진행 중"),
                    GetLocalized("Stage_DayWait_Desc", "공격 피해의 일부가 마을 방어막으로 전환됩니다. 이제 상대(AI)의 턴이 진행됩니다.")
                );
                break;

            case TutorialStep.DayPickBranch:
                ShowBanner(
                    GetLocalized("Stage_DayPickBranch_Title", "나뭇가지 줍기"),
                    GetLocalized("Stage_DayPickBranch_Desc", "필드에 떨어진 나뭇가지를 주워 보세요! 나뭇가지는 재화로 활용할 수 있습니다.")
                );
                break;

            case TutorialStep.DayThreeTurnsNotice:
                ShowBanner(
                    GetLocalized("Stage_DayThreeTurns_Title", "마을 진입 안내"),
                    GetLocalized("Stage_DayThreeTurns_Desc", "서로 번갈아 3턴씩 공격을 마치면 밤이 되어 마을로 진입합니다. 마을 진입까지 계속 플레이해 보세요!")
                );
                break;

            case TutorialStep.NightVillage:
                ApplyNightVillageBanner();
                break;

            case TutorialStep.NightTreeAttack:
                ShowBanner(
                    GetLocalized("Stage_NightTreeAtk_Title", "밤 - 나무의 공격"),
                    GetLocalized("Stage_NightTreeAtk_Desc", "나무가 마을을 공격합니다! 쌓아둔 방어막만큼 피해를 막아냅니다.")
                );
                break;
        }
    }

    private void ShowCenterModal(string title, string description, Action onConfirm)
    {
        if (guideBannerPanel != null) guideBannerPanel.SetActive(false);

        if (introPopupPanel != null)
        {
            _pendingModalConfirm = onConfirm;
            if (introTitleText != null) introTitleText.text = title;
            if (introDescText != null) introDescText.text = description;
            SetButtonText(introConfirmButton, "Stage_Intro_ConfirmBtn", "확인");

            introPopupPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ShowBanner(title, description);
            onConfirm?.Invoke();
        }
    }

    private void HideCenterModal(bool lockCursorIfMainGame)
    {
        if (introPopupPanel != null) introPopupPanel.SetActive(false);

        if (lockCursorIfMainGame &&
            GetStartPhase() == GameStartPhase.MainGame &&
            (ItemOfferCanvasController.instance == null || !ItemOfferCanvasController.instance.isOfferPanelOpened) &&
            (ItemSelectionController.instance == null || !ItemSelectionController.instance.IsItemSelectionActivated))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnClickModalConfirm()
    {
        AudioManager.Instance?.PlaySfx2D("ui_button");

        Action callback = _pendingModalConfirm;
        _pendingModalConfirm = null;

        HideCenterModal(lockCursorIfMainGame: true);
        callback?.Invoke();
    }

    private void RefreshLocalizedTexts()
    {
        RefreshCompletePopupTexts();

        if (introPopupPanel != null && introPopupPanel.activeSelf)
            ApplyStepUI(_currentStep);
        else if (guideBannerPanel != null && guideBannerPanel.activeSelf)
            ApplyStepUI(_currentStep);
    }

    private void RefreshCompletePopupTexts()
    {
        if (completeTitleText != null)
            completeTitleText.text = GetLocalized("Stage_Complete_Title", "튜토리얼 완료!");

        if (completeDescText != null)
            completeDescText.text = GetLocalized("Stage_Complete_Desc", "튜토리얼이 완료되었습니다. 세부 정보는 언제든 G 키(가이드북)에서 확인할 수 있습니다.");

        SetButtonText(returnToLobbyButton, "Stage_Complete_LobbyBtn", "로비로 나가기");
        SetButtonText(continuePlayButton, "Stage_Complete_ContinueBtn", "계속 플레이");
    }

    private void SetButtonText(Button button, string key, string fallback)
    {
        if (button == null) return;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>();
        if (label != null) label.text = GetLocalized(key, fallback);
    }

    private string GetLocalized(string key, string fallback)
    {
        if (LocalizationManager.Instance == null || !LocalizationManager.Instance.IsLoaded)
            return fallback;

        string text = LocalizationManager.Instance.GetText(CSV_Type.Tutorial, key);
        return string.IsNullOrEmpty(text) ? fallback : text;
    }

    private void ShowBanner(string title, string description)
    {
        if (guideBannerPanel != null) guideBannerPanel.SetActive(true);
        if (guideTitleText != null) guideTitleText.text = title;
        if (guideDescText != null) guideDescText.text = description;
    }

    private void CompleteTutorial()
    {
        _currentStep = TutorialStep.Completed;
        StopPostAttackSequence();

        if (introPopupPanel != null) introPopupPanel.SetActive(false);
        if (guideBannerPanel != null) guideBannerPanel.SetActive(false);

        if (completePopupPanel != null)
        {
            RefreshCompletePopupTexts();
            completePopupPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void DismissTutorialGuide()
    {
        _currentStep = TutorialStep.Dismissed;
        _treeAttackUnlocked = true;
        StopPostAttackSequence();
        Time.timeScale = 1f;

        if (introPopupPanel != null) introPopupPanel.SetActive(false);
        if (guideBannerPanel != null) guideBannerPanel.SetActive(false);
        if (completePopupPanel != null) completePopupPanel.SetActive(false);
    }

    private void OnClickContinuePlay()
    {
        Time.timeScale = 1f;
        if (completePopupPanel != null) completePopupPanel.SetActive(false);

        if ((ItemOfferCanvasController.instance == null || !ItemOfferCanvasController.instance.isOfferPanelOpened) &&
            (ItemSelectionController.instance == null || !ItemSelectionController.instance.IsItemSelectionActivated))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnClickReturnToLobby()
    {
        Time.timeScale = 1f;
        if (GameExitHandler.Instance != null)
        {
            GameExitHandler.Instance.RequestLeaveGame();
        }
    }
}
