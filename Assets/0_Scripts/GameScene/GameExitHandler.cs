using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameExitHandler : MonoBehaviourPunCallbacks
{
    public static GameExitHandler Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //룸을 떠난것이 감지된 경우
    public override void OnLeftRoom()
    {
        CameraSwitchManager.Instance.GameCameraToggle(true);
        //씬 변경 로직 수행
        GoToLobby();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //게임이 이미 종료되었는지 확인
        if (MatchResultManager.Instance._isResultResolved) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            MatchResultManager.Instance.TrySetMatchResult(otherPlayer.ActorNumber, MatchResultReason.PlayerLeft);
        }
    }

    /// <summary>
    /// 방을 떠날 때 호출하는 함수
    /// </summary>
    public void RequestLeaveGame()
    {
        SettingCanvasController.instance.CloseSettingPanel();
        //방 안에 있으면
        if (PhotonNetwork.InRoom)
        {
            //씬 이동 동기화 해제
            PhotonNetwork.AutomaticallySyncScene = false;
            //룸 떠나기
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            GoToLobby();
        }
    }
    
    //로비로 씬 변경
    private void GoToLobby()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isSoloPlay = false;
        }
        GameManager.Instance.nextScene = CommonDefine.LOBBYSCENE;
        SceneLoader.Instance.LoadSceneAsync(CommonDefine.LOBBYSCENE, UI_CSV.UI_Load_ReturningToLobby).Forget();
    }
}
