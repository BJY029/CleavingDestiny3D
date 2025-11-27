using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasController : MonoBehaviourPunCallbacks
{
    public static GameCanvasController Instance;

	//캔버스를 끄고 켤때 사용할 캔버스 그룹
	private CanvasGroup canvasGroup;

	public GameObject DayTextObj;
	private Text DayText;
	public GameObject WaveTextObj;
	private Text WaveText;


	private void Awake()
	{
		if(Instance == null) Instance = this;
		else Destroy(gameObject);

		DayText = DayTextObj.GetComponentInChildren<Text>();
		WaveText = WaveTextObj.GetComponentInChildren<Text>();
		canvasGroup = GetComponent<CanvasGroup>();

		DayText.text = string.Empty;
		WaveText.text = string.Empty;

		this.gameObject.SetActive(false);
	}

	//현재 날짜 값을 받아와 텍스트에 반영
	public void UpdateDayText()
	{
		int day = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentDay);
		DayText.text = "Day : " + day.ToString();
	}

	//현재 웨이브 값을 받아와 텍스트에 반영
	public void UpdateWaveText()
	{
		int wave = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
		WaveText.text = "Wave : " + wave.ToString();
	}

	//캔버스를 켜고 끄는 RPC를 실행할 함수
	public void SetActiveCanvas(bool active)
	{
		//모든 클라이언트에게 RPC 호출
		photonView.RPC(nameof(RPC_SetActiveCanvas), RpcTarget.All, active);
	}

	//Canvas를 켜고 끄는 RPC 함수
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
