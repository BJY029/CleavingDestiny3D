using UnityEngine;

public class UIClickSound : MonoBehaviour
{
    [SerializeField] private string audioId = "UI_Click";
    public void PlayUIClickSound()
    {
        AudioManager.Instance.PlaySfx2D(audioId);
    }
}
