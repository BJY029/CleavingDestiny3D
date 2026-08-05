using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using ExitGames.Client.Photon;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;
using Potan.CoreUtils;
using System;

public class StickGameController : MonoBehaviourPunCallbacks
{
    // 나뭇가지(Branch)의 총 개수
    [SerializeField]
    private int branchCount;

    private float SelectDurationDefault = 20.0f;
    private double selectStartTime;
    private double selectDuration;
    private double endTime;

    private bool phaseActive;
    private bool isUpdatedTimer;
    private bool localResolved = false;
    private bool resolveRequested;
    private int playerCount;

    public Canvas BranchGameCanvas;
    public Slider Timer;



    // Singleton
    public static StickGameController Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        BranchGameCanvas.enabled = false;

        Timer.value = 1f;
        playerCount = PhotonNetwork.PlayerList.Length;
    }

    public void BeginStickGame()
    {
        if (phaseActive) return;

        phaseActive = true;
        localResolved = false;
        resolveRequested = false;
        isUpdatedTimer = false;

        playerCount = PhotonNetwork.PlayerList.Length;

        BranchGameCanvas.enabled = true;
        Timer.value = 1f;

        //이미 초기화 된 상태가 존재하면 읽는다.
        if (TryLoadRoundFromRoom()) return;

        if (PhotonNetwork.IsMasterClient) InitializeStickGame();
    }

    public void EndStickGame()
    {
        phaseActive = false;

        if (BranchGameCanvas != null) BranchGameCanvas.enabled = false;
    }



    private bool TryLoadRoundFromRoom()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null) return false;

        Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;

        if (!properties.TryGetValue(StickGameRoomKeys.SelectStartTime, out object startValue)
        || !properties.TryGetValue(StickGameRoomKeys.SelectDuration, out object durationValue))
            return false;

        selectStartTime = Convert.ToDouble(startValue);
        selectDuration = Convert.ToDouble(durationValue);
        endTime = selectStartTime + selectDuration;

        isUpdatedTimer = true;

        if (BranchGameCanvas != null) BranchGameCanvas.enabled = true;

        return true;
    }


    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if (changedProps.ContainsKey(StickGameRoomKeys.SelectStartTime) ||
            changedProps.ContainsKey(StickGameRoomKeys.SelectDuration))
        {
            TryLoadRoundFromRoom();
        }

        if (changedProps.TryGetValue(StickGameRoomKeys.SelectionResolved, out object resolvedValue)
         && (bool)resolvedValue)
        {
            localResolved = true;
            resolveRequested = false;
        }
    }

    // 플레이어 퇴장 처리
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerCount = PhotonNetwork.PlayerList.Length;
    }

    private int _lastTickSec = -1;

    private void Update()
    {
        if (!phaseActive || localResolved || !isUpdatedTimer || PhotonNetwork.CurrentRoom == null) return;

        Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;

        if (properties.TryGetValue(StickGameRoomKeys.SelectionResolved, out var r) && (bool)r)
        {
            localResolved = true;
            return;
        }

        double now = PhotonNetwork.Time;
        //double endTime = selectStartTime + selectDuration;
        double remain = endTime - now;
        Timer.value = Mathf.Clamp01((float)(remain / selectDuration));

        int remainSec = Mathf.Max(0, Mathf.CeilToInt((float)remain));

        if (remainSec > 0 && remainSec != _lastTickSec)
        {
            _lastTickSec = remainSec;
            if (remainSec > 5)
                AudioManager.Instance.PlaySfx2D("Time_Onetick");
            else
                AudioManager.Instance.PlaySfx2D("Time_Ticks");
        }

        bool allPlayerSelected = AreAllPlayerSelected(properties);

        if ((remain <= 0.0 || allPlayerSelected) && PhotonNetwork.IsMasterClient)
        {
            ResolveSelection();
        }
    }

    private bool AreAllPlayerSelected(Hashtable properties)
    {
        if (!properties.TryGetValue(StickGameRoomKeys.StickOwner, out object ownerValue))
            return false;

        int[] owners = (int[])ownerValue;

        return PhotonNetwork.PlayerList.All(player => owners.Contains(player.ActorNumber));
    }

    private void ResolveSelection()
    {
        if (!PhotonNetwork.IsMasterClient || resolveRequested || PhotonNetwork.CurrentRoom == null)
            return;

        localResolved = true;

        Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (!props.TryGetValue(StickGameRoomKeys.StickOwner, out object ownersValue)
        || !props.TryGetValue(StickGameRoomKeys.StickLengths, out object lengthsValue))
        {
            resolveRequested = false;
            Debug.LogError("스틱 데이터 초기화 안됨");
            return;
        }

        int[] owners = (int[])((int[])ownersValue).Clone();
        int[] lengths = (int[])((int[])lengthsValue).Clone();

        var rand = new System.Random();
        int randIdx;

        // key: actorNum, value: selected length
        Dictionary<int, int> setTurns = new Dictionary<int, int>();
        List<int> freeStickIndexes = Enumerable.Range(0, owners.Length).Where(index => owners[index] == -1).ToList();

        // 모든 플레이어를 순회하며
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // 플레이어의 고유 번호를 가져온다.
            int playerActNum = p.ActorNumber;

            int selectedIndex = Array.IndexOf(owners, playerActNum);

            if (selectedIndex >= 0)
            {
                setTurns[playerActNum] = lengths[selectedIndex];
                continue;
            }

            if (freeStickIndexes.Count == 0)
            {
                Debug.LogError("선택 가능한 나뭇가지수 부족");
                resolveRequested = false;
                return;
            }

            int randomListIndex = rand.Next(0, freeStickIndexes.Count);

            int stickIdx = freeStickIndexes[randomListIndex];

            freeStickIndexes.RemoveAt(randomListIndex);

            owners[stickIdx] = playerActNum;
            setTurns[playerActNum] = lengths[stickIdx];
        }

        if (GameManager.Instance.isSoloPlay)
        {
            int aiID = 1000;

            do
            {
                randIdx = rand.Next(0, branchCount);
            }
            while (owners[randIdx] != -1);

            owners[randIdx] = aiID;
            setTurns.Add(aiID, lengths[randIdx]);
        }


        // 길이를 기준으로 내림차순 정렬하여 턴 순서 배열 생성
        var sortedTurnInfo = setTurns.OrderByDescending(t => t.Value).ToList();
        int[] turnOrder = sortedTurnInfo.Select(t => t.Key).ToArray();
        int[] lengthOrder = sortedTurnInfo.Select(t => t.Value).ToArray();
        for (int i = 0; i < turnOrder.Length; i++)
        {
            Debug.Log($"turn order :  {i + 1} = player{turnOrder[i]}, length:{lengthOrder[i]}");
        }
