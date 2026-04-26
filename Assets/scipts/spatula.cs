using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spatula : MonoBehaviour
{
    public float flipForce = 2f;

    private void OnTriggerEnter(Collider other)
    {
        PattyCooker patty = other.GetComponent<PattyCooker>();

        if (patty != null)
        {
            // Flip the patty
            patty.Flip();

            // Optional: add a little physical flip
            Rigidbody rb = patty.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * flipForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * flipForce, ForceMode.Impulse);
            }
        }
    }
}
