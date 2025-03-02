using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable] // Allows each choice to be editable in the Inspector
public class Choice
{
    [Header("Choice")]
    public BoolCondition conditionObject; // External bool condition (optional)
        
    [SerializeField, Tooltip("Displays the name of the assigned condition for clarity")]
    public string choiceName; // Read-only in Inspector
    [Space(35)]

    [Space(15)]
    [SerializeField, Tooltip("Displays the next event name (read-only)")]
    private string nextEventName;
    public TimelineEventBase nextEvent; // Event to transition to
   
    
    [Header("Optional Constraints")]
    public List<Constraint> requiredConditions = new List<Constraint>(); // All must pass

    [System.Serializable]
    public class Constraint
    {
        public BoolCondition condition; // The bool condition to check
        [Header("Condition to be met: if deselected, must be false ")]
        public bool mustBeTrue = true;  // Should this condition be TRUE or FALSE?
    }

    /// <summary>
    /// Updates the read-only string so it shows the current nextEvent name.
    /// </summary>
    public void UpdateNextEventName()
    {
        if (nextEvent != null)
            nextEventName = nextEvent.eventName;
        else
            nextEventName = "None";
    }

    public void UpdateConditionName()
    {
        choiceName = conditionObject != null ? conditionObject.conditionName : "None";
    }
}

public class PlayerChoiceEvent : TimelineEventBase
{
    [Header("Player Choices")]
    public List<Choice> choices = new List<Choice>();

    private bool choiceMade = false; // Ensures only one choice is registered

    public override void Execute()
    {
        Debug.Log("Waiting for player choice: " + eventName);
        choiceMade = false; // Reset choice state when event starts
        SetupPhase();        

        // Start monitoring bool conditions
        StartCoroutine(CheckBoolConditions());
    }

    private IEnumerator CheckBoolConditions()
    {
        while (!choiceMade) // Keeps checking until a choice is made
        {
            foreach (var choice in choices)
            {
                if (choice.conditionObject != null && choice.conditionObject.conditionMet)
                {
                    OnChoiceSelected(choice);
                    yield break; // Stop checking once a choice is made
                }
            }
            yield return null;
        }
    }

    private void OnChoiceSelected(Choice chosen)
    {
        if (choiceMade) return; // Prevent multiple triggers
        choiceMade = true;

        // 🚨 Check constraints before proceeding
        foreach (var constraint in chosen.requiredConditions)
        {
            if (constraint.condition != null)
            {
                bool conditionMet = constraint.condition.conditionMet;

                if ((constraint.mustBeTrue && !conditionMet) || (!constraint.mustBeTrue && conditionMet))
                {
                    Debug.LogWarning($" Choice '{chosen.choiceName}' is blocked! Constraint '{constraint.condition.conditionName}' must be {(constraint.mustBeTrue ? "TRUE" : "FALSE")}.");
                    choiceMade = false; // Allow another choice
                    return;
                }
            }
        }

        Debug.Log($"Player selected: {chosen.choiceName}");       

        // Trigger the next event
        if (chosen.nextEvent != null)
        {
            Debug.Log($" Transitioning to next event: {chosen.nextEvent.eventName}");
            chosen.nextEvent.Execute(); // 🔹 Directly execute the next event
        }
        else
        {
            Debug.LogWarning($"No next event assigned for choice: {chosen.choiceName}");
        }
    } 

    public override bool CheckCondition() => false; // Manually triggered by button clicks

    /// <summary>
    /// Ensures the 'nextEventName' fields are always up to date in the Inspector.
    /// </summary>
    private void OnValidate()
    {
        for (int i = 0; i < choices.Count; i++)
        {
            var tempChoice = choices[i];
            tempChoice.UpdateNextEventName();
            tempChoice.UpdateConditionName();
            choices[i] = tempChoice;  // Important for structs
        }

        
    }

}
