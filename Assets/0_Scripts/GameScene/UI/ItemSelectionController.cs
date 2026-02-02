using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ItemSelectionController : MonoBehaviourPunCallbacks
{
    public static ItemSelectionController instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [Header("Title Text")]
    public TextMeshProUGUI Title;

    [Header("ItemSelection UIs")]
    public GameObject BackGroundPanel;
    public GameObject List_0;
    public GameObject List_1;
    public GameObject List_2;

    [Header("ItemDesc UIs")]
    public GameObject ItemDescPanel;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemRarity;
    public TextMeshProUGUI ItemDesc;

    [Header("ItemDescTable")]
    public TextMeshProUGUI CommonDesc;
    public TextMeshProUGUI HeroDesc;
    public TextMeshProUGUI RareDesc;
    public TextMeshProUGUI LegendaryDesc;

    [Header("Each Item Button")]
    public GameObject ItemBtnPrefab;

    private Dictionary<int, ItemSO> Items = new Dictionary<int, ItemSO>();
    [HideInInspector]
    public bool IsItemSelectionActivated = false;

    void Start()
    {
        ItemDescPanel.SetActive(false);
        BackGroundPanel.SetActive(false);
    }

    public void SetItemSelectionActive(int ActNum, int UID)
    {
        photonView.RPC(nameof(RPC_SetItemSelectionActive), PhotonNetwork.CurrentRoom.GetPlayer(ActNum), ActNum, UID);
    }


    //희생 아이템을 처리하는 함수 수행
    [PunRPC]
    public void RPC_SetItemSelectionActive(int ActNum, int UID)
    {
        //아이템 창 활성화 관련 플래그 활성화
        IsItemSelectionActivated = true;
        //내 actor 번호에 해당되는 INV 정보 가져오기
        int capacity = PhotonPropertyHelper.GetRoomProp<int>(ItemPropKeys.INV_CAPACITY(ActNum));
        string invStr = PhotonPropertyHelper.GetRoomProp<string>(ItemPropKeys.INV(ActNum));

        //인벤토리 슬롯 가져오기
        var invSlots = ItemInfoSerializer.Decode(invStr, capacity);

        //각 인벤토리 슬롯 설정
        for (int i = 0; i < invSlots.Length; i++)
        {
            //해당 아이템 정보 가져오기
            ItemSO item = ItemDB.Instance.Get(invSlots[i].itemID);
            //아이템 정보가 없으면, 추가 안함
            if (item == null) continue;
            //RPC 지연으로 사용된 아이템이 업데이트 되지 않은 경우가 있음
            //해당 버그를 방지하기 위해, uniqueID를 기반으로 사용된 아이템은 추가하지 않는다.
            if (UID == invSlots[i].uniqueId) continue;
            Items.Add(invSlots[i].uniqueId, item);
        }

        //LIST_ 초기화
        InitLists();

        //아이템이 4개 이하라면
        if (Items.Count <= 4)
        {
            List_1.SetActive(false);
            List_2.SetActive(false);
            List_0.SetActive(true);
            foreach (var v in Items)
            {
                Initiate_Init_ItemBtn(List_0.transform, v.Value, v.Key);
            }
        }
        //4개 이상이라면
        else
        {
            List_0.SetActive(false);
            List_1.SetActive(true);
            List_2.SetActive(true);
            int index = 0;
            foreach (var v in Items)
            {
                if (index < 4)
                {
                    Initiate_Init_ItemBtn(List_1.transform, v.Value, v.Key);
                }
                else
                {
                    Initiate_Init_ItemBtn(List_2.transform, v.Value, v.Key);
                }
                index++;
            }
        }

        Title.text = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemSacrifice_Title);
        //TODO : 프로퍼티 설정 및 값 받아와서 적용하기
        SetDescTable(ActNum);

        //UI 활성화 처리
        BackGroundPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //아이템이 선택되었을 경우 실행될 함수
    public void ItemSelected(int RequestAct, int UID)
    {
        CloseItemSelection();
        ItemHandlingSystem.instance.ProcessSacrificeItem(RequestAct, UID);
    }

    //UI 비활성화 처리
    public void CloseItemSelection()
    {
        //마우스 설정 해제
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //리스터 초기화
        InitLists();
        //UI 비활성화
        ItemDescPanel.SetActive(false);
        BackGroundPanel.SetActive(false);
        IsItemSelectionActivated = false;
    }

    //아이템 설명창을 활성화 하는 함수
    public void SetItemDesc(ItemSO item)
    {
        ItemDescPanel.SetActive(true);
        ItemName.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.displayName_ID);
        ItemRarity.text = item.itemClass.ToString();
        ItemDesc.text = LocalizationManager.Instance.GetText(CSV_Type.Item, item.itemDesc_ID);
    }

    public void SetDescTable(int ActNum)
    {
        string CommonRate = (PhotonPropertyHelper.GetRoomProp<float>(ItemPropKeys.COMMON_RATE(ActNum)) * 100f).ToString() + '%';
        string HeroRate = (PhotonPropertyHelper.GetRoomProp<float>(ItemPropKeys.HERO_RATE(ActNum)) * 100f).ToString() + '%';
        string RareRate = (PhotonPropertyHelper.GetRoomProp<float>(ItemPropKeys.RARE_RATE(ActNum)) * 100f).ToString() + '%';
        string LegendaryRate = (PhotonPropertyHelper.GetRoomProp<float>(ItemPropKeys.LEGENDARY_RATE(ActNum)) * 100f).ToString() + '%';

        string DescText = LocalizationManager.Instance.GetText(CSV_Type.UI, UI_CSV.UI_ItemSacrifice_TableDesc);
        CommonDesc.text = DescText + " : " + CommonRate;
        HeroDesc.text = DescText + " : " + HeroRate;
        RareDesc.text = DescText + " : " + RareRate;
        LegendaryDesc.text = DescText + " : " + LegendaryRate;
    }

    //아이템 설명 창을 비활성화 하는 함수
    public void CloseItemDesc()
    {
        ItemName.text = "";
        ItemRarity.text = "";
        ItemDesc.text = "";
        ItemDescPanel.SetActive(false);
    }

    //리스트 초기화 함수
    private void InitLists()
    {
        foreach (Transform child in List_0.transform)
            Destroy(child.gameObject);
        foreach (Transform child in List_1.transform)
            Destroy(child.gameObject);
        foreach (Transform child in List_2.transform)
            Destroy(child.gameObject);
    }

    //아이템 버튼 프리팹 생성 및 초기화 함수
    private void Initiate_Init_ItemBtn(Transform trans, ItemSO item, int UID)
    {
        var go = Instantiate(ItemBtnPrefab, trans);
        var ui = go.GetComponent<ItemBtnController>();
        ui.SetButton(item, UID);
    }
}
