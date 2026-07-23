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
        SetOnly(playerCamera);
    }

    //메인 카메라를 켜고 끄는 함수
    public void GameCameraToggle(bool toggle)
    {
        mainCamera.enabled = toggle;
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
    public void Branch_to_Game()
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

    private void SetOnly(Camera targetCam)
    {
        if (playerCamera != null)
            playerCamera.enabled = false;

        if (branchCamera != null)
            branchCamera.enabled = false;

        if (woodLogMinigameCamera != null)
            woodLogMinigameCamera.enabled = false;

        if (targetCam != null)
            targetCam.enabled = true;
        else
            Debug.LogWarning("[CameraSwitchManager] 전환할 카메라가 없습니다.");
    }
}
