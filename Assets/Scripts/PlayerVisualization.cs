using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerVisualization : NetworkBehaviour
{

    [SerializeField] 
    SpriteRenderer m_SpriteRenderer; 

    void Start()
    {
        // Make player 2 blue
        if (!IsHost && IsOwner)
        {
            m_SpriteRenderer.color = Color.blue;
        }
    }
}
