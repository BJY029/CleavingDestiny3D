using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePrepareCanvasController : MonoBehaviour
{
    public static GamePrepareCanvasController instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [Header("UIs")]
    public GameObject PrepareCanvasObj;
    public TextMeshProUGUI WeatherInfo;
    public TextMeshProUGUI PlayerStatusInfo;
    public TextMeshProUGUI TreeStatusInfo;
    public TextMeshProUGUI BranchGameText;
    public TextMeshProUGUI BranchGameInfo;
    public TextMeshProUGUI CurrentState;

    [SerializeField] private float showGameResultDuration = 3.0f;

    public void SetUnActive()
    {
        PrepareCanvasObj.SetActive(false);
    }

    public void SetPrepareCanvasAsWeather(GameTheme curTheme)
    {
        string weather = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Weather);
        string player = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Player);
        string tree = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Tree);
        switch (curTheme)
        {
            case GameTheme.Clear:
                WeatherInfo.text = weather + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Weather_Clear);
                PlayerStatusInfo.text = player + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Common);
                TreeStatusInfo.text = tree + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Common);
                break;
            case GameTheme.Storm:
                WeatherInfo.text = weather + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Weather_Storm);
                //수정 필요
                PlayerStatusInfo.text = player + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Common);
                TreeStatusInfo.text = tree + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Common);
                break;
            case GameTheme.Fog:
                WeatherInfo.text = weather + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Weather_Fog);
                //수정 필요
                PlayerStatusInfo.text = player + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Common);
                TreeStatusInfo.text = tree + " : " + LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_Common);
                break;
        }

        BranchGameText.text = "";
        BranchGameInfo.text = "";
        CurrentState.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_WaitingMG);

        PrepareCanvasObj.SetActive(true);
    }

    private Coroutine resultCoroutine;

    public void ShowBranchGameResult(bool isFirstTurn)
    {
        if (resultCoroutine != null) StopCoroutine(resultCoroutine);
        resultCoroutine = StartCoroutine(IShowBranchGameResult(isFirstTurn));
    }

    public IEnumerator IShowBranchGameResult(bool isFirstTurn)
    {
        BranchGameText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_MGResult);
        CurrentState.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_WaitingMainG);
        if (isFirstTurn)
        {
            BranchGameInfo.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_MGResult_Win);
        }
        else
        {
            BranchGameInfo.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Prepare_MGResult_Lose);
        }

        PrepareCanvasObj.SetActive(true);
        yield return new WaitForSecondsRealtime(showGameResultDuration);

        resultCoroutine = null;
    }
}
