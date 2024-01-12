using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    // Object To Launch + Properties
    public GameObject objectPrefab; 
    public float launchPower = 10f;
    public float launchDamp = 10f;
    public Transform launchPoint; // Point at which we launch from

    // Visualization
    public LineRenderer trajectoryLineRenderer; 
    public int trajectoryPoints = 30; 

    // Keep track of the current object we are launching
    private GameObject currentObjectToLaunch; 
    private Rigidbody currentObjectRigidbody; 

    // Keep track of mouse + dragging
    private Vector3 initialMousePosition;
    private Vector3 currentMousePosition;
    private bool isDragging = false;

    // Check if player is near
    public float detectionRadius = 1f;
    public LayerMask playerLayer;

    void Update()
    {
        // First click
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPlayerInRadius())
                return;

            initialMousePosition = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
            isDragging = true;

            // Instantiate the object at the launch point
            if (currentObjectToLaunch == null)
            {
                currentObjectToLaunch = Instantiate(objectPrefab, launchPoint.position, Quaternion.identity);
                currentObjectRigidbody = currentObjectToLaunch.GetComponent<Rigidbody>();
                currentObjectRigidbody.Sleep(); // sleep the rb so it doesnt collide
            }

            trajectoryLineRenderer.positionCount = trajectoryPoints;
        }

        // Dragging
        if (isDragging && Input.GetMouseButton(0))
        {
            //currentObjectToLaunch.transform.position = launchPoint.position;
            currentMousePosition = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y);
            DrawTrajectoryLine();
        }

        // FIRE!
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            currentObjectRigidbody.WakeUp(); // Rememeber to wake the rb for collisions
            LaunchObject();
            trajectoryLineRenderer.positionCount = 0; // Clear the line
            currentObjectToLaunch = null; // Clear reference for the next launch
        }
    }

    // Check if player is near
    bool IsPlayerInRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Player")) 
            {
                return true; // Player found within radius
            }
        }

        return false; // Player not found in radius
    }

    //Apply forces
    void LaunchObject()
    {
        if (currentObjectRigidbody != null)
        {
            Vector3 dragVector = initialMousePosition - currentMousePosition;
            Vector3 launchDirection = new Vector3(dragVector.x, dragVector.z, 0) / launchDamp;

            currentObjectRigidbody.AddForce(launchDirection * launchPower, ForceMode.Impulse);
        }
    }

    // Visualize line
    void DrawTrajectoryLine()
    {
        if (currentObjectToLaunch == null) return;

        Vector3 dragVector = initialMousePosition - currentMousePosition;
        Vector3 launchDirection = new Vector3(dragVector.x, dragVector.z, 0) / launchDamp; 

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float simulationTime = i / (float)trajectoryPoints * 2; // Adjust time scale
            Vector3 displacement = launchDirection * launchPower * simulationTime + Physics.gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = launchPoint.position + displacement;
            trajectoryLineRenderer.SetPosition(i, drawPoint);
        }
    }

    public Rigidbody GetCurrentRb()
    {
        return currentObjectRigidbody;
    }
}
