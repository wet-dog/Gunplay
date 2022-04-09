using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerVisualization : NetworkBehaviour
{

    [SerializeField]
    SpriteRenderer m_SpriteRenderer;

    NetworkVariable<Color> m_SpriteColor = new NetworkVariable<Color>(Color.red);

    void Start()
    {
        // Make player 2 blue
        if (!IsHost && IsOwner)
        {
            SetPlayerTwoColorServerRpc();
        }

        // Make sure the network variable is used for new clients joining
        m_SpriteRenderer.color = m_SpriteColor.Value;
    }

    public override void OnNetworkSpawn()
    {
        m_SpriteColor.OnValueChanged += OnColorChanged;
    }

    public override void OnNetworkDespawn()
    {
        m_SpriteColor.OnValueChanged -= OnColorChanged;
    }

    public void OnColorChanged(Color previous, Color current)
    {
        m_SpriteRenderer.color = m_SpriteColor.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerTwoColorServerRpc()
    {
        m_SpriteColor.Value = Color.blue;
    }
}
