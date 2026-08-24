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
    public event Func<bool> OnMenuKeyDown;
    
    private bool _isPlayerActionsEnabled = true;
    private bool _isMenuInputEnabled = true;

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

    // ESC 키를 누른 경우
    private void HandleMenu(InputAction.CallbackContext context)
    {
        if (!_isMenuInputEnabled) return;
        if (OnMenuKeyDown?.Invoke() == true) return;

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

        // 액션 맵 비활성화
        _inputActions.Player.Disable();
        _inputActions.UI.Disable();
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
