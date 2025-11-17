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
    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private Transform branchCanvas;
    //생성될 위치
    [SerializeField] private Transform spawnRoot;
    //생성 간격
    [SerializeField] private float spacing = 1.25f;
    [SerializeField] private float UISpacing = 105.71f;
    //재생성 방지를 위한 플래그
    private bool spawned = false;

    private GameObject[] UIButtons;

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
        UIButtons = new GameObject[length.Length];

        //각 정보에 맞게 나뭇가지 생성 및 설정 진행
        for(int i = 0; i < length.Length; i++)
        {
            Vector3 pos = spawnRoot.position + new Vector3(-5 + (i * spacing), 0, 0);
            Vector2 UIPos = new Vector2(-370 + (i * UISpacing), 0);

			GameObject branch = Instantiate(branchPrefab, pos, Quaternion.Euler(90, 0, 0), spawnRoot);

            GameObject branchBtn = Instantiate(btnPrefab, branchCanvas);
            UIButtons[i] = branchBtn;

            branchBtn.GetComponentInChildren<Text>().text = "" + (i + 1);
			branchBtn.GetComponent<RectTransform>().anchoredPosition = UIPos;

            BranchController bc = branch.GetComponent<BranchController>();
            bc.InitBranch(i, length[i]);

			Button btn = branchBtn.GetComponent<Button>();
            if (btn == null) Debug.LogWarning("null error(btn null)");
            btn.onClick.AddListener(() => bc.OnClickMyStick());
		}
    }

    public void CallBackBranchClick(int index)
    {
        photonView.RPC(nameof(CallbackBranchClickRPC), RpcTarget.All, index);
    }

	[PunRPC]
	public void CallbackBranchClickRPC(int index)
	{
        UIButtons[index].GetComponent<Button>().interactable = false;
        UIButtons[index].GetComponent<Renderer>().material.color = Color.yellow;
	}
}
