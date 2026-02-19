using UnityEngine;
using UnityEngine.UI;

public class CategorySwapper : MonoBehaviour
{
    [SerializeField] GameObject[] categoryObjects;
    [SerializeField] Button[] categoryButtons;

    void Start()
    {
        for (int i = 0; i < categoryButtons.Length; i++)
        {
            int index = i;
            categoryButtons[i].onClick.AddListener(() => SwapCategory(index));
        }
    }

    public void SwapCategory(int index)
    {
        for (int i = 0; i < categoryObjects.Length; i++)
        {
            bool isActive = i == index;
            if (categoryObjects[i].activeSelf != isActive)
                categoryObjects[i].SetActive(isActive);
            if (categoryButtons[i].interactable == isActive)
                categoryButtons[i].interactable = !isActive;
        }
    }
}
