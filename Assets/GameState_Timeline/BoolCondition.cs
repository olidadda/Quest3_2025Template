using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoolCondition : MonoBehaviour
{
    [Header("Condition Name (for Inspector readability)")]
    public string conditionName; // Set a name like "PlayerPressedButton"

    public bool conditionMet = false; // This is what ScriptedEvent checks

    public void SetCondition(bool state)
    {
        conditionMet = state;
    }

    public void ToggleCondition()
    {
        conditionMet = !conditionMet;
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
    }
}
