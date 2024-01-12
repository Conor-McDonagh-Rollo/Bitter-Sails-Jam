using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCreator : MonoBehaviour
{
    public Transform bridge_start;
    public Transform bridge_end;
    public GameObject bridge_segment_prefab;
    public GameObject bridge_hinge_prefab;
    public float segment_spacing;
    public float time_between_segments;

    bool hit = false;
    ParticleSystem other_ps = null;

    void Update()
    {
        if (hit)
        {
            if (!other_ps.isPlaying) // Is ps finished playing
            {
                Destroy(other_ps.gameObject); // then destroy
                hit = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arrow")) // If we hit the target, start making the bridge
        {
            other.gameObject.GetComponent<Rigidbody>().Sleep();
            other_ps = other.gameObject.GetComponent<ParticleSystem>();
            GetComponent<MeshRenderer>().enabled = false;
            other_ps.Play();
            hit = true;
            StartCoroutine(CreateBridge());
        }
    }

    IEnumerator CreateBridge()
    {
        yield return new WaitForSeconds(time_between_segments);
        Vector3 directionToEnd = (bridge_end.position - bridge_start.position).normalized; // Direction to instatiate objects
        float distanceToEnd = Vector3.Distance(bridge_start.position, bridge_end.position); // Distance for number of segments
        int numberOfSegments = Mathf.CeilToInt(distanceToEnd / 
            (bridge_segment_prefab.transform.localScale.x + segment_spacing)); // calculate the number of segments needed

        GameObject bridge_go = Instantiate(new GameObject("bridge"), 
            bridge_start.position, 
            Quaternion.identity); // Object to store segments and hinges inside the hierarchy

        List<Rigidbody> rbs = new List<Rigidbody>(); // Keep track of which rbs to set kinematic back to false on
        List<Rigidbody> hinge_rbs = new List<Rigidbody>();// Keep track of which rbs to set kinematic back to false on
        List<GameObject> second_hinges = new List<GameObject>(); // Each hinge needs a second hinge so its connected to both sides of segments

        for (int i = 0; i < numberOfSegments; i++)
        {
            Vector3 position = bridge_start.position + 
                               directionToEnd * 
                               (bridge_segment_prefab.transform.localScale.x + segment_spacing) * i; // Position of the segment

            GameObject segment = Instantiate(bridge_segment_prefab, 
                                 position, 
                                 Quaternion.identity, 
                                 bridge_go.transform); // Instantiate the segment

            Rigidbody rb = segment.GetComponent<Rigidbody>();
            rb.isKinematic = true; // So that no collisions happen before the bridge is made

            HingeJoint segment_hinge1 = Instantiate(bridge_hinge_prefab,
                                                    position,
                                                    Quaternion.Euler(0, 0, 90),
                                                    bridge_go.transform).AddComponent<HingeJoint>();
            HingeJoint segment_hinge2 = Instantiate(bridge_hinge_prefab,
                                                    position,
                                                    Quaternion.Euler(0, 0, 90),
                                                    bridge_go.transform).AddComponent<HingeJoint>(); // Instatiate both sides of the hinge

            hinge_rbs.Add(segment_hinge1.GetComponent<Rigidbody>());
            hinge_rbs[hinge_rbs.Count - 1].isKinematic = true;
            hinge_rbs.Add(segment_hinge2.GetComponent<Rigidbody>());
            hinge_rbs[hinge_rbs.Count - 1].isKinematic = true;          // Store the rbs to set kinematic to false later

            segment_hinge1.transform.position += new Vector3(directionToEnd.x * 
                                                            (bridge_segment_prefab.transform.localScale.x / 2f) * 1.5f
                                                            , 0
                                                            , -0.5f);
            segment_hinge2.transform.position += new Vector3(directionToEnd.x * 
                                                            (bridge_segment_prefab.transform.localScale.x / 2f) * 1.5f
                                                            , 0
                                                            , 0.5f); // Position the hinges so they look like they connect the segments
            
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
    }
}
