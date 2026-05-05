using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Village
{
    public class VillageSystem : MonoBehaviourPunCallbacks
    {
        public static VillageSystem Instance { get; private set; }
        public static IVillageManager VillageLogic { get; private set; }
        public static IVillageStatProvider VillageStat => VillageStatManager.Instance;

        [SerializeField] private VillageUIManager _uiManager;
        public VillageUIManager UIManager => _uiManager;

        private void Awake()
        {
            if (Instance == null || Instance == this) Instance = this;
            else
            {
                Destroy(gameObject);
                return; // 중복 인스턴스라면 초기화 로직을 수행하지 않고 종료
            }

            // 이미 생성된 로직 인스턴스가 있다면 재사용
            VillageLogic ??= new VillageManager();

            // 씬 로드 시마다 데이터 프로바이더 주입 및 데이터 초기화 (Reset 개념)
            VillageLogic.Initialize(VillageStat);

            _uiManager.Init();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            VillageLogic.SyncFromPhoton(targetPlayer, changedProps);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}