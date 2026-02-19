using UnityEngine;

public class OptionManager : MonoBehaviour
{
    public static OptionManager Instance { get; private set; }

    GameObject optionMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        optionMenu = transform.GetChild(0).gameObject;
    }

    public void SetOptionMenu(bool isActive)
    {
        optionMenu.SetActive(isActive);
    }
}
