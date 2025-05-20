using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

public class MovementDetector : MonoBehaviour
{
    [Header("Tracking Settings")]
    [Tooltip("The total distance in meters the object needs to move cumulatively before triggering the event.")]
    public float cumulativeDistanceThreshold = 0.05f; // 5 centimeters

    [Tooltip("Event fired once when the cumulative distance threshold is reached.")]
    public UnityEvent onDistanceThresholdReached;

    private Vector3 lastPosition;
    private float accumulatedDistance = 0f;
    private bool eventTriggered = false;

    // Use a small threshold to consider the object 'still' and reset accumulation
    private const float StillnessThreshold = 0.001f;

    void Start()
    {
        // Record initial position
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate distance moved since the last frame
        float distanceMovedThisFrame = Vector3.Distance(transform.position, lastPosition);

        // Check if the object moved noticeably
        if (distanceMovedThisFrame > StillnessThreshold)
        {
            // Only accumulate distance if the event hasn't already been triggered
            // since the object was last still.
            if (!eventTriggered)
            {
                accumulatedDistance += distanceMovedThisFrame;

                // Check if we've crossed the threshold
                if (accumulatedDistance >= cumulativeDistanceThreshold)
                {
                    Debug.Log($"{gameObject.name} reached cumulative distance threshold ({cumulativeDistanceThreshold}m).", this);
                    onDistanceThresholdReached.Invoke();
                    eventTriggered = true; // Prevent firing again until reset
                }
            }
        }
        else
        {
            // If the object is considered 'still' (moved less than stillness threshold)
            // reset the accumulated distance and the trigger flag, allowing the event
            // to fire again the *next* time it moves the required cumulative distance.
            if (accumulatedDistance > 0f || eventTriggered) // Only reset if needed
            {
                // Debug.Log($"{gameObject.name} considered still, resetting accumulator.", this);
                accumulatedDistance = 0f;
                eventTriggered = false;
            }
        }

        // Update the last position for the next frame's calculation
        lastPosition = transform.position;
    }

    // Optional: Public method to manually reset the tracker if needed from elsewhere
    public void ResetTracker()
    {
        Debug.Log($"{gameObject.name} tracker manually reset.", this);
        accumulatedDistance = 0f;
        eventTriggered = false;
        lastPosition = transform.position; // Update position on reset too
    }

    // Reset if component is disabled
    void OnDisable()
    {
        // Reset state so it starts fresh if re-enabled
        accumulatedDistance = 0f;
        eventTriggered = false;
    }

    // Reset if component is enabled after the first frame
    void OnEnable()
    {
        if (Time.frameCount > 1) // Avoid running right after Start
        {
            lastPosition = transform.position; // Get current position on enable
            accumulatedDistance = 0f;
            eventTriggered = false;
        }
    }
}