using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Public
    public float movementSpeed = 5.0f;
    public float jumpForce = 7.0f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    public Animator rot_anim;
    public Animator anim;

    public AudioClip player_jump1;
    public AudioClip player_jump2;
    public AudioClip player_land;

    public AudioSource player_walk_as;

    // Private
    private Rigidbody rb;
    public bool isGrounded = true;
    private Vector3 movement;
    public int jumpsRemaining = 2;
    float walkSoundTimer = .35f;

    // Static
    public static AudioSource audioSource;
    public static bool freeze = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool previouslyGrounded = isGrounded;
        float previousVelocity = rb.velocity.y;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        anim.SetBool("Jumping", !isGrounded);
        if (!previouslyGrounded && isGrounded && previousVelocity < -1f)
        {
            audioSource.PlayOneShot(player_land);
        }

        if (!freeze)
        {
            float h = Input.GetAxis("Horizontal");
            movement = new Vector3(h, 0f, 0f) * movementSpeed;
        }
        else
        {
            movement = Vector3.zero;
        }

        if (movement.x > 0f || movement.x < 0f)
        {
            anim.SetBool("Walking", true);
            rot_anim.SetInteger("dir", (int)movement.x);
            walkSoundTimer -= Time.deltaTime;
            if (walkSoundTimer < 0f && isGrounded)
            {
                walkSoundTimer = .35f;
                player_walk_as.pitch = Random.Range(.75f, 1.25f);
                player_walk_as.Play();
            }
        }
        else
        {
            anim.SetBool("Walking", false);
            rot_anim.SetInteger("dir", 0);
        }

        if (Input.GetButtonDown("Jump") && !freeze)
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
        if (jumpsRemaining == 1)
        {
            audioSource.PlayOneShot(player_jump1);
        }
        else
        {
            audioSource.PlayOneShot(player_jump2);
        }
    }
}
