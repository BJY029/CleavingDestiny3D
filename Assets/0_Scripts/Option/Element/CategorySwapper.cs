using UnityEngine;
using UnityEngine.UI;

namespace Option.Element
{

    public class CategorySwapper : MonoBehaviour
    {
        [SerializeField] GameObject[] categoryObjects;
        [SerializeField] Button[] categoryButtons;

        private void Start()
        {
            for (int i = 0; i < categoryButtons.Length; i++)
            {
                int index = i;
                categoryButtons[i].onClick.AddListener(() => SwapCategory(index));
            }
        }

        public void SwapCategory(int index)
        {
            ApplyCategory(index);
            AudioManager.Instance?.PlaySfx2D("ui_button");
        }

        public void SetInitialCategory(int index)
        {
            ApplyCategory(index);
        }

        private void ApplyCategory(int index)
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
}