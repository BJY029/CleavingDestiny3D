using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private DamageTextWorld damageTextPrefab;

    private Camera targetCamera;

    public void SetTargetCamera(Camera camera)
    {
        targetCamera = camera;
    }

    public void ShowDamage(int damage, Vector3 hitPoint)
    {
        Vector3 offset = new Vector3(
            Random.Range(-0.2f, 0.2f), Random.Range(0.3f, 0.5f), Random.Range(-0.2f, 0.2f)
        );

        DamageTextWorld damageText = Instantiate(damageTextPrefab, hitPoint + offset, Quaternion.identity);

        damageText.Initialize(targetCamera, damage);
    }
}
