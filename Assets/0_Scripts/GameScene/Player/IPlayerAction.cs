using UnityEngine;

//Player Action Interface
public interface IPlayerAction
{
    /// <summary>
    /// Player identification number(AI : 1000)
    /// </summary>
    public int PlayerActNum { get; set; }

    /// <summary>
    /// Village phase entry processing function
    /// </summary>
    void VillageUpgradePhase();

    /// <summary>
    /// Village phase end processing function
    /// </summary>
    void VillageUpgradePhaseOut();

    /// <summary>
    /// Turn pass attempt function
    /// </summary>
    /// <param name="IsItRandom"></param>
    void TryHit(bool IsItRandom = false);

    /// <summary>
    /// Tree hitting animation output function
    /// </summary>
    void PlayHit();

    /// <summary>
    /// Inventory key settings
    /// </summary>
    /// <param name="Num"></param>

    void SetInvAdmissionTicket(int Num);


    /// <summary>
    /// Get inventory key
    /// </summary>
    /// <returns>inventory key num</returns>
    int GetInvAdmissionticket();

}
