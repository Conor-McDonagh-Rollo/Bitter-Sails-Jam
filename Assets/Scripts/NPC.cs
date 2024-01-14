using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Transform player;
    bool beginFollowing = false;

    public Vector3 offset;
    public float speed = 0.125f;

    private void FixedUpdate()
    {
        if (beginFollowing)
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime * speed);
            transform.position = smoothedPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.freeze = true;
            GetComponent<Dialogue>().StartDialogue();
        }
    }

    public void BeginFollow()
    {
        StartCoroutine(Follow());
    }

    IEnumerator Follow()
    {
        yield return new WaitForSeconds(2f);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        beginFollowing = true;
    }
}
