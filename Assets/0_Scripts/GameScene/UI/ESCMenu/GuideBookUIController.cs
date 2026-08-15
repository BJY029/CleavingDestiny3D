using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[Serializable]
public class GuidePage
{
    [Tooltip("페이지 제목 다국어 Key")]
    public string pageTitleKey;
    
    [Tooltip("페이지 설명 다국어 Key")]
    public string pageDescriptionKey;
    
    [Tooltip("관련 이미지/스프라이트 (선택 사항)")]
    public string imageResourcePath;

    [NonSerialized] public ItemSO item;
    [NonSerialized] public Sprite image;
}

[Serializable]
public class GuideCategory
{
    [Tooltip("목차에 표시될 카테고리 이름 다국어 Key")]
    public string categoryNameKey;
    
    [Tooltip("해당 카테고리에 속한 페이지들")]
    public List<GuidePage> pages = new List<GuidePage>();

    public bool generateFromItems;
    public bool showPageIndex;
}

[Serializable]
public class GuideBookData
{
    public List<GuideCategory> categories = new List<GuideCategory>();
}

public class GuideBookUIController : MonoBehaviour
{
    private const string GuideDataAddress = "GuideBookData";

    [Header("Data (다국어 Key와 이미지 등록)")]
    private List<GuideCategory> categories = new List<GuideCategory>();

    [Header("UI References")]
    [SerializeField] private GameObject guideBookPanel;
    
    [Header("Category (목차)")]
    [SerializeField] private Transform categoryButtonContainer;
    [SerializeField] private GameObject categoryButtonPrefab;

    [Header("Page Index")]
    [SerializeField] private Transform pageIndexButtonContainer;
    [SerializeField] private GameObject pageIndexButtonPrefab;

    [Header("Page Content")]
    [SerializeField] private TextMeshProUGUI pageTitleText;
    [SerializeField] private TextMeshProUGUI pageDescriptionText;
    [SerializeField] private Image pageImage;
    
    [Header("Navigation")]
    [SerializeField] private Button prevPageBtn;
    [SerializeField] private Button nextPageBtn;
    [SerializeField] private Button closeBtn;

    private int currentCategoryIndex = 0;
    private int currentPageIndex = 0;
    
    private List<Button> spawnedCategoryButtons = new List<Button>();
    private List<Button> spawnedPageIndexButtons = new List<Button>();

    private void Awake()
    {
        guideBookPanel?.SetActive(false);
    }

    private async UniTaskVoid Start()
    {
        prevPageBtn.onClick.AddListener(OnPrevPage);
        nextPageBtn.onClick.AddListener(OnNextPage);
        closeBtn.onClick.AddListener(() => ToggleGuideBook(false));

        await LoadGuideData();
        InitializeCategories();
    }

    private async UniTask LoadGuideData()
    {
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(GuideDataAddress);

        try
        {
            TextAsset json = await handle;
            if (handle.Status != AsyncOperationStatus.Succeeded || json == null)
            {
                Debug.LogError($"[GuideBook] Addressable '{GuideDataAddress}'를 불러오지 못했습니다.");
                return;
            }

            GuideBookData data = JsonUtility.FromJson<GuideBookData>(json.text);
            categories = data?.categories ?? new List<GuideCategory>();
        }
        catch (Exception exception)
        {
            categories.Clear();
            Debug.LogError($"[GuideBook] 데이터 로드 실패: {exception.Message}");
            return;
        }
        finally
        {
            if (handle.IsValid()) Addressables.Release(handle);
        }

        foreach (GuideCategory category in categories)
        {
            category.pages ??= new List<GuidePage>();

            if (category.generateFromItems)
            {
                category.pages.Clear();
                if (ItemDB.Instance == null)
                {
                    Debug.LogWarning("[GuideBook] ItemDB가 없어 아이템 도감을 생성하지 못했습니다.");
                    continue;
                }

                foreach (ItemSO item in ItemDB.Instance.GetItemsList())
                {
                    if (item == null) continue;
                    category.pages.Add(new GuidePage
                    {
                        pageTitleKey = item.displayName_ID,
                        pageDescriptionKey = item.itemDesc_ID,
                        item = item
                    });
                }
            }
            else
            {
                foreach (GuidePage page in category.pages)
                {
                    if (!string.IsNullOrEmpty(page.imageResourcePath))
                        page.image = Resources.Load<Sprite>(page.imageResourcePath);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += UpdatePageUI;
            LocalizationManager.Instance.OnLanguageChanged += UpdateCategoryTexts;
            LocalizationManager.Instance.OnLanguageChanged += UpdatePageIndexTexts;
        }
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= UpdatePageUI;
            LocalizationManager.Instance.OnLanguageChanged -= UpdateCategoryTexts;
            LocalizationManager.Instance.OnLanguageChanged -= UpdatePageIndexTexts;
        }
    }

    private void InitializeCategories()
    {
        if (categories.Count == 0) return;

        // 기존에 생성된 버튼이 있다면 제거
        foreach (var btn in spawnedCategoryButtons)
        {
            if (btn != null) Destroy(btn.gameObject);
        }
        spawnedCategoryButtons.Clear();

        for (int i = 0; i < categories.Count; i++)
        {
            int categoryIndex = i; // 클로저 이슈 방지
            
            GameObject btnObj = Instantiate(categoryButtonPrefab, categoryButtonContainer);
            btnObj.SetActive(true);
            
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnCategorySelected(categoryIndex));
                spawnedCategoryButtons.Add(btn);
            }
        }

