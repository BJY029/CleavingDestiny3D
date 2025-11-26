using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvasController : MonoBehaviour
{
    public static PlayerCanvasController Instance;

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

		HitTextObj.SetActive(false);          
		HitText.text = "";
	}

	public void UpdateGameHitText()
	{
		if (HitText.IsActive())
		{
			if (GameHelper.IsMyTurn())
			{
				HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerHit);
			}
			else
			{
				HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerNHit);
			}
		}
	}

	public void SetHitTextActive()
	{
		//Debug.LogError("my turn: " + myTurn + ", In Game Turn: " + CurrentTurn);

		HitTextObj.SetActive(true);
		HitTextAnim.Play("UI_Player_HitText_Up");
		if (GameHelper.IsMyTurn())
		{
			HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerHit);
		}
		else
		{
			HitText.text = LocalizationManager.Instance.GetText(UI_CSV.UI_PlayerNHit);
		}
	}

	public void SetHitTextUnActive()
	{
		HitTextAnim.Play("UI_Player_HitText_Down");
		HitText.text = "";
	}
}
