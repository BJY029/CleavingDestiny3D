using Photon.Pun;
using System;
using UnityEngine;

public class PhotonHelper
{
    public static PLAYER GetMyRole()
    {
        if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object roleObj))
        {
            if(Enum.TryParse(typeof(PLAYER), roleObj.ToString(), out object roleEnum))
            {
                return (PLAYER)roleEnum;
            }
        }
        return PLAYER.NONE;
    }
}
