using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrillSocket : MonoBehaviour
{
    private XRSocketInteractor socket;
    public AudioSource grillAudioSource;
    private bool isCooking = false;
    private bool inGracePeriod = false;
    private float gracePeriodTime = 5f; // 5 seconds grace period after flipping
    private float graceTimer = 0f;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        socket.selectEntered.AddListener(OnPlaced);
        socket.selectExited.AddListener(OnRemoved);
    }

    void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnPlaced);
        socket.selectExited.RemoveListener(OnRemoved);
    }

    void OnPlaced(SelectEnterEventArgs args)
    {
        // Ensure the interactable object is the correct one
        var interactable = args.interactableObject;
        GameObject pattyObject = interactable.transform.gameObject;

        if (pattyObject != null)
        {
            PattyCooker patty = pattyObject.GetComponent<PattyCooker>();

            if (patty != null)
            {
                patty.StartCooking();
                isCooking = true;

                // Start cooking audio if not already playing
                if (grillAudioSource != null && !grillAudioSource.isPlaying)
                {
                    grillAudioSource.Play();
                    Debug.Log("Grill Audio Started");
                }
            }
        }
    }

    void OnRemoved(SelectExitEventArgs args)
    {
        var interactable = args.interactableObject;
        GameObject pattyObject = interactable.transform.gameObject;

        if (pattyObject != null)
        {
            PattyCooker patty = pattyObject.GetComponent<PattyCooker>();

            if (patty != null)
            {
                patty.StopCooking();
                isCooking = false;

                // Stop cooking audio when patty is removed
                if (grillAudioSource != null && grillAudioSource.isPlaying)
                {
                    grillAudioSource.Stop();
                    Debug.Log("Grill Audio Stopped");
                }
            }
        }
    }

    void Update()
    {
        if (isCooking && inGracePeriod)
        {
            graceTimer += Time.deltaTime;

            if (graceTimer >= gracePeriodTime)
            {
                inGracePeriod = false;
                grillAudioSource.Stop();  // Audio stops to signal burn phase
                Debug.Log("Grace period ended. Grill audio stopped.");
            }
        }
    }

    public void StartGracePeriod()
    {
        inGracePeriod = true;
        graceTimer = 0f;
    }
}