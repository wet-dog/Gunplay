using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerStats : NetworkBehaviour
{
    public static event Action OnInstantiated;

    public override void OnNetworkSpawn()
    {
        if (OnInstantiated != null)
        {
            OnInstantiated();
        }
    }

    void Start()
    {
        if (IsOwner)
        {
            gameObject.AddComponent<CameraController>();
        }
    }
}
