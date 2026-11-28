using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleCanvas : MonoBehaviour
{
    [Header("window")]
    [SerializeField] private GameObject mainWindow;

    [Header("Button")]
    [SerializeField] private Button openBtn;
    [SerializeField] private Button closeBtn;

    [Header("Events")]
    [SerializeField] private UnityEvent onOpened;
    [SerializeField] private UnityEvent onClosed;

    public UnityEvent OnOpened => onOpened;
    public UnityEvent OnClosed => onClosed;

    private void Start()
    {
        if (openBtn != null)
        {
            openBtn.onClick.AddListener(Open);
            openBtn.onClick.AddListener(PlayButtonClickSound);
        }
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(Close);
            closeBtn.onClick.AddListener(PlayButtonClickSound);
        }

        mainWindow.SetActive(false);
    }

    public void Open()
    {
        if (mainWindow.activeSelf) return;

        mainWindow.SetActive(true);
        onOpened?.Invoke();
    }

    public void Close()
    {
        if (!mainWindow.activeSelf) return;

        mainWindow.SetActive(false);
        onClosed?.Invoke();
    }

    private void PlayButtonClickSound()
    {
        AudioManager.Instance.PlaySfx2D("ui_button");
    }
}
