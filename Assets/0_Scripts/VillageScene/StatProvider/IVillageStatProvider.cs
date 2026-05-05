using Photon.Realtime;

namespace Village
{
    public interface IVillageStatProvider
    {
        public VillageBalanceData VillageBalance { get; }

        /// <summary>
        /// 특정 액터 번호(AI 포함)의 건물의 현재 레벨을 가져오기.
        /// actorNumber가 -1(기본값)이면 로컬 플레이어의 정보를 가져옵니다.
        /// </summary>
        int GetVillageLevel(VillageType type, int actorNumber = -1);

        /// <summary>
        /// 건물의 특정 레벨에서 다음 레벨로 가는 업그레이드 비용을 가져오기
        /// </summary>
        int GetLevelUpgradedCost(VillageType facilityType, int currentLevel);

        /// <summary>
        /// 일일 골드 수입 획득량 가져오기 (주로 광산 레벨 비례)
        /// </summary>
        int GetGoldIncomePerDay(int level);

        /// <summary>
        /// 최대 에너지(행동력) 수치 가져오기 (주로 농장 레벨 비례)
        /// </summary>
        int GetMaxEnergy(int level);

        /// <summary>
        /// 일일 에너지(행동력) 회복량 가져오기
        /// </summary>
        int GetEnergyIncomePerDay(int level);

        (int min, int max) GetAxeRangeDamage(int level);

        /// <summary>
        /// 방벽 방어력 수치 가져오기 (주로 상점/방어 시설 레벨 비례)
        /// </summary>
        int GetBarrierArmor(int level);
    }
}
