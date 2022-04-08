using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class UINetwork : NetworkBehaviour
{
    public Text m_TicksText;

    private int m_Ticks;

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

    private void Tick()
    {
        m_Ticks = m_Rollback.m_Tick;

        if (IsHost)
        {
            m_TicksText.text = "Host Ticks: " + m_Ticks.ToString();
        }
        else if (!IsHost)
        {
            m_TicksText.text = "Client Ticks: " + m_Ticks.ToString();
        }
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.NetworkTickSystem.Tick -= Tick; 
    }
}
