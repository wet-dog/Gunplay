using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gunplay.KeyboardState;

public class InputSystem : MonoBehaviour
{
    public KeyboardState KeyboardState = new KeyboardState(false, false, false, false, false);

    void Update()
    {
        KeyboardState.up = Input.GetKey(KeyCode.W);
        KeyboardState.down = Input.GetKey(KeyCode.S);
        KeyboardState.left = Input.GetKey(KeyCode.A);
        KeyboardState.right = Input.GetKey(KeyCode.D);
        KeyboardState.attack = Input.GetButton("Fire1");
    }
}
