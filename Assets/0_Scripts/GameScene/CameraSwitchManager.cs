using UnityEngine;

public class CameraSwitchManager : MonoBehaviour
{
    public static CameraSwitchManager Instance;
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(Instance);
			return;
		}
		Instance = this;
	}
	

	public Camera mainCamera;
    public Camera branchCamera;

    public void Off_ExceptPlayerCam()
    {
        mainCamera.enabled = false;
        branchCamera.enabled = false;
    }
	public void Branch_to_Game()
    {
        ChangeCamera_to_from(branchCamera, mainCamera);
    }

    private void ChangeCamera_to_from(Camera existingCam, Camera changeCam)
    {
        existingCam.enabled = false;
        changeCam.enabled = true;
    }

    private void PlayerCameraOn()
    {
        mainCamera.enabled = false;
        branchCamera.enabled = false;
    }

}
