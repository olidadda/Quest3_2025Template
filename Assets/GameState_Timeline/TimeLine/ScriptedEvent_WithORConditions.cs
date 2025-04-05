using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable] // IMPORTANT: Allows Unity to serialize this class in lists/arrays
public class CompletionConditionEntry
{
    [Tooltip("The BoolCondition to check.")]
    public BoolCondition condition;

    [Tooltip("If TRUE, this specific condition becoming TRUE can trigger event completion EARLY (during sub-event delays or between sub-events, requires 'Allow Skip' on the event to be enabled). If FALSE, this condition is only checked AFTER all sub-events finish.")]
    public bool allowEarlyCompletion = true; // Default to allowing skip

    // Optional: Store the name for easier access later if needed, though can get from condition directly
    // public string conditionName => condition != null ? condition.conditionName : "None";
}


public class ScriptedEvent_WithORConditions : TimelineEventBase
{
    [Space(5)]
    [Header("Boolean Resets after This Event but Before Next Event")]
    [Space(5)]
    public BoolCondition[] conditionsToReset;
    [SerializeField, Tooltip("Readonly display names of reset conditions")]
    private string[] resetConditionNames; // Read-only in Inspector
    [Space(2)]

    //[Space(20)]
    //[Header("Scripted Sub-Events")]
    //[Tooltip("Sequence of actions, sounds, and delays that make up this event.")]
    public List<ScriptedSubEvent> subEvents = new List<ScriptedSubEvent>();


    //[Header("Input Recognition Option: Allow player to skip subevents by satisfying the condition")]
    //public bool allowSkip = false; // Allows skipping sub-events if condition is met

    //[Header("Input Recognition Option: Require subevents to finish before accepting the condition to be fulfilled")]
    //public bool requireAllSubEventsToFinish = false; // Blocks input until sub-events complete
    [Space(45)]

    
    [Header("Event Completion Conditions (OR Logic)")]

    //[Tooltip("This event completes if ANY condition in this list becomes TRUE. Define complex AND logic within the referenced BoolCondition components using their 'Prerequisite Conditions'.")]
    //public List<BoolCondition> completionConditions = new List<BoolCondition>(); // <<< ADD THIS
    //[SerializeField, Tooltip("Read-only display of completion conditions.")]
    //private string[] completionConditionNames; // <<< ADD THIS

    [Header("Event Completion Conditions")] // Simplified Header
    [Tooltip("Define conditions required to complete this event. Use the tickbox to control if a condition can trigger completion early (requires 'Allow Skip' to be enabled on the event).")]
    public List<CompletionConditionEntry> completionConditions = new List<CompletionConditionEntry>(); 
    // <<< Use the new class type                                                                                                      
    // We might not need completionConditionNames anymore if the editor displays it well
    // [SerializeField, Tooltip("Read-only display of completion conditions.")]
    // private string[] completionConditionNames; // <<< Optional: Keep if you still want a flat list display somewhere
    [Space(35)] // Adjust spacing
       

    [Space(30)]
    public TimelineEventBase nextEvent; // 🔹 Defines the next event after this one


    public override void Execute()
    {
        Debug.Log("Starting scripted event: " + eventName);
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

            subEvent.onSubeventTriggered?.Invoke(); //call Unity Event in wrapper


            // ⏳ Optimized Timer 
            float timer = 0f;
            while (timer < subEvent.delayBeforeNext)
            {
                if /*(allowSkip && !requireAllSubEventsToFinish && CheckEarlyCompletionConditions())*/
                    (CheckEarlyCompletionConditions()) // Simply check if any early condition is met
                {
                    Debug.Log("Condition met early! Skipping remaining sub-events.");
                    Transition();
                    yield break;
                }

                yield return new WaitForSeconds(0.3f);
                timer += 0.3f;
            }
        }

