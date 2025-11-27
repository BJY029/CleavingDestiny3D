using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasController : MonoBehaviourPunCallbacks
{
    public static PlayerCanvasController Instance;
	//캔버스를 껴고 킬때 사용할 캔버스 그룹
	private CanvasGroup canvasGroup;

	public Slider EnergySlider;
	public GameObject HitTextObj;

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
			}
			else
			{
				HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerNHit);
			}
		}
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
		}
		else
		{
			//내 턴이 아니면, 내 턴이 아니라는 텍스트로 변경
			HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerNHit);
		}
	}

	//Hit Text를 비활성화 하는 함수
	public void SetHitTextUnActive()
	{
		//HitText를 비활성화 하는 애니메이션에 이벤트로 비활성함수가 삽입되어 있어서 따로 비활성화는 하지 않음
		HitTextAnim.Play("UI_Player_HitText_Down");
		HitText.text = "";
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
