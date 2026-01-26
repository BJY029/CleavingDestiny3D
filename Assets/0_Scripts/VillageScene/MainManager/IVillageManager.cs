using UnityEngine.Events;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

namespace Village
{
    public interface IVillageManager
    {
        /// <summary>
        /// 보유 골드량이 변경될 때 호출되는 이벤트 (변경된 현재 골드량)
        /// </summary>
        event Action<int> OnGoldChanged;

        /// <summary>
        /// 매니저 상태 초기화. 씬 로드 시 호출되어 데이터 제공자를 연결하고 상태를 리셋함.
        /// </summary>
        void Initialize(IVillageStatProvider statProvider);

        /// <summary>
        /// 현재 로컬 플레이어의 보유 골드량 반환
        /// </summary>
        /// <returns>현재 골드</returns>
        int GetMyGold();

        /// <summary>
        /// 골드 획득 또는 차감 (Photon CustomProperties 동기화 포함)
        /// </summary>
        /// <param name="amount">변경할 골드 양 (음수일 경우 차감)</param>
        void AddGold(int amount);

        /// <summary>
        /// 특정 건물 업그레이드 시도 (비용 확인 -> 골드 차감 -> 레벨 상승 -> 스탯 갱신)
        /// </summary>
        /// <param name="facilityType">건물 유형</param>
        /// <returns>업그레이드 성공 여부</returns>
        bool TryUpgradeLevel(VillageType facilityType);

        /// <summary>
        /// Photon 네트워크로부터 변경된 플레이어 속성(골드 등)을 로컬 로직에 동기화
        /// </summary>
        /// <param name="targetPlayer">변경된 속성의 주인 플레이어</param>
        /// <param name="changedProps">변경된 속성 해시테이블</param>
        void SyncFromPhoton(Player targetPlayer, Hashtable changedProps);
    }
}