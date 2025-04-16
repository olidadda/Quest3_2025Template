using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    // --- Parameters (Tune in Inspector) ---
    [Header("Rotation Speed")]
    [Tooltip("Minimum rotation speed in degrees per second.")]
    [SerializeField] private float minSpeed = 15f;

    [Tooltip("Maximum rotation speed in degrees per second.")]
    [SerializeField] private float maxSpeed = 60f;
    // ------------------------------------

    // --- Private Variables ---
    private Vector3 rotationAxis;
    private float rotationSpeed;
    // -----------------------

    // OnEnable is called whenever the GameObject becomes active
    // (Perfect for pooled objects when they are retrieved)
    void OnEnable()
    {
        // --- START CHANGE: Assign random axis and speed ONCE on enable ---
        // Choose a random axis ONCE when the object is activated
        rotationAxis = Random.insideUnitSphere.normalized;

        // Choose a random speed ONCE when the object is activated
        rotationSpeed = Random.Range(minSpeed, maxSpeed);
        // --- END CHANGE ---
    }

    // Update is called once per frame
    void Update()
    {
        // --- START CHANGE: Apply continuous rotation ---
        // Continuously rotate around the chosen axis at the chosen speed
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        // --- END CHANGE ---
    }
}
