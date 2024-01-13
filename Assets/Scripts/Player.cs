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
    public bool isGrounded;
    private Vector3 movement;
    public int jumpsRemaining = 2;

    // Static
    public static AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float h = Input.GetAxis("Horizontal");
        movement = new Vector3(h, 0f, 0f) * movementSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                jumpsRemaining = 2;
            }
            if (jumpsRemaining > 0)
            {
                jumpsRemaining--;
                Jump();
            }
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
}
