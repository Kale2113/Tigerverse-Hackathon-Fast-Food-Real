using System.Collections;
using UnityEngine;


public class PattyCooker : MonoBehaviour {
    [Header("Renderers")]
    public Renderer pattyRenderer;
    public GameObject redSpots; // small plane/decals on top
    public GameObject grillLines; // grill lines that appear when the patty is flipped

    [Header("Colors")]
    public Color rawColor = new Color(0.4f, 0.1f, 0.1f);
    public Color cookedColor = new Color(0.5f, 0.3f, 0.2f);
    public Color burntColor = Color.black;

    [Header("Timers")]
    [Range(1f, 20f)]
    public float timeToCook = 5f; // Time to cook
    [Range(1f, 20f)]
    public float timeToBurn = 5f; // Time to burn

    private bool isCooking = false;
    private bool isFlipped = false;
    private bool isBurnt = false; // This will track whether the patty is burnt
    private Coroutine cookRoutine;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private float redSpotsAppearTime; // Randomized time for red spots

    public float flipForce = 2f; // Force applied during the flip

    void Awake() {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        SetColor(rawColor);
        redSpots.SetActive(false);
        grillLines.SetActive(false); // Grill lines are hidden at the start
        }

    // Called by socket when placed
    public void StartCooking() {
        if (isCooking) return;

        isCooking = true;
        redSpotsAppearTime = Random.Range(timeToCook / 2f, timeToCook); // Randomize when the red spots appear
        cookRoutine = StartCoroutine(CookRoutine());
        }

    // Called when removed from grill
    public void StopCooking() {
        if (cookRoutine != null)
            StopCoroutine(cookRoutine);

        isCooking = false;
        }

    IEnumerator CookRoutine() {
        float timer = 0f;
        float colorLerpTime = timeToCook; // Time it takes to go from raw to cooked
        Color startColor = rawColor;
        Color endColor = cookedColor;

        // Gradual color transition from raw to cooked
        while (timer < colorLerpTime) {
            timer += Time.deltaTime;
            pattyRenderer.material.color = Color.Lerp(startColor, endColor, timer / colorLerpTime);
            yield return null;
            }

        // Ensure it's fully cooked
        pattyRenderer.material.color = cookedColor;

        // Wait until random time to start showing red spots
        yield return new WaitForSeconds(redSpotsAppearTime - timeToCook / 2f); // Wait until halfway

        // Show red spots
        redSpots.SetActive(true);

        // Wait until it's done cooking or burn
        timer = 0f;
        float burnTime = timeToBurn;

        // Gradual transition from cooked to burnt
        while (timer < burnTime) {
            timer += Time.deltaTime;
            pattyRenderer.material.color = Color.Lerp(cookedColor, burntColor, timer / burnTime);
            yield return null;
            }

        // Ensure it's fully burnt
        pattyRenderer.material.color = burntColor;
        isBurnt = true; // Mark as burnt
        }

    public bool IsBurnt() {
        return isBurnt;
        }

    // This method is now the trigger for the flip
    public void Flip() {
        if (!isFlipped) {
            StartCoroutine(SmoothFlip());
            }
        }

    // The smooth flip logic
    IEnumerator SmoothFlip() {
        isFlipped = true;

        float flipTime = 0.5f; // Time for the flip
        float elapsedTime = 0f;

        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(180f, 0f, 0f); // Rotate 180 degrees on the X-axis

        // Smoothly rotate from initial to target rotation
        while (elapsedTime < flipTime) {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / flipTime);
            elapsedTime += Time.deltaTime;
            yield return null;
            }

        // Ensure the final rotation is exactly as expected
        transform.rotation = targetRotation;

        // Apply some force for the physical flip (if Rigidbody exists)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) {
            rb.AddForce(Vector3.up * flipForce, ForceMode.Impulse); // Apply upward force to simulate the flip
            rb.AddTorque(Vector3.right * 10f, ForceMode.Impulse);  // Apply torque for rotation effect
            }

        // Change visual effects: Remove red spots and show grill lines
        redSpots.SetActive(false); // Hide red spots
        grillLines.SetActive(true); // Show grill lines
        }

    void SetColor(Color c) {
        pattyRenderer.material.color = c;
        }
    }