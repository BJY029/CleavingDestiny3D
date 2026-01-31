using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InventoryBarrier : MonoBehaviourPunCallbacks, ILookInteractable
{
	private Collider barrierCollider;
	private MeshRenderer barrierRenderer;
	private GameObject ownerPlayer;

	private Dictionary<int, GameObject> authorizedPlayers;
	private void Awake()
	{
		barrierCollider = GetComponent<Collider>();
		barrierRenderer = GetComponent<MeshRenderer>();
		authorizedPlayers = new Dictionary<int, GameObject>();
	}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		//턴 정보 초기화 되면, 기존의 모든 입장 권한 삭제
		if (propertiesThatChanged.TryGetValue(RoomPropKeys.CurrentTurnActor, out var taObj))
		{
			DenyVisitor();
		}
	}

	public void GrantPermission(int ActNum, GameObject player)
	{
		authorizedPlayers.Add(ActNum, player);
		AllowVisitor(ActNum);
	}

	public void RevokePermission()
	{
		DenyVisitor();
	}

	/// <summary>
	/// Set so that only the inventory owner can pass through the barrier. It is called and set when created.
	/// </summary>
	/// <param name="player"></param>
	public void SetPermission(GameObject player)
	{
		if (player == null || PhotonNetwork.LocalPlayer == null) return;

		ownerPlayer = player;

		Physics.IgnoreCollision(barrierCollider, ownerPlayer.GetComponent<Collider>(), true);
		barrierRenderer.enabled = false;
	}

	private void AllowVisitor(int ActNum)
	{
		if (authorizedPlayers.ContainsKey(ActNum))
		{
			GameObject player = authorizedPlayers[ActNum];
			Physics.IgnoreCollision(barrierCollider, player.GetComponent<Collider>(), true);
			//barrierRenderer.enabled = false;
			barrierCollider.enabled = false;
		}
	}

	private void DenyVisitor()
	{
		GameObject player;
		foreach (var v in authorizedPlayers)
		{
			player = v.Value;
			Physics.IgnoreCollision(barrierCollider, player.GetComponent<Collider>(), false);
		}
		authorizedPlayers.Clear();
		barrierCollider.enabled = true;
	}


	public void OnLookEnter(PlayerController pc)
	{
		if (pc.gameObject == ownerPlayer) return;

		int lockpickCnt = ItemHandlingSystem.instance.HasLockPick(PhotonNetwork.LocalPlayer.ActorNumber);
		if (lockpickCnt <= 0)
			InventoryLockCanvasController.Instance.SetLockpickUI(UI_CSV.UI_Item_LockPick_Warning);
		else
			InventoryLockCanvasController.Instance.SetLockpickUI(UI_CSV.UI_Item_LockPick_Has, lockpickCnt);
	}

	public void OnLookExit(PlayerController pc)
	{
		if (pc.gameObject == ownerPlayer) return;
		InventoryLockCanvasController.Instance.UnSetLockpickUI();
	}

	public void OnInteract(PlayerController pc)
	{
		if (pc.gameObject == ownerPlayer) return;
		if (LockpickController.instance.IsGameActive()) return;

		int lockpickCnt = ItemHandlingSystem.instance.HasLockPick(PhotonNetwork.LocalPlayer.ActorNumber);
		if (lockpickCnt <= 0) return;

		int ActNum = PhotonNetwork.LocalPlayer.ActorNumber;

		int ownerPlayerActNum = photonView.Owner.ActorNumber;

		ItemHandlingSystem.instance.UseLockPick(ActNum);
		InventoryLockCanvasController.Instance.UnSetLockpickUI();
		LockpickController.instance.SetGameActive(ownerPlayerActNum, this, pc);

	}
}
