using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PattyCooker : MonoBehaviour
{
    [Header("Renderers")]
    public Renderer pattyRenderer;
    public GameObject redSpots;
    public GameObject grillLines;  // Grill lines will appear after flip
    public GameObject finishedPattyPrefab;

    [Header("Colors")]
    public Color rawColor = new Color(0.4f, 0.1f, 0.1f);
    public Color cookedColor = new Color(0.5f, 0.3f, 0.2f);
    public Color burntColor = Color.black;

    [Header("Timers")]
    [Range(1f, 20f)] public float timeToCook = 5f;
    [Range(1f, 20f)] public float timeToBurn = 5f;  // Time for burn phase after grace period ends

    private bool isCooking = false;
    private bool isFlipped = false;
    private bool isBurnt = false;
    private bool inGracePeriod = false;
    private bool canServe = false;  // Flag to track if the patty can be served (after grace period)
    private bool isBurning = false;  // Flag to track if patty is burning after grace period

    private Coroutine cookRoutine;
    private Coroutine burnRoutine;

    // Getter for checking if patty can be served
    public bool CanBeServed()
    {
        return isFlipped && !isBurnt && inGracePeriod;  // The patty can be served if it's flipped and not burnt, and grace period is over
    }

    // Getter for checking if patty is burnt
    public bool IsBurnt()
    {
        return isBurnt;
    }

    void Awake()
    {
        SafeInit();
        SetColor(rawColor);  // Start with raw color
    }

    void SafeInit()
    {
        if (redSpots != null) redSpots.SetActive(false);
        if (grillLines != null) grillLines.SetActive(false);
    }

    // Start cooking the patty
    public void StartCooking()
    {
        if (isCooking || isBurnt) return;

        isCooking = true;
        inGracePeriod = false;  // Reset grace period
        canServe = false;  // Ensure serve flag is reset

        cookRoutine = StartCoroutine(CookRoutine());
    }

    // Stop cooking the patty
    public void StopCooking()
    {
        isCooking = false;

        if (cookRoutine != null)
            StopCoroutine(cookRoutine);
    }

    IEnumerator CookRoutine()
    {
        float t = 0f;
        // RAW ? COOKED
        while (t < timeToCook)
        {
            t += Time.deltaTime;
            SetColor(Color.Lerp(rawColor, cookedColor, t / timeToCook));

            // Show red spots after 50% of the cooking time (randomly at any point after 50%)
            if (!isFlipped && t >= timeToCook * 0.5f && redSpots != null && !redSpots.activeInHierarchy)
            {
                redSpots.SetActive(true);
            }

            yield return null;
        }

        SetColor(cookedColor);
    }

    // Flip the patty (Visual only, no physics)
    public void Flip(bool isFinalFlip = false)
    {
        if (isFlipped) return;

        // Set flipped state to true
        isFlipped = true;

        // Disable the red spots since the patty is flipped
        if (redSpots != null)
            redSpots.SetActive(false);

        // Enable the grill lines to visually show the flip
        if (grillLines != null)
            grillLines.SetActive(true);

        // If this is the final flip (i.e., it's the second spatula interaction), start the grace period
        if (isFinalFlip)
        {
            StartCoroutine(StartGracePeriodCoroutine());
        }
    }

    // Start grace period after flip
    private IEnumerator StartGracePeriodCoroutine()
    {
        // Grace period starts after flip
        yield return new WaitForSeconds(5f);  // Grace period after flip
        inGracePeriod = true;
        canServe = true;  // Allow serving after grace period

        // After the grace period, start the burn phase if not served
        StartBurnPhase();
    }

    // Start the burn phase after grace period
    private void StartBurnPhase()
    {
        // If the patty hasn't been served yet, begin the burn phase
        if (!isBurning && !CanBeServed())
        {
            isBurning = true;
            burnRoutine = StartCoroutine(BurnRoutine());
        }
    }

    // Handle the burn phase
    IEnumerator BurnRoutine()
    {
        float t = 0f;
        // COOKED ? BURNT (smooth transition)
        while (t < timeToBurn)
        {
            t += Time.deltaTime;
            SetColor(Color.Lerp(cookedColor, burntColor, t / timeToBurn));

            yield return null;
        }

        SetColor(burntColor);
        isBurnt = true;

        // Deactivate red spots and grill lines once it's burnt
        if (redSpots != null)
            redSpots.SetActive(false);
        if (grillLines != null)
            grillLines.SetActive(false);

        // Destroy the patty after burn phase
        Destroy(gameObject);
    }

    // Serve the patty (instantiate the finished patty and remove the original)
    public void ServePatty()
    {
        if (canServe && !isBurnt)
        {
            // Instantiate the finished patty on the tray
            Debug.Log("Patty Served!");
            Instantiate(finishedPattyPrefab, transform.position, Quaternion.identity);

            // Destroy the cooked patty from the grill
            Destroy(gameObject);
        }
    }

    // Set the color of the patty (used for cooking stages)
    void SetColor(Color color)
    {
        if (pattyRenderer != null)
        {
            Material mat = new Material(pattyRenderer.material);  // Ensure unique material
            mat.color = color;
            pattyRenderer.material = mat;
        }
    }
}

