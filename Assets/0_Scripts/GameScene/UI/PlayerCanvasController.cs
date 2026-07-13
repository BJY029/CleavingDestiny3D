using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class PlayerCanvasController : MonoBehaviourPunCallbacks
{
	public static PlayerCanvasController Instance;
	//캔버스를 껴고 킬때 사용할 캔버스 그룹
	private CanvasGroup canvasGroup;

	[Header("UI")]
	public TextMeshProUGUI EnergyValue;
	public TextMeshProUGUI VillageHP;
	public TextMeshProUGUI DamageValue;
	public TextMeshProUGUI BarrierValue;
	public TextMeshProUGUI TreeMultValue;
	public GameObject HitTextObj;

	[SerializeField] private GameObject gaugeRoot;
	[SerializeField] private Slider gaugeSlider;
	public TextMeshProUGUI minDamage;
	public TextMeshProUGUI maxDamage;

	[Header("Gauge")]
	[SerializeField] private float speed = 1.8f;

	[Header("Warning")]
	[SerializeField] private GameObject WarningObj;

	[Header("ItemNotifyHolder")]
	[SerializeField] private GameObject Holder;

	[Header("Timer")]
	public GameObject TimerObj;
	public TextMeshProUGUI TimerText;

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
	private Animator HitTextAnim;

	private TextMeshProUGUI WarningText;
	private Animator WarningTextAnim;


	private ItemNotifyController INC;

	private float _startTime = -1f;
	private float _endTime = -1f;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		HitText = HitTextObj.GetComponentInChildren<TextMeshProUGUI>();
		WarningText = WarningObj.GetComponentInChildren<TextMeshProUGUI>();

		HitTextAnim = HitTextObj.GetComponent<Animator>();
		WarningTextAnim = WarningObj.GetComponent<Animator>();
		canvasGroup = GetComponent<CanvasGroup>();

		HitTextObj.SetActive(false);
		WarningObj.SetActive(false);
		MissionPanel.SetActive(false);
		CloseGauge();
		HitText.text = "";
		WarningText.text = "";
		InitTimer();
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

		TimerText.text = remainTime.ToString("F0");
	}

	private void InitTimer()
	{
		TimerText.text = "";
		_startTime = -1f;
		_endTime = -1f;
	}

	//타이머가 설정되면, 시작, 끝 시간을 받아와서 저장한다.
	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.TryGetValue(RoomPropKeys.PlayerTurnStartEndTime, out var value))
		{
			if (value is Vector2 times)
			{
				_startTime = times.x;
				_endTime = times.y;
			}
		}
	}


	//턴 정보에 따라서 Hit Text를 변경하는 함수
	public void UpdateGameHitText()
	{
		//Hit 텍스트가 활성화 되어있고
		if (HitText.IsActive())
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
		//Debug.LogError("my turn: " + myTurn + ", In Game Turn: " + CurrentTurn);
		//오브젝트 활성화
		HitTextObj.SetActive(true);
		//텍스트 오브젝트를 띄우는 애니메이션 재생
		HitTextAnim.Play("UI_Player_HitText_Up");
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
		//HitText를 비활성화 하는 애니메이션에 이벤트로 비활성함수가 삽입되어 있어서 따로 비활성화는 하지 않음
		HitTextAnim.Play("UI_Player_HitText_Down");
		CloseGauge();
		HitText.text = "";
	}

	public void SetWarningTextActive(string textId)
	{
		WarningObj.SetActive(true);
		WarningText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, textId);
		WarningTextAnim.Play("UI_Player_Warning_Up");
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
	public void updatePlayerStatus(string Energy, string HP, string Damage, string Barrier, string TreeMult)
	{
		//if (!photonView.IsMine) return;

		EnergyValue.text = Energy;
		VillageHP.text = HP;
		DamageValue.text = Damage;
		BarrierValue.text = Barrier;
		TreeMultValue.text = TreeMult;
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
		// 	canvasGroup = GetComponent<CanvasGroup>();
		// 	canvasGroup.alpha = 1f;
		// 	canvasGroup.interactable = true;
		// 	canvasGroup.blocksRaycasts = true;
		// }
		// else
		// {
		// 	canvasGroup = GetComponent<CanvasGroup>();
		// 	canvasGroup.alpha = 0f;
		// 	canvasGroup.interactable = false;
		// 	canvasGroup.blocksRaycasts = false;
		// }
	}
}
