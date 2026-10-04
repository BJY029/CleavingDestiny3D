using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using PrimeTween;
using UnityEngine.Serialization;

public class PlayerCanvasController : MonoBehaviourPunCallbacks
{
	public static PlayerCanvasController Instance;
	//캔버스를 껴고 킬때 사용할 캔버스 그룹
	private CanvasGroup canvasGroup;

	[Header("UI")]
	public TextMeshProUGUI EnergyValue;
	public TextMeshProUGUI VillageHP;
	public Slider VillageHPSlider;
	public Slider ShieldValueSlider;
	public Slider AddShieldValueSlider;
	public TextMeshProUGUI DamageValue;
	public TextMeshProUGUI BarrierValue;
	public TextMeshProUGUI TreeMultValue;
	public TextMeshProUGUI MyTurnText;
	public CanvasGroup HitTextObj;

	[SerializeField] private GameObject gaugeRoot;
	[SerializeField] private Slider gaugeSlider;
	public TextMeshProUGUI minDamage;
	public TextMeshProUGUI maxDamage;

	[Header("Gauge")]
	[SerializeField] private float speed = 1.8f;

	[Header("Warning")]
	[SerializeField] private GameObject WarningObj;
	private TextMeshProUGUI WarningText;
	private Animator WarningTextAnim;

	[Header("Aim UI Tween")]
	[Tooltip("아이템을 수령하지 않고 나무를 조준할 때 표시할 경고 UI")]
	[SerializeField] private CanvasGroup unclaimedItemWarningObj;
	private RectTransform unclaimedWarningRect;
	[FormerlySerializedAs("unclaimedWarningShowDuration")]
	[SerializeField, Min(0.01f)] private float warningShowDuration = 0.42f;
	[FormerlySerializedAs("unclaimedWarningHideDuration")]
	[SerializeField, Min(0.01f)] private float warningHideDuration = 0.25f;
	[FormerlySerializedAs("unclaimedWarningSlideDistance")]
	[SerializeField] private float warningSlideDistance = 100f;
	private Vector2 unclaimedWarningPosition;
	private Sequence unclaimedWarningTween;
	private bool unclaimedWarningVisible;
	private bool isLookingAtTree;

	[Header("ItemNotifyHolder")]
	[SerializeField] private GameObject Holder;

	[Header("Timer")]
	public GameObject TimerObj;
	public TextMeshProUGUI TimerText;
	public Image ProgressRing;
	public float duration;

	[Header("Branch")]
	public TextMeshProUGUI BranchCount;
	public GameObject BranchInteractObj;

	[Header("Prefabs")]
	public GameObject ItemNotifyPrefab;
	public GameObject ItemStolenNotifyPrefab;

	[Header("Mission Info")]
	public GameObject MissionPanel;
	public TextMeshProUGUI MissionState;
	public TextMeshProUGUI MissionName;
	public TextMeshProUGUI MissionContext;
	public Color ReadyState;
	public Color StartState;
	public Color SuccessState;
	public Color FailedState;
	private Coroutine gaugeCo;
	//플레이어의 Hit 관련 UI가 활성화되었는지 여부
	[HideInInspector]
	public bool selecting;

	private TextMeshProUGUI HitText;
	private RectTransform hitTextRect;
	private Vector2 hitTextPosition;
	private Sequence hitTextTween;
	private bool hitTextVisible;


	private ItemNotifyController INC;

	private float _startTime = -1f;
	private float _endTime = -1f;
	private int _lastTimerSec = -1;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		if (HitTextObj != null) HitText = HitTextObj.GetComponentInChildren<TextMeshProUGUI>(true);
		WarningText = WarningObj.GetComponentInChildren<TextMeshProUGUI>();

		WarningTextAnim = WarningObj.GetComponent<Animator>();
		canvasGroup = GetComponent<CanvasGroup>();

