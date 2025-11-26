using UnityEngine;
using UnityEngine.UI;

public class GameCanvasController : MonoBehaviour
{
    public static GameCanvasController Instance;

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

		DayText.text = string.Empty;
		WaveText.text = string.Empty;

		this.gameObject.SetActive(false);
	}

	public void UpdateDayText()
	{
		int day = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentDay);
		DayText.text = "Day : " + day.ToString();
	}

	public void UpdateWaveText()
	{
		int wave = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentWave);
		WaveText.text = "Wave : " + wave.ToString();
	}
}
