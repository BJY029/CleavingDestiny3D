using UnityEngine;
using System.Collections;

public class PickUpBranchSpawner : MonoBehaviour
{
    [SerializeField] private BranchPool branchPool;
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
        SpawnInitialBranchs();

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private void SpawnInitialBranchs()
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

            if (branchPool.ActiveCount >= maxActiveBranchCount) continue;

            TrySpawnBranch();
        }
    }

    private bool TrySpawnBranch()
    {
        if (branchPool.ActiveCount >= maxActiveBranchCount) return false;

        if (spawnAreas == null || spawnAreas.Length == 0) return false;

        //랜덤 구역 설정
        int startIndex = Random.Range(0, spawnAreas.Length);

        for (int i = 0; i < spawnAreas.Length; i++)
        {
            int index = (startIndex + i) % spawnAreas.Length;
            BranchSpawnArea area = spawnAreas[index];

            //만약 해당 구역에서 스폰할 공간을 찾지 못한 경우 다음 Area 탐색
            if (area == null) continue;

            if (!area.TryGetSpawnPose(out Vector3 position, out Quaternion rotation)) continue;

            BranchPickUp branch = branchPool.Get(position, rotation);

            return branch != null;
        }

        return false;
    }
}
