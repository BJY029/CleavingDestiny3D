using UnityEngine;
using TMPro;
using Photon.Pun;
using System;

public class InventoryLockCanvasController : MonoBehaviourPunCallbacks
{
	public static InventoryLockCanvasController Instance;

	[SerializeField]
	private GameObject LockpickUI;
	private TextMeshProUGUI LockpickText;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);

		LockpickText = LockpickUI.GetComponentInChildren<TextMeshProUGUI>();
		LockpickUI.SetActive(false);
	}

	public void SetLockpickUI(string command, int cnt = 0)
	{
		LockpickUI.SetActive(true);
		string text = LocalizationManager.Instance.GetText(CSV_Type.UI, command);

		LockpickText.text = text;

		if (cnt > 0) LockpickText.text += " : " + cnt.ToString();
	}

	public void UnSetLockpickUI()
	{
		LockpickUI.SetActive(false);
	}
}
