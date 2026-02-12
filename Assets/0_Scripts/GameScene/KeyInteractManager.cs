using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using Photon.Pun;

public class KeyInteractManager : MonoBehaviour
{
    public static KeyInteractManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    //F키가 눌리면 발생될 이벤트
    public event Action OnInteractFKeyDown;
    public event Action OnInteractSpaceKeyDown;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SettingCanvasController.instance.ToggleSettingPanel();
        }

        //만약 'F'키가 눌린 경우
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            //'F'키 이벤트 실행
            //관련 이벤트는 PlayerController.cs에서 처리(HandleInteractFKey())
            OnInteractFKeyDown?.Invoke();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnInteractSpaceKeyDown?.Invoke();
        }

        // K키가 눌리고 마스터 클라이언트이며 아직 마을 페이즈가 아닌 경우 강제 시작
        if (Keyboard.current.kKey.wasPressedThisFrame && !TurnManager.Instance.isUpgradePhase)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            TurnManager.Instance.StartVillageUpgradePhase();
        }
    }
}
