using UnityEngine;
using UnityEngine.UI;

public class ToggleCanvas : MonoBehaviour
{
    [Header("window")]
    [SerializeField] private GameObject mainWindow;

    [Header("Button")]
    [SerializeField] private Button openBtn;
    [SerializeField] private Button closeBtn;

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
        if (!mainWindow.activeSelf) mainWindow.SetActive(true);
    }

    public void Close()
    {
        if (mainWindow.activeSelf) mainWindow.SetActive(false);
    }

    private void PlayButtonClickSound()
    {
        AudioManager.Instance.PlaySfx2D("ui_button");
    }
}
