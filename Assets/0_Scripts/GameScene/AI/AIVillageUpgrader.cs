using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Village;

public class AIVillageUpgrader : AILogicModule
{
    IVillageManager villageManager;
    VillageSceneManager villageSceneManager;

    public async UniTask EnterVillage()
    {
        // 빌리지 시스템 찾기
        while (VillageSystem.Instance == null)
        {
            await UniTask.Delay(100); // 0.1초 대기 후 재시도
        }

        villageManager = VillageSystem.VillageLogic;

        // AI 판단 후 마을 업그레이드
        Debug.Log($"[AI {brain.MyActorNum}] 마을 업그레이드 단계 진입");

        // 모든 처리 후 레디 상태로 전환
        if (villageSceneManager == null)
        {
            villageSceneManager = FindFirstObjectByType<VillageSceneManager>();
        }



        // 현재 AI는 항상 준비 완료된 상태로 간주하므로 별도의 프로퍼티 업데이트 없음
        // villageSceneManager.SetPlayerReady(brain.MyActorNum, true);
    }

    public void ExitVillage()
    {

    }
}
