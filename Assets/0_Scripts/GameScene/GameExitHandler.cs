using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;

public class GameExitHandler : MonoBehaviourPunCallbacks
{
    public static GameExitHandler instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    //메인화면 로딩 창
    [Header("Loading UIs")]
    public GameObject LoadingPanel;
    public TextMeshProUGUI SceneLoadingText;

    //로딩 화면 관련 값들
    private float DotDuration = 0.4f;
    private float WaitDuration = 0.5f;

    //프로퍼티 초기화 플래그
    private bool isClearingProperties = false;
    //초기화될 프로퍼티 키들을 저장하는 해시 컨테이너
    private HashSet<string> keysToClear = new HashSet<string>();

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
            MatchResultManager.Instance.TrySetMatchResult(otherPlayer.ActorNumber, MatchResultReason.Player_Left);
        }
    }

    /// <summary>
    /// 방을 떠날 때 호출하는 함수
    /// </summary>
    public void RequestLeaveGame()
    {
        //로딩창 활성화
        if (LoadingPanel != null) LoadingPanel.SetActive(true);
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

    //플레이어 프로퍼티를 초기화 하는 함수
    private void InitPlayerProps()
    {
        //플래그 활성화
        isClearingProperties = true;
        //해시 초기화
        keysToClear.Clear();
        //플레이어의 프로퍼티를 받아오기 위한 해시 선언
        ExitGames.Client.Photon.Hashtable allClear = new ExitGames.Client.Photon.Hashtable();

        //로컬 플레이어(호출자)의 프로퍼티를 돌아보면서
        foreach (var key in PhotonNetwork.LocalPlayer.CustomProperties.Keys)
        {
            //키 값을 따로 저장
            string keyStr = key.ToString();
            keysToClear.Add(keyStr);
            //프로퍼티 초기화 수행
            allClear[keyStr] = null;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(allClear);
        //2초간 초기화가 이루어지지 않은 경우 콜백이 일어나지 않았다고 판단, 강제로 방을 떠나도록 설정
        //Invoke(nameof(ForceToLeaveRoom), 2.0f);
    }

    // //플레이어 프로퍼티 감지
    // public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    // {
    //     //프로퍼티 초기화인 경우에만 실행된다.
    //     if (!isClearingProperties || !targetPlayer.IsLocal) return;

    //     //프로퍼티 키 값들을 돌아보면서
    //     foreach (var key in changedProps.Keys)
    //     {
    //         //해당되는 키 값에 매칭되는 프로퍼티가 존재하면서 동시에 초기화 되었는지 확인
    //         string keyStr = key.ToString();
    //         if (keysToClear.Contains(keyStr) && changedProps[key] == null)
    //         {
    //             //초기화 된 경우, Key 해시에서 제거
    //             keysToClear.Remove(keyStr);
    //         }
    //     }

    //     //만약 Key 해시의 크기가 0인 경우
    //     if (keysToClear.Count == 0)
    //     {
    //         //강제 퇴장 로직 해제
    //         CancelInvoke(nameof(ForceToLeaveRoom));
    //         //방 퇴장 실행
    //         ProceedToLeaveRoom();
    //     }
    // }

    //방을 나가는 로직
    private void ProceedToLeaveRoom()
    {
        isClearingProperties = false;

    }

    //로비로 씬 변경
    private void GoToLobby()
    {
        //비동기로 씬 로딩
        GameManager.Instance.nextScene = CommonDefine.LOBBYSCENE;
        StartCoroutine(LoadScene());
    }

    //로비로 씬을 비동기로 로딩하는 코루틴
    IEnumerator LoadScene()
    {
        yield return null;
        //다음 씬 설정
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameManager.Instance.nextScene);
        // 즉시 전환되지 않도록 설정
        asyncOperation.allowSceneActivation = false;

        string originText = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_Loading);
        int curDot = 0;
        string Dot = "";

        //90퍼가 될 때까지 점 애니메이션 출력
        while (asyncOperation.progress < 0.9f)
        {
            Dot = "";
            for (int i = 0; i < curDot; i++)
            {
                Dot += ".";
            }
            SceneLoadingText.text = originText + Dot;
            curDot = (curDot + 1) % 4;

            yield return new WaitForSeconds(DotDuration);
        }

        //로딩 완료 시 씬 변경
        SceneLoadingText.text = originText + "...Done!";
        yield return new WaitForSeconds(WaitDuration);

        asyncOperation.allowSceneActivation = true;
    }
}
