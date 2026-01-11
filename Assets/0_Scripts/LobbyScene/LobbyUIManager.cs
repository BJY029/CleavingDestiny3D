using UnityEngine;
using TMPro;

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
		isConnectedUI.GetComponentInChildren<TextMeshProUGUI>().text = "Disconnected";
		NicknameUI.GetComponentInChildren<TextMeshProUGUI>().text = "Nickname : ";
	}

	public void setConnectedText(string text)
	{
		isConnectedUI.GetComponentInChildren<TextMeshProUGUI>().text = text;
	}

	public void setNickname(string name)
	{
		NicknameUI.GetComponentInChildren<TextMeshProUGUI>().text = "Nickname : " + name;
	}	
}