        //if (requireAllSubEventsToFinish)
        //{
        //    Debug.Log("Sub-events finished. Resetting condition check...");
        //    // Flush any previous presses as we want them to be effective only once subevents have finished
        //    if (completionConditions != null)
        //    {
        //        foreach (CompletionConditionEntry entry in completionConditions)
        //        {
        //            if (entry != null && entry.condition != null)
        //            {
        //                // Call SetCondition on the 'condition' field INSIDE the entry
        //                entry.condition.SetCondition(false); 
        //            }
        //        }
        //    }
        //}
                
        // Wait for the condition to be met if necessary
        if (!CheckFinalCompletionConditions()) // Check if we aren't already done (e.g., allowSkip=false but conditions met during subevents)
        {
            if (completionConditions != null && completionConditions.Count > 0) // Only wait if conditions are actually defined
            {
                Debug.Log($"'{eventName}': Waiting for completion conditions...");
                yield return new WaitUntil(CheckFinalCompletionConditions); // Wait until ANY completion condition is met
                Debug.Log($"'{eventName}': Completion conditions met!");
            }
            else
            {
                // If no conditions defined, and we got here, the event completes immediately after sub-events.
                // This case is hit if requireAllSubEventsToFinish=true OR allowSkip=false, and the list is empty.
                Debug.Log($"'{eventName}': No completion conditions defined. Completing after sub-events.");
            }
        }
        else
        {
            Debug.Log($"'{eventName}': Completion conditions were already met.");
        }

        ResetConditions(); //Reset specified bool conditions for when I come back to this event
        Transition();
    }


    /// <summary>
    /// Checks if ANY condition MARKED for early completion is currently met.
    /// Used for the 'allowSkip' functionality during sub-events.
    /// </summary>
    /// <returns>True if an early-completion condition is met, false otherwise.</returns>
    private bool CheckEarlyCompletionConditions()
    {
        if (completionConditions == null || completionConditions.Count == 0)
        {
            return false; // Cannot complete early if no conditions exist
        }

        // Check only conditions marked with allowEarlyCompletion = true
        return completionConditions.Any(entry =>
            entry != null &&
            entry.condition != null &&
            entry.allowEarlyCompletion && // <<< The new check
            entry.condition.conditionMet);
    }

    /// <summary>
    /// Checks if ANY condition in the list is currently met, regardless of its early completion setting.
    /// Used after sub-events are finished or for the final state check.
    /// </summary>
    /// <returns>True if any completion condition is met, false otherwise.</returns>
    private bool CheckFinalCompletionConditions()
    {
        if (completionConditions == null || completionConditions.Count == 0)
        {
            // Default to true: If no conditions, event completes after sub-events.
            // Change to 'return false;' if it should never complete without conditions.
            return true;
        }

        // Check ALL conditions in the list
        return completionConditions.Any(entry =>
            entry != null &&
            entry.condition != null &&
            entry.condition.conditionMet);
    }
    ///// <summary>
    ///// Checks if ANY condition in the completionConditions list is met.
    ///// </summary>
    //private bool AllConditionsMet() 
    //{
    //    // If list is null or empty, default to true (completes immediately after subevents)
    //    // Change to 'return false;' if an empty list should mean it never completes automatically.
    //    if (completionConditions == null || completionConditions.Count == 0)
    //    {
    //        return true;
    //    }

    //    // Use Linq.Any to check if any non-null condition in the list has its 'conditionMet' flag set to true.
    //    // Make sure your BoolCondition class has a public way to read 'conditionMet' (e.g., public field or public property/getter).
    //    // Assuming 'conditionMet' is public as per your BoolCondition script:
    //    return completionConditions.Any(cond => cond != null && cond.conditionMet);
    //}

    public override bool CheckCondition()
    {
        // For external classes checking this event
        return CheckFinalCompletionConditions(); 
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
   

    private void OnValidate()
    {       
        UpdateResetConditionNames(); // Updates reset condition names in Inspector
                
        // Check if the completion conditions list is empty or only contains null conditions
        if (completionConditions == null || !completionConditions.Any(entry => entry?.condition != null))
        {
            Debug.LogWarning($"ScriptedEvent '{eventName}' has no valid Completion Conditions set. It will complete immediately after sub-events finish (based on current logic).", this);
        }

        completionConditions?.RemoveAll(item => item == null);
    }
}
