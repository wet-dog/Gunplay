using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerStats : NetworkBehaviour
{
    void Start()
    {
        if (IsOwner)
        {
            gameObject.AddComponent<CameraController>();
            gameObject.AddComponent<Rollback>();
        }
    }
}
