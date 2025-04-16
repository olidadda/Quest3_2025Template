using UnityEngine;
using System.Collections;

public class ContaminantController : MonoBehaviour
{
    [HideInInspector] public bool isStuck = false;
    [HideInInspector] public float timeToUnstick = 0f;

    private Coroutine pushBackCoroutine = null;
    private Vector3 currentVelocity = Vector3.zero; // Store velocity locally now
    private float baseRandomStrength; // Store base randomness to restore it

    // Public accessor for velocity (Sim script will set this)
    public Vector3 Velocity
    {
        get { return currentVelocity; }
        set { currentVelocity = value; }
    }
    public void GetStuck(float duration)
    {
        StopPushBack();
        isStuck = true;
        timeToUnstick = duration;
        currentVelocity = Vector3.zero;
    }

    public void GetUnstuck()
    {
        isStuck = false;
        timeToUnstick = 0f;
        // Optional: Revert color/effect?
    }

    // Called by the main simulation script while stuck
    public bool UpdateStuckTimer(float deltaTime)
    {
        if (isStuck)
        {
            timeToUnstick -= deltaTime;
            if (timeToUnstick <= 0)
            {
                GetUnstuck();
                return true; // Return true indicates it just became unstuck
            }
        }
        return false; // Still stuck or wasn't stuck
    }

    // --- NEW: Initiate Push Back ---
    public void StartPushBack(Vector3 direction, float speed, float duration, float randomStrength)
    {
        StopPushBack(); // Ensure only one push runs at a time
        baseRandomStrength = randomStrength; // Remember original randomness
        pushBackCoroutine = StartCoroutine(PushBackRoutine(direction, speed, duration));
    }

    // --- NEW: Stop Push Back ---
    private void StopPushBack()
    {
        if (pushBackCoroutine != null)
        {
            StopCoroutine(pushBackCoroutine);
            pushBackCoroutine = null;
            // Restore normal random motion immediately (or let next Update handle it)
            // currentVelocity = Random.insideUnitSphere * baseRandomStrength; // Optional immediate reset
        }
    }

    // --- NEW: Push Back Coroutine ---
    private IEnumerator PushBackRoutine(Vector3 direction, float speed, float duration)
    {
        float timer = 0f;
        Vector3 pushVelocity = direction.normalized * speed;

        while (timer < duration)
        {
            // Combine push velocity with some reduced random motion during push
            Vector3 randomMotion = Random.insideUnitSphere * baseRandomStrength * 0.3f; // Less random during push
            currentVelocity = pushVelocity + randomMotion;

            timer += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Push finished
        pushBackCoroutine = null;
        // Velocity will be recalculated normally in the next Sim update
    }

    // --- NEW: Helper to check if being pushed ---
    public bool IsBeingPushedBack()
    {
        return pushBackCoroutine != null;
    }

    // --- NEW: Need to apply movement here if Sim doesn't ---
    // If ReverseOsmosisSim ONLY calculates velocity and Controller applies it
    void Update()
    {
        if (!isStuck && !IsBeingPushedBack())
        {
            // If Sim isn't setting velocity every frame, add basic random motion here?
            // NO - Sim script SHOULD calculate and assign velocity every frame.
        }

        // Apply calculated velocity (needs to be done somewhere!)
        // If Sim does it: remove this Update
        // If Controller does it: uncomment below
        // transform.position += currentVelocity * Time.deltaTime;
    }
}