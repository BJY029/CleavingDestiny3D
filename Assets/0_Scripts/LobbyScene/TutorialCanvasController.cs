using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvasController : MonoBehaviour
{
    [Header("window")]
    [SerializeField] private GameObject tutorialBackground;

    [Header("Pages")]
    [SerializeField] private GameObject[] pages;

    [Header("Buttons")]
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button nextBtn;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI pageText;

    [Header("Open Settings")]
    [SerializeField] private bool resetPageOnOpen = true;

    private int currentPage;

    private void Start()
    {
        ShowPage(0);
        tutorialBackground.SetActive(false);

        if (prevBtn != null) prevBtn.onClick.AddListener(PlayButtonClickSound);
        if (nextBtn != null) nextBtn.onClick.AddListener(PlayButtonClickSound);
    }

    public void Toggle()
    {
        if (tutorialBackground.activeSelf) Close();
        else Open();
    }

    public void Open()
    {
        ShowPage(resetPageOnOpen ? 0 : currentPage);
        tutorialBackground.SetActive(true);
        AudioManager.Instance.PlaySfx2D("ui_button");
    }

    public void Close()
    {
        tutorialBackground.SetActive(false);
        AudioManager.Instance.PlaySfx2D("ui_button");
    }

    public void Next()
    {
        ShowPage(currentPage + 1);
    }

    public void Prev()
    {
        ShowPage(currentPage - 1);
    }

    private void ShowPage(int index)
    {
        if (pages == null || pages.Length == 0) return;

        currentPage = Mathf.Clamp(index, 0, pages.Length - 1);

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null) pages[i].SetActive(i == currentPage);
        }

        if (prevBtn != null) prevBtn.interactable = currentPage > 0;

        if (nextBtn != null) nextBtn.interactable = currentPage < pages.Length - 1;

        if (pageText != null) pageText.text = $"{currentPage + 1} / {pages.Length}";

        if (titleText != null)
        {
            string title = LocalizationManager.Instance.GetText(CSV_Type.Tutorial, $"Header_{currentPage + 1}");
            titleText.text = title;
        }
    }

    private void PlayButtonClickSound()
    {
        AudioManager.Instance.PlaySfx2D("ui_button");
    }
}
