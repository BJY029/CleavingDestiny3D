using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class PickUpBranchSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private BranchSpawnArea[] spawnAreas;

    [Header("Spawn Count")]
    [SerializeField] private int initialSpawnCount = 4;
    [SerializeField] private int maxActiveBranchCount = 8;

    [Header("Spawn Delay")]
    [SerializeField] private float minSpawnDelay = 5f;
    [SerializeField] private float maxSpawnDelay = 12f;

    private Coroutine spawnCoroutine;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        StartSpawnLoop(true);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        if (newMasterClient.IsLocal) StartSpawnLoop(false);
        else StopSpawnLoop();
    }

    private void StartSpawnLoop(bool spawnInitialBranches)
    {
        StopSpawnLoop();

        if (spawnInitialBranches) SpawnInitialBranches();

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void StopSpawnLoop()
    {
        if (spawnCoroutine == null) return;

        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
    }


    private void SpawnInitialBranches()
    {
        int spawnCount = Mathf.Min(initialSpawnCount, maxActiveBranchCount);

        for (int i = 0; i < spawnCount; i++)
        {
            TrySpawnBranch();
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);

            yield return new WaitForSeconds(delay);

            TrySpawnBranch();
        }
    }

    private bool TrySpawnBranch()
    {
        if (!PhotonNetwork.IsMasterClient) return false;

        BranchNetworkManager networkManager = BranchNetworkManager.Instance;

        if (networkManager == null) return false;

        if (networkManager.ActiveCount >= maxActiveBranchCount) return false;

        if (spawnAreas == null || spawnAreas.Length == 0) return false;

        if (networkManager.PrefabCount <= 0) return false;

        //랜덤 구역 설정
        int startIndex = Random.Range(0, spawnAreas.Length);

        for (int i = 0; i < spawnAreas.Length; i++)
        {
            int index = (startIndex + i) % spawnAreas.Length;
            BranchSpawnArea area = spawnAreas[index];

            //만약 해당 구역에서 스폰할 공간을 찾지 못한 경우 다음 Area 탐색
            if (area == null) continue;

            if (!area.TryGetSpawnPose(out Vector3 position, out Quaternion rotation)) continue;

            int prefabIndex = Random.Range(0, networkManager.PrefabCount);

            networkManager.MasterSpawnBranch(prefabIndex, position, rotation);

            return true;
        }

        return false;
    }
}
