using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
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
