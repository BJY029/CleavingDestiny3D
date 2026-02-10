using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public PlayerSetting playerDefaultSetting;
	public RoomSetting roomDefaultSetting;

	public int maxRoomPlayerCount = 2;
	public bool isSoloPlay = false;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
		Init();
	}

	public GameObject playerObj;
	public string nextScene;

	void Init()
	{
		nextScene = "";
	}
}
