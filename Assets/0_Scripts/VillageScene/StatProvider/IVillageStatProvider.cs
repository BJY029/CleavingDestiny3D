using Photon.Realtime;

namespace Village
{
    public interface IVillageStatProvider
    {
        /// <summary>
        /// 특정 플레이어의 건물의 현재 레벨을 안전하게 가져오기
        /// </summary>
        /// <param name="type">건물의 유형</param>
        /// <param name="targetPlayer">대상 플레이어, null일 경우 현재 플레이어</param>
        /// <returns>오류시 0 반환</returns>
        int GetVillageLevel(VillageType type, Player targetPlayer = null);

        /// <summary>
        /// 건물의 다음 레벨 업그레이드 비용을 가져오기 (현재 레벨 기준)
        /// </summary>
        /// <param name="facilityType">건물의 유형</param>
        /// <param name="targetPlayer">대상 플레이어, null일 경우 현재 플레이어</param>
        /// <returns>오류시 0 반환</returns>
        int GetLevelUpgradedCost(VillageType facilityType, Player targetPlayer = null);

        /// <summary>
        /// 건물의 특정 레벨에서 다음 레벨로 가는 업그레이드 비용을 가져오기
        /// </summary>
        /// <param name="facilityType">건물의 유형</param>
        /// <param name="currentLevel">기준 레벨</param>
        /// <param name="targetPlayer">대상 플레이어, null일 경우 현재 플레이어</param>
        /// <returns>비용</returns>
        int GetLevelUpgradedCost(VillageType facilityType, int currentLevel, Player targetPlayer = null);

        /// <summary>
        /// 일일 골드 수입 획득량 가져오기 (주로 광산 레벨 비례)
        /// </summary>
        /// <param name="level">건물 레벨</param>
        /// <returns>일일 골드 수입량</returns>
        int GetGoldIncomePerDay(int level);

        /// <summary>
        /// 최대 에너지(행동력) 수치 가져오기 (주로 농장 레벨 비례)
        /// </summary>
        /// <param name="level">건물 레벨</param>
        /// <returns>최대 에너지</returns>
        float GetMaxEnergy(int level);

        /// <summary>
        /// 일일 에너지(행동력) 회복량 가져오기
        /// </summary>
        /// <param name="level">건물 레벨</param>
        /// <returns>일일 에너지 회복량</returns>
        float GetEnergyIncomePerDay(int level);

        (float min, float max) GetAxeRangeDamage(int level);

        /// <summary>
        /// 방벽 방어력 수치 가져오기 (주로 상점/방어 시설 레벨 비례)
        /// </summary>
        /// <param name="level">건물 레벨</param>
        /// <returns>방벽 방어력</returns>
        float GetBarrierArmor(int level);
    }
}
