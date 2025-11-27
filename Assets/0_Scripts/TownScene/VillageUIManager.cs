using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VillageUIManager : MonoBehaviourPunCallbacks
{
    public static VillageUIManager Instance;
    StringBuilder currentGoldText = new StringBuilder();
    public TextMeshProUGUI goldText;

    [Header("UIs")]
    public Slider TimeSlider;
    public UnityEngine.UI.Button OpenVillageBtn;
    public UnityEngine.UI.Button CloseVillageBtn;
    public GameObject VillagePanel;
    private CanvasGroup canvasGroup;

    private float startTime;
    private float endTime;
    private float duration;
    private bool isUpgradePhase;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(Instance);

        VillageManager.Instance.OnGoldChanged.AddListener(UpdateGoldText);
        UpdateGoldText(VillageManager.Instance.GetMyGold());
		canvasGroup = GetComponent<CanvasGroup>();
		OpenVillageBtn.onClick.AddListener(OnClickOpenVillage);
		CloseVillageBtn.onClick.AddListener(OnClickCloseVillage);
	}

	private void Start()
	{
		canvasGroup.alpha = 0f;       // 완전 투명
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	private void Update()
	{
        if (!isUpgradePhase) return;

        float now = (float)PhotonNetwork.Time;
        float remain = endTime - now;
        TimeSlider.value = remain / duration;
	}

	private void UpdateGoldText(int gold)
    {
        currentGoldText.Clear();
        currentGoldText.Append("Gold: ");
        currentGoldText.Append(gold);
        goldText.text = currentGoldText.ToString();
    }

    public void OnClickUpgradeHouseButton()
    {
        VillageManager.Instance.TryUpgradeLevel(VillageUpgradeIndex.House);
    }

    public void OnClickAddGoldButton()
    {
        VillageManager.Instance.AddGold(100);
    }

    public void OnClickOpenVillage()
    {
        VillagePanel.SetActive(true);
        OpenVillageBtn.gameObject.SetActive(false);
    }

    public void OnClickCloseVillage()
    {
        VillagePanel.SetActive(false);
        OpenVillageBtn.gameObject.SetActive(true);
    }


    public void SetActiveCanvas(bool active)
    {
        photonView.RPC(nameof(RPC_SetActiveCanvas), RpcTarget.All, active);
    }

    [PunRPC]
    private void RPC_SetActiveCanvas(bool active)
    {
        if(active)
        {
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;

			Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
		}
        else
        {
			VillagePanel.SetActive(false);
			OpenVillageBtn.gameObject.SetActive(true);

			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(RoomPropKeys.VillageUpgradeStartEndTime))
        {
            float[] times = PhotonPropertyHelper.GetRoomProp<float[]>(RoomPropKeys.VillageUpgradeStartEndTime);
            startTime = times[0];
            endTime = times[1];
            duration = endTime - startTime;
        }

		if (propertiesThatChanged.ContainsKey(RoomPropKeys.IsVillageUpgradePhase))
		{
			isUpgradePhase = PhotonPropertyHelper.GetRoomProp<bool>(RoomPropKeys.IsVillageUpgradePhase);
		}
	}
}