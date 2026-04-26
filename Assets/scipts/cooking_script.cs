using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PattyCooker : MonoBehaviour
{
    [Header("Renderers")]
    public Renderer pattyRenderer;
    public GameObject redSpots; // small plane/decals on top

    [Header("Colors")]
    public Color rawColor = new Color(0.4f, 0.1f, 0.1f);
    public Color cookedColor = new Color(0.5f, 0.3f, 0.2f);
    public Color burntColor = Color.black;

    [Header("Timers")]
    public float timeToCook = 5f;
    public float timeToBurn = 5f;

    private bool isCooking = false;
    private bool isFlipped = false;

    private Coroutine cookRoutine;

    private XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        SetColor(rawColor);
        redSpots.SetActive(false);
    }

    // Called by socket when placed
    public void StartCooking()
    {
        if (isCooking) return;

        isCooking = true;
        cookRoutine = StartCoroutine(CookRoutine());
    }

    // Called when removed from grill
    public void StopCooking()
    {
        if (cookRoutine != null)
            StopCoroutine(cookRoutine);

        isCooking = false;
    }

    IEnumerator CookRoutine()
    {
        // Stage 1: Raw ? Cooked
        yield return new WaitForSeconds(timeToCook);

        SetColor(cookedColor);

        // Show flip indicator (red spots)
        redSpots.SetActive(true);

        // Wait for flip OR continue to burn
        float timer = 0f;

        while (!isFlipped && timer < timeToBurn)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        redSpots.SetActive(false);

        // If not flipped ? burn
        if (!isFlipped)
        {
            SetColor(burntColor);
        }
        else
        {
            // Reset for second side cooking
            isFlipped = false;
            StartCoroutine(CookRoutine());
        }
    }

    public void Flip()
    {
        isFlipped = true;
    }

    void SetColor(Color c)
    {
        pattyRenderer.material.color = c;
    }
}
