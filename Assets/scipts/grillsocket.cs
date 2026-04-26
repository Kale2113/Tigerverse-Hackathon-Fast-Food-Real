using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrillSocket : MonoBehaviour {
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
    public PattyTray pattyTray; // Reference to the tray where patties will be added

    void Awake() {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        }

    void OnEnable() {
        socket.selectEntered.AddListener(OnPlaced);
        socket.selectExited.AddListener(OnRemoved);
        }

    void OnDisable() {
        socket.selectEntered.RemoveListener(OnPlaced);
        socket.selectExited.RemoveListener(OnRemoved);
        }

    void OnPlaced(SelectEnterEventArgs args) {
        PattyCooker patty = args.interactableObject.transform.GetComponent<PattyCooker>();
        if (patty != null) {
            patty.StartCooking();
            }
        }

    void OnRemoved(SelectExitEventArgs args) {
        PattyCooker patty = args.interactableObject.transform.GetComponent<PattyCooker>();
        if (patty != null) {
            patty.StopCooking();

            // Once cooked, add the patty to the tray
            pattyTray.AddPattyToTray(patty.gameObject);
            }
        }
    }