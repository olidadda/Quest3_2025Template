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

   

    public void SetCondition(bool state)
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

    public void ToggleCondition()
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

    public void ResetCondition()
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
