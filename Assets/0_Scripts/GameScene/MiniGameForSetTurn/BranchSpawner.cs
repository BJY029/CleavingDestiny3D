using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

public class BranchSpawner : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject branchPrefab;
    [SerializeField] private Transform spawnRoot;
    [SerializeField] private float spacing = 1.25f;

    private bool spawned = false;

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if(changedProps.ContainsKey("StickLengths"))
        {
            SpawnSticks();
        }
    }

    private void SpawnSticks()
    {
        if (spawned) return;
        spawned = true;

        var room = PhotonNetwork.CurrentRoom;
        int[] length = (int[])room.CustomProperties["StickLengths"];

        for(int i = 0; i < length.Length; i++)
        {
            Vector3 pos = spawnRoot.position + new Vector3(-5 + (i * spacing), 0, 0);

            GameObject branch = Instantiate(branchPrefab, pos, Quaternion.identity, spawnRoot);

            BranchController bc = branch.GetComponent<BranchController>();
            bc.setIndex(i);
            bc.setLength(length[i]);
        }
    }
}
