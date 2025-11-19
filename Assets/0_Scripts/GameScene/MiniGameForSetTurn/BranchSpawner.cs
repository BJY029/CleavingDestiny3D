using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Linq;

public class BranchSpawner : MonoBehaviourPunCallbacks
{
    public static BranchSpawner Instance;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
	}

	//생성할 나뭇가지 프리팹
	[SerializeField] private GameObject branchPrefab;
    //[SerializeField] private GameObject btnPrefab;
    //[SerializeField] private Transform branchCanvas;
    //생성될 위치
    [SerializeField] private Transform spawnRoot;
    //생성 간격
    [SerializeField] private float spacing = 1.25f;
    [SerializeField] private float UISpacing = 105.71f;
    //재생성 방지를 위한 플래그
    private bool spawned = false;

    private GameObject[] branchs;

	//만약 RoomProperties가 업데이트 된 경우
	public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        //업데이트 된 정보가 나뭇가지 관련 정보(즉 처음 관련 정보가 설정된 경우)이면
        if(changedProps.ContainsKey("StickLengths"))
        {
            //나뭇가지 생성
            SpawnSticks();
        }
    }

    private void SpawnSticks()
    {
        //나뭇가지가 생성된 후 라면 실행 안함
        if (spawned) return;
        spawned = true;

        //해당 정보 가져온 후
        var room = PhotonNetwork.CurrentRoom;
        int[] length = (int[])room.CustomProperties["StickLengths"];
        branchs = new GameObject[length.Length];

        //각 정보에 맞게 나뭇가지 생성 및 설정 진행
        for(int i = 0; i < length.Length; i++)
        {
            Vector3 pos = spawnRoot.position + new Vector3(-5 + (i * spacing), 0, 0);
            //Vector2 UIPos = new Vector2(-370 + (i * UISpacing), 0);

			GameObject branch = Instantiate(branchPrefab, pos, Quaternion.Euler(90, 0, 0), spawnRoot);
			branchs[i] = branch;

			//GameObject branchBtn = Instantiate(btnPrefab, branchCanvas);


			//branchBtn.GetComponentInChildren<Text>().text = "" + (i + 1);
			//var rt = branchBtn.GetComponent<RectTransform>();
			//rt.anchoredPosition = UIPos;

			BranchController bc = branch.GetComponent<BranchController>();
            bc.InitBranch(i, length[i]);

			//Button btn = branchBtn.GetComponent<Button>();
            //if (btn == null) Debug.LogWarning("null error(btn null)");
            //btn.onClick.AddListener(() => bc.OnClickMyStick());
		}
    }

    public void CallBackBranchClick(int index, int clickActorNumber)
    {
        photonView.RPC(nameof(CallbackBranchClickRPC), RpcTarget.All, index, clickActorNumber);
	}

	[PunRPC]
	public void CallbackBranchClickRPC(int index, int clickActorNumber)
	{
        //UIButtons[index].GetComponent<Button>().interactable = false;
        //UIButtons[index].GetComponent<Renderer>().material.color = Color.yellow;
        //만약 나뭇가지를 누른 플레이어가 나라면
        if(PhotonNetwork.LocalPlayer.ActorNumber == clickActorNumber)
        {
            //모든 나뭇가지의 상호작용을 막는다.
            for(int i = 0; i < branchs.Length; i++)
            {
                branchs[i].GetComponent<CapsuleCollider>().enabled = false;
            }
		}
		else //다른 플레이어의 이벤트인 경우
		{
            //클릭된 나뭇가지의 상호작용만 막는다.
			branchs[index].GetComponent<CapsuleCollider>().enabled = false;
		}
        //선택된 나뭇가지는 더 이상 선택되지 않도록 설정한다.
        branchs[index].GetComponent<BranchController>().SetSelected();
        //클릭 횟수 증가
		StickGameController.Instance.selectCount += 1;
	}
}
