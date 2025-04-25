using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Continuously rotates the GameObject this script is attached to around a specified axis.
/// Provides controls for axis, speed, and activation state via the Inspector and public methods.
/// </summary>
public class ContinuousRotator : MonoBehaviour
{
    [Header("Rotation Settings")]

    [SerializeField]
    [Tooltip("The axis around which the object will rotate (in local space). This vector will be normalized automatically.")]
    private Vector3 rotationAxis = Vector3.up; // Default to rotating around the Y-axis

    [SerializeField]
    [Tooltip("The speed of rotation in degrees per second.")]
    private float rotationSpeed = 45f; // Default speed

    [Header("Activation")]

    [SerializeField]
    [Tooltip("If true, the object will start rotating automatically when the scene starts.")]
    private bool rotateOnStart = true;

    // --- Private State ---
    private bool isRotating = false;
    private Vector3 normalizedRotationAxis;

    // --- Unity Lifecycle Methods ---

    void Awake()
    {
        // Normalize the axis at the start to ensure consistent speed regardless of input vector magnitude
        // Check if the vector is not zero before normalizing to avoid errors/warnings
        if (rotationAxis.sqrMagnitude > Mathf.Epsilon)
        {
            normalizedRotationAxis = rotationAxis.normalized;
        }
        else
        {
            // If the axis is zero, default to something sensible (like Vector3.up) and warn the user
            Debug.LogWarning($"Rotation axis on {gameObject.name} was zero. Defaulting to Vector3.up.", this);
            normalizedRotationAxis = Vector3.up;
        }
    }

    void Start()
    {
        // Set the initial rotation state based on the inspector setting
        isRotating = rotateOnStart;
    }

    void Update()
    {
        // Only rotate if the isRotating flag is true
        if (isRotating)
        {
            // Calculate the rotation amount for this frame (degrees)
            float rotationAmount = rotationSpeed * Time.deltaTime;

            // Apply the rotation around the specified normalized axis in local space
            // Use Space.Self to rotate around the object's own axis
            transform.Rotate(normalizedRotationAxis, rotationAmount, Space.Self);

            // Alternative: To rotate around a world axis, use Space.World:
            // transform.Rotate(normalizedRotationAxis, rotationAmount, Space.World);
        }
    }

    // --- Public Control Methods ---

    /// <summary>
    /// Starts the rotation.
    /// </summary>
    public void StartRotation()
    {
        isRotating = true;
    }

    /// <summary>
    /// Stops the rotation.
    /// </summary>
    public void StopRotation()
    {
        isRotating = false;
    }

    /// <summary>
    /// Toggles the current rotation state (on/off).
    /// </summary>
    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    /// <summary>
    /// Directly sets whether the object should be rotating.
    /// </summary>
    /// <param name="shouldRotate">True to enable rotation, false to disable.</param>
    public void SetRotationActive(bool shouldRotate)
    {
        isRotating = shouldRotate;
    }

    /// <summary>
    /// Checks if the object is currently set to rotate.
    /// </summary>
    /// <returns>True if rotation is active, false otherwise.</returns>
    public bool IsRotating()
    {
        return isRotating;
    }
}
