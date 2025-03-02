using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// A simplified scripted event that only plays through sub-events in sequence,
/// each with a delay, and then transitions to the next event.
/// No conditions or user input are involved.
/// </summary>
public class SimpleScriptedEvent : TimelineEventBase
{
    [Header("Sub-Events (No Conditions)")]
    public List<ScriptedSubEvent> subEvents = new List<ScriptedSubEvent>();

    [Header("Optional Next Event")]
    public TimelineEventBase nextEvent;

    // Called by the timeline or your own logic
    public override void Execute()
    {
        Debug.Log("Starting simple scripted event: " + eventName);
        SetupPhase();  // If your TimelineEventBase does any preparation
        StartCoroutine(PlaySubEvents());
    }

    private IEnumerator PlaySubEvents()
    {
        foreach (var subEvent in subEvents)
        {
            // Print a message if provided
            if (!string.IsNullOrEmpty(subEvent.message))
                Debug.Log(subEvent.message);

            // Play audio if both clip and source are assigned
            if (subEvent.audioClip != null && subEvent.audioSource != null)
                subEvent.audioSource.PlayOneShot(subEvent.audioClip);

            // Activate / Deactivate objects
            foreach (var obj in subEvent.activateObjects)
                obj.SetActive(true);
            foreach (var obj in subEvent.deactivateObjects)
                obj.SetActive(false);

            // Delay until we move to the next sub-event
            yield return new WaitForSeconds(subEvent.delayBeforeNext);
        }

        // Once all sub-events have played, proceed
        Transition();
    }

    // Moves to the next event (if one is assigned)

    public override bool CheckCondition() => false; // Manually triggered by button clicks

    private void Transition()
    {
        if (nextEvent != null)
        {
            Debug.Log($"{eventName} complete, transitioning to {nextEvent.eventName}");
            nextEvent.Execute();
        }
        else
        {
            Debug.LogWarning($"⚠️ {eventName} has no next event assigned!");
        }
    }
}
