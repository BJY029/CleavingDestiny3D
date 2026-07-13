using UnityEngine;
using Photon.Pun;

public class DamageTextManager : MonoBehaviourPun
{
    public static DamageTextManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private DamageTextWorld damageTextPrefab;
    public GameObject TreeCenter;
    [SerializeField] private float othersTextRadius = 1.6f;
    [SerializeField] private float othersTextHeight = 1.5f;

    private Camera targetCamera;

    private Vector3? cachedLocalHitPoint;
    public void CacheLocalHitPoint(Vector3 hitPoint) { cachedLocalHitPoint = hitPoint; }


    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
    }

    public void ShowHiddenDamage()
    {
        ShowDamageNearTree(-1);
    }


    public void ShowResolveDamage(int finalDaamge, bool isLocalAttacker)
    {
        if (finalDaamge < 0) return;

        if (isLocalAttacker && cachedLocalHitPoint.HasValue)
        {
            ShowDamage(finalDaamge, cachedLocalHitPoint.Value);
            cachedLocalHitPoint = null;
            return;
        }

        ShowDamageNearTree(finalDaamge);
    }

    public void ShowDamage(int damage, Vector3 hitPoint)
    {
        Vector3 offset = new Vector3(
            Random.Range(-0.2f, 0.2f), Random.Range(0.3f, 0.5f), Random.Range(-0.2f, 0.2f)
        );

        DamageTextWorld damageText = Instantiate(damageTextPrefab, hitPoint + offset, Quaternion.identity);

        damageText.Initialize(targetCamera, damage);
    }

    public void ShowDamageNearTree(int damage)
    {
        if (TreeCenter == null) return;

        Vector3 treePos = TreeCenter.transform.position;
        Vector3 direction;

        if (targetCamera != null) direction = targetCamera.transform.position - treePos;
        else direction = Vector3.forward;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = targetCamera != null ? -targetCamera.transform.forward : Vector3.forward;
            direction.y = 0f;
        }

        direction.Normalize();

        Vector3 spawnPos = treePos + direction * othersTextRadius + Vector3.up * othersTextHeight;

        Vector3 offset = new Vector3(
            Random.Range(-0.15f, 0.15f),
            Random.Range(0.1f, 0.25f),
            Random.Range(-0.15f, 0.15f)
         );

        DamageTextWorld damageText = Instantiate(damageTextPrefab, spawnPos + offset, Quaternion.identity);
        damageText.Initialize(targetCamera, damage);
    }
}
