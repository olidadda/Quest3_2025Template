using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoolCondition : MonoBehaviour
{
    [Header("Condition Name (for Inspector readability)")]
    public string conditionName; // Set a name like "PlayerPressedButton"

    public bool conditionMet = false; // This is what ScriptedEvent checks

    [Header("Prerequisite Conditions")]
    [Tooltip("All of these conditions must be TRUE before this condition can be set to TRUE")]
    [SerializeField] List<BoolCondition> neededConditions = new List<BoolCondition>();

   

    public virtual void SetCondition(bool state)
    {
        // If trying to set to TRUE and there are prerequisites
        if (state && neededConditions.Count > 0)
        {
            // Check if all needed conditions are met
            foreach (var condition in neededConditions)
            {
                if (condition != null && !condition.conditionMet)
                {
                    Debug.LogWarning($"Cannot set '{conditionName}' to TRUE because prerequisite '{condition.conditionName}' is not met.");
                    return; // Don't set the condition
                }
            }
        }

        // If we get here, either setting to FALSE or all prerequisites are met
        conditionMet = state;
    }
    public bool CanBeSet()
    {
        if (neededConditions.Count == 0)
            return true;

        foreach (var condition in neededConditions)
        {
            if (condition != null && !condition.conditionMet)
                return false;
        }

        return true;
    }

    public virtual void ToggleCondition()
    {
        if (!conditionMet || CanBeSet())
        {
            conditionMet = !conditionMet;
        }
        else
        {
            Debug.LogWarning($"Cannot toggle '{conditionName}' because prerequisites are not met.");
        }
    }

    public bool CheckPrerequisites() // Or CanBeSet() if you prefer that name
    {
        // If no prerequisites are assigned, it can always be set.
        if (neededConditions == null || neededConditions.Count == 0)
        {
            return true;
        }

        // Check each condition in the list
        foreach (var condition in neededConditions)
        {
            // If a condition reference is null (missing) or its state is false, prerequisites fail.
            if (condition == null || !condition.conditionMet)
            {
                // Optional: Log which specific prerequisite failed, useful for debugging
                // if (condition != null) { // Check condition isn't null before accessing its name
                //     Debug.LogWarning($"Prerequisite '{condition.conditionName}' for '{this.conditionName}' is not met.");
                // } else {
                //     Debug.LogWarning($"A null prerequisite condition exists for '{this.conditionName}'.");
                // }
                return false; // Found a prerequisite not met (or null)
            }
        }

        // If the loop completes without returning false, all prerequisites are met.
        return true;
    }

    public virtual void ResetCondition()
    {
        if (conditionMet)
        {
            conditionMet = false; // Auto-reset after being checked            
        }        
    }

    public bool GetCondition()
    {
        if (conditionMet)
        {
            return true;           
        }
        return false;
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(conditionName))
            conditionName = gameObject.name;

        if (conditionMet)
        {
            Debug.LogWarning($"WARNING: BoolCondition '{conditionName}' is still TRUE in Editor! Should it be reset?");
        }

        // Check for circular dependencies
        foreach (var condition in neededConditions)
        {
            if (condition == this)
            {
                Debug.LogError($"Error: BoolCondition '{conditionName}' references itself as a prerequisite!");
                neededConditions.Remove(this); // Remove self-reference
            }
        }
    }
}
