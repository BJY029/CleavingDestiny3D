using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Village.Outside
{

    public class ReadyChecker : MonoBehaviour
    {
        [SerializeField] Image readyImagePrefab;
        [SerializeField] RectTransform playerReadyContainer;
        Image[] playerReadyImages;

        public void Initialize(int playerCount)
        {
            playerReadyImages = new Image[playerCount];

            for (int i = 0; i < playerCount; i++)
            {
                Image readyImage = Instantiate(readyImagePrefab, playerReadyContainer);
                readyImage.color = Color.black; // 초기 상태는 준비 안됨
                readyImage.gameObject.SetActive(true);
                playerReadyImages[i] = readyImage;
            }
        }

        public void SetPlayerReady(int playerIndex, bool isReady)
        {
            if (playerReadyImages == null || playerIndex < 0 || playerIndex >= playerReadyImages.Length)
                return;

            playerReadyImages[playerIndex].color = isReady ? Color.green : Color.black;
        }

    }
}