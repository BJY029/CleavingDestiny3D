using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StickGameController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int branchCount;
    public static StickGameController Instance;
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
	}


	private int stickCount = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
               InitSticks();
        }
    }

    public void InitSticks()
    {       
        int[] lengths = new int[branchCount];
        for(int i = 0; i < branchCount; i++)
        {
            lengths[i] = i + 1;
        }
        int[] owners = new int[branchCount];

        int temp;
        var rand = new System.Random();

        for(int i = 0; i < branchCount; i++)
        {
            int randIdx = rand.Next(0, branchCount);
            temp = lengths[randIdx];
            lengths[randIdx] = lengths[i];
            lengths[i] = temp;

            owners[i] = -1;
        }

        var props = new ExitGames.Client.Photon.Hashtable
        {
            ["StickLengths"] = lengths,
            ["StickOwner"] = owners,
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

	public void OnClickStick(int stickIndex)
	{
		if (!PhotonNetwork.InRoom) return;

		// 한 번만 선택하게 막는 체크는 로컬/서버 둘 다에서
		photonView.RPC(nameof(RequestPickStick), RpcTarget.MasterClient, stickIndex);
	}

	[PunRPC]
	private void RequestPickStick(int stickIndex, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		var room = PhotonNetwork.CurrentRoom;
		var props = room.CustomProperties;

		var owners = (int[])props["StickOwner"];
		var lengths = (int[])props["StickLengths"];

        // 범위 체크
        if (stickIndex < 0 || stickIndex >= owners.Length)
        {
            Debug.LogError("Error of Branch number");
            return;
        }

		int actorNumber = info.Sender.ActorNumber;

        // 이미 누가 뽑은 가지면 거절
        if (owners[stickIndex] != -1)
        {
            Debug.LogWarning("This branch already picked");
            return;
        }

        // 이 유저가 이미 다른 가지를 뽑았는지도 체크
        if (owners.Contains(actorNumber))
        {
            Debug.LogWarning("Player already has branch");
            return;
        }

		// 배정
		owners[stickIndex] = actorNumber;

		var newProps = new ExitGames.Client.Photon.Hashtable
		{
			["StickOwner"] = owners
		};
		room.SetCustomProperties(newProps);

	}
}
