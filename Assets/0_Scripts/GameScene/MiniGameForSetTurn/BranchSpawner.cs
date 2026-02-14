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

    //������ �������� ������
    [SerializeField] private GameObject branchPrefab;
    //[SerializeField] private GameObject btnPrefab;
    //[SerializeField] private Transform branchCanvas;
    //������ ��ġ
    [SerializeField] private Transform spawnRoot;
    //���� ����
    [SerializeField] private float spacing = 1.25f;
    //[SerializeField] private float UISpacing = 105.71f;
    //����� ������ ���� �÷���
    private bool spawned = false;

    private GameObject[] branchs;

    private void Start()
    {
        // 이미 룸 속성에 StickLengths가 있다면 즉시 생성 (중간에 입장한 경우 등)
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("StickLengths"))
        {
            SpawnSticks();
        }
    }

    //���� RoomProperties�� ������Ʈ �� ���
    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        //������Ʈ �� ������ �������� ���� ����(�� ó�� ���� ������ ������ ���)�̸�
        if (changedProps.ContainsKey("StickLengths"))
        {
            //�������� ����
            SpawnSticks();
        }
    }

    private void SpawnSticks()
    {
        //���������� ������ �� ��� ���� ����
        if (spawned) return;
        spawned = true;

        //�ش� ���� ������ ��
        var room = PhotonNetwork.CurrentRoom;
        int[] length = (int[])room.CustomProperties["StickLengths"];
        branchs = new GameObject[length.Length];

        //�� ������ �°� �������� ���� �� ���� ����
        for (int i = 0; i < length.Length; i++)
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
        //���� ���������� ���� �÷��̾ �����
        if (PhotonNetwork.LocalPlayer.ActorNumber == clickActorNumber)
        {
            //��� ���������� ��ȣ�ۿ��� ���´�.
            for (int i = 0; i < branchs.Length; i++)
            {
                branchs[i].GetComponent<CapsuleCollider>().enabled = false;
            }
        }
        else //�ٸ� �÷��̾��� �̺�Ʈ�� ���
        {
            //Ŭ���� ���������� ��ȣ�ۿ븸 ���´�.
            branchs[index].GetComponent<CapsuleCollider>().enabled = false;
        }
        //���õ� ���������� �� �̻� ���õ��� �ʵ��� �����Ѵ�.
        branchs[index].GetComponent<BranchController>().SetSelected();
        //Ŭ�� Ƚ�� ����
        StickGameController.Instance.selectCount += 1;
    }
}
