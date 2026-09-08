using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using Photon.Pun;
using Potan.CoreUtils;

public class KeyInteractManager : MonoSceneSingleton<KeyInteractManager>
{
    private InputSystem_Actions _inputActions;

    public event Action<Vector2> OnMoveInput; // 이동 입력 이벤트
    public event Action<Vector2> OnMousePositionInput; // 마우스 위치 입력 이벤트
    public event Action<bool> OnRunInput; // 달리기 입력 이벤트 (true: 달리기 시작, false: 달리기 종료)
    public event Action OnInteractKeyDown;
    public event Action OnInteractKeyUp;
    public event Action OnInteractSpaceKeyDown;
    public event Action OnTabKeyDown;
    public event Action<int> OnQuickSlotKeyDown; // 1~5 퀵슬롯/건물 단축키 (리바인딩 지원)
    public event Func<bool> OnMenuKeyDown;
    
    private bool _isPlayerActionsEnabled = true;
    private bool _isMenuInputEnabled = true;
    private bool _isQuickSlotEnabled = true;

    protected override void Awake()
    {
        base.Awake();

        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        if (_inputActions == null) return;

        _inputActions.Player.Move.performed += HandleMove;
        _inputActions.Player.Move.canceled += HandleMove;
        _inputActions.Player.Look.performed += HandleMousePosition;
        _inputActions.Player.Look.canceled += HandleMousePosition;
        _inputActions.Player.Interact.started += HandleInteract; // Interact 액션이 시작될 때 (키 누름)
        _inputActions.Player.Interact.canceled += HandleInteract; // Interact 액션이 취소될 때 (키 뗌)
        _inputActions.Player.Jump.performed += HandleInteractSpace; // Jump 액션을 미니게임 키로 사용
        _inputActions.Player.Tab.performed += HandleTab;
        _inputActions.UI.Menu.performed += HandleMenu; // UI 메뉴 액션
        _inputActions.Player.Sprint.performed += HandleSprint; // 달리기 시작
        _inputActions.Player.Sprint.canceled += HandleSprint; // 달리기 종료
        _inputActions.Player.QuickSlot1.started += HandleQuickSlot1;
        _inputActions.Player.QuickSlot1.performed += HandleQuickSlot1;
        _inputActions.Player.QuickSlot2.started += HandleQuickSlot2;
        _inputActions.Player.QuickSlot2.performed += HandleQuickSlot2;
        _inputActions.Player.QuickSlot3.started += HandleQuickSlot3;
        _inputActions.Player.QuickSlot3.performed += HandleQuickSlot3;
        _inputActions.Player.QuickSlot4.started += HandleQuickSlot4;
        _inputActions.Player.QuickSlot4.performed += HandleQuickSlot4;
        _inputActions.Player.QuickSlot5.started += HandleQuickSlot5;
        _inputActions.Player.QuickSlot5.performed += HandleQuickSlot5;

        // 액션 맵 활성화
        _inputActions.Player.Enable();
        _inputActions.UI.Enable();
    }

    private void HandleSprint(InputAction.CallbackContext context)
    {
        if (!_isPlayerActionsEnabled) return; // 플레이어 액션이 비활성화된 경우 무시
        OnRunInput?.Invoke(context.performed);
    }

    private void HandleMousePosition(InputAction.CallbackContext context)
    {
        if (!_isPlayerActionsEnabled) return; // 플레이어 액션이 비활성화된 경우 무시
        
        Vector2 mousePosition = context.ReadValue<Vector2>();
        OnMousePositionInput?.Invoke(mousePosition);
    }

    private readonly System.Collections.Generic.List<Action> _menuActionStack = new();

    /// <summary>
    /// ESC 키를 눌렀을 때 실행될 닫기/취소 Action을 스택 최상단에 추가합니다.
    /// 중복 추가를 방지하기 위해 이미 포함되어 있다면 맨 위로 끌어올립니다.
    /// </summary>
    public void PushMenuAction(Action action)
    {
        if (action == null) return;
        _menuActionStack.Remove(action);
        _menuActionStack.Add(action);
    }

    /// <summary>
    /// 버튼 클릭 등으로 메뉴가 먼저 닫혔을 때 스택에서 해당 Action을 안전하게 제거합니다.
    /// </summary>
    public void RemoveMenuAction(Action action)
    {
        if (action == null) return;
        _menuActionStack.Remove(action);
    }

    /// <summary>
    /// 스택의 최상단 Action을 꺼내어 실행합니다. 실행된 Action이 있다면 true를 반환합니다.
    /// </summary>
    public bool PopAndExecuteMenuAction()
    {
        while (_menuActionStack.Count > 0)
        {
            int lastIndex = _menuActionStack.Count - 1;
            Action action = _menuActionStack[lastIndex];
            _menuActionStack.RemoveAt(lastIndex);

            if (action != null)
            {
                action.Invoke();
                return true;
            }
        }
        return false;
    }

    // ESC 키를 누른 경우
    private void HandleMenu(InputAction.CallbackContext context)
    {
        if (!_isMenuInputEnabled) return;

        // 1. ESC 스택에 등록된 메뉴 닫기 액션 우선 실행
        if (PopAndExecuteMenuAction()) return;

        // 2. 레거시 OnMenuKeyDown 이벤트 호환
        if (OnMenuKeyDown != null)
        {
            var invocationList = OnMenuKeyDown.GetInvocationList();
            for (int i = invocationList.Length - 1; i >= 0; i--)
            {
                var handler = (Func<bool>)invocationList[i];
                if (handler.Invoke()) return;
            }
        }

        // 3. 닫을 창이 없으면 환경설정창 토글
        SettingCanvasController.instance?.ToggleSettingPanel();
    }

