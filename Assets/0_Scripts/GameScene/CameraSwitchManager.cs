using UnityEngine;

public class CameraSwitchManager : MonoBehaviour
{
    public static CameraSwitchManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SetOnly(branchCamera);
    }

    public Camera mainCamera;
    public Camera branchCamera;
    public Camera woodLogMinigameCamera;

    [Header("Runtime Assigned")]
    [SerializeField] private Camera playerCamera;

    // Player가 스폰된 뒤 자기 카메라를 등록하는 함수
    public void RegisterPlayerCamera(Camera cam)
    {
        playerCamera = cam;

        // 기본 상태는 플레이어 카메라 ON
        SetCameraState(playerCamera, false);
    }

    //메인 카메라를 켜고 끄는 함수
    public void GameCameraToggle(bool toggle)
    {
        SetCameraState(mainCamera, toggle);
    }

    public void BranchCameraOn()
    {
        SetOnly(branchCamera);
    }

    public void MainCameraOn()
    {
        SetOnly(mainCamera);
    }

    public void PlayerCameraOn()
    {
        if (playerCamera == null) return;
        SetOnly(playerCamera);
    }

    public void Off_ExceptPlayerCam()
    {
        SetOnly(playerCamera);
    }

    public void Player_to_LogMiniGame()
    {
        SetOnly(woodLogMinigameCamera);
    }

    public void LogMiniGame_to_Player()
    {
        SetOnly(playerCamera);
    }

    private void SetOnly(Camera targetCamera)
    {
        SetCameraState(mainCamera, false);
        SetCameraState(branchCamera, false);
        SetCameraState(woodLogMinigameCamera, false);
        SetCameraState(playerCamera, false);

        if (targetCamera == null)
        {
            Debug.LogWarning(
                "[CameraSwitchManager] 전환할 카메라가 없습니다.");
            return;
        }

        SetCameraState(targetCamera, true);
    }

    private static void SetCameraState(
        Camera targetCamera,
        bool active)
    {
        if (targetCamera == null)
        {
            return;
        }

        targetCamera.enabled = active;

        AudioListener[] listeners =
            targetCamera.GetComponentsInChildren<AudioListener>(true);

        foreach (AudioListener listener in listeners)
        {
            listener.enabled = active;
        }
    }
}
