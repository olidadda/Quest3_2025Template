using UnityEngine;

public class ContaminantController : MonoBehaviour
{
    [HideInInspector] public bool isStuck = false;
    [HideInInspector] public float timeToUnstick = 0f;

    // Optional: You could add fields here later if different
    // contaminant types should have different sticking properties.
    // public float baseStickChance = 0.1f;
    // public float minStickDuration = 1.0f;
    // public float maxStickDuration = 3.0f;

    public void GetStuck(float duration)
    {
        isStuck = true;
        timeToUnstick = duration;
        // Optional: Maybe change color or add a small effect?
    }

    public void GetUnstuck()
    {
        isStuck = false;
        timeToUnstick = 0f;
        // Optional: Revert color/effect?
    }

    // Called by the main simulation script while stuck
    public bool UpdateStuckTimer(float deltaTime)
    {
        if (isStuck)
        {
            timeToUnstick -= deltaTime;
            if (timeToUnstick <= 0)
            {
                GetUnstuck();
                return true; // Return true indicates it just became unstuck
            }
        }
        return false; // Still stuck or wasn't stuck
    }
}