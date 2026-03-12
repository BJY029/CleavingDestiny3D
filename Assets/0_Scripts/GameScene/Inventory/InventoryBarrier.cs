using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InventoryBarrier : MonoBehaviourPunCallbacks, ILookInteractable
{
	public int owner;
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


	public void OnLookEnter(IPlayerAction pc)
	{
		// 1. as 연산자로 캐스팅을 시도합니다. (타입이 다르면 null이 들어감)
		PlayerController playerCtrl = pc as PlayerController;
		AIController aiCtrl = pc as AIController;

		// 2. 둘 다 null이라면 잘못된 타입이 들어온 것
		if (playerCtrl == null && aiCtrl == null)
		{
			Debug.LogError("pc is not PlayerController or AIController");
			return;
		}

		// 3. GameObject 소유자 확인 (둘 중 null이 아닌 쪽의 gameObject를 참조)
		GameObject targetObj = (playerCtrl != null) ? playerCtrl.gameObject : aiCtrl.gameObject;
		if (targetObj == ownerPlayer) return;


		int lockpickCnt = ItemHandlingSystem.instance.HasLockPick((playerCtrl != null) ? playerCtrl.PlayerActNum : aiCtrl.PlayerActNum);
		if (lockpickCnt <= 0)
			InventoryLockCanvasController.Instance.SetLockpickUI(UI_CSV.UI_Item_LockPick_Warning);
		else
			InventoryLockCanvasController.Instance.SetLockpickUI(UI_CSV.UI_Item_LockPick_Has, lockpickCnt);
	}

	public void OnLookExit(IPlayerAction pc)
	{
		// 1. as 연산자로 캐스팅을 시도합니다. (타입이 다르면 null이 들어감)
		PlayerController playerCtrl = pc as PlayerController;
		AIController aiCtrl = pc as AIController;

		// 2. 둘 다 null이라면 잘못된 타입이 들어온 것
		if (playerCtrl == null && aiCtrl == null)
		{
			Debug.LogError("pc is not PlayerController or AIController");
			return;
		}

		// 3. GameObject 소유자 확인 (둘 중 null이 아닌 쪽의 gameObject를 참조)
		GameObject targetObj = (playerCtrl != null) ? playerCtrl.gameObject : aiCtrl.gameObject;
		if (targetObj == ownerPlayer) return;

		InventoryLockCanvasController.Instance.UnSetLockpickUI();
	}

	public void OnInteract(IPlayerAction pc)
	{
		// 1. as 연산자로 캐스팅을 시도합니다. (타입이 다르면 null이 들어감)
		PlayerController playerCtrl = pc as PlayerController;
		AIController aiCtrl = pc as AIController;

		// 2. 둘 다 null이라면 잘못된 타입이 들어온 것
		if (playerCtrl == null && aiCtrl == null)
		{
			Debug.LogError("pc is not PlayerController or AIController");
			return;
		}

		// 3. GameObject 소유자 확인 (둘 중 null이 아닌 쪽의 gameObject를 참조)
		GameObject targetObj = (playerCtrl != null) ? playerCtrl.gameObject : aiCtrl.gameObject;
		if (targetObj == ownerPlayer) return;

		if (LockpickController.instance.IsGameActive()) return;

		int ActNum = (playerCtrl != null) ? playerCtrl.PlayerActNum : aiCtrl.PlayerActNum;

		int lockpickCnt = ItemHandlingSystem.instance.HasLockPick(ActNum);
		if (lockpickCnt <= 0) return;

		ItemHandlingSystem.instance.UseLockPick(ActNum);
		InventoryLockCanvasController.Instance.UnSetLockpickUI();
		LockpickController.instance.SetGameActive(owner, this, pc);

	}
}
