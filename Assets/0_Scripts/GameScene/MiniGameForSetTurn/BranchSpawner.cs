using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections;

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
    [SerializeField] private float startX = -5f;

    [SerializeField] private float topAlignZOffset = 0f;
    [SerializeField] private bool topIsMaxZ = true;
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
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable changedProps)
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
            Vector3 pos = spawnRoot.position + new Vector3(startX + (i * spacing), 0, 0);
            //Vector2 UIPos = new Vector2(-370 + (i * UISpacing), 0);

            GameObject branch = Instantiate(branchPrefab, pos, Quaternion.Euler(90, 0, 0), spawnRoot);
            branchs[i] = branch;

            //GameObject branchBtn = Instantiate(btnPrefab, branchCanvas);


            //branchBtn.GetComponentInChildren<Text>().text = "" + (i + 1);
            //var rt = branchBtn.GetComponent<RectTransform>();
            //rt.anchoredPosition = UIPos;

            BranchController bc = branch.GetComponent<BranchController>();
            bc.InitBranch(i, length[i]);

            float targetTopZ = spawnRoot.position.z + topAlignZOffset;
            AlignBranchTopZ(branch, targetTopZ);

            //Button btn = branchBtn.GetComponent<Button>();
            //if (btn == null) Debug.LogWarning("null error(btn null)");
            //btn.onClick.AddListener(() => bc.OnClickMyStick());
        }
    }

    private void AlignBranchTopZ(GameObject branch, float targetTopZ)
    {
        Renderer[] renderers = branch.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0) return;

        Bounds totalBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            totalBounds.Encapsulate(renderers[i].bounds);
        }

        float currentTopZ = topIsMaxZ ? totalBounds.max.z : totalBounds.min.z;

        float moveZ = targetTopZ - currentTopZ;

        branch.transform.position += Vector3.forward * moveZ;
    }

    public void CallBackBranchClick(int index, int clickActorNumber)
    {
        photonView.RPC(nameof(CallbackBranchClickRPC), RpcTarget.All, index, clickActorNumber);
    }

    [PunRPC]
    public void CallbackBranchClickRPC(int index, int clickActorNumber)
    {
        GameObject selectedBranch = branchs[index];
        if (PhotonNetwork.LocalPlayer.ActorNumber == clickActorNumber)
        {
            for (int i = 0; i < branchs.Length; i++)
            {
                branchs[i].GetComponent<CapsuleCollider>().enabled = false;
            }
        }
        else
        {

            branchs[index].GetComponent<CapsuleCollider>().enabled = false;
        }

        branchs[index].GetComponent<BranchController>().SetSelected();

        StartCoroutine(PlayBranchScaleEffect(selectedBranch.transform));
    }


    [SerializeField] private float selectedScaleMultiplier = 1.12f;
    [SerializeField] private float scaleUpDuration = 0.1f;
    [SerializeField] private float scaleDownDuration = 0.12f;


    private IEnumerator PlayBranchScaleEffect(Transform target)
    {
        if (target == null) yield break;

        Vector3 originalScale = target.localScale;

        Vector3 selectedScale = new Vector3(originalScale.x * 2.5f, originalScale.y, originalScale.z * 2.5f);

        Vector3 enalrgedScale = new Vector3(originalScale.x * selectedScaleMultiplier,
        originalScale.y, originalScale.z * selectedScaleMultiplier);

        float elapsedTime = 0f;

        while (elapsedTime < scaleUpDuration)
        {
            if (target == null) yield break;

            elapsedTime += Time.deltaTime;

            float ratio = Mathf.Clamp01(elapsedTime / scaleDownDuration);
            ratio = Mathf.SmoothStep(0f, 1f, ratio);

            target.localScale = Vector3.Lerp(originalScale, enalrgedScale, ratio);

            yield return null;
        }

        target.localScale = enalrgedScale;
        elapsedTime = 0f;

        while (elapsedTime < scaleDownDuration)
        {
            if (target == null) yield break;

            elapsedTime += Time.deltaTime;

            float ratio = Mathf.Clamp01(elapsedTime / scaleDownDuration);
            ratio = Mathf.SmoothStep(0f, 1f, ratio);

            target.localScale = Vector3.Lerp(enalrgedScale, selectedScale, ratio);

            yield return null;
        }

        target.localScale = selectedScale;
    }
}
