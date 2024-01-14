using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCreator : MonoBehaviour
{
    public Transform bridge_start;
    public Transform bridge_end;
    public GameObject bridge_segment_prefab;
    public GameObject bridge_hinge_prefab;
    public GameObject poof_ps_prefab;
    public GameObject explode_ps_prefab;
    public float segment_spacing;
    public float time_between_segments;

    // Audio stuff
    public AudioClip bridge_building;
    public AudioClip bridge_completed;
    public AudioClip cannonball_break;
    public AudioClip target_spin;
    public AudioClip poof;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow")) // If we hit the target, start making the bridge
        {
            other.gameObject.GetComponent<Rigidbody>().Sleep();
            Instantiate(explode_ps_prefab, other.transform.position, Quaternion.identity);
            Player.audioSource.PlayOneShot(cannonball_break);
            Destroy(other.gameObject);
            anim.Play("spin");
            Player.audioSource.PlayOneShot(target_spin);
            StartCoroutine(CreateBridge());
        }
    }

    IEnumerator CreateBridge()
    {
        yield return new WaitForSeconds(time_between_segments);
        Vector3 directionToEnd = (bridge_end.position - bridge_start.position).normalized; // Direction to instatiate objects
        Quaternion rotationFromDirection = Quaternion.FromToRotation(Vector3.right, directionToEnd);
        float distanceToEnd = Vector3.Distance(bridge_start.position, bridge_end.position); // Distance for number of segments
        int numberOfSegments = Mathf.CeilToInt(distanceToEnd / 
            (bridge_segment_prefab.transform.localScale.x + segment_spacing)); // calculate the number of segments needed

        GameObject bridge_go = Instantiate(new GameObject("bridge"), 
            bridge_start.position, 
            Quaternion.identity); // Object to store segments and hinges inside the hierarchy

        List<Rigidbody> rbs = new List<Rigidbody>(); // Keep track of which rbs to set kinematic back to false on
        List<Rigidbody> hinge_rbs = new List<Rigidbody>();// Keep track of which rbs to set kinematic back to false on
        List<GameObject> second_hinges = new List<GameObject>(); // Each hinge needs a second hinge so its connected to both sides of segments

        float audio_step = 2f / numberOfSegments;
        Player.audioSource.pitch = 1f;

        for (int i = 0; i < numberOfSegments; i++)
        {
            Player.audioSource.PlayOneShot(bridge_building);
            Player.audioSource.pitch += audio_step;

            Vector3 position = bridge_start.position + 
                               directionToEnd * 
                               (bridge_segment_prefab.transform.localScale.x + segment_spacing) * i; // Position of the segment

            GameObject segment = Instantiate(bridge_segment_prefab, 
                                 position, 
                                 rotationFromDirection, 
                                 bridge_go.transform); // Instantiate the segment

            Rigidbody rb = segment.GetComponent<Rigidbody>();
            rb.isKinematic = true; // So that no collisions happen before the bridge is made

            HingeJoint segment_hinge1 = Instantiate(bridge_hinge_prefab,
                                                    position,
                                                    rotationFromDirection,
                                                    bridge_go.transform).AddComponent<HingeJoint>();
            HingeJoint segment_hinge2 = Instantiate(bridge_hinge_prefab,
                                                    position,
                                                    rotationFromDirection,
                                                    bridge_go.transform).AddComponent<HingeJoint>(); // Instatiate both sides of the hinge

            hinge_rbs.Add(segment_hinge1.GetComponent<Rigidbody>());
            hinge_rbs[hinge_rbs.Count - 1].isKinematic = true;
            hinge_rbs.Add(segment_hinge2.GetComponent<Rigidbody>());
            hinge_rbs[hinge_rbs.Count - 1].isKinematic = true;          // Store the rbs to set kinematic to false later

            segment_hinge1.transform.position += new Vector3(directionToEnd.x * 
                                                            (bridge_segment_prefab.transform.localScale.x / 2f) * 1.5f
                                                            , -0.25f
                                                            , -0.5f);
            segment_hinge2.transform.position += new Vector3(directionToEnd.x * 
                                                            (bridge_segment_prefab.transform.localScale.x / 2f) * 1.5f
                                                            , -0.25f
                                                            , 0.3f); // Position the hinges so they look like they connect the segments
            
            ///////// Set the connected body and the joint limits of the first hinges /////////
            segment_hinge1.connectedBody = rb;
            segment_hinge2.connectedBody = rb;
            segment_hinge1.useLimits = true;
            segment_hinge2.useLimits = true;
            JointLimits sh1jl = segment_hinge1.limits;
            sh1jl.max = 5;
            segment_hinge1.limits = sh1jl;
            JointLimits sh2jl = segment_hinge2.limits;
            sh2jl.max = 5;
            segment_hinge2.limits = sh2jl;
            ///////////////////////////////////////////////////////////////////////////////////

            /////// Do the same but for the previous hinges and connect them to this rb ///////
            if (second_hinges.Count > 0)
            {
                HingeJoint h1 = second_hinges[0].AddComponent<HingeJoint>();
                HingeJoint h2 = second_hinges[1].AddComponent<HingeJoint>();

                h1.connectedBody = rb;
                h2.connectedBody = rb;
                h1.useLimits = true;
                h2.useLimits = true;
                JointLimits h1jl = h1.limits;
                h1jl.max = 5;
                h1.limits = h1jl;
                JointLimits h2jl = h2.limits;
                h2jl.max = 5;
                h2.limits = h2jl;

                second_hinges.Clear();
            }
            second_hinges.Add(segment_hinge1.gameObject);
            second_hinges.Add(segment_hinge2.gameObject);
            ///////////////////////////////////////////////////////////////////////////////////

            // Freeze the start and end so that they anchor
            if (i == 0 || i == numberOfSegments - 1)
            {
                rb.constraints = RigidbodyConstraints.FreezePosition;
            }

            rbs.Add(rb);
            yield return new WaitForSeconds(time_between_segments);
        }

        // Reset the kinematic to false (apart from the anchored start and end)
        for (int i = 0; i < rbs.Count; i++)
        {
            if (i == 0 || i == rbs.Count - 1)
            {
                continue;
            }
            rbs[i].isKinematic = false;
        }
        for (int i = 0; i < hinge_rbs.Count; i++)
        {
            hinge_rbs[i].isKinematic = false;
        }

        Player.audioSource.PlayOneShot(bridge_completed);
        Player.audioSource.pitch = 1f;
        Player.audioSource.PlayOneShot(poof);

        Instantiate(poof_ps_prefab, transform.position, Quaternion.identity);
        Destroy(transform.parent.gameObject);
    }

    public void StartBridge()
    {
        StartCoroutine(CreateBridge());
    }
}