    private void HandleTab(InputAction.CallbackContext context)
    {
        OnTabKeyDown?.Invoke();
    }

    // 점프 키를 누른 경우 (미니게임 인터랙트)
    private void HandleInteractSpace(InputAction.CallbackContext context)
    {
        OnInteractSpaceKeyDown?.Invoke();
    }

    // F 키(기본 인터랙트 키)를 누른 경우
    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnInteractKeyDown?.Invoke();
        }
        else if (context.canceled)
        {
            OnInteractKeyUp?.Invoke();
        }
    }

    // 이동 입력
    private void HandleMove(InputAction.CallbackContext context)
    {
        if (!_isPlayerActionsEnabled) return; // 플레이어 액션이 비활성화된 경우 무시
        
        Vector2 inputVector = context.ReadValue<Vector2>();
        OnMoveInput?.Invoke(inputVector);
    }

    private void OnDisable()
    {
        if (_inputActions == null) return;

        // 메모리 누수 방지용 구독 해제
        _inputActions.Player.Move.performed -= HandleMove;
        _inputActions.Player.Move.canceled -= HandleMove;
        _inputActions.Player.Look.performed -= HandleMousePosition;
        _inputActions.Player.Look.canceled -= HandleMousePosition;
        _inputActions.Player.Interact.started -= HandleInteract;
        _inputActions.Player.Interact.canceled -= HandleInteract;
        _inputActions.Player.Jump.performed -= HandleInteractSpace;
        _inputActions.Player.Tab.performed -= HandleTab;
        _inputActions.UI.Menu.performed -= HandleMenu;
        _inputActions.Player.Sprint.performed -= HandleSprint;
        _inputActions.Player.Sprint.canceled -= HandleSprint;
        _inputActions.Player.QuickSlot1.started -= HandleQuickSlot1;
        _inputActions.Player.QuickSlot1.performed -= HandleQuickSlot1;
        _inputActions.Player.QuickSlot2.started -= HandleQuickSlot2;
        _inputActions.Player.QuickSlot2.performed -= HandleQuickSlot2;
        _inputActions.Player.QuickSlot3.started -= HandleQuickSlot3;
        _inputActions.Player.QuickSlot3.performed -= HandleQuickSlot3;
        _inputActions.Player.QuickSlot4.started -= HandleQuickSlot4;
        _inputActions.Player.QuickSlot4.performed -= HandleQuickSlot4;
        _inputActions.Player.QuickSlot5.started -= HandleQuickSlot5;
        _inputActions.Player.QuickSlot5.performed -= HandleQuickSlot5;

        // 액션 맵 비활성화
        _inputActions.Player.Disable();
        _inputActions.UI.Disable();
    }

    private void HandleQuickSlot1(InputAction.CallbackContext context) => TriggerQuickSlot(1);
    private void HandleQuickSlot2(InputAction.CallbackContext context) => TriggerQuickSlot(2);
    private void HandleQuickSlot3(InputAction.CallbackContext context) => TriggerQuickSlot(3);
    private void HandleQuickSlot4(InputAction.CallbackContext context) => TriggerQuickSlot(4);
    private void HandleQuickSlot5(InputAction.CallbackContext context) => TriggerQuickSlot(5);

    private int _lastQuickSlotFrame = -1;
    private int _lastQuickSlotIndex = -1;

    private void TriggerQuickSlot(int slotIndex)
    {
        if (!_isQuickSlotEnabled) return;

        if (Time.frameCount == _lastQuickSlotFrame && _lastQuickSlotIndex == slotIndex) return;
        _lastQuickSlotFrame = Time.frameCount;
        _lastQuickSlotIndex = slotIndex;

        OnQuickSlotKeyDown?.Invoke(slotIndex);
    }

    public void SetQuickSlotEnabled(bool enabled)
    {
        _isQuickSlotEnabled = enabled;
    }

    public void SetPlayerActionsEnabled(bool enabled)
    {
        if (_inputActions == null) return;

        if (enabled)
        {
            _isPlayerActionsEnabled = true;
        }
        else
        {
            _isPlayerActionsEnabled = false;
            
            // 해제 시 잔여 입력에 대한 안전 이벤트 강제 클리어 전송
            OnMoveInput?.Invoke(Vector2.zero);
            OnMousePositionInput?.Invoke(Vector2.zero);
            OnRunInput?.Invoke(false);
        }
    }

    public void SetMenuInputEnabled(bool enabled)
    {
        _isMenuInputEnabled = enabled;
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        // if (Keyboard.current == null) return;

        // if (Keyboard.current.escapeKey.wasPressedThisFrame)
        // {
        //     SettingCanvasController.instance.ToggleSettingPanel();
        // }

        // //만약 'F'키가 눌린 경우
        // if (Keyboard.current.fKey.wasPressedThisFrame)
        // {
        //     //'F'키 이벤트 실행
        //     //관련 이벤트는 PlayerController.cs에서 처리(HandleInteractFKey())
        //     OnInteractKeyDown?.Invoke();
        // }

        // if (Keyboard.current.spaceKey.wasPressedThisFrame)
        // {
        //     OnInteractSpaceKeyDown?.Invoke();
        // }

        // K키가 눌리고 마스터 클라이언트이며 아직 마을 페이즈가 아닌 경우 강제 시작 - 디버그용
        if (Keyboard.current.kKey.wasPressedThisFrame && !TurnManager.Instance.isUpgradePhase)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            TurnManager.Instance.StartVillageUpgradePhase();
        }
    }
#endif
}
