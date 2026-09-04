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

        [Header("Outside Transition Timings (Seconds)")]
        [Tooltip("화면 스위칭 순간의 초고속 페이드 시간 (물리적 오브젝트 겹침 방지)")]
        [SerializeField] private float dipFadeDuration = 0.15f;
        [Tooltip("외곽 지도가 펼쳐지는 줌아웃 연출 시간")]
        [SerializeField] private float outsideRevealDuration = 0.35f;
        [Tooltip("외곽에서 마을 아이콘으로 파고드는 줌인 시간")]
        [SerializeField] private float villageDiveDuration = 0.2f;
        [Tooltip("마을로 착륙하는 줌인 시간")]
        [SerializeField] private float villageLandingDuration = 0.3f;

        [Header("Camera Sizes")]
        [SerializeField] private float insideSizeOrigin = 5.4f;    // 마을 원래 사이즈
        [SerializeField] private float outsideSizeOrigin = 10f;    // 지도 전체 사이즈 (프레임 보임)
        [SerializeField] private float outsideSizeZoomed = 8f;     // 지도 확대 사이즈 (프레임 안 보임)
        [SerializeField] private float outsideSizeDive = 7.2f;     // 지도 상 마을로 파고들 때 사이즈

        private CinemachineBrain brain;
        private bool _isTransitioning = false;
        public bool IsOutsideActive { get; private set; } = false;
        private System.Action _returnToVillageAction;
        [SerializeField] private VillageBuildingManager villageBuildingManager;
        [SerializeField] private Button readyButton;
        LocalizedText readyButtonText;
        private VillageSceneManager villageSceneManager;
        bool isReady = false;

        const string readyText = "UI_Ready";
        const string notReadyText = "UI_NotReady";

        private void Awake()
        {
            _returnToVillageAction = ReturnToVillageFromEsc;
        }

        private void OnEnable()
        {
            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnInteractSpaceKeyDown -= HandleSpaceKey;
                KeyInteractManager.Instance.OnInteractSpaceKeyDown += HandleSpaceKey;
            }
        }

        private void ReturnToVillageFromEsc()
        {
            _ = ReturnToVillage();
        }

        private void OnDisable()
        {
            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnInteractSpaceKeyDown -= HandleSpaceKey;
                KeyInteractManager.Instance.RemoveMenuAction(_returnToVillageAction);
            }
        }

        void Start()
        {
            // 메인 카메라에서 브레인 가져오기
            brain = CinemachineBrain.GetActiveBrain(0);

            outsideUI.SetActive(false);
            villageSceneManager = FindFirstObjectByType<VillageSceneManager>();
            if (villageBuildingManager == null)
            {
                villageBuildingManager = FindFirstObjectByType<VillageBuildingManager>();
            }

            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnInteractSpaceKeyDown -= HandleSpaceKey;
                KeyInteractManager.Instance.OnInteractSpaceKeyDown += HandleSpaceKey;
            }

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
            if (KeyInteractManager.Instance != null)
            {
                KeyInteractManager.Instance.OnInteractSpaceKeyDown -= HandleSpaceKey;
            }

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
            readyChecker.Initialize(PlayerManager.Instance.TotalPlayerCount);

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

            // 남은 칸 수는 AI로 간주하여 항상 준비된 상태로 표시 (빈 슬롯이 있을 경우)
            for (int i = players.Length; i < readyChecker.PlayerCount; i++)
            {
                readyChecker.SetPlayerReady(i, true);
            }
        }

        public void RemoveHP()
        {
            float currentHp = PlayerStatus.Instance.GetCurrentVillageHP();
            PhotonPropertyHelper.SetPlayerProp(PhotonNetwork.LocalPlayer.ActorNumber, PlayerPropKeys.VillageHP, currentHp - 500f);
        }

        public async Awaitable GotoOutside()
        {
            if (_isTransitioning || IsOutsideActive) return;
            _isTransitioning = true;

            try
            {
                // 1. 마을 카메라가 뒤로 살짝 물러나는 전조 모션 (Zoom-out)
                _ = insideCam.TweenOrthoSize(insideSizeOrigin * 1.15f, dipFadeDuration, Ease.OutQuad);

                // 2. 초고속 디졸브 페이드인 (0.15초)
                await FadeCanvas.Instance.FadeInAsync(dipFadeDuration);

                // 3. 오브젝트 및 카메라 스위칭
                villageInsideObject.SetActive(false);
                foreach (var obj in outsideObjectsToEnable)
                {
                    obj.SetActive(true);
                }

                await CutToCamera(outsideCam);
                outsideCam.Lens.OrthographicSize = outsideSizeZoomed;

                outsideUI.SetActive(true);
                UpdateVillageHpBar();

                // 4. 초고속 페이드아웃과 동시에 지도가 시원하게 펼쳐지는 줌아웃 연출
                var fadeTask = FadeCanvas.Instance.FadeOutAsync(dipFadeDuration);
                _ = outsideCam.TweenOrthoSize(outsideSizeOrigin, outsideRevealDuration, Ease.OutCubic);

                await fadeTask;
                IsOutsideActive = true;
                KeyInteractManager.Instance?.PushMenuAction(_returnToVillageAction);
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        public async Awaitable ReturnToVillage()
        {
            if (_isTransitioning || !IsOutsideActive) return;
            _isTransitioning = true;
            IsOutsideActive = false;
            KeyInteractManager.Instance?.RemoveMenuAction(_returnToVillageAction);

            try
            {
                // 1. 지도 상의 마을 그림을 향해 스우욱 파고드는 줌인 연출
                _ = outsideCam.TweenOrthoSize(outsideSizeDive, villageDiveDuration, Ease.InCubic);

                // 2. 초고속 디졸브 페이드인 (0.15초)
                await FadeCanvas.Instance.FadeInAsync(dipFadeDuration, delay: villageDiveDuration * 0.4f);

                outsideUI.SetActive(false);
                outsideCam.gameObject.SetActive(false);
                villageInsideObject.SetActive(true);

                foreach (var obj in outsideObjectsToEnable)
                {
                    obj.SetActive(false);
                }

                // 3. 마을 카메라로 즉시 컷 및 착륙 초기값 설정
                await CutToCamera(insideCam);
                insideCam.Lens.OrthographicSize = insideSizeOrigin * 1.18f;

                // 4. 초고속 페이드아웃과 동시에 마을 전경으로 부드럽게 착륙
                var fadeTask = FadeCanvas.Instance.FadeOutAsync(dipFadeDuration);
                _ = insideCam.TweenOrthoSize(insideSizeOrigin, villageLandingDuration, Ease.OutCubic);

                await fadeTask;
            }
            finally
            {
                _isTransitioning = false;
            }
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

            //float damage = PlayerStatus.Instance.GetExpectedVillageDamage(TreeStatus.Instance.getTreeAtkPow());
            float damage = PlayerStatus.Instance.GetExpetedTreePoison(TreeStatus.Instance.getTreeAtkPow());

            villageHpBar.UpdateStats(currentHp, maxHp, shield, damage);
        }

        void ToggleReadyButton()
        {
            isReady = !isReady;
            readyButtonText.TextID = isReady ? readyText : notReadyText;
            villageSceneManager.SetPlayerReady(PhotonNetwork.LocalPlayer.ActorNumber, isReady);
        }

        private void HandleSpaceKey()
        {
            if (_isTransitioning) return;

            // 1. 이미 외곽 뷰인 경우: Space로 준비 토글
            if (IsOutsideActive)
            {
                ToggleReadyButton();
                return;
            }

            // 2. 마을 현황(Tab UI) 등 풀스크린 메뉴가 열려있으면 무시
            var statusUI = FindFirstObjectByType<VillageStatusUI>();
            if (statusUI != null && statusUI.IsOpen) return;

            // 3. 건물 UI가 열려있는 경우: 건물 닫고 외곽으로 전환
            if (villageBuildingManager != null && villageBuildingManager.IsBuildingOpen)
            {
                _ = SwitchFromBuildingToOutside();
                return;
            }

            // 4. 마을 전경인 경우: 외곽으로 나가기
            _ = GotoOutside();
        }

        private async Awaitable SwitchFromBuildingToOutside()
        {
            if (villageBuildingManager != null && villageBuildingManager.IsBuildingOpen)
            {
                villageBuildingManager.ExitBuilding();
                while (villageBuildingManager.IsBuildingOpen)
                {
                    await Awaitable.NextFrameAsync();
                }
            }
            await GotoOutside();
        }
    }
}