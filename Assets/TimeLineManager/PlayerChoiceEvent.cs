using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable] // Allows each choice to be editable in the Inspector
public class Choice
{
    public string choiceName; // Displayed in Inspector    
    public BoolCondition conditionObject; // External bool condition (optional)
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
            TimeLine.Instance.TriggerEvent(chosen.nextEvent);
        }
        else
        {
            Debug.LogWarning($"No next event assigned for choice: {chosen.choiceName}");
        }
    } 

    public override bool CheckCondition() => false; // Manually triggered by button clicks
}
