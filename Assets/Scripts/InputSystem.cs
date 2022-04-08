using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gunplay.KeyboardState;

public class InputSystem : MonoBehaviour
{
    KeyboardState m_KeyboardState = new KeyboardState(false, false, false, false, false);

    void Update()
    {
        m_KeyboardState.up = Input.GetButton("W");
        m_KeyboardState.down = Input.GetButton("S");
        m_KeyboardState.left = Input.GetButton("A");
        m_KeyboardState.right = Input.GetButton("D");
        m_KeyboardState.attack = Input.GetButton("Fire1");
    }
}
