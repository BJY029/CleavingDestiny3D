using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using PrimeTween;
using Photon.Pun;


public class ItemBtnController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemSO Item;
    private int UID;
    public Button ItemButton;
    public Image ItemImg;

    private Tween hoverTween;

    //아이템 버튼 초기화 함수
    public void SetButton(ItemSO item, int uid)
    {
        Item = item;
        UID = uid;

        ColorBlock cb = ItemButton.colors;
        cb.normalColor = ItemColorManager.instance.GetNormalColor(item.itemClass);
        cb.highlightedColor = ItemColorManager.instance.GetBriteColor(item.itemClass);
        cb.pressedColor = ItemColorManager.instance.GetDarkerColor(item.itemClass);

        ItemButton.colors = cb;

        ItemImg.sprite = item.Icon;

        ItemButton.onClick.AddListener(() => OnButtonClicked());
    }

    //버튼이 클릭되면 호출될 함수
    private void OnButtonClicked()
    {
        int RequestActNum = PhotonNetwork.LocalPlayer.ActorNumber;
        //uid를 기반으로 아이템을 찾기 때문에, 인자로 넘긴다.
        ItemSelectionController.instance.ItemSelected(RequestActNum, UID);
    }

    //호버링 관련 처리
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverTween.Stop();
        hoverTween = Tween.Scale(transform, endValue: 1.1f, duration: 0.15f, ease: Ease.OutQuad);

        ItemSelectionController.instance.SetItemDesc(Item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverTween.Stop();
        hoverTween = Tween.Scale(transform, endValue: 1f, duration: 0.15f, ease: Ease.InQuad);

        ItemSelectionController.instance.CloseItemDesc();
    }
}
