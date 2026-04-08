using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Village;
using Photon.Pun;
using Photon.Realtime;

public class AIVillageUpgrader : AILogicModule
{
    IVillageManager villageManager;
    IVillageStatProvider statProvider;
    VillageSceneManager villageSceneManager;

    // 건물별 고유 선호도 (기본값)
    Dictionary<VillageType, float> buildingWeights = new Dictionary<VillageType, float>
    {
        { VillageType.Farm, 1.0f },
        { VillageType.Forge, 0.8f },
        { VillageType.Shop, 0.7f },
        { VillageType.Barrier, 0.9f },
        { VillageType.Mine, 1.2f } // 광산 기본 선호도 가장 높음
    };

    Dictionary<AIResult, int> score = new();

    enum AIResult
    {
        UpgradeFarm,
        UpgradeForge,
        UpgradeShop,
        UpgradeBarrier,
        UpgradeMine,
        BuyItem,
        None
    }

    void SetDependency(IVillageManager villageManager, IVillageStatProvider statProvider)
    {
        this.villageManager = villageManager;
        this.statProvider = statProvider;
    }

    public async UniTask EnterVillage()
    {
        while (VillageSystem.Instance == null)
        {
            await UniTask.Delay(100);
        }

        SetDependency(VillageSystem.VillageLogic, VillageSystem.VillageStat);

        Debug.Log($"[AI {brain.MyActorNum}] 마을 업그레이드 단계 진입");

        if (villageSceneManager == null)
        {
            villageSceneManager = FindFirstObjectByType<VillageSceneManager>();
        }

        while (true)
        {
            AIContext context = brain.GetCurAIStat(brain.MyActorNum);
            int curGold = villageManager.GetMyGold(brain.MyActorNum);

            AIResult bestAction = AIResult.None;
            int bestScore = -1;

            for (int i = 0; i <= (int)AIResult.BuyItem; i++)
            {
                AIResult potentialResult = (AIResult)i;
                int currentScore = CalculateScore(potentialResult, context, curGold);
                score[potentialResult] = currentScore;

                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestAction = potentialResult;
                }
            }

            if (bestAction == AIResult.None || bestScore <= 0)
                break;

            if (bestAction == AIResult.BuyItem)
            {
                // TODO: 상점 아이템 구매 로직 연결 필요 (현재는 업그레이드 우선)
                break;
            }

            var buildingType = bestAction switch
            {
                AIResult.UpgradeFarm => VillageType.Farm,
                AIResult.UpgradeForge => VillageType.Forge,
                AIResult.UpgradeShop => VillageType.Shop,
                AIResult.UpgradeBarrier => VillageType.Barrier,
                AIResult.UpgradeMine => VillageType.Mine,
                _ => VillageType.Mine,
            };

            Debug.Log($"[AI {brain.MyActorNum}] 업그레이드 선택: {buildingType} (점수: {bestScore})");

            bool success = villageManager.TryUpgradeLevel(buildingType, brain.MyActorNum);
            if (!success) break;

            await UniTask.Yield();
        }

        Debug.Log($"[AI {brain.MyActorNum}] 마을 업그레이드 완료");

        if (villageSceneManager != null)
        {
            villageSceneManager.SetPlayerReady(brain.MyActorNum, true);
        }
    }

    private int CalculateScore(AIResult potentialResult, AIContext context, int curGold)
    {
        if (potentialResult == AIResult.None) return -1;

        // 아이템 구매 점수 (인벤토리가 비어있을 때만 고려)
        if (potentialResult == AIResult.BuyItem)
        {
            int itemCount = brain.GetPlayerItemCnt(brain.MyActorNum);
            if (itemCount < context.curInvCap && curGold > 400)
            {
                float emptyRatio = 1.0f - ((float)itemCount / context.curInvCap);
                return (int)(emptyRatio * 50f);
            }
            return 0;
        }

        var buildingType = potentialResult switch
        {
            AIResult.UpgradeFarm => VillageType.Farm,
            AIResult.UpgradeForge => VillageType.Forge,
            AIResult.UpgradeShop => VillageType.Shop,
            AIResult.UpgradeBarrier => VillageType.Barrier,
            AIResult.UpgradeMine => VillageType.Mine,
            _ => VillageType.Mine,
        };

        int curLevel = statProvider.GetVillageLevel(buildingType, brain.MyActorNum);
        int upgradeCost = statProvider.GetLevelUpgradedCost(buildingType, curLevel);
        if (upgradeCost > curGold || upgradeCost <= 0) return -1;

        float score = 50f; // 기본 점수

        // 건물별 고유 가중치 적용
        if (buildingWeights.TryGetValue(buildingType, out float weight))
        {
            score *= weight;
        }

        float waveProgress = (float)context.curWaveCnt / Mathf.Max(1, context.maxWaveCnt);
        bool isEarlyGame = waveProgress < 0.35f;

        switch (buildingType)
        {
            case VillageType.Mine:
                // 광산: 초반 경제의 핵심. 
                // 초반일수록, 현재 레벨이 낮을수록 점수 폭등
                float mineEarlyBonus = isEarlyGame ? 150f : 60f;
                score += (1.0f - waveProgress) * mineEarlyBonus;
                if (curLevel < 2) score += 50f; // 최우선 2레벨 달성 유도
                break;

            case VillageType.Farm:
                // 농장: 기력이 부족하면 아이템 사용이 불가능함.
                // if (context.curMaxEnergy < 50) score += 60f;
                float energyLackRatio = 1.0f - ((float)context.curEnergy / Mathf.Max(1, context.curMaxEnergy));
                score += energyLackRatio * 40f;
                break;

            case VillageType.Forge:
                // 대장간: 나무를 잡기 위한 공격력. 후반으로 갈수록 중요.
                // 절대 HP가 아닌 진행률 기반으로 판단
                score += waveProgress * 80f;
                // 나무 체력이 많이 남았을 때 보너스 (비율로 계산)
                // 평균 10000 HP 기준 0~40점
                float treeHpFactor = Mathf.Clamp01(context.curTreeHP / 10000f);
                score += treeHpFactor * 40f;
                break;

            case VillageType.Barrier:
                // 방벽: 생존을 위한 최후의 수단.
                float villageHpRatio = Mathf.Clamp01(context.curVillageHP / 100f);
                if (villageHpRatio < 0.5f) score += (1.0f - villageHpRatio) * 150f;
                if (context.curTreeToxicDmg > 15) score += 50f;
                break;

            case VillageType.Shop:
                // 시장: 좋은 아이템 확률 증가. 중반 이후 효율 발생.
                score += waveProgress * 70f;
                if (curGold > 1200) score += 30f;
                break;
        }

        // 레벨이 높아질수록 비용 대비 효율을 고려해 점수 감쇠
        // (한 우물만 파는 것보다 골고루 올리는 것이 이득이 되도록)
        score -= (curLevel * 35f);

        return (int)Mathf.Max(0, score);
    }

    public void ExitVillage()
    {

    }
}
