using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Ensure there's a Rigidbody
public class ReturnHomeOnDrop : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Maximum distance from the origin before returning.")]
    public float maxDistanceFromOrigin = 5.0f; // Adjust as needed

    [Header("State (Internal)")]
    [SerializeField] // Show in inspector for debugging, but not meant to be set manually
    private Vector3 initialPosition;
    [SerializeField]
    private Quaternion initialRotation;
    [SerializeField]
    private bool isHeld = false; // Flag to prevent returning while grabbed

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("ReturnHomeOnDrop requires a Rigidbody component!", this);
            enabled = false; // Disable script if no Rigidbody
            return;
        }

        // Record the starting position and rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void FixedUpdate() // Use FixedUpdate for physics-related checks
    {
        // Only check for return condition if the object is NOT currently held
        if (!isHeld)
        {
            float distance = Vector3.Distance(transform.position, initialPosition);

            if (distance > maxDistanceFromOrigin)
            {
                ReturnToOrigin();
            }
        }
    }

    /// <summary>
    ///Teleports the object back to its starting position and rotation, nullifying velocity.
    /// </summary>
    public void ReturnToOrigin()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep(); // Force Rigidbody to sleep state
        }

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        Debug.Log($"{gameObject.name} returned to origin.", this);
    }

    /// <summary>
    /// Called by the grab detection script when the object is picked up.
    /// </summary>
    public void NotifyGrabbed()
    {
        isHeld = true;
        Debug.Log($"{gameObject.name} notified it is GRABBED.", this);
    }

    /// <summary>
    /// Called by the grab detection script when the object is released.
    /// </summary>
    public void NotifyReleased()
    {
        isHeld = false;
        Debug.Log($"{gameObject.name} notified it is RELEASED.", this);
        // Optional: Immediately check distance upon release
        // FixedUpdate will handle it anyway, but you could force a check:
        // if (Vector3.Distance(transform.position, initialPosition) > maxDistanceFromOrigin)
        // {
        //     ReturnToOrigin();
        // }
    }

    // Optional: Reset position if the component is disabled while held
    void OnDisable()
    {
        if (isHeld)
        {
            // Decide if you want it to return home immediately when disabled while held
            // ReturnToOrigin();
            // Or just mark it as not held so FixedUpdate checks next time it's enabled
            isHeld = false;
        }
    }
}