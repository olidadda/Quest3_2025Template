using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FaderValueCalculator : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Transform of the fader knob object that moves.")]
    [SerializeField] private Transform faderKnob;

    [Tooltip("An empty GameObject marking the minimum position of the fader's track.")]
    [SerializeField] private Transform minPositionMarker;

    [Tooltip("An empty GameObject marking the maximum position of the fader's track.")]
    [SerializeField] private Transform maxPositionMarker;

    [Header("Configuration")]
    [Tooltip("The local axis along which the fader knob moves.")]
    [SerializeField] private Axis movementAxis = Axis.Y; // Adjust based on your setup (Y=Green, X=Red, Z=Blue)

    [Header("Output")]
    [Tooltip("The calculated normalized value (0.0 to 1.0)")]
    [SerializeField][Range(0f, 1f)] private float currentValue; // Serialized for Inspector debugging

    // Public property for other scripts to access the value
    public float CurrentValue => currentValue;

    // Event triggered when the value changes
    public UnityEvent<float> OnValueChanged;

    private float previousValue = -1f; // Initialize to a value outside the 0-1 range

    // Enum to make selecting the axis easier in the Inspector
    public enum Axis { X, Y, Z }

    void Update()
    {
        if (faderKnob == null || minPositionMarker == null || maxPositionMarker == null)
        {
            Debug.LogWarning("FaderValueCalculator: References not set!");
            return;
        }

        // Get positions in world space
        Vector3 knobPos = faderKnob.position;
        Vector3 minPos = minPositionMarker.position;
        Vector3 maxPos = maxPositionMarker.position;

        float currentPosComponent = 0f;
        float minPosComponent = 0f;
        float maxPosComponent = 0f;

        // Extract the relevant position component based on the selected axis
        switch (movementAxis)
        {
            case Axis.X:
                currentPosComponent = knobPos.x;
                minPosComponent = minPos.x;
                maxPosComponent = maxPos.x;
                break;
            case Axis.Y:
                currentPosComponent = knobPos.y;
                minPosComponent = minPos.y;
                maxPosComponent = maxPos.y;
                break;
            case Axis.Z:
                currentPosComponent = knobPos.z;
                minPosComponent = minPos.z;
                maxPosComponent = maxPos.z;
                break;
        }

        // Calculate total travel distance on the chosen axis
        float totalDistance = maxPosComponent - minPosComponent;

        // Avoid division by zero if min and max points are the same
        if (Mathf.Approximately(totalDistance, 0f))
        {
            currentValue = 0.5f; // Or 0f or 1f depending on preference
        }
        else
        {
            // Calculate how far along the knob is
            float currentTravel = currentPosComponent - minPosComponent;

            // Calculate normalized value and clamp it between 0 and 1
            currentValue = Mathf.Clamp01(currentTravel / totalDistance);
        }

        // Check if the value has actually changed significantly
        if (!Mathf.Approximately(currentValue, previousValue))
        {
            OnValueChanged.Invoke(currentValue); // Trigger the event
            previousValue = currentValue;
            // --- Optional Debug Line ---
            // Debug.Log($"Fader Value: {currentValue:F2}");
        }
    }
}
