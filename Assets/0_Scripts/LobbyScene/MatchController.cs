using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MatchController : MonoBehaviourPunCallbacks
{
	/*  FLAGS  */
	//���� ��ġ����ŷ ������ Ȯ��
	private bool isFindingMatch = false;
	//�ؽ�Ʈ ���⿡ ���� �÷���
	private bool spining;
	//�� ��ȯ ���� ���� ����
	private bool allowSceneChange;
	//���� �й� ���� �÷���
	private bool roleDistribution;


	//��ġ����ŷ ���� Ÿ�̸� ���� �ڷ�ƾ
	private Coroutine timerCoroutine;
	//�ε� �ؽ�Ʈ ���� �ڷ�ƾ
	private Coroutine spiningCoroutine;
	//�ؽ�Ʈ ���⿡ ���� �ð�
	private float duration = 0.4f;




	//��ġ����ŷ ���� UI ���
	[Header("Loading Panel")]
	public GameObject LoadingPanel;
	public Text LoadingText;
	public Text SceneLoadingText;
	public Text Timer;
	public Button StopMatching;

	private void Awake()
	{
		allowSceneChange = false;
		roleDistribution = false;
		spining = false;
		SceneLoadingText.text = "";
	}

	//��ġ����ŷ�� �õ��ϴ� �Լ�
	protected void FindMatch()
	{
		//���� ������ ���¿���
		if (PhotonNetwork.IsConnectedAndReady)
		{
			if (LoadingPanel == null) return;

			//��ġ����ŷ �÷��� ����
			isFindingMatch = true;

			//�ε� �г� ����
			LoadingPanel.transform.localScale = Vector3.one;

			//...�� ���������� ��µǷη� �Ѵ�.
			LoadingText.text = "Looking for an opponent";
			if (spiningCoroutine != null)
			{
				spining = false;
				StopCoroutine(spiningCoroutine);
			}
			spiningCoroutine = StartCoroutine(SpiningDots(LoadingText));


			//������ �� �ɼ� ����
			RoomOptions roomOptions = new RoomOptions();
			//�ִ� �� �ο� �� �� �� ���� �����ϰ� ����
			roomOptions.MaxPlayers = 2;
			roomOptions.IsVisible = true;
			roomOptions.IsOpen = true;

			//Ÿ�̸� ���� �ڷ�ƾ�� �������̸� ���߰�
			if (timerCoroutine != null)
			{
				StopCoroutine(timerCoroutine);
			}
			//Ÿ�̸Ӹ� �����Ѵ�.
			timerCoroutine = StartCoroutine(UpdateTimer());

			//JoinRandomroom ���� �õ� : ���ǿ� �´� ������ �濡 ������ �õ��Ѵ�.
			//���� �� CreateRoom ���� : ���� ������ �� �ִ� ���� ��ã����, ���� �����ϰ�, �ش� ���� ù��° �÷��̾�� �����Ѵ�.
			//�ֿ� �Ķ����
			//expectedCustomRoomProperties : Ư�� Ŀ���� �Ӽ��� ���� �游 ���͸��Ͽ� ���� ���� �� ���(ex : map : "desert")
			//roomOption : ������ ���� ����
			//typedLobby : Ư�� �κ� �����Ͽ�, �� �ȿ����� ���� ã�ų� �����ϰ� ���� �� ����Ѵ�.(������ null�� �ؼ� �⺻�κ�� ����)
			PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, null, null, null, roomOptions);
		}
		else
		{
			Debug.LogError("Not connected to server.");
		}
	}

	//��ġ ����ŷ ��� ��ư�� ������ ����� �Լ�
	protected void CancelMatch()
	{
		//���ʿ� ��ġ����ŷ ���� �ƴϿ��ٸ� ���� x
		if (!isFindingMatch) return;
		//��ġ����ŷ �÷��� ����
		isFindingMatch = false;

		//���� �濡 �����ִ� ��� �濡�� ������.
		if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();

		//Ÿ�̸� �ʱ�ȭ
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
			StopCoroutine(spiningCoroutine);
			Timer.text = "00:00";
		}

		//�ε� �г� ��Ȱ��ȭ(ũ�� 0���� ����)
		if (LoadingPanel != null) LoadingPanel.transform.localScale = Vector3.zero;
	}

	//Ÿ�̸Ӹ� �����ϴ� �ڷ�ƾ
	IEnumerator UpdateTimer()
	{
		float elapsedTime = 0f;
		while (isFindingMatch)
		{
			elapsedTime += Time.deltaTime;

			float min = Mathf.FloorToInt(elapsedTime / 60);
			float sec = Mathf.FloorToInt(elapsedTime % 60);

			Timer.text = string.Format("{0:00}:{1:00}", min, sec);

			yield return null;
		}
	}

	//�濡 ���������� �������� �� ����� �Լ�
	//������ �����ϴ� �濡 �ش�Ǵ� �÷��̾ �濡 �����ϰ� �Ǹ� ������ �Լ�
	public override void OnJoinedRoom()
	{
		CheckPlayersInRoom();
	}

	//�ǵ�ġ �ʰ� ���� ���� ���, ��ġ����ŷ�� ����Ѵ�.
	public override void OnLeftRoom()
	{
		if (isFindingMatch)
			CancelMatch();
	}

	//Ư�� �÷��̾ �濡 �����ϸ� ������ �Լ�
	//�ش� �Լ��� ���� ����� ����ϰ� �ִ� �÷��̾, Ư�� �÷��̾ �濡 ������ �����ϰ� �ȴ�.
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		CheckPlayersInRoom();
	}

	//���� ���� ���� Ȯ�� �� ���� ����
	private void CheckPlayersInRoom()
	{
		//�ο� ������ �����Ǹ�
		if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
		{
			GameManager.Instance.nextScene = CommonDefine.GAMESCENE;

			//���� �й�
			//RoleDistribution();
			//�� �ε�
			StartCoroutine(LoadScene());
			//�� ��ȯ �ڷ�ƾ ����
			StartCoroutine(StopTimerAndFinalizeMatch());
		}
		else
		{
			//...�� ���������� ��µǵ��� �Ѵ�.
			LoadingText.text = "Waiting for opponent";
			if (spiningCoroutine != null)
			{
				spining = false;
				StopCoroutine(spiningCoroutine);
			}
			spiningCoroutine = StartCoroutine(SpiningDots(LoadingText));
		}
	}

	//Ư�� �ؽ�Ʈ�� ...�� �ݺ������� ��µǴ� �ڷ�ƾ
	IEnumerator SpiningDots(Text texts)
	{
		yield return null;
		spining = true;

		string originText = texts.text;
		int curDot = 0;
		string Dot = "";
		while (spining)
		{
			Dot = "";
			for (int i = 0; i < curDot; i++)
			{
				Dot += ".";
			}
			texts.text = originText + Dot;
			curDot = (curDot + 1) % 4;

			yield return new WaitForSeconds(duration);
		}

	}

	//������ �����ϱ� ���� �ڷ�ƾ
	IEnumerator StopTimerAndFinalizeMatch()
	{
		//��ġ����ŷ �÷��� ����
		isFindingMatch = false;

		//Ÿ�̸� ����
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
		}
		LoadingText.text = "Matching success! Moving to gamescene";

		//��ġ����ŷ ��� ���ϰ� ��ư ��Ȱ��ȭ
		StopMatching.gameObject.SetActive(false);

		//���� �񵿱�� �ε尡 �Ϸ�ǰ�, ���� �й谡 ������ ���� ��ȯ�Ѵ�.
		yield return new WaitUntil(() => allowSceneChange == true && roleDistribution == true);

		//�ش� ���� �����̸�, ���� �Ⱥ��̰� �����ϰ� �� �̵�
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.CurrentRoom.IsOpen = false;
			//�ٸ� �÷��̾�鵵 ���� �̵��ȴ�.(AutomaticallySyncScene = true ���� �ؾ� ��)
			PhotonNetwork.LoadLevel("GameScene");
		}
	}

	//�÷��̾�� ������ �й��ϴ� �Լ�
	//���� �ʿ�-------------------------
	private void RoleDistribution()
	{
		//���� MasterClient�� �ƴ϶�� �ش� �Լ��� �������� �ʴ´�.
		if (!PhotonNetwork.IsMasterClient) return;

		//���� ���� �÷��̾���� ����Ʈ�� �ҷ��´�.
		var players = PhotonNetwork.PlayerList.ToList();
		//���� ��ü ���� ��, (0 ~ players.count - 1) ��, 0 Ȥ�� 1�� ���ڸ� ��ȯ�޴´�.
		System.Random rand = new System.Random();
		int r = rand.Next(players.Count);

		Player p1;
		//���� 0�̸� �״���̰� 0��°�� ��ġ�� �÷��̾ p1
		if (r == 0)
		{
			p1 = players[0];
		}
		else//1�̸� 1��°�� ��ġ�� �÷��̾ p1�� �����Ѵ�.
		{
			p1 = players[1];
		}

		//�÷��̾���� ���鼭
		foreach (var player in players)
		{
			//�ش� �÷��̾ p1 �÷��̾�� ���� ���, P1 �Ҵ�, �ƴϸ� P2 �Ҵ�
			string role = (player == p1) ? "P1" : "P2";

			//�÷��̾� ���� ������ HashTable�� �����ϰ�
			var props = new ExitGames.Client.Photon.Hashtable
			{
				{"Role", role },
			};

			//�ش� �÷��̾��� CustomProperties�� ������ ������Ʈ �Ѵ�.
			player.SetCustomProperties(props);
		}

		//��� ���� �й谡 ������ �÷��׸� true�� �����Ѵ�.
		roleDistribution = true;
	}

	//�񵿱�� ���� �ε��ϴ� �Լ�
	IEnumerator LoadScene()
	{
		yield return null;
		//�񵿱�� �� �ε�
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameManager.Instance.nextScene);
		//�� ��ȯ ����
		asyncOperation.allowSceneActivation = false;

		//�ؽ�Ʈ ���� ����
		string originText = "Loading Scene";
		int curDot = 0;
		string Dot = "";

		//�񵿱� �� �ε��� ���� ������
		while (!asyncOperation.isDone)
		{
			//���� �ε� ���̶��
			if (asyncOperation.progress < 0.9f)
			{
				//���������� �ؽ�Ʈ ���
				Dot = "";
				for (int i = 0; i < curDot; i++)
				{
					Dot += ".";
				}
				SceneLoadingText.text = originText + Dot;
				curDot = (curDot + 1) % 4;

				yield return new WaitForSeconds(duration);
			}
			else //�ε��� �Ϸ�Ǹ�
			{
				//�� ��ȯ ��� ��
				asyncOperation.allowSceneActivation = true;
				//�� ��ȯ�� �����ϵ��� �����Ѵ�.
				allowSceneChange = true;
				yield break;
			}
		}
	}
}
