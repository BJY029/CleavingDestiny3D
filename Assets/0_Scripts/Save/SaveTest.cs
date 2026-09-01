using UnityEngine;

public class SaveTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"현재 나뭇가지 : {PlayerProfile.BranchCount}");
        Debug.Log($"현재 도끼 : {PlayerProfile.EquippedAxeSkinId}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerProfile.AddBranch(10);

            Debug.Log($"나뭇가지 +10 : {PlayerProfile.BranchCount}");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SaveManager.Save();

            Debug.Log("저장");
        }
    }
}
