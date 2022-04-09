using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Gunplay.KeyboardState;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Camera cam;

    Vector2 movement;
    Vector2 mousePos;

    //public InputSystem inputSystem;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //movement.x = 0.0f;
        //movement.y = 0.0f;

        //if (inputSystem.KeyboardState.right)
        //{
        //    Debug.Log("Right");
        //    movement.x += 1.0f;
        //}
        //if (inputSystem.KeyboardState.left)
        //{
        //    Debug.Log("Left");
        //    movement.x -= 1.0f;
        //}
        //if (inputSystem.KeyboardState.up)
        //{
        //    Debug.Log("Up");
        //    movement.y += 1.0f;
        //}
        //if (inputSystem.KeyboardState.down)
        //{
        //    Debug.Log("Down");
        //    movement.y -= 1.0f;
        //}


        //movement.x = Input.GetAxisRaw("Horizontal");
        //movement.y = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    public void MovePlayer(InputSystem inputSystem)
    {
        movement.x = 0.0f;
        movement.y = 0.0f;
        if (inputSystem.KeyboardState.right)
        {
            Debug.Log("Right");
            movement.x += 1.0f;
        }
        if (inputSystem.KeyboardState.left)
        {
            Debug.Log("Left");
            movement.x -= 1.0f;
        }
        if (inputSystem.KeyboardState.up)
        {
            Debug.Log("Up");
            movement.y += 1.0f;
        }
        if (inputSystem.KeyboardState.down)
        {
            Debug.Log("Down");
            movement.y -= 1.0f;
        }

        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
