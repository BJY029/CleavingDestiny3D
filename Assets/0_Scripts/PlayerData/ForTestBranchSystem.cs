using UnityEngine;

public class ForTestBranchSystem : MonoBehaviour
{
    public void AddBranch500()
    {
        PlayerProfile.AddBranch(500);
        SaveManager.Save();
    }

    public void InitBranch0()
    {
        PlayerProfile.InitBranch();
        SaveManager.Save();
    }
}
