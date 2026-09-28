using UnityEngine;
using TMPro;

public class LobbyUIManager : MonoBehaviour
{
	//�̱���
	public static LobbyUIManager instance;

	private void Awake()
	{
		if (instance == null)
			instance = this;
	}

	//�г��Ӱ� ���� ������ ǥ���ϱ�
	[Header("UI Elements")]
	public GameObject isConnectedUI;
	public GameObject NicknameUI;
	
	TextMeshProUGUI isConnectedText;
	TextMeshProUGUI NicknameText;

	private void Start()
	{
		isConnectedText = isConnectedUI.GetComponentInChildren<TextMeshProUGUI>();
		NicknameText = NicknameUI.GetComponentInChildren<TextMeshProUGUI>();
		
		setConnectedText("Disconnected");
		setNickname(string.Empty);
	}

	public void setConnectedText(string text)
	{
		if (isConnectedText == null)
		{
			isConnectedText = isConnectedUI.GetComponentInChildren<TextMeshProUGUI>();
		}
		isConnectedText.text = text;
	}

	public void setNickname(string name)
	{
		if (NicknameText == null)
		{
			NicknameText = NicknameUI.GetComponentInChildren<TextMeshProUGUI>();
		}
		NicknameText.text = "Nickname : " + name;
	}	
}
