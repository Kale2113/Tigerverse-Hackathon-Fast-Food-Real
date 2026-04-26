using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spatula : MonoBehaviour {
    public float flipForce = 2f;

    private void OnTriggerEnter(Collider other) {
        // Ensure we only interact with the right object
        PattyCooker patty = other.GetComponent<PattyCooker>();

        if (patty != null) {
            // Flip the patty
            patty.Flip();

            // Optional: add a little physical flip (with force and torque)
            Rigidbody rb = patty.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddForce(Vector3.up * flipForce, ForceMode.Impulse); // Upward flip force
                rb.AddTorque(Random.insideUnitSphere * flipForce, ForceMode.Impulse); // Random torque for rotation
                }

            // Optional: Update visual feedback (show red spots if needed)
            patty.redSpots.SetActive(true); // Show red spots as a visual cue
            }
        }
    }