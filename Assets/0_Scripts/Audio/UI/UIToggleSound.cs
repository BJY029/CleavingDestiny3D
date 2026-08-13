using UnityEngine;

public class UIToggleSound : MonoBehaviour
{
    [SerializeField] private string audioId = "UI_Toggle";
    public void PlayUIToggleSound()
    {
        AudioManager.Instance.PlaySfx2D(audioId);
    }
}