        UpdateCategoryTexts();
    }

    private void UpdateCategoryTexts()
    {
        for (int i = 0; i < spawnedCategoryButtons.Count; i++)
        {
            if (i >= categories.Count) break;

            Button btn = spawnedCategoryButtons[i];
            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null && LocalizationManager.Instance != null)
            {
                btnText.text = LocalizationManager.Instance.GetText(CSV_Type.GuideBook, categories[i].categoryNameKey);
            }
        }
    }

    public void ToggleGuideBook(bool isOn)
    {
        guideBookPanel.SetActive(isOn);
        
        if (isOn)
        {
            // 가이드북을 열 때 항상 첫 번째 카테고리의 첫 번째 페이지 표시
            if (categories.Count > 0)
            {
                OnCategorySelected(0);
            }
        }
    }

    private void OnCategorySelected(int categoryIndex)
    {
        if (categoryIndex < 0 || categoryIndex >= categories.Count) return;

        currentCategoryIndex = categoryIndex;
        currentPageIndex = 0;

        InitializePageIndex();
        UpdatePageUI();
    }

    private void InitializePageIndex()
    {
        foreach (Button button in spawnedPageIndexButtons)
        {
            if (button != null) Destroy(button.gameObject);
        }
        spawnedPageIndexButtons.Clear();

        GuideCategory category = categories[currentCategoryIndex];
        bool showIndex = category.showPageIndex && pageIndexButtonContainer != null && pageIndexButtonPrefab != null;
        if (pageIndexButtonContainer != null)
            pageIndexButtonContainer.gameObject.SetActive(showIndex);
        if (!showIndex) return;

        for (int i = 0; i < category.pages.Count; i++)
        {
            int pageIndex = i;
            Button button = Instantiate(pageIndexButtonPrefab, pageIndexButtonContainer).GetComponent<Button>();
            if (button == null) continue;

            button.gameObject.SetActive(true);
            button.onClick.AddListener(() => SelectPage(pageIndex));
            spawnedPageIndexButtons.Add(button);
        }

        UpdatePageIndexTexts();
    }

    private void UpdatePageIndexTexts()
    {
        if (categories.Count == 0 || spawnedPageIndexButtons.Count == 0 || LocalizationManager.Instance == null) return;

        GuideCategory category = categories[currentCategoryIndex];
        for (int i = 0; i < spawnedPageIndexButtons.Count && i < category.pages.Count; i++)
        {
            GuidePage page = category.pages[i];
            TextMeshProUGUI text = spawnedPageIndexButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = LocalizationManager.Instance.GetText(GetPageTableType(page), page.pageTitleKey);
        }
    }

    private void SelectPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= categories[currentCategoryIndex].pages.Count) return;
        currentPageIndex = pageIndex;
        UpdatePageUI();
    }

    private void OnPrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageUI();
        }
    }

    private void OnNextPage()
    {
        var currentCategory = categories[currentCategoryIndex];

        if (currentPageIndex < currentCategory.pages.Count - 1)
        {
            currentPageIndex++;
            UpdatePageUI();
        }
    }

    private void UpdatePageUI()
    {
        if (categories.Count == 0) return;
        var currentCategory = categories[currentCategoryIndex];
        
        if (currentCategory.pages == null || currentCategory.pages.Count == 0)
        {
            ClearPageUI();
            return;
        }

        var currentPage = currentCategory.pages[currentPageIndex];

        if (LocalizationManager.Instance != null)
        {
            CSV_Type tableType = GetPageTableType(currentPage);
            if (pageTitleText != null) 
                pageTitleText.text = LocalizationManager.Instance.GetText(tableType, currentPage.pageTitleKey);
            
            if (pageDescriptionText != null) 
                pageDescriptionText.text = LocalizationManager.Instance.GetText(tableType, currentPage.pageDescriptionKey);
        }

        if (pageImage != null)
        {
            Sprite sprite = currentPage.item != null ? currentPage.item.Icon : currentPage.image;
            if (sprite != null)
            {
                pageImage.gameObject.SetActive(true);
                pageImage.sprite = sprite;
            }
            else
            {
                pageImage.gameObject.SetActive(false);
            }
        }

        // 내비게이션 버튼 상태 업데이트
        if (prevPageBtn != null) prevPageBtn.interactable = (currentPageIndex > 0);
        if (nextPageBtn != null) nextPageBtn.interactable = (currentPageIndex < currentCategory.pages.Count - 1);
        for (int i = 0; i < spawnedPageIndexButtons.Count; i++)
            spawnedPageIndexButtons[i].interactable = i != currentPageIndex;
    }

    private static CSV_Type GetPageTableType(GuidePage page) =>
        page.item != null ? CSV_Type.Item : CSV_Type.GuideBook;

    private void ClearPageUI()
    {
        if (pageTitleText != null) pageTitleText.text = "";
        if (pageDescriptionText != null) pageDescriptionText.text = "";
        if (pageImage != null) pageImage.gameObject.SetActive(false);
        if (prevPageBtn != null) prevPageBtn.interactable = false;
        if (nextPageBtn != null) nextPageBtn.interactable = false;
    }
}
