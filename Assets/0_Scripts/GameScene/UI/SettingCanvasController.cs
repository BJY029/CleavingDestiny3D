using UnityEngine;
using UnityEngine.UI;

public class SettingCanvasController : MonoBehaviour
{
    public static SettingCanvasController instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Background.GetComponent<RectTransform>().localScale = Vector3.one;
        Background?.SetActive(false);
    }

    [Header("Panel")]
    public GameObject Background;

    [Header("Buttons")]
    public Button CloseBtn;
    public Button QuitGameBtn;
    public Button LobbyBtn;

    public bool IsSettingPanelOpened { get; private set; } = false;

    private void Start()
    {
        CloseBtn.onClick.AddListener(ToggleSettingPanel);
        LobbyBtn.onClick.AddListener(GameExitHandler.instance.RequestLeaveGame);
    }


    public void ToggleSettingPanel()
    {
        IsSettingPanelOpened = !IsSettingPanelOpened;
        Background.SetActive(IsSettingPanelOpened);
        if (IsSettingPanelOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
