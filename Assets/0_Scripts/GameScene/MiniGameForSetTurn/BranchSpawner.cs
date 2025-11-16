using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

public class BranchSpawner : MonoBehaviourPunCallbacks
{
    //생성할 나뭇가지 프리팹
	[SerializeField] private GameObject branchPrefab;
    //생성될 위치
    [SerializeField] private Transform spawnRoot;
    //생성 간격
    [SerializeField] private float spacing = 1.25f;
    //재생성 방지를 위한 플래그
    private bool spawned = false;

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

        //각 정보에 맞게 나뭇가지 생성 및 설정 진행
        for(int i = 0; i < length.Length; i++)
        {
            Vector3 pos = spawnRoot.position + new Vector3(-5 + (i * spacing), 0, 0);

            GameObject branch = Instantiate(branchPrefab, pos, Quaternion.Euler(90, 0, 0), spawnRoot);

            BranchController bc = branch.GetComponent<BranchController>();
            bc.setIndex(i);
            bc.setLength(length[i]);
        }
    }
}
