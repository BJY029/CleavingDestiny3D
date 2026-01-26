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
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // 로직 클래스 생성 시 데이터 프로바이더(StatManager)를 주입
            VillageLogic = new VillageManager(VillageStatManager.Instance);

            _uiManager.Init();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            VillageLogic.SyncFromPhoton(targetPlayer, changedProps);
        }
    }
}