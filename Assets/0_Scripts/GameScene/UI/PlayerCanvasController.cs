using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasController : MonoBehaviourPunCallbacks
{
    public static PlayerCanvasController Instance;
	//캔버스를 껴고 킬때 사용할 캔버스 그룹
	private CanvasGroup canvasGroup;

	[Header("UI")]
	public Text EnergyValue;
	public Text VillageHP;
	public Text DamageValue;
	public Text BarrierValue;
	public GameObject HitTextObj;
	[SerializeField] private GameObject gaugeRoot;
	[SerializeField] private Slider gaugeSlider;
	public Text minDamage;
	public Text maxDamage;

	[Header("Gauge")]
	[SerializeField] private float speed = 1.8f;

	private Coroutine gaugeCo;
	private bool selecting;

	private Text HitText;
	private Animator HitTextAnim;

	private void Awake()
	{
		if(Instance == null) Instance = this;
		else Destroy(gameObject);

		HitText = HitTextObj.GetComponentInChildren<Text>();
		HitTextAnim = HitTextObj.GetComponent<Animator>();
		canvasGroup = GetComponent<CanvasGroup>();

		HitTextObj.SetActive(false);
		CloseGauge();
		HitText.text = "";
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
				HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerHit);
				OpenGauge();
			}
			else
			{
				HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerNHit);
				CloseGauge();
			}
		}
	}

	public void OpenGauge()
	{
		gaugeRoot.SetActive(true);
		selecting = true;
		maxDamage.text = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MaxAtkPow).ToString();
		minDamage.text = PhotonPropertyHelper.GetPlayerProp<int>(PhotonNetwork.LocalPlayer, PlayerPropKeys.MinAtkPow).ToString();
		if (gaugeCo != null) StopCoroutine(gaugeCo);
		gaugeCo = StartCoroutine(GaugeLoop());
	}

	public void CloseGauge()
	{
		selecting = false;
		if(gaugeCo!= null) StopCoroutine(gaugeCo);
		gaugeCo = null;
		gaugeRoot.SetActive(false);
	}

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

	public float SelectNow()
	{
		if (!selecting) return -1;

		selecting = false;
		if(gaugeCo != null) StopCoroutine (gaugeCo);

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
			HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerHit);
			OpenGauge();
		}
		else
		{
			//내 턴이 아니면, 내 턴이 아니라는 텍스트로 변경
			HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerNHit);
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

	//현재 플레이어 상태 UI를 업데이트 하는 함수
	public void updatePlayerStatus(string Energy, string HP, string Damage, string Barrier)
	{
		//if (!photonView.IsMine) return;

		EnergyValue.text = Energy;
		VillageHP.text = HP;
		DamageValue.text = Damage;
		BarrierValue.text = Barrier;
	}

	//캔버스를 켜고 끄는 RPC 함수를 실행할 함수
	public void SetActiveCanvas(bool active)
	{
		//RPC 함수 호출
		photonView.RPC(nameof(RPC_SetActiveCanvas), RpcTarget.All, active);
	}

	[PunRPC]
	private void RPC_SetActiveCanvas(bool active)
	{
		if (active)
		{
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}
		else
		{
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}
	}
}
