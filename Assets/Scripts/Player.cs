using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public
    public float movementSpeed = 5.0f;
    public float jumpForce = 7.0f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    // Private
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float h = Input.GetAxis("Horizontal");
        movement = new Vector3(h, 0f, 0f) * movementSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}
