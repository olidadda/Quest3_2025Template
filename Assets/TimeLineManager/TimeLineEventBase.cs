using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class TimelineEventBase : MonoBehaviour
{
    [Header("Event Setup")]
    public string eventName;
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
        foreach (var obj in objectsToActivate) obj.SetActive(true);
        foreach (var obj in objectsToDeactivate) obj.SetActive(false);
    }

    
}

