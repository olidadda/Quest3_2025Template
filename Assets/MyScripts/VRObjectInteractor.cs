using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;       // TextMeshPro namespace
using UnityEngine.Events; // For UnityEvent

public class VRObjectInteractor : MonoBehaviour
{
    [Header("External Components")]
    [Tooltip("AudioSource component used for playing all clips.")]
    [SerializeField] private AudioSource sharedAudioSource;
    [Tooltip("Text Mesh Pro UI element to display object explanations.")]
    [SerializeField] private TextMeshPro infoTextDisplay;

    [Header("Interaction Setup")]
    [Tooltip("List of GameObjects that become interactive.")]
    [SerializeField] private List<GameObject> interactiveObjects;
    [Tooltip("Audio clip to play when interaction phase starts.")]
    [SerializeField] private AudioClip introAudio;
    [Tooltip("The Tag assigned to your VR Hand colliders.")]
    [SerializeField] public string vrHandTag = "VRHand"; // IMPORTANT: Make sure your hands have this tag!

    [Header("Completion Logic")]
    [Tooltip("Time in seconds AFTER intro audio finishes before triggering completion if not all objects are touched.")]
    [SerializeField] private float interactionTimeout = 30.0f;
    [Tooltip("Event triggered when all objects are touched OR the timeout occurs.")]
    public UnityEvent OnInteractionComplete;

    // --- Private State ---
    private HashSet<GameObject> touchedObjects = new HashSet<GameObject>();
    private bool isInteractionActive = false; // Becomes true AFTER intro audio finishes
    private bool introAudioPlayed = false;
    private bool completionTriggered = false;
    private float currentTimeoutTimer = 0f;

    void Awake()
    {
        // Basic validation
        if (sharedAudioSource == null)
        {
            Debug.LogError("VRObjectInteractor: Shared Audio Source is not assigned!", this);
            this.enabled = false;
            return;
        }
        if (infoTextDisplay == null)
        {
            Debug.LogError("VRObjectInteractor: Info Text Display is not assigned!", this);
            this.enabled = false;
            return;
        }
        if (interactiveObjects == null || interactiveObjects.Count == 0)
        {
            Debug.LogWarning("VRObjectInteractor: Interactive Objects list is empty.", this);
            // Allow continuing, but interaction won't work fully
        }
    }

    void Update()
    {
        // 1. Check if intro audio has finished to enable interaction
        if (introAudioPlayed && !isInteractionActive && !completionTriggered)
        {
            // Check if the specific intro audio is done OR if no intro audio was assigned
            if (introAudio == null || !sharedAudioSource.isPlaying)
            {
                Debug.Log("Intro audio finished or skipped. Interaction Enabled.");
                isInteractionActive = true;
                currentTimeoutTimer = interactionTimeout; // Start timeout timer
                if (infoTextDisplay != null) infoTextDisplay.text = "Explore the components."; // Initial prompt
            }
        }

        // 2. Handle timeout only when interaction is truly active
        if (isInteractionActive && !completionTriggered)
        {
            currentTimeoutTimer -= Time.deltaTime;
            if (currentTimeoutTimer <= 0f)
            {
                TriggerCompletion("Timeout");
            }
        }
    }

    // --- Public Methods to Control the Interaction Phase ---

    /// <summary>
    /// Call this externally (e.g., after animation finishes) to begin the interaction sequence.
    /// </summary>
    public void StartInteraction()
    {
        if (introAudioPlayed || completionTriggered)
        {
            Debug.LogWarning("Interaction already started or completed. Call StopInteraction first if restarting.", this);
            return;
        }

        Debug.Log("VRObjectInteractor: Starting Interaction Phase...");
        ResetStateInternally(); // Ensure clean state

        // Play intro audio if assigned
        if (introAudio != null && sharedAudioSource != null)
        {
            sharedAudioSource.Stop(); // Stop any previous sounds
            sharedAudioSource.PlayOneShot(introAudio);
            introAudioPlayed = true;
            // Interaction will enable in Update() once this finishes
        }
        else
        {
            // No intro audio, enable interaction immediately
            introAudioPlayed = true; // Mark as 'done'
            isInteractionActive = true;
            currentTimeoutTimer = interactionTimeout;
            if (infoTextDisplay != null) infoTextDisplay.text = "Explore the components by touching them to see some information on the text panel.";
            //Debug.Log("No intro audio assigned. Interaction Enabled Immediately.");
        }
    }

    /// <summary>
    /// Call this externally (e.g., when recombination starts) to stop interaction.
    /// </summary>
    public void StopInteraction()
    {
        //Debug.Log("VRObjectInteractor: Stopping Interaction Phase.");
        ResetStateInternally();
    }


    // --- Internal Methods ---

    /// <summary>
    /// Called by VRInteractableObject component when a VR hand interacts.
    /// </summary>
    public void NotifyObjectTouched(GameObject touchedObject)
    {
        if (!isInteractionActive || completionTriggered) return; // Only process if interaction is live

        FestoObjectInfo info = touchedObject.GetComponent<FestoObjectInfo>();
        if (info == null)
        {
            Debug.LogWarning($"Touched object '{touchedObject.name}' is missing FestoObjectInfo component.", touchedObject);
            return;
        }

        Debug.Log($"VRObjectInteractor: Handling touch for {touchedObject.name}");

        // Play specific audio
        if (sharedAudioSource != null && info.explanationAudio != null)
        {
            sharedAudioSource.Stop(); // Stop intro/previous object's audio
            sharedAudioSource.PlayOneShot(info.explanationAudio);
        }

        // Display specific text
        if (infoTextDisplay != null)
        {
            infoTextDisplay.text = info.explanationText;
        }

        // Track unique touches
        if (touchedObjects.Add(touchedObject))
        {
            Debug.Log($"New object registered. Total touched: {touchedObjects.Count}/{GetValidObjectCount()}");
        }

        // Reset timeout timer on interaction
        currentTimeoutTimer = interactionTimeout;

        // Check if all objects have been touched
        CheckForCompletion();
    }

    private void CheckForCompletion()
    {
        if (completionTriggered) return;

        if (touchedObjects.Count >= GetValidObjectCount())
        {
            TriggerCompletion("All Objects Touched");
        }
    }

    private void TriggerCompletion(string reason)
    {
        if (completionTriggered) return; // Ensure it only runs once

        Debug.Log($"VRObjectInteractor: Interaction Complete! Reason: {reason}");
        completionTriggered = true;
        isInteractionActive = false; // Disable further interaction

        if (sharedAudioSource != null) sharedAudioSource.Stop();
        // Optional: Set completion text
        // if (infoTextDisplay != null) infoTextDisplay.text = "Interaction Complete.";

        OnInteractionComplete?.Invoke(); // Fire the event
    }

    private void ResetStateInternally()
    {
        touchedObjects.Clear();
        isInteractionActive = false;
        introAudioPlayed = false;
        completionTriggered = false;
        currentTimeoutTimer = 0f;

        if (sharedAudioSource != null) sharedAudioSource.Stop();
        if (infoTextDisplay != null) infoTextDisplay.text = ""; // Clear text
    }

    private int GetValidObjectCount()
    {
        if (interactiveObjects == null) return 0;
        int count = 0;
        foreach (var obj in interactiveObjects) { if (obj != null) count++; }
        return count;
    }
}