		hitTextPosition = InitializeTweenUI(HitTextObj, out hitTextRect);
		WarningObj.SetActive(false);
		unclaimedWarningPosition = InitializeTweenUI(unclaimedItemWarningObj, out unclaimedWarningRect);
		MissionPanel.SetActive(false);
		BranchInteractObj.SetActive(false);
		MyTurnText.gameObject.SetActive(false);
		CloseGauge();
		if (HitText != null) HitText.text = "";
		WarningText.text = "";
		InitTimer();
	}


	public override void OnEnable()
	{
		base.OnEnable();

		GameSessionData.OnBranchCountChanged += UpdateBranchCount;
		UpdateBranchCount(GameSessionData.CollectedBranchCount);
		KeyInteractManager.OnKeyBindingsChanged += UpdateGameHitText;
	}

	public override void OnDisable()
	{
		base.OnDisable();

		GameSessionData.OnBranchCountChanged -= UpdateBranchCount;
		KeyInteractManager.OnKeyBindingsChanged -= UpdateGameHitText;
		isLookingAtTree = false;
		unclaimedWarningVisible = false;
		unclaimedWarningTween.Stop();
		if (unclaimedItemWarningObj != null) unclaimedItemWarningObj.alpha = 0f;
		if (unclaimedWarningRect != null) unclaimedWarningRect.anchoredPosition = unclaimedWarningPosition;
		unclaimedItemWarningObj?.gameObject.SetActive(false);
		hitTextVisible = false;
		hitTextTween.Stop();
		if (HitTextObj != null) HitTextObj.alpha = 0f;
		if (hitTextRect != null) hitTextRect.anchoredPosition = hitTextPosition;
		HitTextObj?.gameObject.SetActive(false);
	}



	//만약, 현재 타이머가 설정되었고, 시작 시간 또한 초기화 된 경우
	private void Update()
	{
		if (!TimeManager.instance.TurnTimerActivated || _startTime == -1f) return;

		//시간 계산 수행
		float remainTime = _endTime - (float)PhotonNetwork.Time;

		if (remainTime < 0)
		{
			remainTime = 0;
			InitTimer();
		}

		int curSecond = Mathf.CeilToInt(remainTime);
		if (curSecond != _lastTimerSec)
		{
			TimerText.SetText("{0}", curSecond);
			_lastTimerSec = curSecond;
		}
		ProgressRing.fillAmount = duration > 0f ? Mathf.Clamp01(remainTime / duration) : 0f;
	}

	public void MyTurnActive()
	{
		MyTurnText.gameObject.SetActive(true);
		AudioManager.Instance.PlaySfx2D("MyTurn");
	}

	private void InitTimer()
	{
		TimerText.text = "";
		_startTime = -1f;
		_endTime = -1f;
		_lastTimerSec = -1;
	}

	private void UpdateBranchCount(int count)
	{
		BranchCount.text = count.ToString();
	}

	//타이머가 설정되면, 시작, 끝 시간을 받아와서 저장한다.
	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		UpdateUnclaimedItemWarning();
		if (propertiesThatChanged.TryGetValue(RoomPropKeys.PlayerTurnStartEndTime, out var value))
		{
			if (value is Vector2 times)
			{
				_startTime = times.x;
				_endTime = times.y;
			}
		}

		if (propertiesThatChanged.TryGetValue(RoomPropKeys.TurnTime, out var time))
		{
			duration = (float)time;
		}
	}


	//턴 정보에 따라서 Hit Text를 변경하는 함수
	public void UpdateGameHitText()
	{
		//Hit 텍스트가 활성화 되어있고
		if (hitTextVisible && HitText != null)
		{
			//내 턴이면
			if (GameHelper.IsMyTurn())
			{
				//내 턴에 해당되는 텍스트로 변경
				HitText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_PlayerHit);
				OpenGauge();
			}
			else
			{
				HitText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_PlayerNHit);
				CloseGauge();
			}
		}
	}

	//Hit 데미지 게이지 열기 
	public void OpenGauge()
	{
		gaugeRoot.SetActive(true);
		selecting = true;
		// maxDamage.text = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MaxAtkPow).ToString();
		maxDamage.SetText("{0}", PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MaxAtkPow));
		// minDamage.text = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MinAtkPow).ToString();
		minDamage.SetText("{0}", PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.MinAtkPow));
		if (gaugeCo != null) StopCoroutine(gaugeCo);
		gaugeCo = StartCoroutine(GaugeLoop());
	}

	//Hit 데미지 게이지 닫기
	public void CloseGauge()
	{
		selecting = false;
		if (gaugeCo != null) StopCoroutine(gaugeCo);
		gaugeCo = null;
		gaugeRoot.SetActive(false);
	}

	//데미지 게이지 값을 변경하는 루프 코루틴
	private IEnumerator GaugeLoop()
	{
		float t = gaugeSlider.maxValue; // 0~1 범위에서 시작

		while (selecting)
		{
			t -= Time.deltaTime * speed;   // speed가 클수록 빨리 내려감
			if (t <= gaugeSlider.minValue) t = gaugeSlider.maxValue;

			gaugeSlider.value = t;        // 그대로 1 -> 0
			yield return null;
		}
	}

	//특정 시점에 hit가 눌리면, 해당 시점의 데미지 게이지 값을 반환
	public float SelectNow()
	{
		if (!selecting) return -1;

		selecting = false;
		if (gaugeCo != null) StopCoroutine(gaugeCo);

		return gaugeSlider.maxValue - gaugeSlider.value;
	}


	//Hit text를 활성화 하는 함수
	public void SetHitTextActive()
	{
		if (HitTextObj == null || HitText == null) return;
		if (!hitTextVisible)
		{
			hitTextVisible = true;
			hitTextTween.Stop();
			hitTextTween = ShowTweenUI(HitTextObj, hitTextRect, hitTextPosition);
		}
		//내 턴인 경우
		if (GameHelper.IsMyTurn())
		{
			//내 턴에 해당되는 텍스트로 변경
			HitText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_PlayerHit);
			// OpenGauge();
		}
		else
		{
			//내 턴이 아니면, 내 턴이 아니라는 텍스트로 변경
			HitText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_PlayerNHit);
			CloseGauge();
		}
	}

	//Hit Text를 비활성화 하는 함수
	public void SetHitTextUnActive()
	{
		CloseGauge();
		if (!hitTextVisible || HitTextObj == null) return;
		hitTextVisible = false;
		hitTextTween.Stop();
		hitTextTween = Sequence.Create(useUnscaledTime: true)
			.Group(Tween.Alpha(HitTextObj, 0f, warningHideDuration, Ease.InOutSine))
			.OnComplete(this, controller =>
			{
				controller.HitTextObj.gameObject.SetActive(false);
				controller.HitText.text = "";
			});
	}

	public void SetLookingAtTree(bool looking)
	{
		isLookingAtTree = looking;
		UpdateUnclaimedItemWarning();
	}

	private void UpdateUnclaimedItemWarning()
	{
		if (unclaimedItemWarningObj == null) return;

		bool show = false;
		if (isLookingAtTree && PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer != null)
		{
			int actor = PhotonNetwork.LocalPlayer.ActorNumber;
			string offer = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.OFFER(actor));
			show = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor) == actor
				&& !string.IsNullOrEmpty(offer) && offer != ERROR.FULL_INV.ToString();
		}

		if (show == unclaimedWarningVisible) return;
		unclaimedWarningVisible = show;
		unclaimedWarningTween.Stop();

		if (show)
		{
			unclaimedWarningTween = ShowTweenUI(unclaimedItemWarningObj, unclaimedWarningRect, unclaimedWarningPosition);
		}
		else
		{
			unclaimedWarningTween = Sequence.Create(useUnscaledTime: true)
				.Group(Tween.Alpha(unclaimedItemWarningObj, 0f, warningHideDuration, Ease.InOutSine))
				.OnComplete(unclaimedItemWarningObj, group => group.gameObject.SetActive(false));
		}
	}

	public void SetWarningTextActive(string textId)
	{
		WarningObj.SetActive(true);
		WarningText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, textId);
		WarningTextAnim.Play("UI_Player_Warning_Up");
	}

	private static Vector2 InitializeTweenUI(CanvasGroup group, out RectTransform rect)
	{
		rect = null;
		if (group == null) return Vector2.zero;
		rect = group.GetComponent<RectTransform>();
		if (group.TryGetComponent<Animator>(out var animator)) animator.enabled = false;
		group.alpha = 0f;
		group.interactable = false;
		group.blocksRaycasts = false;
		group.gameObject.SetActive(false);
		return rect != null ? rect.anchoredPosition : Vector2.zero;
	}

	private Sequence ShowTweenUI(CanvasGroup group, RectTransform rect, Vector2 position)
	{
		if (!group.gameObject.activeSelf && rect != null)
			rect.anchoredPosition = position + Vector2.down * warningSlideDistance;
		group.gameObject.SetActive(true);
		return Sequence.Create(useUnscaledTime: true)
			.Group(Tween.Alpha(group, 1f, warningShowDuration, Ease.InOutSine))
			.Group(rect != null
				? Tween.UIAnchoredPositionY(rect, position.y, warningShowDuration, Ease.OutBack)
				: default);
	}

	public void PopUpItemNotify(string itemId, Player player)
	{
		photonView.RPC(nameof(RPC_PopUpItemNotify), RpcTarget.All, itemId, player);
	}

	public void PopUpItemStolenNotify(string itemId, Player FromPlayer, Player ToPlayer)
	{
		photonView.RPC(nameof(RPC_PopUpItemStolenNotify), RpcTarget.All, itemId, FromPlayer, ToPlayer);
	}

	[PunRPC]
	public void RPC_PopUpItemNotify(string itemId, Player player)
	{
		GameObject Notify = Instantiate(ItemNotifyPrefab, Holder.transform);
		INC = Notify?.GetComponent<ItemNotifyController>();

		INC.SetActive(ItemDB.Instance.Get(itemId), player);
	}

	[PunRPC]
	public void RPC_PopUpItemStolenNotify(string itemId, Player FromPlayer, Player ToPlayer)
	{
		GameObject Notify = Instantiate(ItemStolenNotifyPrefab, Holder.transform);
		INC = Notify?.GetComponent<ItemNotifyController>();

		INC.SetStolenActive(ItemDB.Instance.Get(itemId), FromPlayer, ToPlayer);
	}

	//현재 플레이어 상태 UI를 업데이트 하는 함수
	public void updatePlayerStatus(float Energy, float MaxEnergy, float HP, float Damage, float Barrier, float maxVillageHP)
	{
		//if (!photonView.IsMine) return;

		EnergyValue.text = Energy.ToString() + " / " + MaxEnergy.ToString();
		VillageHP.text = HP.ToString();
		if (Barrier > 0.0f) VillageHP.text += "\n+ " + Barrier.ToString();
		DamageValue.text = Damage.ToString();
		//BarrierValue.text = Barrier.ToString();
		//TreeMultValue.text = "X " + TreeMult.ToString();
		SetVillageShileldSlider(HP, Barrier, maxVillageHP);
	}

	public void UpdateDmgMulitValue(float value, int actNum)
	{
		Player target = PhotonNetwork.CurrentRoom.GetPlayer(actNum);
		photonView.RPC(nameof(RPC_UpdateDmgMultiValue), target, value);
	}

	[PunRPC]
	private void RPC_UpdateDmgMultiValue(float value)
	{
		TreeMultValue.text = "X " + value.ToString();
	}

	public void SetVillageShileldSlider(float villageHP, float shiledValue, float maxVillageHP)
	{
		float shownHP = Mathf.Clamp(villageHP, 0f, maxVillageHP);
		float shownShield = Mathf.Max(0f, shiledValue);
		float totalValue = shownHP + shownShield;
		float overflow = Mathf.Max(0f, totalValue - maxVillageHP);

		AddShieldValueSlider.maxValue = 3000;
		ShieldValueSlider.maxValue = maxVillageHP;
		VillageHPSlider.maxValue = maxVillageHP;

		ShieldValueSlider.value = Mathf.Min(totalValue, maxVillageHP);
		VillageHPSlider.value = shownHP;
		AddShieldValueSlider.value = overflow;
	}

	//캔버스를 켜고 끄는 RPC 함수를 실행할 함수
	public void SetActiveCanvas(bool active)
	{
		//RPC 함수 호출
		photonView.RPC(nameof(RPC_SetActiveCanvas), RpcTarget.All, active);
	}


	public void ToggleMissionUI(bool toggle)
	{
		MissionPanel.SetActive(toggle);
	}

	public void SetMissionUI(string missionName, string missionContext, NewDrugMissionState state)
	{
		MissionName.text = missionName;
		MissionContext.text = missionContext;

		Image missionPanelImg = MissionPanel.GetComponent<Image>();
		switch (state)
		{
			case NewDrugMissionState.PendingNextDay:
				MissionState.text = LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_UI_RESERVED");
				missionPanelImg.color = ReadyState;
				break;
			case NewDrugMissionState.Active:
				MissionState.text = LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_UI_INPROGRESS");
				missionPanelImg.color = StartState;
				break;
			case NewDrugMissionState.Complete:
				MissionState.text = LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_UI_SUCCESS");
				missionPanelImg.color = SuccessState;
				break;
			case NewDrugMissionState.Failed:
				MissionState.text = LocalizationManager.Instance.GetText(CSV_Type.Mission, "M_UI_FAILED");
				missionPanelImg.color = FailedState;
				break;
			default: break;
		}
	}

	[PunRPC]
	private void RPC_SetActiveCanvas(bool active)
	{
		if (TryGetComponent(out canvasGroup))
		{
			canvasGroup.alpha = active ? 1f : 0f;
			canvasGroup.interactable = active;
			canvasGroup.blocksRaycasts = active;
		}
		// if (active)
		// {
		//  canvasGroup = GetComponent<CanvasGroup>();
		//  canvasGroup.alpha = 1f;
		//  canvasGroup.interactable = true;
		//  canvasGroup.blocksRaycasts = true;
		// }
		// else
		// {
		//  canvasGroup = GetComponent<CanvasGroup>();
		//  canvasGroup.alpha = 0f;
		//  canvasGroup.interactable = false;
		//  canvasGroup.blocksRaycasts = false;
		// }
	}
}

