using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class TimelineEventBase : MonoBehaviour
{
    
    public string eventName;
    [Space(30)]
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;
    

    //[Header("Next Events Based on Conditions")]
    //public TimelineEventBase nextEventCondition1;
    //public TimelineEventBase nextEventCondition2;

    public abstract void Execute(); // Called when the phase starts
    public abstract bool CheckCondition(); // Check if phase should transition
       

    //public void Transition()
    //{
    //    if (CheckCondition())
    //    {
    //        Debug.Log(eventName + " condition met, transitioning...");
    //        TimeLine.Instance.TriggerEvent(nextEventCondition1);
    //    }
    //    else
    //    {
    //        TimeLine.Instance.TriggerEvent(nextEventCondition2);
    //    }
    //}

    protected void SetupPhase()
    {
        if (objectsToActivate != null)
        {
            foreach (var obj in objectsToActivate)
            {
                if (obj != null) obj.SetActive(true);
                else Debug.LogWarning("⚠️ Missing reference in objectsToActivate!");
            }
        }

        if (objectsToDeactivate != null)
        {
            foreach (var obj in objectsToDeactivate)
            {
                if (obj != null) obj.SetActive(false);
                else Debug.LogWarning("⚠️ Missing reference in objectsToDeactivate!");
            }
        }
    }

    
}

