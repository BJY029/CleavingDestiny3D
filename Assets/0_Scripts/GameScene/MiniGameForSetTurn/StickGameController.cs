using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StickGameController : MonoBehaviourPunCallbacks
{
    //생성할 나뭇가지 수
    [SerializeField]
    private int branchCount;

    //싱글턴
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


    void Start()
    {
        //MasterClient만 나뭇가지 정보 설정
        if(PhotonNetwork.IsMasterClient)
        {
               InitSticks();
        }
    }

    //나뭇 가지 정보를 초기화 하는 함수
    public void InitSticks()
    {       
        //나뭇가지 개수만큼 배열 생성 및 초기화
        int[] lengths = new int[branchCount];
        for(int i = 0; i < branchCount; i++)
        {
            lengths[i] = i + 1;
        }
        //각 나뭇가지 주인 정보를 저장할 배열 생성
        int[] owners = new int[branchCount];

        //나뭇 가지 길이를 중복 없이 섞기 위해서 다음과 같은 반복문 실행
        int temp;
        var rand = new System.Random();

        for(int i = 0; i < branchCount; i++)
        {
            int randIdx = rand.Next(0, branchCount);
            temp = lengths[randIdx];
            lengths[randIdx] = lengths[i];
            lengths[i] = temp;

            //각 나뭇가지의 주인은 현재 없기 때문에 -1로 설정
            owners[i] = -1;
        }

        //RoomProperty에 저장
        var props = new ExitGames.Client.Photon.Hashtable
        {
            ["StickLengths"] = lengths,
            ["StickOwner"] = owners,
        };

        //업데이트
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    //특정 나뭇가지가 클릭될 경우 호출될 함수
	public void OnClickStick(int stickIndex)
	{
		if (!PhotonNetwork.InRoom) return;

		// 한 번만 선택하게 막는 체크는 로컬/서버 둘 다에서
		photonView.RPC(nameof(RequestPickStick), RpcTarget.MasterClient, stickIndex);
	}

    //MasterClient에서 실행될 RPC 함수
	[PunRPC]
	private void RequestPickStick(int stickIndex, PhotonMessageInfo info)
	{
		if (!PhotonNetwork.IsMasterClient) return;

        //나뭇가지 관련 정보 불러오기
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

        //해당 함수를 호출한 actor 숫자 가져오기
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

		// 주인 배정
		owners[stickIndex] = actorNumber;

        //업데이트 된 정보 업로드
		var newProps = new ExitGames.Client.Photon.Hashtable
		{
			["StickOwner"] = owners
		};
		room.SetCustomProperties(newProps);

	}
}
