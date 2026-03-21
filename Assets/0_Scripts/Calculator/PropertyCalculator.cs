using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PropertyCalculator : MonoBehaviour
{
    public static PropertyCalculator Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    //ActorNumber 플레이어의 마을에 가해지는 마을 독성 데미지
    //AI Player의 ActorNumber는 PlayerManager의 AIActNum 통해 얻어올 수 있음
    public float GetFinalTreeToxicDamage(int ActorNumber)
    {
        //원본 마을 독성 데미지
        float originTreeToxicDmg = PhotonPropertyHelper.GetRoomProp<float>(RoomPropKeys.TreeAtkPow);
        //플레이어 고유 곱셈기, 마을 독성 데미지에 곱해진다.
        float playerMultiplier = PhotonPropertyHelper.GetPlayerProp<float>(ActorNumber, PlayerPropKeys.TreeAtkMulti);

        //실제 플레이어가 받는 독성 데미지
        return originTreeToxicDmg * playerMultiplier;
    }
}
