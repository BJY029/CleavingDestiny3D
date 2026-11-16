using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    private IPlayerAction p;
    private PlayerController pc;
    private AIController ac;
    private PlayerEffectPoints ep;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        p = GetComponent<IPlayerAction>();
        ac = p as AIController;
        pc = p as PlayerController;
        ep = ac != null ? ac.GetComponent<PlayerEffectPoints>() : pc.GetComponent<PlayerEffectPoints>();
    }

    public void PlayFootstep()
    {
        if (ep == null) Init();
        AudioManager.Instance.PlaySfx3D("PlayFootsteps", ep.Foot.position);
    }
}
