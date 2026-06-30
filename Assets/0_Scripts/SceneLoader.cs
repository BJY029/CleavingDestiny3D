using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("UI References")]
    public GameObject loadingPanel;
    public TextMeshProUGUI sceneLoadingText;
    public TextMeshProUGUI mainLoadingText;
    public TextMeshProUGUI timer;
    public Button stopMatching;

    private const float DotDuration = 0.4f;
    private const float WaitDuration = 0.5f;

    private CancellationTokenSource dotsCancelToken;
    private bool isAnimating = false;
    private string currentLoadingKey = UI_CSV.UI_Load_Loading;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopDotsAnimation();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드가 완료되면 로딩창을 비활성화합니다.
        HideLoadingUI();
    }

    /// <summary>
    /// 로딩 UI를 즉시 띄우고 점 애니메이션을 시작합니다 (포톤 LoadLevel 동기화 씬 전환용)
    /// </summary>
    public void ShowLoadingUI(string overrideTextKey = null)
    {
        currentLoadingKey = overrideTextKey;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // overrideTextKey가 지정되었을 때만 mainLoadingText의 내용을 업데이트합니다.
        if (mainLoadingText != null && !string.IsNullOrEmpty(overrideTextKey))
        {
            if (LocalizationManager.Instance != null)
            {
                mainLoadingText.text = LocalizationManager.Instance.GetText(CSV_Type.UI, overrideTextKey);
            }
            else
            {
                mainLoadingText.text = overrideTextKey;
            }
        }
        
        StopDotsAnimation();
        dotsCancelToken = new CancellationTokenSource();
        StartDotsAnimation(dotsCancelToken.Token).Forget();
    }

    /// <summary>
    /// 로딩 UI를 비활성화하고 애니메이션을 멈춥니다.
    /// </summary>
    public void HideLoadingUI()
    {
        StopDotsAnimation();
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private void StopDotsAnimation()
    {
        if (dotsCancelToken != null)
        {
            dotsCancelToken.Cancel();
            dotsCancelToken.Dispose();
            dotsCancelToken = null;
        }
        isAnimating = false;
    }

    private async UniTask StartDotsAnimation(CancellationToken token)
    {
        isAnimating = true;
        string originText = "";
        
        // 보조 로딩 텍스트는 언제나 일반 로딩("로딩 중") 텍스트를 기준으로 점 애니메이션을 수행합니다.
        if (LocalizationManager.Instance != null)
        {
            originText = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_Load_Loading);
        }
        else
        {
            originText = "Loading";
        }

        string[] dots = { originText + ".", originText + "..", originText + "..." };
        int idx = 0;

        try
        {
            while (!token.IsCancellationRequested && isAnimating)
            {
                if (sceneLoadingText != null)
                {
                    sceneLoadingText.text = dots[idx];
                    idx = (idx + 1) % 3;
                }
                await UniTask.WaitForSeconds(DotDuration, cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            // 의도된 취소 처리
        }
        finally
        {
            if (sceneLoadingText != null)
            {
                sceneLoadingText.text = originText + "...Done!";
            }
        }
    }

    /// <summary>
    /// [일반 로컬 씬 로딩] 로컬 클라이언트 단독으로 비동기 씬 전환을 할 때 사용합니다. (예: 인게임 -> 로비 퇴장 시)
    /// </summary>
    public async UniTask LoadSceneAsync(string sceneName, string overrideTextKey = null)
    {
        ShowLoadingUI(overrideTextKey);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        // 90% 완료될 때까지 로딩 애니메이션 재생
        while (asyncOperation.progress < 0.9f)
        {
            await UniTask.WaitForSeconds(DotDuration);
        }

        await UniTask.WaitForSeconds(WaitDuration);
        asyncOperation.allowSceneActivation = true;

        await asyncOperation.ToUniTask();
        // OnSceneLoaded 콜백에서 자동으로 HideLoadingUI()가 호출되므로 여기서는 생략 가능합니다.
    }
}
