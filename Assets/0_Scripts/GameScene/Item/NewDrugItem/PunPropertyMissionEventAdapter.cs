using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class PunPropertyMissionEventAdapter : MonoBehaviourPunCallbacks
{
    private int prevTurnIndex;
    private int prevWaveIndex;
    private float prevTreeHP;
    private Dictionary<int, float> prevDefenseByActor = new Dictionary<int, float>();
    private Dictionary<int, int> prevEnergyByActor = new Dictionary<int, int>();

    private bool initialized;

    private void InitFromRoomProperties()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

        prevTurnIndex = (int)GetInfo(props, RoomPropKeys.CurrentTurn);
        prevWaveIndex = (int)GetInfo(props, RoomPropKeys.CurrentWave);
        prevTreeHP = (float)GetInfo(props, RoomPropKeys.TreeHP);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (!initialized)
            InitFromRoomProperties();

        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;

        HandleTreeHPChanged(propertiesThatChanged);

        HandleTurnChanged(propertiesThatChanged);
        HandleWaveChanged(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    private void HandleTurnChanged(Hashtable changedProps)
    {
        if (!changedProps.ContainsKey(RoomPropKeys.CurrentTurn)) return;
        int newTurnIndex = (int)changedProps[RoomPropKeys.CurrentTurn];

        if (newTurnIndex == prevTurnIndex) return;

        int oldTurnIndex = prevTurnIndex;

        SendEvent(new NewDrugGameEvent
        {
            Type = NewDrugGameEventType.TurnEnded,
            TurnIndex = oldTurnIndex,
        });

        prevTurnIndex = newTurnIndex;

        SendEvent(new NewDrugGameEvent
        {
            Type = NewDrugGameEventType.TurnStarted,
            TurnIndex = newTurnIndex,
        });
    }

    private void HandleWaveChanged(Hashtable changedProps)
    {
        if (!changedProps.ContainsKey(RoomPropKeys.CurrentWave)) return;
        int newWaveIndex = (int)changedProps[RoomPropKeys.CurrentWave];

        if (newWaveIndex == prevWaveIndex) return;

        int oldWaveIndex = prevWaveIndex;

        SendEvent(new NewDrugGameEvent
        {
            Type = NewDrugGameEventType.WaveEnded,
            WaveIndex = oldWaveIndex,
        });

        prevWaveIndex = newWaveIndex;

        SendEvent(new NewDrugGameEvent
        {
            Type = NewDrugGameEventType.WaveStarted,
            WaveIndex = newWaveIndex,
        });
    }

    private void HandleTreeHPChanged(Hashtable changedProps)
    {
        if (!changedProps.ContainsKey(RoomPropKeys.TreeHP)) return;

        float newTreeHP = (float)changedProps[RoomPropKeys.TreeHP];
        float damage = prevTreeHP - newTreeHP;

    }

    private void SendEvent(NewDrugGameEvent gameEvent)
    {
        if (NewDrugMissionManager.instance == null) return;

        NewDrugMissionManager.instance.ReceiveGameEvent(gameEvent);
    }

    private object GetInfo(Hashtable props, string key)
    {
        if (props == null) return -1;
        return props[key];
    }
}
