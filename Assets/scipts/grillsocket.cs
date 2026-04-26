using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrillSocket : MonoBehaviour
{
    private XRSocketInteractor socket;

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
        PattyCooker patty = args.interactableObject.transform.GetComponent<PattyCooker>();
        if (patty != null)
        {
            patty.StartCooking();
        }
    }

    void OnRemoved(SelectExitEventArgs args)
    {
        PattyCooker patty = args.interactableObject.transform.GetComponent<PattyCooker>();
        if (patty != null)
        {
            patty.StopCooking();
        }
    }
}