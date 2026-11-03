using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class InventoryNewDrug : MonoBehaviourPunCallbacks, ILookInteractable
{
    public ItemSO NewDrugItem;

    [Header("ToolTipInfos")]
    public GameObject ToolTipPanel;
    public TextMeshProUGUI ItemNameText;
    public TextMeshProUGUI ItemDescText;

    private int ownerActorNumber;

    private void Awake()
    {
        ItemDescText.fontSizeMax = Mathf.Max(ItemDescText.fontSizeMax, ItemDescText.fontSize);
        ItemDescText.enableAutoSizing = true;
    }

    private void RefreshTooltip()
    {
        if (NewDrugItem != null && ToolTipPanel.activeSelf) InitItemInfos();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changed) => RefreshTooltip();
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player player, ExitGames.Client.Photon.Hashtable changed)
    {
        if (player.IsLocal) RefreshTooltip();
    }

    private void InitItemInfos()
    {
        ItemNameText.text = LocalizationManager.Instance.GetText(CSV_Type.Item, NewDrugItem.displayName_ID);
        ItemDescText.text = ItemStatusText.GetItemDescription(NewDrugItem);
        if (IsMine(PhotonNetwork.LocalPlayer.ActorNumber))
        {
            string reason = InventoryAuthority.Instance.GetItemUseBlockReason(ownerActorNumber, NewDrugItem, out int missingEnergy);
            if (reason != null) ItemDescText.text = "<b><color=#B83A32>" +
                LocalizationManager.Instance.GetFormatText(CSV_Type.UI, reason, missingEnergy) + "</color></b>\n\n" + ItemDescText.text;
        }
    }

    public void InitOwnerActorNum(int actorNum)
    {
        ownerActorNumber = actorNum;
    }

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
            if (pc.GetInvAdmissionticket() != ownerActorNumber) return;
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
        //아이템 훔치기인 경우
        if (!IsMine(ActNum))
        {
            return;
            //Debug.Log("stealing item activated");
            //if (pc.GetInvAdmissionticket() != owenrActorNumber) return;
            //int ToActorNum = ActNum;
            //키 제거
            //pc.SetInvAdmissionTicket(-1);
            //TODO: 선택된 아이템 상호작용한 플레이어의 인벤토리로 옮기기
            //InventoryAuthority.Instance.RequestSteelItem(owenrActorNumber, ToActorNum, -1, this);
            //return;
        }
        string reason = InventoryAuthority.Instance.GetItemUseBlockReason(ActNum, NewDrugItem, out int missingEnergy);
        if (reason != null)
        {
            if (playerCtrl != null) InventoryAuthority.Instance.ShowItemUseWarning(reason, missingEnergy);
            InitItemInfos();
            return;
        }

        ToolTipPanel.SetActive(false);
        Debug.Log("Interacted!!");

        if (playerCtrl != null)
        {
            playerCtrl.PlayUseItemAnimation(transform, NewDrugItem.itemClass, ItemDB.Instance.GetMat(NewDrugItem.itemId).mainTexture);
        }
        InventoryAuthority.Instance.RequestUseNewDrugItem(ActNum, this);
    }

    public bool HasItem()
    {
        return NewDrugItem != null;
    }

    public bool IsMine(int actNum) => actNum == ownerActorNumber;
}
