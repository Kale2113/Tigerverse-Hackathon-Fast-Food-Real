using System.Collections.Generic;
using UnityEngine;

public class Spatula : MonoBehaviour
{
    private HashSet<PattyCooker> flippedPatties = new HashSet<PattyCooker>();  // Track flipped patties

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has a PattyCooker component
        PattyCooker patty = other.GetComponent<PattyCooker>();

        if (patty != null)
        {
            // If patty is not flipped yet, flip it (cosmetic flip)
            if (!patty.CanBeServed()) // CanBeServed checks if it's flipped and not burnt
            {
                patty.Flip();  // Flip the patty visually
            }
            else
            {
                // If the patty is ready to be served, this is the second interaction
                patty.Flip(true);  // Start the grace period after the flip
                patty.ServePatty();  // Serve the finished patty
            }
        }
    }
}
