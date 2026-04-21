using Photon.Pun;
using Photon.Realtime;
using PrimeTween;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Village.Building;


namespace Village.Outside
{
    public class VillageOutsideManager : MonoBehaviour
    {
        [SerializeField] VillageBuilding compassBuilding;
        [SerializeField] VillageBuilding villageBuilding;
        [SerializeField] CinemachineCamera insideCam;
        [SerializeField] CinemachineCamera outsideCam;
        [SerializeField] GameObject villageInsideObject;

        [SerializeField] GameObject[] outsideObjectsToEnable;

        [SerializeField] VillageHpBar villageHpBar;
        [SerializeField] GameObject outsideUI;

        [SerializeField] ReadyChecker readyChecker;

        private const float insideSizeOrigin = 5.4f;    // 마을 원래 사이즈
        private const float outsideSizeOrigin = 10f;  // 지도 전체 사이즈 (프레임 보임)
        private const float outsideSizeZoomed = 8f;     // 지도 확대 사이즈 (프레임 안 보임)

        private CinemachineBrain brain;

        [SerializeField] Button readyButton;
        LocalizedText readyButtonText;
        private VillageSceneManager villageSceneManager;
        bool isReady = false;

        const string readyText = "UI_Ready";
        const string notReadyText = "UI_NotReady";

        void Start()
        {
            // 메인 카메라에서 브레인 가져오기
            brain = CinemachineBrain.GetActiveBrain(0);

            outsideUI.SetActive(false);
            villageSceneManager = FindFirstObjectByType<VillageSceneManager>();

            compassBuilding.OnVillageClicked += CompassBuildingClicked;
            villageBuilding.OnVillageClicked += VillageBuildingClicked;

            if (villageSceneManager != null)
            {
                villageSceneManager.OnPlayerReadyListUpdated += UpdateReadyChecker;
            }

            InitializeReadyChecker();

            readyButton.onClick.AddListener(ToggleReadyButton);
            readyButtonText = readyButton.GetComponentInChildren<LocalizedText>();

            outsideCam.gameObject.SetActive(false);
        }

        void CompassBuildingClicked(VillageBuilding building) => _ = GotoOutside();
        void VillageBuildingClicked(VillageBuilding building) => _ = ReturnToVillage();


        // 이벤트 구독 해제
        private void OnDestroy()
        {
            compassBuilding.OnVillageClicked -= CompassBuildingClicked;
            villageBuilding.OnVillageClicked -= VillageBuildingClicked;

            if (villageSceneManager != null)
            {
                villageSceneManager.OnPlayerReadyListUpdated -= UpdateReadyChecker;
            }
        }

        // ReadyChecker 초기화 및 최초 상태 반영
        private void InitializeReadyChecker()
        {
            if (readyChecker == null) return;

            // 현재 방의 플레이어 수만큼 슬롯 생성
            Player[] players = PhotonNetwork.PlayerList;
            readyChecker.Initialize(players.Length);

            UpdateReadyChecker();
        }

        // 모든 플레이어의 준비 상태를 확인하여 UI 갱신
        private void UpdateReadyChecker()
        {
            if (readyChecker == null) return;

            Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                Player p = players[i];
                bool isPlayerReady = false;

                if (p.CustomProperties.TryGetValue(PlayerPropKeys.PlayerVillageReady, out object isReadyObj))
                {
                    isPlayerReady = (bool)isReadyObj;
                }

                readyChecker.SetPlayerReady(i, isPlayerReady);
            }
        }

