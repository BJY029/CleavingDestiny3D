using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BranchSpawnArea : MonoBehaviour
{
    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundOffset = 0.03f;
    [SerializeField] private float maxSlopeAngle = 35f;

    [Header("Spawn Validation")]
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private int maxSearchAttempts = 10;

    [Header("Raycast")]
    [SerializeField] private float rayStartHeight = 5f;
    [SerializeField] private float rayDistance = 20f;

    private BoxCollider spawnArea;

    private void Awake()
    {
        TryGetComponent<BoxCollider>(out spawnArea);
    }

    public bool TryGetSpawnPose(out Vector3 position, out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;

        for (int i = 0; i < maxSearchAttempts; i++)
        {
            Vector3 randomPoint = GetRandomPoint();
            Vector3 rayStart = randomPoint + Vector3.up * rayStartHeight;

            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, rayDistance, groundLayer, QueryTriggerInteraction.Ignore))
                continue;

            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (slopeAngle > maxSlopeAngle) continue;

            Vector3 spawnPosition = hit.point + hit.normal * groundOffset;

            if (Physics.CheckSphere(spawnPosition, checkRadius, blockLayer, QueryTriggerInteraction.Ignore))
                continue;

            position = spawnPosition;

            Quaternion groundRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Quaternion randomYRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), hit.normal);

            rotation = randomYRotation * groundRotation;

            return true;
        }

        return false;
    }

    private Vector3 GetRandomPoint()
    {
        Vector3 center = spawnArea.center;
        Vector3 size = spawnArea.size;

        float randomX = Random.Range(-size.x * 0.5f, size.x * 0.5f);
        float randomZ = Random.Range(-size.z * 0.5f, size.z * 0.5f);

        Vector3 localPoint = center + new Vector3(randomX, 0f, randomZ);

        return transform.TransformPoint(localPoint);
    }
}
