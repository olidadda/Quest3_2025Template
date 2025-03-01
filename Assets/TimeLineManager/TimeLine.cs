using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLine : MonoBehaviour
{
    public static TimeLine Instance;

    [Header("Timeline Events")]
    public List<TimelineEventBase> events = new List<TimelineEventBase>();
    //public Dictionary<string, TimelineEventBase> eventLookup = new Dictionary<string, TimelineEventBase>();

    private TimelineEventBase currentEvent;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    void Start()
    {
        if (events.Count > 0) TriggerEvent(events[0]);
    }

    //public void RegisterEvents()
    //{
    //    eventLookup.Clear();
    //    foreach (var e in events)
    //    {
    //        if (e != null && !eventLookup.ContainsKey(e.eventName))
    //            eventLookup.Add(e.eventName, e);
    //    }
    //}

    //public void AddEvent(TimelineEventBase newEvent)
    //{
    //    if (newEvent != null && !events.Contains(newEvent))
    //    {
    //        events.Add(newEvent);
    //        Debug.Log("Manually added event: " + newEvent.eventName);
    //    }
    //}

    public void TriggerEvent(TimelineEventBase newEvent)
    {
        if (newEvent == null) return;

        currentEvent = newEvent;
        currentEvent.Execute();
    }
}
