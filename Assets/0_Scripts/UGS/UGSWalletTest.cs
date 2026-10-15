using UnityEngine;

public class UGSWalletTest : MonoBehaviour
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private enum TestResult { Win, Loss, Draw }

    [SerializeField] private int collectedSticks = 5;
    [SerializeField] private TestResult result = TestResult.Win;
    [SerializeField] private bool showTestPanel = true;

    private UGSManager.RewardRequest lastRequest;
    private bool busy;
    private string status = "버튼을 눌러 테스트하세요. 실제 계정의 재화가 변경됩니다.";

    [ContextMenu("UGS Test/1. 지갑 조회")]
    public async void RefreshWallet()
    {
        if (!BeginTest()) return;
        try
        {
            await UGSManager.Instance.LoadPlayerData();
            Finish("지갑 조회 성공: " + WalletText());
        }
        catch (System.Exception e) { Fail(e); }
        finally { busy = false; }
    }

    [ContextMenu("UGS Test/2. 새 보상 정산")]
    public async void SubmitNewReward()
    {
        if (!BeginTest()) return;
        try
        {
            lastRequest = NewRequest(collectedSticks);
            var response = await UGSManager.Instance.SubmitMatchRewardAsync(lastRequest);
            LogResponse(response);
        }
        catch (System.Exception e) { Fail(e); }
        finally { busy = false; }
    }

    [ContextMenu("UGS Test/3. 같은 ID 서버 재호출")]
    public async void ReplayReward()
    {
        if (!BeginTest()) return;
        try
        {
            if (lastRequest == null) throw new System.InvalidOperationException("먼저 새 보상을 정산하세요.");
            var manager = UGSManager.Instance;
            await manager.EnsureInitializedAsync();
            int coinsBefore = manager.PlayerData["coins"];
            int sticksBefore = manager.PlayerData["sticks"];
            var response = await manager.ReplayCompletedRewardForTestAsync(lastRequest);
            if (!response.accepted || response.coins != coinsBefore || response.sticks != sticksBefore)
                throw new System.InvalidOperationException("중복 테스트 실패: 서버 잔액 또는 승인 상태가 달라졌습니다.");
            Finish("PASS: 같은 ID로 서버에 다시 요청해도 잔액 유지. " + WalletText());
        }
        catch (System.Exception e) { Fail(e); }
        finally { busy = false; }
    }

    [ContextMenu("UGS Test/4. 음수 획득량 거부")]
    public async void RejectNegativeReward()
    {
        if (!BeginTest()) return;
        try
        {
            var manager = UGSManager.Instance;
            await manager.EnsureInitializedAsync();
            int coinsBefore = manager.PlayerData["coins"];
            int sticksBefore = manager.PlayerData["sticks"];
            var response = await manager.SubmitMatchRewardAsync(NewRequest(-1));
            if (response == null || response.accepted || response.rejectionReason != "invalid_sticks" ||
                response.coins != coinsBefore || response.sticks != sticksBefore)
                throw new System.InvalidOperationException("음수 테스트 실패: 거부 이유 또는 잔액을 확인하세요.");
            Finish("PASS: 음수 획득량 거부, 잔액 유지. " + WalletText());
        }
        catch (System.Exception e) { Fail(e); }
        finally { busy = false; }
    }

    [ContextMenu("UGS Test/5. 대기 정산 재시도")]
    public async void RetryPending()
    {
        if (!BeginTest()) return;
        try
        {
            await UGSManager.Instance.RetryPendingRewardsAsync();
            Finish("대기 정산 처리 완료. " + WalletText());
        }
        catch (System.Exception e) { Fail(e); }
        finally { busy = false; }
    }

    private UGSManager.RewardRequest NewRequest(int sticks)
    {
        return new UGSManager.RewardRequest
        {
            settlementId = System.Guid.NewGuid().ToString("N"),
            submittedAt = System.DateTime.UtcNow.ToString("O"),
            collectedSticks = sticks,
            result = result.ToString().ToLowerInvariant()
        };
    }

    private bool BeginTest()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[UGS Test] Play 모드에서 실행하세요.", this);
            return false;
        }
        if (busy) return false;
        busy = true;
        status = "요청 중…";
        return true;
    }

    private string WalletText()
    {
        var manager = UGSManager.Instance;
        return $"coins={manager.PlayerData["coins"]}, sticks={manager.PlayerData["sticks"]}";
    }

    private void LogResponse(UGSManager.WalletResponse response)
    {
        if (response == null) throw new System.InvalidOperationException("정산 응답이 없습니다.");
        Finish($"accepted={response.accepted}, +coins={response.awardedCoins}, +sticks={response.awardedSticks}\n" +
            $"잔액 coins={response.coins}, sticks={response.sticks}\n" +
            $"ID={response.settlementId}, reason={response.rejectionReason}");
    }

    private void Finish(string message)
    {
        status = message;
        Debug.Log("[UGS Test] " + message, this);
    }

    private void Fail(System.Exception error)
    {
        status = "실패: " + error.Message;
        Debug.LogException(error, this);
    }

    private void OnGUI()
    {
        if (!showTestPanel) return;
        GUILayout.BeginArea(new Rect(15, 15, 450, 360), GUI.skin.box);
        GUILayout.Label("UGS Wallet Test — 실제 재화 변경");
        GUILayout.Label($"Inspector 설정: sticks={collectedSticks}, result={result}");
        bool wasEnabled = GUI.enabled;
        GUI.enabled = wasEnabled && !busy;
        if (GUILayout.Button("1. 지갑 조회")) RefreshWallet();
        if (GUILayout.Button("2. 새 보상 정산")) SubmitNewReward();
        if (GUILayout.Button("3. 같은 ID 서버 재호출")) ReplayReward();
        if (GUILayout.Button("4. 음수 획득량 거부")) RejectNegativeReward();
        if (GUILayout.Button("5. 대기 정산 재시도")) RetryPending();
        GUI.enabled = wasEnabled;
        GUILayout.Label(status);
        GUILayout.EndArea();
    }
#endif
}
