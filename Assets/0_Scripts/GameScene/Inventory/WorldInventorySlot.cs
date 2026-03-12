using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class WorldInventorySlot : MonoBehaviour, ILookInteractable
{
	//해당 인벤토리 슬롯 소유자
	private int ownerActor;

	//슬롯 인덱스
	public int slotIndex;
	//해당 슬롯 Mesh 렌더러
	private MeshRenderer quadRenderer;

	//해당 슬롯이 현재 가지고 있는 아이템 정보
	public ItemSO currentItem { get; private set; }
	//해당 슬롯의 머테리얼 관련 정보
	private MaterialPropertyBlock mpb;


	//슬롯 툴팁 오브젝트
	[Header("ToolTipInfos")]
	public GameObject ToolTipPanel;
	public TextMeshProUGUI ItemNameText;
	public TextMeshProUGUI ItemRarityText;
	public TextMeshProUGUI ItemPriceText;
	public TextMeshProUGUI ItemDescText;

	[Header("NullTexture")]
	public Texture2D nullTexture;

	private void Awake()
	{
		//머테리얼 정보 
		mpb = new MaterialPropertyBlock();
		quadRenderer = GetComponent<MeshRenderer>();


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
	public bool IsMine(int actNum) => actNum == ownerActor;

	//Ray가 Enter한 경우
	public void OnLookEnter(IPlayerAction pc)
	{
		PlayerController playerCtrl = pc as PlayerController;
		AIController aiCtrl = pc as AIController;

		if (playerCtrl == null && aiCtrl == null)
		{
			Debug.LogError("pc is not PlayerController or AIController");
			return;
		}

		int ActNum = (playerCtrl != null) ? playerCtrl.PlayerActNum : aiCtrl.PlayerActNum;

		if (!HasItem()) return;
		if (!IsMine(ActNum))
		{
			if (pc.GetInvAdmissionticket() != ownerActor) return;
		}
		InitItemInfos();
		ToolTipPanel.SetActive(true);
		//Highlight();
	}

	//Ray가 Exit 한 경우
	public void OnLookExit(IPlayerAction pc)
	{
		//if (!IsMine) return;
		ToolTipPanel.SetActive(false);
		//Highlight();
	}

	//특정 슬롯으로부터 상호작용 발동 시 해당 슬롯의 아이템 사용 로직 활성화
	public void OnInteract(IPlayerAction pc)
	{
		PlayerController playerCtrl = pc as PlayerController;
		AIController aiCtrl = pc as AIController;

		if (playerCtrl == null && aiCtrl == null)
		{
			Debug.LogError("pc is not PlayerController or AIController");
			return;
		}

		int ActNum = (playerCtrl != null) ? playerCtrl.PlayerActNum : aiCtrl.PlayerActNum;

		if (!HasItem())
		{
			Debug.LogWarning("Non Item in slot");
			return;
		}
		if (!IsMine(ActNum))
		{
			Debug.Log("stealing item activated");
			if (pc.GetInvAdmissionticket() != ownerActor) return;
			int ToActorNum = ActNum;
			//키 제거
			pc.SetInvAdmissionTicket(-1);
			//TODO: 선택된 아이템 상호작용한 플레이어의 인벤토리로 옮기기
			InventoryAuthority.Instance.RequestSteelItem(ownerActor, ToActorNum, slotIndex, this);
			return;
		}
		if (!GameHelper.IsMyTurn()) return;

		ToolTipPanel.SetActive(false);
		Debug.Log("Interacted!!");
		InventoryAuthority.Instance.RequestUseItem(slotIndex, ActNum, this);

	}

	public void DeleteItemByUID()
	{

	}

	public void SetCurrentItemNull(bool success)
	{
		if (success)
		{
			SetIcon(null);
			currentItem = null;
		}
	}

	private void Highlight(bool on)
	{

	}
}
