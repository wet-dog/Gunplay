using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class UINetwork : NetworkBehaviour
{
    // todo Refactor to serialize fields
    public Text TicksText;
    public Text RemoteTicksText;
    public Text RemoteTickDeltaText;

    private int m_Ticks;
    private int m_RemoteTicks;
    private int m_RemoteTickDelta;

    private Rollback m_Rollback;

    public void Initialize()
    {
        if (m_Rollback != null) return;

        var playerGameObject = NetworkManager.Singleton.LocalClient.PlayerObject;
        Assert.IsNotNull(playerGameObject, "playerGameObject is null");
        
        m_Rollback = playerGameObject.GetComponent<Rollback>();
        Assert.IsNotNull(m_Rollback, "m_Rollback is null");

        NetworkManager.NetworkTickSystem.Tick += Tick;
    }

    // Is Tick() the correct update function here?
    private void Tick()
    {
        m_Ticks = m_Rollback.m_Tick;
        m_RemoteTicks = m_Rollback.m_RemoteTick;
        m_RemoteTickDelta = m_Rollback.m_TickDelta;

        TicksText.text = "Tick [" + m_Ticks.ToString() + "]";
        RemoteTicksText.text = "Remote Tick [" + m_RemoteTicks.ToString() + "]";
        RemoteTickDeltaText.text = "Tick Delta [" + m_RemoteTickDelta.ToString() + "]";
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -= Tick; 
    }
}
