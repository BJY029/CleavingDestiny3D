using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MatchResultManager : MonoBehaviourPunCallbacks
{
    //싱글턴
    public static MatchResultManager Instance;
    //경기 결과 결정 여부
    private bool _isResultResolved = false;
    //경기 결과 이유
    private MatchResultReason _lastResaon = MatchResultReason.NONE;
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    //초기화자
    private bool IsInitializer() => PhotonNetwork.IsMasterClient;

    //Room Property 감지
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //나무 체력이 변경된 경우
        if (propertiesThatChanged.ContainsKey(RoomPropKeys.TreeHP) && IsInitializer())
        {
            //게임 종료 확인
            TryResolveResultByTreeHP();
        }

        //게임 Phase가 END로 변경된 경우
        if (propertiesThatChanged.ContainsKey(RoomPropKeys.GamePhase))
        {
            GamePhaseValue phase = PhotonPropertyHelper.GetRoomProp<GamePhaseValue>(RoomPropKeys.GamePhase);
            if (phase == GamePhaseValue.END)
            {
                //다른 클라이언트들에게 게임 종료 처리
                AnnounceResultFromRoomProp();
            }
        }
    }

    //플레이어 프로퍼티 감지
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!IsInitializer() || _isResultResolved) return;

        //밤 페이즈의 나무 공격인 경우, 해당 처리를 하지 않기 위해 설정
        bool IsTreeBulkDamage = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsTreeBulkDamage);
        //마을 체력이 변경되었고, 밤에 발생한 일괄 데미지가 아닌 경우
        if (changedProps.ContainsKey(PlayerPropKeys.VillageHP) && !IsTreeBulkDamage)
        {
            Debug.Log("Not a bulk damage");
            //게임 종료 확인
            TryResolveResultByVillageHP();
        }
    }

    //나무 체력 기반 게임 종료 확인
    public bool TryResolveResultByTreeHP()
    {
        if (!IsInitializer() || _isResultResolved) return false;

        //현재 나무 체력
        float CurTreeHP = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeHP);
        Debug.Log("CurrentTreeHP : " + CurTreeHP);
        //0 이상인 경우 종료가 아님
        if (CurTreeHP > 0f) return false;

        //게임 종료 처리, 패배한 플레이어의 번호 가져오기
        int LostActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.CurrentTurnActor);
        //패배 처리 설정 
        TrySetMatchResult(LostActor, MatchResultReason.TREE_DESTROYED);

        return true;
    }

    //마을 체력 기반 게임 종료 확인
    public bool TryResolveResultByVillageHP()
    {
        if (!IsInitializer() || _isResultResolved) return false;

        //만약 아직 각 플레이어들의 프로퍼티 설정이 끝나지 않은 경우, 실행하지 않음
        if (!isAllPlayerInit()) return false;

        //각 플레이어들의 마을 체력 프로퍼티를 확인하여, 0 이하인 플레이어들을 받아온다.
        Player[] destroyedVillage = PhotonNetwork.PlayerList.Where(
            p => PhotonPropertyHelper.GetPlayerProp<float>(p, PlayerPropKeys.VillageHP) <= 0f).ToArray();

        //만약 마을이 파괴된 플레이어가 없는 경우
        if (destroyedVillage.Length == 0) return false;
        //마을이 파괴된 플레이어가 1명 있는 경우
        if (destroyedVillage.Length == 1)
        {
            int LostActor = destroyedVillage[0].ActorNumber;
            TrySetMatchResult(LostActor, MatchResultReason.VILLAGE_DESTROYED);
        }
        //마을이 파괴된 플레이어가 2명 이상인 경우, 무승부로 처리
        else TrySetMatchResult(-1, MatchResultReason.DRAW);

        return true;
    }

    //모든 플레이어 프로퍼티가 초기화되었는지 확인
    private bool isAllPlayerInit()
    {
        return PhotonNetwork.PlayerList.All(p => p.CustomProperties.TryGetValue(PlayerPropKeys.IsReady, out var v) && (bool)v);
    }

    //게임 종료 처리 수행
    private void TrySetMatchResult(int LoserActorNum, MatchResultReason reason)
    {
        if (!IsInitializer() || _isResultResolved) return;

        //현재까지 진행된 턴 수 받아오기
        int resolveTrunIndex = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.TurnIndex);
        //프로퍼티 설정
        var ht = new ExitGames.Client.Photon.Hashtable
        {
            {RoomPropKeys.MatchLoserActor, LoserActorNum},
            {RoomPropKeys.MatchResultReason, reason.ToString()},
            {RoomPropKeys.MatchResolveTurnIndex, resolveTrunIndex},
            {RoomPropKeys.GamePhase, GamePhaseValue.END}, //해당 값도 업데이트 되면서 동시에 다른 클라에게 게임 종료 전파됨
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);

        //게임 종료 설정
        _isResultResolved = true;
        _lastResaon = reason;

        //디버그
        string loserText = "Player" + LoserActorNum;
        Debug.Log($"[MatchResult] Loser : {loserText}, Reason : {reason}, TurnCnt : {resolveTrunIndex}");
    }

    //게임 상테가 END로 변경되면 각 클라에게 호출될 함수
    private void AnnounceResultFromRoomProp()
    {
        if (_isResultResolved) return;

        //각 프로퍼티 가져온다.
        int LoserActor = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.MatchLoserActor);
        string reasonStr = PhotonPropertyHelper.GetRoomProp<string>(RoomPropKeys.MatchResultReason);
        int resolvedTurn = PhotonPropertyHelper.GetRoomProp<int>(RoomPropKeys.MatchResolveTurnIndex);

        //실제 해당되는 Enum이 있는지 확인
        //있으면, out 으로  reason이 반환되며, 없으면 조건문 내에서 NONE으로 설정됨
        if (!Enum.TryParse(reasonStr, out MatchResultReason reason))
        {
            reason = MatchResultReason.NONE;
        }

        _lastResaon = reason;
        _isResultResolved = true;

        string loserText = "Player" + LoserActor;
        Debug.Log($"[MatchResult] Loser : {loserText}, Reason : {reason}, TurnCnt : {resolvedTurn}");
    }
}
