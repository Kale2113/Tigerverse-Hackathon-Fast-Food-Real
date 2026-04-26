using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PattyTray : MonoBehaviour {
    [Header("Setup")]
    public Transform stackPosition; // Where the patties should stack
    public float stackHeight = 0.1f; // The height at which each new patty is placed

    private List<GameObject> pattyStack = new List<GameObject>(); // A list to track the patties

    // Add a patty to the tray
    public void AddPattyToTray(GameObject patty) {
        // Position the patty on top of the last one
        Vector3 newPosition = stackPosition.position + new Vector3(0, pattyStack.Count * stackHeight, 0);
        patty.transform.position = newPosition;
        pattyStack.Add(patty); // Keep track of the patty in the stack

        // Optional: Make sure the patty's Rigidbody is set to Kinematic so it doesn't fall off
        Rigidbody rb = patty.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.isKinematic = true;
            }
        }
    }