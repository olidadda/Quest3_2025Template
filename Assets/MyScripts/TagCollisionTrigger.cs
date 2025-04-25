using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TagCollisionTrigger : MonoBehaviour
{
    [Header("Tag Matching")]
    //[Tooltip("One of the required tags for the collision pair.")]
    //public string requiredTag1 = "Untagged"; // Set default to avoid errors
    [Tooltip("The required tag for object to collide with.")]
    public string requiredTag = "Untagged"; // Set default to avoid errors

    [Header("Effects")]
    [Tooltip("The BoolCondition script instance whose condition will be set to true.")]
    public BoolCondition targetCondition;

    [Tooltip("Event triggered when matching tags collide.")]
    public UnityEvent onMatch;

    // This function is called when a collision starts
    void OnTriggerEnter(Collider other)
    {
        
        GameObject collisionObject = other.gameObject; 
        
        bool match = collisionObject.CompareTag(requiredTag);
                       
        if (match)
        {

            print("TRIGGER: " + this.gameObject.tag + " , other: " + other.gameObject.tag);
            // 1. Set the BoolCondition to true (if assigned)
            if (targetCondition != null)
            {                
                targetCondition.SetCondition(true);               
            }
            else
            {
                Debug.LogWarning("Target Condition not set in the inspector!", this.gameObject);
            }

            // 2. Invoke the UnityEvent
            onMatch.Invoke();
        }
    }
}
