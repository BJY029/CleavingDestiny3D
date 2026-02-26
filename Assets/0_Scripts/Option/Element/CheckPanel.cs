using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button acceptButton;
    [SerializeField] Button cancelButton;

    int waitTime = -1;

    Action OnAceeptClicked;
    Action OnCancelClicked;
    LocalizedString localizedMessage;

    CancellationTokenSource cancellationTokenSource;

    private void Start()
    {
        acceptButton.onClick.AddListener(OnAccept);
        cancelButton.onClick.AddListener(OnCancel);
    }

    public void Show(LocalizedString message, Action onAccept, Action onCancel)
    {
        localizedMessage = message;
        messageText.SetText(message);
        SetPanel(onAccept, onCancel);
    }

    public void Show(LocalizedString message, Action onAccept, Action onCancel, float paramValue)
    {
        localizedMessage = message;
        messageText.SetText(message, paramValue);
        SetPanel(onAccept, onCancel);
    }

    public void ShowWithTimeout(LocalizedString message, Action onAccept, Action onCancel, int timeoutSeconds)
    {
        Show(message, onAccept, onCancel, timeoutSeconds);
        waitTime = timeoutSeconds;

        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
        cancellationTokenSource = new CancellationTokenSource();
        CheckTimeOut().Forget();
    }

    async UniTaskVoid CheckTimeOut()
    {
        try
        {
            while (waitTime > -1)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationTokenSource.Token);
                waitTime--;
                messageText.SetText(localizedMessage, waitTime);

                if (waitTime == 0)
                {
                    OnCancel();
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 타임아웃이 취소된 경우 예외가 발생하지만, 이는 정상적인 흐름이므로 무시
        }
    }

    void OnAccept()
    {
        waitTime = 0;
        OnAceeptClicked?.Invoke();
        gameObject.SetActive(false);
    }

    void OnCancel()
    {
        waitTime = -1;
        OnCancelClicked?.Invoke();
        gameObject.SetActive(false);
    }

    private void SetPanel(Action onAccept, Action onCancel)
    {
        OnAceeptClicked = onAccept;
        OnCancelClicked = onCancel;
        gameObject.SetActive(true);
        waitTime = -1;
    }
}
