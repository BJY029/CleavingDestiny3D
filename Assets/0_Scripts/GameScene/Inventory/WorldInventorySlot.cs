using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class WorldInventorySlot : MonoBehaviour, ILookInteractable
{
	//해당 인벤토리 슬롯 소유자
	private int ownerActor;

	//슬롯 인덱스
    public int slotIndex;
	//해당 슬롯 Mesh 렌더러
    private MeshRenderer quadRenderer;
	//슬롯이 빈 경우 표시할 텍스쳐
    private Texture2D nullTexture;
	//해당 슬롯이 현재 가지고 있는 아이템 정보
    private ItemSO currentItem;
	//해당 슬롯의 머테리얼 관련 정보
    private MaterialPropertyBlock mpb;

	//슬롯 툴팁 오브젝트
	[Header("ToolTipInfos")]
	public GameObject ToolTipPanel;
	public Text ItemNameText;
	public Text ItemRarityText;
	public Text ItemPriceText;
	public Text ItemDescText;

	private void Awake()
	{
		//머테리얼 정보 
		mpb = new MaterialPropertyBlock();
		quadRenderer = GetComponent<MeshRenderer>();

		//아무 아이템이 없을 경우 삽입 할 텍스쳐
		nullTexture = Resources.Load<Texture2D>("Material/Item/Textures/invisible");

		if (nullTexture == null) Debug.LogWarning("투명 텍스처를 찾지 못했습니다.");
	}

	//현재 슬롯이 아이템을 가지고 있는지 확인
	public bool HasItem()
	{
		return currentItem != null;
	}

	//슬롯을 설정하는 함수
	public void SetSlot(ItemSO item, int owner)
	{
		//슬롯 보유자 설정
		ownerActor = owner;
		//슬롯에 넣을 아이템 설정
		currentItem = item;

		//아이템이 없는 경우
		if (item == null)
		{
			// 아이템이 null이면 투명 텍스처로 변경하고 종료
			SetIcon(nullTexture);
			return;
		}

		// 아이템이 있다면 DB에서 텍스처를 가져와 설정
		if (ItemDB.Instance != null)
		{
			// Material을 가져와서 텍스처 빼내는 방식
			Texture targetTex = ItemDB.Instance.GetMat(item.itemId).mainTexture;
			//아이콘 설정
			SetIcon(targetTex);
		}

		//툴팁 정보 초기화
		InitItemInfos();
	}

	//툴팁 정보 초기화
	private void InitItemInfos()
	{
		ItemNameText.text = LocalizationManager.Instance.GetText(CSV_Type.Item, currentItem.displayName_ID);
		ItemRarityText.text = currentItem.itemClass.ToString();
		ItemPriceText.text = "Cost : " + currentItem.itemCost.ToString();
		ItemDescText.text = LocalizationManager.Instance.GetText(CSV_Type.Item, currentItem.itemDesc_ID);
	}

	//아이콘 설정
	public void SetIcon(Texture tex)
	{
		if (quadRenderer == null) return;

		//기존 설정 가져오기
		quadRenderer.GetPropertyBlock(mpb);

		if (tex != null)
		{
			// 아이템이 있을 때
			// 텍스쳐 적용 + 색상을 "하얀색(불투명)"으로 변경
			mpb.SetTexture("_BaseMap", tex);
			mpb.SetColor("_BaseColor", Color.white);
		}
		else
		{
			// 아이템이 없을 때:
			// 텍스쳐 제거 + 색상을 투명하게 변경 (안 보이게)
			mpb.SetTexture("_BaseMap", nullTexture); // 혹은 null
			mpb.SetColor("_BaseColor", new Color(1, 1, 1, 0)); // Alpha = 0
		}

		// 최종 적용
		quadRenderer.SetPropertyBlock(mpb);
	}

	//슬롯이 내 것인지 확인
	public bool IsMine => PhotonNetwork.LocalPlayer.ActorNumber == ownerActor;

	//Ray가 Enter한 경우
	public void OnLookEnter(PlayerController pc)
    {
        if (!HasItem()) return;
		if (!IsMine) return;
		InitItemInfos();
		ToolTipPanel.SetActive(true);
        //Highlight();
    }

	//Ray가 Exit 한 경우
    public void OnLookExit(PlayerController pc)
    {
		if (!IsMine) return;
		ToolTipPanel.SetActive(false);
        //Highlight();
    }

	//이제 구현해야 할 부분
    public void OnInteract(PlayerController pc)
    {
        if(!HasItem()) return;
		if (!IsMine) return;
		if (!GameHelper.IsMyTurn()) return;

		SetIcon(null);
		ToolTipPanel.SetActive(false);
		Debug.Log("Interacted!!");
        //InventoryAuthority.Instance.RequestUseItem();
		currentItem = null;
    }

    private void Highlight(bool on)
    {

    }
}
