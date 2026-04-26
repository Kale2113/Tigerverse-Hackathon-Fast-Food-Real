using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PattyTray : MonoBehaviour
{
    public Transform stackPosition;
    public float stackHeight = 0.05f;

    private List<GameObject> stack = new();

    public void AddPattyToTray(GameObject patty)
    {
        if (!patty || !stackPosition) return;
        if (stack.Contains(patty)) return;

        Rigidbody rb = patty.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        patty.transform.SetParent(stackPosition);

        patty.transform.position =
            stackPosition.position + Vector3.up * (stack.Count * stackHeight);

        patty.transform.rotation = stackPosition.rotation;

        stack.Add(patty);
    }
}