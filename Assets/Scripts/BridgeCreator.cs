using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCreator : MonoBehaviour
{
    public Transform bridge_start;
    public Transform bridge_end;
    public GameObject bridge_segment_prefab;
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
        if (other.CompareTag("Arrow"))
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
        yield return new WaitForSeconds(0.1f);
        Vector3 directionToEnd = (bridge_end.position - bridge_start.position).normalized;
        float distanceToEnd = Vector3.Distance(bridge_start.position, bridge_end.position);
        int numberOfSegments = Mathf.CeilToInt(distanceToEnd / (bridge_segment_prefab.transform.localScale.x + segment_spacing));

        GameObject artic_body_go = Instantiate(new GameObject(), bridge_start.position, Quaternion.identity);
        ArticulationBody artic_body = artic_body_go.AddComponent<ArticulationBody>();

        List<Rigidbody> rbs = new List<Rigidbody>();

        for (int i = 0; i < numberOfSegments; i++)
        {
            Vector3 position = bridge_start.position + directionToEnd * (bridge_segment_prefab.transform.localScale.x + segment_spacing) * i;
            GameObject segment = Instantiate(bridge_segment_prefab, position, Quaternion.identity);
            Rigidbody rb = segment.GetComponent<Rigidbody>();
            rb.isKinematic = true;

            segment.GetComponent<FixedJoint>().connectedArticulationBody = artic_body;

            if (i == 0 || i == numberOfSegments - 1)
            {
                rb.constraints = RigidbodyConstraints.FreezePosition;
            }

            rbs.Add(rb);
            yield return new WaitForSeconds(time_between_segments);
        }

        for (int i = 0; i < numberOfSegments; i++)
        {
            rbs[i].isKinematic = false;
        }
    }
}
