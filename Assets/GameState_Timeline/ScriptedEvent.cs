using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Allows sub-events to be editable in Inspector
public class ScriptedSubEvent
{
    public string message;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public GameObject[] activateObjects;
    public GameObject[] deactivateObjects;
    public float delayBeforeNext = 0f; // Time before executing next subevent
    
}

public class ScriptedEvent : TimelineEventBase
{
    [Header("Scripted Sub-Events")]
    public List<ScriptedSubEvent> subEvents = new List<ScriptedSubEvent>();

    [Header("Event Completion Condition")]
    public BoolCondition conditionObject; // Direct reference to a bool-holding script
    public TimelineEventBase nextEvent; // 🔹 Defines the next event after this one

    [SerializeField, Tooltip("Displays the name of the assigned condition for clarity")]
    private string assignedConditionName; // Read-only in Inspector

    public override void Execute()
    {
        Debug.Log("Starting scripted event: " + eventName);
        SetupPhase();
        UpdateConditionName(); // Ensure condition name is updated
        StartCoroutine(PlaySubEvents());
    }

    private IEnumerator PlaySubEvents()
    {
        foreach (var subEvent in subEvents)
        {
            if (!string.IsNullOrEmpty(subEvent.message))
                Debug.Log(subEvent.message);

            if (subEvent.audioClip != null && subEvent.audioSource != null)
                subEvent.audioSource.PlayOneShot(subEvent.audioClip);

            foreach (var obj in subEvent.activateObjects) obj.SetActive(true);
            foreach (var obj in subEvent.deactivateObjects) obj.SetActive(false);

            yield return new WaitForSeconds(subEvent.delayBeforeNext);
        }

        // Wait for the condition to be met
        if (conditionObject != null)
        {
            Debug.Log("Waiting for external condition...");
            yield return new WaitUntil(() => conditionObject.conditionMet);
        }

        Transition();
    }


    public override bool CheckCondition()
    {
        return conditionObject == null || conditionObject.conditionMet;
    }

    //public void Transition()
    //{
    //    if (nextEvent != null)
    //    {
    //        Debug.Log($"{eventName} complete, transitioning to {nextEvent.eventName}");
    //        TimeLine.Instance.TriggerEvent(nextEvent);
    //    }
    //    else
    //    {
    //        Debug.LogWarning($" {eventName} has no next event assigned!");
    //    }
    //}

    public void Transition()
    {
        if (nextEvent != null)
        {
            Debug.Log($"{eventName} complete, transitioning to {nextEvent.eventName}");
            nextEvent.Execute(); // 🔹 Directly execute the next event
        }
        else
        {
            Debug.LogWarning($"⚠️ {eventName} has no next event assigned!");
        }
    }

    private void UpdateConditionName()
    {
        assignedConditionName = conditionObject != null ? conditionObject.conditionName : "None";
    }

    private void OnValidate()
    {
        UpdateConditionName(); // Ensure UI updates when assigning a condition
    }
}

