using UnityEngine;

public class ForTestBranchSystem : MonoBehaviour
{
#if !UNITY_EDITOR
    private void Awake()
    {
        gameObject.SetActive(false);
    }
#endif

    public void AddBranch500()
    {
#if UNITY_EDITOR
        PlayerProfile.AddBranch(500);
        SaveManager.Save();
#endif
    }

    public void InitBranch0()
    {
#if UNITY_EDITOR
        PlayerProfile.InitBranch();
        SaveManager.Save();
#endif
    }

    public void ResetSaveData()
    {
#if UNITY_EDITOR
        SaveManager.ResetSaveForTest();
#endif
    }
}
