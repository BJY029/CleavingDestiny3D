using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Village.Building
{
    public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI itemGoldText;
        [SerializeField] Image selectedItemHighlight;
        public CanvasGroup canvasGroup; // 아이템의 상호작용 가능 여부를 제어하기 위한 CanvasGroup

        [Header("Feedback Settings")]
        [SerializeField] private bool useScaleEffect = true;
        [SerializeField] private float hoverScale = 1.06f;
        [SerializeField] private float scaleDuration = 0.12f;
        [SerializeField] private Ease hoverEase = Ease.OutQuad;
        [SerializeField] private Ease exitEase = Ease.OutQuad;

        [Header("Sound Settings")]
        [SerializeField] private bool useClickSound = true;
        [SerializeField] private string clickSound = "ui_button";
        [SerializeField] private bool useHoverSound = false;
        [SerializeField] private string hoverSound = "UI_Hover";
        [SerializeField] private float hoverSoundCooldown = 0.05f;

        public ShopUI ParentShopUI { get; set; } // 아이템이 속한 ShopUI 참조

        ItemSO currentItem;
        public ItemSO ItemData => currentItem;
        public int Price { get; private set; }
        public bool IsEmpty => currentItem == null; // 아이템이 없는 경우 true 반환
        public bool IsSelected { get; private set; } // 아이템이 선택된 상태인지 여부

        private Vector3 _originalScale = Vector3.one;
        private Tween _scaleTween;
        private float _lastHoverSoundTime = -999f;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        private void OnEnable()
        {
            ResetScaleImmediate();
        }

        private void OnDisable()
        {
            _scaleTween.Stop();
            ResetScaleImmediate();
        }

        public void SetShopItem(ItemSO item)
        {
            currentItem = item;
            if (item == null)
            {
                itemIcon.sprite = null; // 아이템이 없는 경우 아이콘 초기화
                itemGoldText.SetText(string.Empty); // 가격 텍스트 초기화
                ResetScaleImmediate();
                return;
            }

            itemIcon.sprite = item.Icon; // 아이템 아이콘 설정

            // 등급별 가격 설정
            Price = VillageSystem.VillageStat.VillageBalance.GetItemPrice(item.itemClass);
            itemGoldText.SetText("{0} <color=yellow>G</color>", Price);
        }

        public string GetItemDescription()
        {
            if (currentItem == null)
                return string.Empty;

            return LocalizationManager.Instance.GetText(CSV_Type.Item, currentItem.itemDesc_ID); // 아이템 설명 반환
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            selectedItemHighlight.gameObject.SetActive(selected); // 선택된 아이템 강조 표시 활성화/비활성화
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsEmpty)
                return;

            if (useHoverSound && !string.IsNullOrEmpty(hoverSound))
            {
                if (Time.unscaledTime - _lastHoverSoundTime >= hoverSoundCooldown)
                {
                    _lastHoverSoundTime = Time.unscaledTime;
                    AudioManager.Instance?.PlaySfx2D(hoverSound);
                }
            }

            if (useScaleEffect)
            {
                AnimateScale(_originalScale * hoverScale, hoverEase);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (useScaleEffect)
            {
                AnimateScale(_originalScale, exitEase);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsEmpty)
                return;

            if (useClickSound && !string.IsNullOrEmpty(clickSound))
            {
                AudioManager.Instance?.PlaySfx2D(clickSound);
            }

            ParentShopUI.ShopItemSelect(this); // 클릭된 아이템을 ShopUI로 전달하여 처리
        }

        private void AnimateScale(Vector3 endScale, Ease ease)
        {
            _scaleTween.Stop();
            _scaleTween = Tween.Scale(transform, endScale, scaleDuration, ease, useUnscaledTime: true);
        }

        private void ResetScaleImmediate()
        {
            _scaleTween.Stop();
            transform.localScale = _originalScale;
        }
    }
}