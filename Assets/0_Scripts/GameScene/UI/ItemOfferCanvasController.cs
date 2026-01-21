using UnityEngine;
using UnityEngine.InputSystem;


public class ItemOfferCanvasController : MonoBehaviour
{
	//싱글턴
    public static ItemOfferCanvasController instance;

	private void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}

	private void Start()
	{
		OffersPanel.SetActive(false);
		OpenCloseText.SetActive(false);
	}

	//키 입력 받기
	private void Update()
	{
		//선택하는 시간이 아닌 경우 감지 안함
		if (!isChoosingPhase) return;
		if (Keyboard.current == null) return;
		//tap키 눌리는 것을 감지
		if (Keyboard.current.tabKey.wasPressedThisFrame)
		{
			//내 턴인 경우(검증용)
			if(GameHelper.IsMyTurn())
				//패널을 연다.
				ToggleOfferPanel();
		}
	}

	[Header("Prefabs for each selection")]
	public GameObject offerPrefab;

	[Header("UI Panel")]
	public GameObject OffersPanel;
	public GameObject OpenCloseText;

	//현재 상태 명시
	private bool isChoosingPhase = false;
	//PlayerController에서 사용될 움직임 제한용 플래그
	[HideInInspector]
	public bool isOfferPanelOpened = false;

	public void initItemOfferPanel(string offerList, int actorNum)
	{
		// 1) offerList 방어 (null/빈문자면 닫기)
		if (string.IsNullOrEmpty(offerList))
		{
			Close();
			return;
		}

		// 2) 기존 프리팹 제거
		foreach (Transform child in OffersPanel.transform)
			Destroy(child.gameObject);

		Debug.Log($"offered string : {offerList}");
		// 3) Decode 예외 방어
		var decoded = offerList.Split("|");
	

		if (decoded == null)
		{
			Close();
			return;
		}

		// 4) 생성
		foreach (var of in decoded)
		{
			var go = Instantiate(offerPrefab, OffersPanel.transform);
			var ui = go.GetComponent<ItemOfferUIController>();
			if (ui == null)
			{
				Debug.LogError("offerPrefab has no ItemOfferUIController");
				continue;
			}
			ui.SetItem(of);
		}

		ActiveOfferPanel();
	}

	//선택 패널을 최초로 열 때 호출될 함수
	private void ActiveOfferPanel()
	{
		//마우스 관련 설정
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		//UI 설정
		OpenCloseText.SetActive(true);
		OpenCloseText.GetComponent<CanvasGroup>().alpha = 1.0f;
		//HIT UI 비활성화(상호작용도 막음)
		PlayerCanvasController.Instance.SetHitTextUnActive();

		OffersPanel.SetActive(true);
		//선택 Phase 임을 명시
		isChoosingPhase = true;
		//움직임 제한용 플래그
		isOfferPanelOpened = true;
	}

	//선택 창 토글 함수
	private void ToggleOfferPanel()
	{
		//현재 상태에 따라서 패널을 열고 닫는다.
		bool isActive = OffersPanel.activeSelf;
		OffersPanel.SetActive(!isActive);
		//닫혀있던 상태인 경우 -> 열림 처리
		if (!isActive)
		{
			//마우스 활성화
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			//UI 처리
			OpenCloseText.GetComponent<CanvasGroup>().alpha = 1.0f;
			//HIT UI 비활성화(상호작용도 막음)
			PlayerCanvasController.Instance.SetHitTextUnActive();
			//움직임 제한
			isOfferPanelOpened = true;
		}
		else
		{
			//마우스 비활성화
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			//UI 처리
			OpenCloseText.GetComponent<CanvasGroup>().alpha = 0.5f;
			//움직임 활성화
			isOfferPanelOpened = false;
		}
		
	}

	public void Close()
	{
		OffersPanel?.SetActive(false);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		OpenCloseText?.SetActive(false);
		isOfferPanelOpened = false;

		if (OffersPanel != null)
		{
			foreach (Transform child in OffersPanel.transform)
				Destroy(child.gameObject);
		}

		isChoosingPhase = false;
	}


	//아이템이 선택 된 경우(패널이 켜진 상태였다고 가정)
	public void SelectedItem(string itemId)
	{
		//패널 닫기
		Close();

		foreach (Transform child in OffersPanel.transform)
		{
			Destroy(child.gameObject);
		}

		InventoryAuthority.Instance.RequestTakeOffer(itemId);
	}

	//제한 시간이 모두 지나버린 경우
	public void TurnOver()
	{
		//선택을 이미 한 경우
		if (!isChoosingPhase) return;
		Close();
	}
}
