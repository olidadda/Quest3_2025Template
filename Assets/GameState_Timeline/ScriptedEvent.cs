using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
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
    [Space(20)]
    [Header("Resets Boolean Conditions before running Event")]
    [Space(5)]
    public BoolCondition[] conditionsToReset;
    [SerializeField, Tooltip("Displays names of reset conditions")]
    private string[] resetConditionNames; // Read-only in Inspector
    [Space(2)]
    [Space(20)]
    [Header("Scripted Sub-Events")]
    public List<ScriptedSubEvent> subEvents = new List<ScriptedSubEvent>();
    [Header("Input Recognition Option: Allow player to skip subevents by satisfying the condition")]
    public bool allowSkip = false; // Allows skipping sub-events if condition is met
    [Header("Input Recognition Option: Require subevents to finish before accepting the condition to be fulfilled")]
    public bool requireAllSubEventsToFinish = false; // Blocks input until sub-events complete
    [Space(45)]

    [Header("Event Completion Condition")]
    public BoolCondition conditionObject; // Direct reference to a bool-holding script
    [SerializeField, Tooltip("Displays the name of the assigned condition for clarity")]
    private string assignedConditionName; // Read-only in Inspector
    [Space(35)]

    [Tooltip("Any additional conditions that must also be met.")]
    public List<BoolCondition> additionalConditions; // Extra conditions to check  
    [SerializeField, Tooltip("Names of any assigned additional conditions (read-only)")]
    private string[] additionalConditionNames; // Read-only in Inspector

    [Space(30)]
    public TimelineEventBase nextEvent; // 🔹 Defines the next event after this one


    public override void Execute()
    {
        Debug.Log("Starting scripted event: " + eventName);
        ResetConditions(); //Reset specified bool conditions before running the event
        SetupPhase();        
        StartCoroutine(PlaySubEvents());
    }

    
    private void ResetConditions()
    {
        if (conditionsToReset != null)
        {
            List<string> conditionNames = new List<string>();
            foreach (var condition in conditionsToReset)
            {
                if (condition != null)
                {
                    condition.ResetCondition();
                    conditionNames.Add(condition.conditionName);
                    Debug.Log($"Resetting condition: {condition.conditionName}");
                }
            }
            resetConditionNames = conditionNames.ToArray();
        }
    }

    
    private IEnumerator PlaySubEvents()
    {
        /*If requireAllSubEventsToFinish is false:
        The script checks allowSkip && conditionObject.conditionMet during sub-events.
        If that’s ever true, it will skip remaining sub - events and transition immediately.

        If requireAllSubEventsToFinish is true: The script allows sub-events to play out fully.
        Only after they all complete do you reset conditionObject.conditionMet to false.
        Then it waits(yield return new WaitUntil(() => conditionObject.conditionMet)) for a fresh signal to trigger the transition.*/


        foreach (var subEvent in subEvents)
        {
            if (!string.IsNullOrEmpty(subEvent.message))
                Debug.Log(subEvent.message);

            if (subEvent.audioClip != null && subEvent.audioSource != null)
                subEvent.audioSource.PlayOneShot(subEvent.audioClip);

            foreach (var obj in subEvent.activateObjects) obj.SetActive(true);
            foreach (var obj in subEvent.deactivateObjects) obj.SetActive(false);



            // ⏳ Optimized Timer 
            float timer = 0f;
            while (timer < subEvent.delayBeforeNext)
            {
                if (!requireAllSubEventsToFinish && allowSkip && AllConditionsMet())
                {
                    Debug.Log("Condition met early! Skipping remaining sub-events.");
                    Transition();
                    yield break;
                }

                yield return new WaitForSeconds(0.3f); 
                timer += 0.3f;
            }
        }  

        if (requireAllSubEventsToFinish)
        {
            Debug.Log("Sub-events finished. Resetting condition check...");
            conditionObject.conditionMet = false; // Flush any previous presses as we want them to be effective only once subevents have finished
        }

        // Wait for the condition to be met
        if (conditionObject != null)
        {
            Debug.Log("Waiting for external condition...");
            yield return new WaitUntil(AllConditionsMet);
        }

        Transition();
    }

    /// <summary>
    /// Checks if the primary condition and all additional conditions (if any) are met.
    /// </summary>
    private bool AllConditionsMet()
    {
        // If there is no primary condition, consider it met by default
        bool primaryMet = (conditionObject == null) || conditionObject.conditionMet;
        if (!primaryMet) return false;

        // Check additional conditions
        if (additionalConditions != null)
        {
            foreach (var c in additionalConditions)
            {
                if (c != null && !c.conditionMet)
                {
                    return false; // Fails if any assigned condition is not met
                }
            }
        }
        return true;
    }

    public override bool CheckCondition()
    {
        // For external classes checking this event
        return AllConditionsMet();
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
    private void UpdateResetConditionNames()
    {
        if (conditionsToReset != null)
        {
            List<string> conditionNames = new List<string>();
            foreach (var condition in conditionsToReset)
            {
                if (condition != null)
                    conditionNames.Add(condition.conditionName);
            }
            resetConditionNames = conditionNames.ToArray();
        }
        else
        {
            resetConditionNames = new string[0]; // Empty list if no conditions exist
        }
    }
    private void UpdateConditionName()
    {
        assignedConditionName = conditionObject != null ? conditionObject.conditionName : "None";
    }
    private void UpdateAdditionalConditionNames()
    {
        if (additionalConditions != null && additionalConditions.Count > 0)
        {
            List<string> names = new List<string>();
            foreach (var c in additionalConditions)
            {
                if (c != null)
                    names.Add(c.conditionName);
            }
            additionalConditionNames = names.ToArray();
        }
        else
        {
            additionalConditionNames = new string[0];
        }
    }

    private void OnValidate()
    {
        UpdateConditionName(); // Ensure UI updates when assigning a condition
        UpdateResetConditionNames(); // Updates reset condition names in Inspector
        UpdateAdditionalConditionNames();  // Updates the names of secondary conditions in the Inspector

        // Check if both the primary condition is null, and
        // additionalConditions is either null or empty:
        if (conditionObject == null &&
            (additionalConditions == null || additionalConditions.Count == 0))
        {
            Debug.LogWarning($"ScriptedEvent '{eventName}' has no primary or secondary conditions set! " +
                             "Without at least one condition, the event will rely solely on sub-event execution.");
        }
    }
}

