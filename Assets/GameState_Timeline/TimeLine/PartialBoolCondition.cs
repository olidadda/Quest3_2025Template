using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Inherits from BoolCondition for compatibility
public class PartialBoolCondition : BoolCondition
{
    [Header("Partial Source Conditions")]
    [Tooltip("ALL Bool Conditions in this list must be TRUE (in addition to base prerequisites) for this condition's state to become TRUE.")]
    [SerializeField] private List<BoolCondition> partialSourceConditions = new List<BoolCondition>();

    [Tooltip("Check the source conditions every frame? Turn OFF for performance if you trigger checks manually via CheckSourcesAndUpdate().")]
    [SerializeField] private bool continuousCheck = true;

    // --- Unity Methods ---

    protected void Start() // Use protected if base class has Start, otherwise private/void
    {
        // Perform an initial check to set the state correctly at the beginning
        CheckSourcesAndUpdate();
    }

    void Update()
    {
        // Continuously check source conditions if enabled
        if (continuousCheck)
        {
            CheckSourcesAndUpdate();
        }
    }

    // --- Core Logic ---

    /// <summary>
    /// Checks if all conditions in the partialSourceConditions list are currently met.
    /// </summary>
    private bool AreAllSourcesMet()
    {
        if (partialSourceConditions == null || partialSourceConditions.Count == 0)
        {
            // If no sources are required, this part of the check passes.
            return true;
        }

        // Use Linq's All() for a concise check:
        // Returns true if the list is empty OR if every item in the list
        // is not null AND its conditionMet property is true.
        return partialSourceConditions.All(condition => condition != null && condition.conditionMet);
    }

    /// <summary>
    /// Checks source conditions and base prerequisites, then updates the main conditionMet state.
    /// Public if you want to trigger checks manually from other scripts when continuousCheck is off.
    /// </summary>
    public void CheckSourcesAndUpdate()
    {
        bool basePrereqsMet = base.CheckPrerequisites(); // Check prerequisites from BoolCondition base class
        bool allSourcesMet = AreAllSourcesMet();

        // The final desired state requires both base prereqs and all source partials to be met
        bool desiredState = basePrereqsMet && allSourcesMet;

        // Get the current state and update if changed.
        // Assumes base class uses 'conditionMet' or protected '_conditionMet'.
        // We'll directly modify the base class's state variable.
        // ** This bypasses the base SetCondition logic/checks **
        if (conditionMet != desiredState)
        {
            // If base uses private _conditionMet, make it protected in BoolCondition.cs
            // protected bool _conditionMet = false;
            // Then use: _conditionMet = desiredState;
            // If base uses public conditionMet (less ideal), use:
            conditionMet = desiredState; // Assuming public access for now

            Debug.Log($"PartialBoolCondition '{conditionName}' main state updated to {conditionMet} (BasePrereqs: {basePrereqsMet}, SourcesMet: {allSourcesMet}).");
        }
    }


    // --- Overriding Base Methods (Recommended) ---

    // Prevent external calls from manually setting the state when controlled by sources.
    public override void SetCondition(bool state)
    {
        Debug.LogWarning($"SetCondition({state}) called on PartialBoolCondition '{conditionName}'. State is managed by source conditions. Call ignored.", this);
        // Do not call base.SetCondition(state);
    }

    public override void ToggleCondition()
    {
        Debug.LogWarning($"ToggleCondition called on PartialBoolCondition '{conditionName}'. State is managed by source conditions. Call ignored.", this);
        // Do not call base.ToggleCondition();
    }

    public override void ResetCondition()
    {
        // Resetting this component doesn't reset its sources.
        // So, the most logical action is to ignore the call or just re-check.
        Debug.LogWarning($"ResetCondition called on PartialBoolCondition '{conditionName}'. State is managed by source conditions. Call ignored (state will reflect sources).", this);
        // Optionally force a re-check if not using continuous checking:
        // if (!continuousCheck) CheckSourcesAndUpdate();
    }


    // --- Editor Validation & Readability ---
    private void OnValidate()
    {
        // base.OnValidate(); // Call if base class has validation logic

        // Remove null entries added in the inspector
        //partialSourceConditions?.RemoveAll(item => item == null);

        // Prevent self-referencing
        if (partialSourceConditions != null && partialSourceConditions.Contains(this))
        {
            Debug.LogError($"Error: PartialBoolCondition '{conditionName}' references itself in 'Partial Source Conditions'! Removing self.", this);
            partialSourceConditions.Remove(this);
        }

        // Readability Check (Logs warnings in Console)
        if (partialSourceConditions != null)
        {
            for (int i = 0; i < partialSourceConditions.Count; i++)
            {
                BoolCondition source = partialSourceConditions[i];
                if (source != null)
                {
                    // Check if the source condition has a meaningful name in *its* inspector
                    if (string.IsNullOrEmpty(source.conditionName) || source.conditionName.Contains("Unnamed") || source.conditionName.Contains("Condition"))
                    {
                        Debug.LogWarning($"Readability Suggestion: Source Condition {i} ('{source.gameObject.name}') on '{this.conditionName}' has a generic name ('{source.conditionName}'). Consider giving it a specific name in its own Inspector.", source.gameObject);
                    }
                }
            }
        }

        // Warn if aggregating but list is empty (means it only relies on base prerequisites)
        if (partialSourceConditions != null && partialSourceConditions.Count == 0)
        {
            Debug.Log($"Info: PartialBoolCondition '{conditionName}' has an empty 'Partial Source Conditions' list. Its state will depend only on its base prerequisites ('Needed Conditions').", this);
        }
    }

    // --- Helper to display names in the Inspector (Requires Custom Editor - More Complex) ---
    // To *directly* show the names next to the list items requires a Custom Editor script.
    // The OnValidate check above provides console warnings as a simpler alternative.
}
