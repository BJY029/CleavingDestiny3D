using UnityEngine;

public class SkyboxRotationController : MonoBehaviour
{
    [Header("하늘 회전 설정")]
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private bool useUnscaledTime;

    private Material runtimeSkybox;
    private float currentRotation;

    private static readonly int RotationProperty =
        Shader.PropertyToID("_Rotation");

    private void Awake()
    {
        Material currentSkybox = RenderSettings.skybox;

        if (currentSkybox == null)
        {
            Debug.LogWarning(
                "[SkyboxRotationController] RenderSettings에 Skybox가 없습니다."
            );
            enabled = false;
            return;
        }

        if (!currentSkybox.HasProperty(RotationProperty))
        {
            Debug.LogWarning(
                $"[SkyboxRotationController] " +
                $"{currentSkybox.shader.name} 셰이더에 _Rotation 속성이 없습니다."
            );

            enabled = false;
            return;
        }

        // 프로젝트의 원본 Material이 수정되지 않도록 복제본을 사용한다.
        runtimeSkybox = new Material(currentSkybox);
        RenderSettings.skybox = runtimeSkybox;

        currentRotation =
            runtimeSkybox.GetFloat(RotationProperty);
    }

    private void Update()
    {
        float deltaTime = useUnscaledTime
            ? Time.unscaledDeltaTime
            : Time.deltaTime;

        currentRotation = Mathf.Repeat(
            currentRotation + rotationSpeed * deltaTime,
            360f
        );

        runtimeSkybox.SetFloat(
            RotationProperty,
            currentRotation
        );
    }

    private void OnDestroy()
    {
        if (runtimeSkybox != null)
        {
            Destroy(runtimeSkybox);
        }
    }
}