#if SOLO_PLAYER_FIRST
        DevLog.Log("<color=green>SOLO_PLAYER_FIRST is defined. Forcing Player to be first.</color>");

        // 플레이어가 첫번째가 되도록 강제 설정 (테스트용)
        int playerActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        int playerIndex = System.Array.IndexOf(turnOrder, playerActorNum);
        if (playerIndex > 0)
        {
            // 플레이어가 있는 위치와 첫 번째 위치의 값을 교환
            (turnOrder[playerIndex], turnOrder[0]) = (turnOrder[0], turnOrder[playerIndex]);

            // 길이 순서도 동일하게 교환
            (lengthOrder[playerIndex], lengthOrder[0]) = (lengthOrder[0], lengthOrder[playerIndex]);
            DevLog.Log($"Turn order after forcing player first: {string.Join(", ", turnOrder)}");
        }

#endif
        // 룸 프로퍼티에 저장
        Hashtable resultProperties = new()
    {
        {
            StickGameRoomKeys.TurnInfo,
            turnOrder
        },
        {
            StickGameRoomKeys.StickOwner,
            owners
        },
        {
            StickGameRoomKeys.SelectionResolved,
            true
        },
        {
            StickGameRoomKeys.SelectionResolvedTime,
            PhotonNetwork.Time
        }
    };
        PhotonNetwork.CurrentRoom.SetCustomProperties(resultProperties);
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
    public void InitializeStickGame()
    {
        if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null) return;

        // 나뭇가지 개수만큼 배열 생성 및 초기화
        int[] lengths = new int[branchCount];
        int[] owners = new int[branchCount];
        for (int i = 0; i < branchCount; i++)
        {
            lengths[i] = i + 1;
            // 소유주 배열을 -1(소유주 없음)로 초기화
            owners[i] = -1;
        }
        // 각 나뭇가지의 소유주를 저장할 배열 생성


        // 나뭇가지의 길이를 무작위로 섞기 위해 셔플 알고리즘 사용
        System.Random random = new();

        for (int i = lengths.Length - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            (lengths[i], lengths[randomIndex]) = (lengths[randomIndex], lengths[i]);
        }

        double now = PhotonNetwork.Time;

        Hashtable properties = new()
    {
        {
            StickGameRoomKeys.StickLengths,
            lengths
        },
        {
            StickGameRoomKeys.StickOwner,
            owners
        },
        {
            StickGameRoomKeys.SelectStartTime,
            now
        },
        {
            StickGameRoomKeys.SelectDuration,
            (double)SelectDurationDefault
        },
        {
            StickGameRoomKeys.SelectionResolved,
            false
        },
        {
            StickGameRoomKeys.SelectionResolvedTime,
            0d
        }
    };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    // 특정 나뭇가지를 클릭했을 때 호출되는 함수
    public void OnClickStick(int stickIndex)
    {
        if (!PhotonNetwork.InRoom || !phaseActive || localResolved) return;

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

        int[] owners = (int[])((int[])props["StickOwner"]).Clone();
        int[] lengths = (int[])((int[])props["StickLengths"]).Clone();

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
