using UnityEngine;
using Photon.Pun;

public class PunLocalWeatherSetup : MonoBehaviourPun
{
    [Header("로컬 플레이어 전용")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;

    private void Awake()
    {
        bool isLocal = photonView.IsMine;

        if (playerCamera != null)
        {
            playerCamera.enabled = isLocal;
        }

        if (audioListener != null)
        {
            audioListener.enabled = isLocal;
        }
    }
}