        public void RemoveHP()
        {
            float currentHp = PlayerStatus.Instance.GetCurrentVillageHP();
            PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageHP, currentHp - 500f);
        }

        public async Awaitable GotoOutside()
        {
            // 줌아웃 시동
            _ = insideCam.TweenOrthoSize(insideSizeOrigin * 1.2f, 1.0f, Ease.OutQuad);

            await FadeCanvas.Instance.FadeInAsync(0.4f);

            // 2. 오브젝트/카메라 교체 + 강제 컷
            villageInsideObject.SetActive(false);

            foreach (var obj in outsideObjectsToEnable)
            {
                obj.SetActive(true);
            }


            // 즉시 전환 (블렌드 스킵)
            await CutToCamera(outsideCam);

            // 3. 바깥 카메라 초기값 설정 (이미 Cut 되었으므로 즉시 적용됨)
            outsideCam.Lens.OrthographicSize = outsideSizeZoomed;

            outsideUI.SetActive(true);
            UpdateVillageHpBar();

            // 4. 즉시 밝아짐 (딜레이 삭제됨)
            var fadeTask = FadeCanvas.Instance.FadeOutAsync(0.5f);

            // 5. [연출] Reveal 효과
            _ = outsideCam.TweenOrthoSize(outsideSizeOrigin, 0.8f, Ease.OutCubic);

            await fadeTask;
        }

        public async Awaitable ReturnToVillage()
        {
            // 빨려들어가는 느낌
            _ = outsideCam.TweenOrthoSize(outsideSizeZoomed, 0.5f, Ease.InCubic);

            await FadeCanvas.Instance.FadeInAsync(0.5f, endDelay: 0.1f);

            outsideUI.SetActive(false);

            // 2. 오브젝트/카메라 교체 + 강제 컷(Cut)
            outsideCam.gameObject.SetActive(false);
            villageInsideObject.SetActive(true);

            foreach (var obj in outsideObjectsToEnable)
            {
                obj.SetActive(false);
            }

            // 즉시 전환 (블렌드 스킵)
            await CutToCamera(insideCam);

            // 내부 카메라 초기값 설정 (이미 Cut 되었으므로 튀지 않음)
            insideCam.Lens.OrthographicSize = insideSizeOrigin * 1.3f;

            // 즉시 밝아짐 (딜레이 삭제됨)
            var fadeTask = FadeCanvas.Instance.FadeOutAsync(0.5f);

            // 착륙
            _ = insideCam.TweenOrthoSize(insideSizeOrigin, 0.8f, Ease.OutCubic);

            await fadeTask;
        }

        /// <summary>
        /// 시네머신 브레인의 블렌드 시간을 잠시 0으로 만들어 'Cut' 연출을 강제함
        /// </summary>
        private async Awaitable CutToCamera(CinemachineCamera targetCam)
        {
            float originalTime = 0f;
            bool brainExists = brain != null;

            if (brainExists)
            {
                // 현재 설정된 기본 블렌드 시간을 저장하고 0으로 변경 (구조체 복사 방식 주의)
                var blendDef = brain.DefaultBlend;
                originalTime = blendDef.Time;

                blendDef.Time = 0f;
                brain.DefaultBlend = blendDef;
            }

            // 타겟 카메라 활성화
            targetCam.gameObject.SetActive(true);

            // 한 프레임을 대기하여 시네머신이 변경된 '0초 블렌드'를 감지하고 즉시 이동하도록 함
            await Awaitable.NextFrameAsync();

            if (brainExists)
            {
                // 블렌드 시간 원상복구
                var blendDef = brain.DefaultBlend;
                blendDef.Time = originalTime;
                brain.DefaultBlend = blendDef;
            }
        }

        void UpdateVillageHpBar()
        {
            // 마을 체력바 갱신
            float currentHp = PlayerStatus.Instance.GetCurrentVillageHP();
            float maxHp = PlayerStatus.Instance.GetMaxVillageHp();
            float shield = PlayerStatus.Instance.GetCurrentBarrier();

            float damage = PlayerStatus.Instance.GetExpectedVillageDamage(TreeStatus.Instance.getTreeAtkPow());

            villageHpBar.UpdateStats(currentHp, maxHp, shield, damage);
        }

        void ToggleReadyButton()
        {
            isReady = !isReady;
            readyButtonText.TextID = isReady ? readyText : notReadyText;
            villageSceneManager.SetLocalPlayerReady(isReady);
        }
    }
}