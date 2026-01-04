using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;

public class StickGameController : MonoBehaviourPunCallbacks
{
    //생성할 나뭇가지 수
    [SerializeField]
    private int branchCount;

    private float selectDurarionDefault = 20.0f;
    private float selectStartTime;
    private float selectDuration;
    private float endTime;
    private bool localResolved = false;
    
    public Canvas BranchGameCanvas;
    public int selectCount;
    public Slider Timer;
    private int playerCount;
    private bool isUpdatedTimer;

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

        selectCount = 0;
	}

	

	void Start()
    {
        BranchGameCanvas.transform.localScale = Vector3.zero;
        playerCount = PhotonNetwork.PlayerList.Length;

        Timer.value = 1f;
        //MasterClient만 나뭇가지 정보 설정
        //if(PhotonNetwork.IsMasterClient)
        //{
        //       InitSticks();
        //}

        var room = PhotonNetwork.CurrentRoom;
        var props = room.CustomProperties;

        isUpdatedTimer = false;
        if(props.ContainsKey("SelectStartTime"))
        {
            selectStartTime = (float)props["SelectStartTime"];
            selectDuration = (float)props["SelectDuration"];
            isUpdatedTimer = true;
            return;
        }

        //타이머 초기화 조건은 게임에서의 ActorNumber가 가장 낮은 플레이어가 수행
        //(MasterClient)에 의존하지 않기 위한 설정
        if(IsTimerInitializer())
        {
            //모든 클라이언트가 동일한 값 기반 동작하기 위해 Photon 서버 시간이용
            float now = (float)PhotonNetwork.Time;

            var ht = new ExitGames.Client.Photon.Hashtable
            {
                ["SelectStartTime"] = now,
                ["SelectDuration"] = selectDurarionDefault,
                ["SelectionResolved"] = false
            };

            room.SetCustomProperties(ht);
            selectStartTime = now;
            selectDuration = selectDurarionDefault;
            endTime = selectStartTime + selectDuration;
			isUpdatedTimer = true;
        }
    }

	public override void OnRoomPropertiesUpdate(Hashtable changedProps)
	{
		if (changedProps.ContainsKey("SelectStartTime"))
		{
			var room = PhotonNetwork.CurrentRoom;
			selectStartTime = (float)room.CustomProperties["SelectStartTime"];
			selectDuration = (float)room.CustomProperties["SelectDuration"];
			endTime = selectStartTime + selectDuration;
			isUpdatedTimer = true;
			Debug.Log(selectStartTime + " " + selectDuration + " " + endTime);
		}
	}

	//예외 처리
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		playerCount = PhotonNetwork.PlayerList.Length;
	}

	private void Update()
	{
        if (localResolved) return;;
        if (!isUpdatedTimer) return;
        var room = PhotonNetwork.CurrentRoom;

        if(room == null) return;

        if(room.CustomProperties.TryGetValue("SelectionResolved", out var r) && (bool)r)
        {
            localResolved = true;
            return;
        }

        float now = (float)PhotonNetwork.Time;
        //double endTime = selectStartTime + selectDuration;
        float remain = endTime - now;
        Timer.value = remain/selectDuration;

        if(remain <= 0.0 || selectCount >= playerCount)
        {
            ResolveSelection();
        }
	}

    private void ResolveSelection()
    {
        Debug.LogError("time 0 or all selected");
		
		if (!IsTimerInitializer())
        {
            //localResolved = true;
            return;
        }
		localResolved = true;
		var room = PhotonNetwork.CurrentRoom;
        var props = room.CustomProperties;
		var owners = (int[])props["StickOwner"];
		var lengths = (int[])props["StickLengths"];
		var rand = new System.Random();
		int randIdx;

        //key: actorNum, value: selected length
		Dictionary<int, int> setTurns = new Dictionary<int, int>();

        //각 플레이어를 돌면서
		foreach (Player p in PhotonNetwork.PlayerList)
		{
            //플레이어들의 식별 번호를 가져온다.
			int playerActNum = p.ActorNumber;
            int flag = 0;
            //owner 리스트를 돌아보면서
			for (int i = 0; i < owners.Length; i++)
			{
                //해당 플레이어가 선택한 나뭇가지가 있으면
				if (owners[i] == playerActNum)
				{
                    //딕셔너리 삽입
					setTurns.Add(playerActNum, lengths[i]);
                    flag = 1;
                    break;
				}
			}
            if (flag == 1) continue;
            //만약 해당 플레이어가 선택한 나뭇가지가 없으면
			do
			{
                //무작위 숫자 선택
				randIdx = rand.Next(0, branchCount);
			} while (owners[randIdx] != -1); //중복되지 않도록 설정
            //해당 랜덤 나뭇가지를 플레이어게 임의로 배치
			owners[randIdx] = playerActNum;
            //딕셔너리 삽입
			setTurns.Add(playerActNum, lengths[randIdx]);
		}
        //value를 기반으로 딕셔너리 정렬 및 배열로 변환
        var sortedTurnInfo = setTurns.OrderByDescending(t => t.Value).ToList();
        int[] turnOrder = sortedTurnInfo.Select(t =>t.Key).ToArray();
        int[] lengthOrder = sortedTurnInfo.Select(t => t.Value).ToArray();
		for (int i = 0;i < turnOrder.Length; i++)
        {
            Debug.LogError($"turn order :  {i + 1} = player{turnOrder[i]}, length:{lengthOrder[i]}");
        }
        //Properties에 삽입
        var newProps = new ExitGames.Client.Photon.Hashtable
        {
            ["TurnInfo"] = turnOrder,
            ["StickOwner"] = owners,
            ["SelectionResolved"] = true,
        };
		room.SetCustomProperties(newProps);
	}

    //Masterclient가 아닌 가장 낮은 번호의 ActorNumber를 가진 사람이 타이머 및 처리 진행
	private bool IsTimerInitializer()
    {
        var players = PhotonNetwork.PlayerList;
        int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
        int minActor = players.Min(p => p.ActorNumber);
        return myActor == minActor;
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

        //디버그용
        //for(int i = 0; i < branchCount; i++)
        //{
        //    Debug.LogError($"index{i+1} length = {lengths[i]}");
        //}

        //RoomProperty에 저장
        var props = new ExitGames.Client.Photon.Hashtable
        {
            ["StickLengths"] = lengths,
            ["StickOwner"] = owners,
        };

        //업데이트
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
		BranchGameCanvas.transform.localScale = Vector3.zero;
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
        //Debug.LogError($"Player {actorNumber} has picked {stickIndex+1} stick. Length : {lengths[stickIndex]}");

        //업데이트 된 정보 업로드
		var newProps = new ExitGames.Client.Photon.Hashtable
		{
			["StickOwner"] = owners
		};
		room.SetCustomProperties(newProps);

        BranchSpawner.Instance.CallBackBranchClick(stickIndex, actorNumber);
	}
}
