using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BettingSystemController : MonoBehaviourPunCallbacks
{
    public static BettingSystemController instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        BettingSystemActivated = false;
    }

    //관련 UI를 컨트롤러하는 스크립트
    public BettingSystemUIController betUIController;

    //UI 활성화 여부를 관리하는 변수
    public bool BettingSystemActivated { get; private set; }

    //배팅 거절 시 배팅 금액의 환수 비율
    [SerializeField] private float conversionRate = 0.5f;
    //최소 환수 값
    [SerializeField] private int minReward = 1;
    //최대 환수 값
    [SerializeField] private int maxReward = 5;
    //기력 부족할 때 감소되는 마을 체력 값
    [SerializeField] private int villageHPBasicValue = 100;

    //actorNumber로 Player 객체 찾기
    private Player GetPlayer(int actorNumber)
    {
        if (!PhotonNetwork.InRoom) return null;
        return PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
    }

    //환수 되는 배팅 에너지 계산 함수
    private int CalculateDeclineReward(int betEnergy)
    {
        return Mathf.Clamp(Mathf.CeilToInt(betEnergy * conversionRate), minReward, maxReward);
    }

    //배팅 게임을 시작하는 함수
    public void StartBettingGame(Player requestPlayer, Player targetPlayer)
    {
        if (requestPlayer == null || targetPlayer == null) return;

        //MasterClient가 책임지고 수행
        photonView.RPC(nameof(RPC_StartBettingGame), RpcTarget.MasterClient, requestPlayer.ActorNumber, targetPlayer.ActorNumber);
    }

    //유효한 요청인지 확인하는 함수
    public void RequestVerification(int requestActorNumber, int targetActorNumber, int betEnergy)
    {
        //MasterClient가 책임지고 수행
        photonView.RPC(nameof(RPC_RequestVerification), RpcTarget.MasterClient, betEnergy, requestActorNumber, targetActorNumber);
    }

    //실제 미니 게임을 시작하는 함수
    public void StartLogWoodGame(int requestActorNumber, int targetActorNumber, int betEnergy)
    {
        //MasterClient가 책임지고 수행
        photonView.RPC(nameof(RPC_StartLogWoodGame), RpcTarget.MasterClient, requestActorNumber, targetActorNumber, betEnergy);
    }

    //배팅 게임을 거절하는 함수
    public void RejectBettingGame(int requestActorNumber, int targetActorNumber, int betEnergy)
    {
        //MasterClient가 책임지고 수행
        photonView.RPC(nameof(RPC_RejectBettingGame), RpcTarget.MasterClient, requestActorNumber, targetActorNumber, betEnergy);
    }

    [PunRPC]
    private void RPC_StartBettingGame(int requestActorNumber, int targetActorNumber, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        BettingSystemActivated = true;

        Player requestPlayer = GetPlayer(requestActorNumber);
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (requestPlayer == null || targetPlayer == null)
        {
            Debug.LogWarning("[Betting] 플레이어를 찾을 수 없습니다.");
            return;
        }

        //요청자의 현재 기력량 가져오기
        int requestPlayerCurEng = PhotonPropertyHelper.GetPlayerProp<int>(requestActorNumber, PlayerPropKeys.Energy);

        //UI 업데이트
        betUIController.Master_StartBetGame(requestActorNumber, targetActorNumber, requestPlayerCurEng);
    }

    [PunRPC]
    private void RPC_RequestVerification(int betEnergy, int requestActorNumber, int targetActorNumber, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Player requestPlayer = GetPlayer(requestActorNumber);
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (requestPlayer == null || targetPlayer == null)
        {
            Debug.LogWarning("[Betting] 플레이어를 찾을 수 없습니다.");
            return;
        }

        if (info.Sender.ActorNumber != requestActorNumber)
        {
            Debug.LogWarning("[Betting] 요청자 정보가 일치하지 않습니다.");
            return;
        }

        if (betEnergy <= 0)
        {
            Debug.LogWarning("[Betting] 배팅 기력은 1 이상이어야 합니다.");
            return;
        }

        //요청자 현재 기력 가져오기
        int requestPlayerCurEng = PhotonPropertyHelper.GetPlayerProp<int>(requestActorNumber, PlayerPropKeys.Energy);

        if (requestPlayerCurEng < betEnergy)
        {
            Debug.LogWarning("[Betting] 요청자의 기력이 부족합니다.");
            return;
        }

        //타겟 기력 가져오기
        int targetPlayerCurEng = PhotonPropertyHelper.GetPlayerProp<int>(targetActorNumber, PlayerPropKeys.Energy);

        //거절 시 환수되는 기력 계산
        int declineReward = CalculateDeclineReward(betEnergy);

        //관련 UI 처리
        betUIController.Master_RequestSended(requestActorNumber, targetActorNumber, targetPlayerCurEng, betEnergy, declineReward, villageHPBasicValue);
    }

    //실제 게임을 시작하는 함수
    [PunRPC]
    private void RPC_StartLogWoodGame(int requestActorNumber, int targetActorNumber, int betEnergy, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Player requestPlayer = GetPlayer(requestActorNumber);
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (requestPlayer == null || targetPlayer == null)
        {
            Debug.LogWarning("[Betting] 플레이어를 찾을 수 없습니다.");
            return;
        }

        if (info.Sender.ActorNumber != targetActorNumber)
        {
            Debug.LogWarning("[Betting] 수락자 정보가 일치하지 않습니다.");
            return;
        }

        int requestPlayerCurEng = PhotonPropertyHelper.GetPlayerProp<int>(requestActorNumber, PlayerPropKeys.Energy);
        int targetPlayerCurEng = PhotonPropertyHelper.GetPlayerProp<int>(targetActorNumber, PlayerPropKeys.Energy);

        if (requestPlayerCurEng < betEnergy)
        {
            Debug.LogWarning("[Betting] 요청자의 기력이 부족하여 결투를 시작할 수 없습니다.");
            betUIController.Master_CloseBetPanels(requestActorNumber, targetActorNumber, false, 0);
            return;
        }

        //UI를 닫고 초기화
        betUIController.Master_CloseBetPanels(requestActorNumber, targetActorNumber, false, 0);

        //게임 시작 함수 호출
        WoodChopController.instance.RequestStartDual(requestPlayer, targetPlayer, betEnergy);

        //UI 활성화 플래그 해제
        BettingSystemActivated = false;
    }

    //게임 거절 시 호출
    [PunRPC]
    private void RPC_RejectBettingGame(int requestActorNumber, int targetActorNumber, int betEnergy, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Player requestPlayer = GetPlayer(requestActorNumber);
        Player targetPlayer = GetPlayer(targetActorNumber);

        if (requestPlayer == null || targetPlayer == null)
        {
            Debug.LogWarning("[Betting] 플레이어를 찾을 수 없습니다.");
            return;
        }

        if (info.Sender.ActorNumber != targetActorNumber)
        {
            Debug.LogWarning("[Betting] 거절자 정보가 일치하지 않습니다.");
            return;
        }

        //Master가 거절된 게임 처리
        Master_HandleDecline(requestActorNumber, targetActorNumber, betEnergy, false);
    }

    private void Master_HandleDecline(int requestActorNumber, int targetActorNumber, int betEnergy, bool isAutoDecline)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        //환수되는 기력 값을 처리하는 함수
        int declineReward = CalculateDeclineReward(betEnergy);
        int requestEnergy = PhotonPropertyHelper.GetPlayerProp<int>(requestActorNumber, PlayerPropKeys.Energy);
        int newRequestEnergy = requestEnergy + declineReward;

        PhotonPropertyHelper.SetPlayerProp(requestActorNumber, PlayerPropKeys.Energy, newRequestEnergy);

        betUIController.Master_CloseBetPanels(requestActorNumber, targetActorNumber, isAutoDecline, declineReward);
        BettingSystemActivated = false;
    }

    //미니 게임 결과 값을 반영하는 함수
    public void Master_SettleBetResult(int winnerActorNumber, int loserActorNumber, int betEnergy)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int winnerEnergy = PhotonPropertyHelper.GetPlayerProp<int>(winnerActorNumber, PlayerPropKeys.Energy);
        int winnerEngMax = PhotonPropertyHelper.GetPlayerProp<int>(winnerActorNumber, PlayerPropKeys.MaxEnergy);
        int loserEnergy = PhotonPropertyHelper.GetPlayerProp<int>(loserActorNumber, PlayerPropKeys.Energy);

        int energyTakenFromLoser = Mathf.Min(loserEnergy, betEnergy);
        int shortage = betEnergy - energyTakenFromLoser;

        int newWinnerEnergy = Mathf.Min(winnerEnergy + betEnergy, winnerEngMax);
        int newLoserEnergy = loserEnergy - energyTakenFromLoser;

        PhotonPropertyHelper.SetPlayerProp(winnerActorNumber, PlayerPropKeys.Energy, newWinnerEnergy);
        PhotonPropertyHelper.SetPlayerProp(loserActorNumber, PlayerPropKeys.Energy, newLoserEnergy);

        //초과된 기력 값에 대해 마을 체력을 감소시키는 계산 수행
        if (shortage > 0)
        {
            float loserVillageHP = PhotonPropertyHelper.GetPlayerProp<float>(loserActorNumber, PlayerPropKeys.VillageHP);
            float villageDamage = shortage * villageHPBasicValue;
            float newLoserVillageHP = Mathf.Max(0f, loserVillageHP - villageDamage);

            PhotonPropertyHelper.SetPlayerProp(loserActorNumber, PlayerPropKeys.VillageHP, newLoserVillageHP);

            Debug.Log($"[Betting] 기력 부족분 발생 / 부족 기력: {shortage}, 마을 체력 피해: {villageDamage}");
        }
    }
}