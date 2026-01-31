using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        MovementControl();
    }

    public void MovementControl()
    {
        float moveX = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.W))
        {
            moveZ = moveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveZ = -moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = moveSpeed;
        }

        rb.velocity = new Vector3(moveX, rb.velocity.y, moveZ);
    }
}
