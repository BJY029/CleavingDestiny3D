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
    // 나뭇가지(Branch)의 총 개수
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

    // Singleton
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
        // 마스터 클라이언트가 스틱들을 초기화
        //if(PhotonNetwork.IsMasterClient)
        //{
        //       InitSticks();
        //}

        var room = PhotonNetwork.CurrentRoom;
        var props = room.CustomProperties;

        isUpdatedTimer = false;
        if (props.ContainsKey("SelectStartTime"))
        {
            selectStartTime = (float)props["SelectStartTime"];
            selectDuration = (float)props["SelectDuration"];
            endTime = selectStartTime + selectDuration;
            isUpdatedTimer = true;
            return;
        }

        // 타이머 초기화는 방장(MasterClient) 혹은 가장 빠른 번호의 플레이어가 수행
        // (MasterClient)가 존재하지 않는 상황 대비
        if (IsTimerInitializer())
        {
            // 모든 클라이언트가 최대한 동시에 타이머를 시작하기 위해 Photon 서버 시간을 기준으로 설정
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
            BranchGameCanvas.transform.localScale = Vector3.one;
            Debug.Log(selectStartTime + " " + selectDuration + " " + endTime);
        }
    }

    // 플레이어 퇴장 처리
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerCount = PhotonNetwork.PlayerList.Length;
    }

    private void Update()
    {
        if (localResolved) return; ;
        if (!isUpdatedTimer) return;
        var room = PhotonNetwork.CurrentRoom;

        if (room == null) return;

        if (room.CustomProperties.TryGetValue("SelectionResolved", out var r) && (bool)r)
        {
            localResolved = true;
            return;
        }

        float now = (float)PhotonNetwork.Time;
        //double endTime = selectStartTime + selectDuration;
        float remain = endTime - now;
        Timer.value = remain / selectDuration;

        if (remain <= 0.0 || selectCount >= playerCount)
        {
            Debug.Log($"Time's up or all selected: remain={remain}, selectCount={selectCount}, playerCount={playerCount}");
            ResolveSelection();
        }
    }

    private void ResolveSelection()
    {
        Debug.Log("time 0 or all selected");

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

        // key: actorNum, value: selected length
        Dictionary<int, int> setTurns = new Dictionary<int, int>();

        // 모든 플레이어를 순회하며
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // 플레이어의 고유 번호를 가져온다.
            int playerActNum = p.ActorNumber;
            int flag = 0;
            // 소유자 리스트를 확인하면서
            for (int i = 0; i < owners.Length; i++)
            {
                // 해당 플레이어가 스틱을 이미 선택했다면
                if (owners[i] == playerActNum)
                {
                    // 정보 추가
                    setTurns.Add(playerActNum, lengths[i]);
                    flag = 1;
                    break;
                }
            }
            if (flag == 1) continue;
            // 아직 스틱을 선택하지 않은 플레이어라면 (타임아웃 등)
            do
            {
                // 랜덤으로 스틱 선택
                randIdx = rand.Next(0, branchCount);
            } while (owners[randIdx] != -1); // 중복되지 않도록 방지
            // 해당 스틱의 소유자로 플레이어 설정
            owners[randIdx] = playerActNum;
            // 정보 추가
            setTurns.Add(playerActNum, lengths[randIdx]);
        }

        if (GameManager.Instance.isSoloPlay)
        {
            int player = PhotonNetwork.PlayerList.Length;
            int aiID = 1000;
            for (int i = player + 1; i <= GameManager.Instance.maxRoomPlayerCount; i++)
            {
                do
                {
                    // 랜덤으로 스틱 선택
                    randIdx = rand.Next(0, branchCount);
                } while (owners[randIdx] != -1); // 중복되지 않도록 방지

                // 해당 스틱의 소유자로 플레이어 설정
                owners[randIdx] = aiID;
                // 정보 추가
                setTurns.Add(aiID, lengths[randIdx]);
                aiID++;
            }
        }


        // 길이를 기준으로 내림차순 정렬하여 턴 순서 배열 생성
        var sortedTurnInfo = setTurns.OrderByDescending(t => t.Value).ToList();
        int[] turnOrder = sortedTurnInfo.Select(t => t.Key).ToArray();
        int[] lengthOrder = sortedTurnInfo.Select(t => t.Value).ToArray();
        for (int i = 0; i < turnOrder.Length; i++)
        {
            Debug.Log($"turn order :  {i + 1} = player{turnOrder[i]}, length:{lengthOrder[i]}");
        }
        // 룸 프로퍼티에 저장
        var newProps = new ExitGames.Client.Photon.Hashtable
        {
            ["TurnInfo"] = turnOrder,
            ["StickOwner"] = owners,
            ["SelectionResolved"] = true,
        };
        room.SetCustomProperties(newProps);
    }

    // 마스터 클라이언트가 아니더라도 가장 낮은 ActorNumber를 가진 유저가 타이머 초기화를 수행
    private bool IsTimerInitializer()
    {
        var players = PhotonNetwork.PlayerList;
        int myActor = PhotonNetwork.LocalPlayer.ActorNumber;
        int minActor = players.Min(p => p.ActorNumber);
        return myActor == minActor;
    }

    // 스틱 및 초기 데이터를 초기화하는 함수
    public void InitSticks()
    {
        // 나뭇가지 개수만큼 배열 생성 및 초기화
        int[] lengths = new int[branchCount];
        for (int i = 0; i < branchCount; i++)
        {
            lengths[i] = i + 1;
        }
        // 각 나뭇가지의 소유주를 저장할 배열 생성
        int[] owners = new int[branchCount];

        // 나뭇가지의 길이를 무작위로 섞기 위해 셔플 알고리즘 사용
        int temp;
        var rand = new System.Random();

        for (int i = 0; i < branchCount; i++)
        {
            int randIdx = rand.Next(0, branchCount);
            temp = lengths[randIdx];
            lengths[randIdx] = lengths[i];
            lengths[i] = temp;

            // 소유주 배열을 -1(소유주 없음)로 초기화
            owners[i] = -1;
        }

        // 테스트용
        //for(int i = 0; i < branchCount; i++)
        //{
        //    Debug.LogError($"index{i+1} length = {lengths[i]}");
        //}

        // 룸 프로퍼티에 저장
        var props = new ExitGames.Client.Photon.Hashtable
        {
            ["StickLengths"] = lengths,
            ["StickOwner"] = owners,
        };

        // 업데이트
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        BranchGameCanvas.transform.localScale = Vector3.zero;
    }

    // 특정 나뭇가지를 클릭했을 때 호출되는 함수
    public void OnClickStick(int stickIndex)
    {
        if (!PhotonNetwork.InRoom) return;

        // 마스터 클라이언트에게 스틱 선택 요청
        photonView.RPC(nameof(RequestPickStick), RpcTarget.MasterClient, stickIndex);
    }

    // 마스터 클라이언트에서 실행될 RPC 함수
    [PunRPC]
    private void RequestPickStick(int stickIndex, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 현재 스틱 상태 정보를 가져옴
        var room = PhotonNetwork.CurrentRoom;
        var props = room.CustomProperties;

        var owners = (int[])props["StickOwner"];
        var lengths = (int[])props["StickLengths"];

        // 유효성 체크
        if (stickIndex < 0 || stickIndex >= owners.Length)
        {
            Debug.LogError("Error of Branch number");
            return;
        }

        // 요청을 보낸 플레이어의 번호 확인
        int actorNumber = info.Sender.ActorNumber;

        // 이미 선택된 스틱인지 확인
        if (owners[stickIndex] != -1)
        {
            Debug.LogWarning("This branch already picked");
            return;
        }

        // 플레이어가 이미 스틱을 가지고 있는지 확인
        if (owners.Contains(actorNumber))
        {
            Debug.LogWarning("Player already has branch");
            return;
        }

        // 소유자 설정
        owners[stickIndex] = actorNumber;
        //Debug.LogError($"Player {actorNumber} has picked {stickIndex+1} stick. Length : {lengths[stickIndex]}");

        // 룸 프로퍼티 업데이트 및 시각적 효과 처리
        var newProps = new ExitGames.Client.Photon.Hashtable
        {
            ["StickOwner"] = owners
        };
        room.SetCustomProperties(newProps);

        BranchSpawner.Instance.CallBackBranchClick(stickIndex, actorNumber);
    }
}
