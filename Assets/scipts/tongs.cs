using UnityEngine;


public class Tongs : MonoBehaviour {
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;  // The tongs should be a grab interactable to be picked up
    public float pickUpForce = 5f;  // Force to apply when picking up the patty

    private void OnTriggerEnter(Collider other) {
        PattyCooker patty = other.GetComponent<PattyCooker>(); // Check if the collided object is a patty

        if (patty != null && patty.IsBurnt())  // Check if the patty is burnt
        {
            // If the patty is burnt, allow the tongs to pick it up
            PickUpPatty(patty);
            }
        }

    private void PickUpPatty(PattyCooker patty) {
        // Make sure the patty is grabbed by the tongs
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = patty.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null) {
            // Apply force to move the patty to the tongs
            Rigidbody pattyRb = patty.GetComponent<Rigidbody>();
            if (pattyRb != null) {
                // Use the tongs to lift the patty
                pattyRb.velocity = Vector3.zero;
                pattyRb.angularVelocity = Vector3.zero;

                // Apply a force to simulate lifting
                pattyRb.AddForce(transform.up * pickUpForce, ForceMode.Impulse);
                }
            }
        }
    }