using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
	//싱글턴
	public static LobbyUIManager instance;

	private void Awake()
	{
		if (instance == null)
			instance = this;
	}

	//닉네임과 연결 정보를 표시하기
	[Header("UI Elements")]
	public GameObject isConnectedUI;
	public GameObject NicknameUI;

	private void Start()
	{
		isConnectedUI.GetComponentInChildren<Text>().text = "Disconnected";
		NicknameUI.GetComponentInChildren<Text>().text = "Nickname : ";
	}

	public void setConnectedText(string text)
	{
		isConnectedUI.GetComponentInChildren<Text>().text = text;
	}

	public void setNickname(string name)
	{
		NicknameUI.GetComponentInChildren<Text>().text = "Nickname : " + name;
	}	
}
