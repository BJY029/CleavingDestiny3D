using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private async void Start()
    {
        try
        {
            await UGSManager.Instance.RetryPendingRewardsAsync();
            GameSessionRewardManager.RetryCurrentReward();
        }
        catch (System.Exception e) { Debug.LogWarning($"[UGS] Pending rewards will retry on the next lobby visit: {e.Message}"); }
    }

    private void Awake()
    {
        SaveManager.Initialize();
    }
}
