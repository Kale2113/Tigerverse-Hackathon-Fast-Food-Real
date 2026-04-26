using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PattyBoxBothHandsSpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject pattyPrefab;
    public Transform spawnPoint;

    private XRDirectInteractor currentInteractor;

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    private bool canSpawn = true;

    // ?? edge detection (prevents spam)
    private bool leftGripLastFrame = false;
    private bool rightGripLastFrame = false;

    private void OnTriggerEnter(Collider other)
    {
        XRDirectInteractor interactor = other.GetComponentInParent<XRDirectInteractor>();

        if (interactor != null)
        {
            currentInteractor = interactor;

            leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            Debug.Log("?? Hand entered box");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        XRDirectInteractor interactor = other.GetComponentInParent<XRDirectInteractor>();

        if (interactor != null && interactor == currentInteractor)
        {
            currentInteractor = null;

            Debug.Log("?? Hand left box");
        }
    }

    private void Update()
    {
        if (currentInteractor == null || !canSpawn)
            return;

        bool leftGrip = false;
        bool rightGrip = false;

        if (leftHandDevice.isValid)
            leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out leftGrip);

        if (rightHandDevice.isValid)
            rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out rightGrip);

        // ?? only trigger on NEW press
        bool leftPressed = leftGrip && !leftGripLastFrame;
        bool rightPressed = rightGrip && !rightGripLastFrame;

        if (leftPressed || rightPressed)
        {
            SpawnPatty();
        }

        leftGripLastFrame = leftGrip;
        rightGripLastFrame = rightGrip;
    }

    private void SpawnPatty()
    {
        if (pattyPrefab == null || currentInteractor == null)
            return;

        GameObject patty = Instantiate(pattyPrefab);

        Transform attach = currentInteractor.attachTransform;

        patty.transform.SetPositionAndRotation(attach.position, attach.rotation);

        Rigidbody rb = patty.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        var grab = patty.GetComponent<XRGrabInteractable>();

        if (grab != null)
        {
            StartCoroutine(ForceGrabNextFrame(grab));
        }

        Debug.Log("?? Patty spawned into hand");

        StartCoroutine(SpawnCooldown());
    }

    private IEnumerator ForceGrabNextFrame(XRGrabInteractable grab)
    {
        yield return null;

        if (currentInteractor != null && grab != null)
        {
            grab.interactionManager.SelectEnter(
            (IXRSelectInteractor)currentInteractor,
            (IXRSelectInteractable)grab
            );
        }
    }

    private IEnumerator SpawnCooldown()
    {
        canSpawn = false;
        yield return new WaitForSeconds(0.25f);
        canSpawn = true;
    }
}
