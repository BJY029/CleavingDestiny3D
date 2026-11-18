using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("UI References")]
    public GameObject loadingPanel;
    public TextMeshProUGUI sceneLoadingText;
    public TextMeshProUGUI mainLoadingText;
    public TextMeshProUGUI tipsText;
    public TextMeshProUGUI timer;
    public Button stopMatching;

    private const float DotDuration = 0.4f;
    private const float WaitDuration = 0.5f;

    private CancellationTokenSource dotsCancelToken;
    private bool isLoadingCanvasActivated;

    private Queue<int> randomTipNums = new Queue<int>();
    [Header("Tip Settings")]
    [SerializeField, Min(1)] private int tipsMinNum = 1;
    [SerializeField, Min(1)] private int tipsMaxNum = 37;
    [SerializeField, Min(1f)] private float duration = 5f;
    private float elapsedTime;
    private bool tipsInitialized;
    private Language tipsLanguage;
    private int lastTipNum = -1;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            HideLoadingUI();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void FillQueueWithoutDup(int min, int max)
    {
        randomTipNums.Clear();

        if (min < 1 || max < min)
        {
            Debug.LogWarning("[SceneLoader] 팁 번호 범위를 확인하세요.", this);
            return;
        }

        List<int> tempNums = new List<int>();
        var localization = LocalizationManager.Instance;
        for (int i = min; i <= max; i++)
        {
            string key = $"TIP_ITEM_{i:D3}";
            string text = localization.GetRawText(CSV_Type.MatchmakingTips, key);
            if (!string.IsNullOrWhiteSpace(text) && text != key) tempNums.Add(i);
            if (i == int.MaxValue) break;
        }

        for (int i = tempNums.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            int temp = tempNums[i];
            tempNums[i] = tempNums[randomIndex];
            tempNums[randomIndex] = temp;
        }

        // 새 묶음의 첫 팁이 직전 팁과 같은 경우에도 연속 중복을 피한다.
        if (tempNums.Count > 1 && tempNums[0] == lastTipNum)
        {
            int swapIndex = UnityEngine.Random.Range(1, tempNums.Count);
            (tempNums[0], tempNums[swapIndex]) = (tempNums[swapIndex], tempNums[0]);
        }
        foreach (int num in tempNums) randomTipNums.Enqueue(num);
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

    private void Update()
    {
        UpdateTips();
    }

    private void UpdateTips()
    {
        if (!isLoadingCanvasActivated || tipsText == null) return;

        var localization = LocalizationManager.Instance;
        if (localization == null || !localization.IsLoaded)
        {
            tipsText.text = string.Empty;
            tipsInitialized = false;
            return;
        }

        // 최초 로딩 완료 또는 언어 변경 직후에는 기다리지 않고 첫 팁을 표시한다.
        if (!tipsInitialized || tipsLanguage != localization.currentLanguage)
        {
            tipsInitialized = true;
            tipsLanguage = localization.currentLanguage;
            FillQueueWithoutDup(tipsMinNum, tipsMaxNum);
            if (randomTipNums.Count == 0)
            {
                Debug.LogWarning("[SceneLoader] 표시할 팁이 없습니다. CSV 등록과 번호 범위를 확인하세요.", this);
                lastTipNum = -1;
                tipsText.text = string.Empty;
                return;
            }
            ShowNextTip();
            return;
        }

        if (randomTipNums.Count == 0 && lastTipNum < 0) return;
        elapsedTime += Time.unscaledDeltaTime;
        if (elapsedTime >= Mathf.Max(1f, duration)) ShowNextTip();
    }

    private void ShowNextTip()
    {
        elapsedTime = 0f;
        if (randomTipNums.Count == 0) FillQueueWithoutDup(tipsMinNum, tipsMaxNum);
        if (randomTipNums.Count == 0)
        {
            lastTipNum = -1;
            tipsText.text = string.Empty;
            return;
        }

        lastTipNum = randomTipNums.Dequeue();
        tipsText.text = LocalizationManager.Instance.GetText(CSV_Type.MatchmakingTips, $"TIP_ITEM_{lastTipNum:D3}");
    }

    private void ResetTips()
    {
        isLoadingCanvasActivated = false;
        tipsInitialized = false;
        elapsedTime = 0f;
        lastTipNum = -1;
        randomTipNums.Clear();
        if (tipsText != null) tipsText.text = string.Empty;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance != this) return;
        if (mode == LoadSceneMode.Single && scene.name == CommonDefine.LOBBYSCENE)
            FadeCanvas.Instance?.SetFade(false);
        // 씬 로드가 완료되면 로딩창을 비활성화합니다.
        HideLoadingUI();
    }

    /// <summary>
    /// 로딩 UI를 즉시 띄우고 점 애니메이션을 시작합니다 (포톤 LoadLevel 동기화 씬 전환용)
    /// </summary>
    public void ShowLoadingUI(string overrideTextKey = null)
    {
        StopDotsAnimation();
        ResetTips();

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            loadingPanel.transform.localScale = Vector3.one;
        }
        SetTimerVisible(false);
        if (stopMatching != null) stopMatching.gameObject.SetActive(false);
        if (sceneLoadingText != null) sceneLoadingText.gameObject.SetActive(true);
        if (mainLoadingText != null) mainLoadingText.gameObject.SetActive(true);

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

        dotsCancelToken = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        StartDotsAnimation(dotsCancelToken.Token).Forget();
    }

    // The persistent panel is reused; restore every child changed by scene loading.
    public void ShowMatchmakingUI()
    {
        HideLoadingUI();
        if (loadingPanel != null)
        {
            loadingPanel.transform.localScale = Vector3.one;
            loadingPanel.SetActive(true);
        }
        if (mainLoadingText != null) mainLoadingText.gameObject.SetActive(true);
        SetTimerVisible(true);
        if (stopMatching != null)
        {
            stopMatching.gameObject.SetActive(true);
            stopMatching.interactable = true;
        }
        isLoadingCanvasActivated = true;
        if (tipsText != null) tipsText.gameObject.SetActive(true);
        UpdateTips();
    }

    /// <summary>
    /// 로딩 UI를 비활성화하고 애니메이션을 멈춥니다.
    /// </summary>
    public void HideLoadingUI()
    {
        StopDotsAnimation();
        ResetTips();
        if (mainLoadingText != null) mainLoadingText.text = string.Empty;
        if (sceneLoadingText != null)
        {
            sceneLoadingText.text = string.Empty;
            sceneLoadingText.gameObject.SetActive(false);
        }
        if (timer != null) timer.text = "00:00";
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private void SetTimerVisible(bool visible)
    {
        if (timer == null) return;
        Transform group = timer.transform;
        // TimerBG and its text form one group directly under the shared loading panel.
        if (loadingPanel != null && group.IsChildOf(loadingPanel.transform))
            while (group.parent != loadingPanel.transform) group = group.parent;
        group.gameObject.SetActive(visible);
        if (visible) timer.gameObject.SetActive(true);
    }

    private void StopDotsAnimation()
    {
        if (dotsCancelToken != null)
        {
            dotsCancelToken.Cancel();
            dotsCancelToken.Dispose();
            dotsCancelToken = null;
        }
    }

    private async UniTask StartDotsAnimation(CancellationToken token)
    {
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
            while (!token.IsCancellationRequested)
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
