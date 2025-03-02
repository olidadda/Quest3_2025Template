using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class TimeLine : MonoBehaviour
{
    /*
        📌 Timeline Event System - How It Works

        This system manages a flexible sequence of events in Unity, allowing for both 
        linear and branching paths based on player choices and conditions.

        - The Timeline holds a list of TimelineEventBase objects, which represent different 
          types of events (e.g., scripted sequences, player choices, and scene transitions).
        - Events do not follow a strict order; they can transition to different events based 
          on player actions and conditions, allowing dynamic storytelling or gameplay flow.
        - Each event can activate/deactivate objects, play audio, or wait for external conditions 
          before progressing.

        - BoolCondition objects act as global flags that determine event transitions. 
          These conditions are stored in a dedicated "Conditions" GameObject in the scene 
          and can be referenced or modified by any event, interaction, or script in the game.
        - ScriptedEvent executes a sequence of predefined actions, optionally waiting for a 
          BoolCondition before continuing.
        - PlayerChoiceEvent presents multiple choices, each linked to a BoolCondition. 
          When a choice is made, its corresponding BoolCondition is set, triggering the 
          next event in the timeline.
        - SceneTransitionEvent loads a new scene and ensures the timeline continues after loading.

        Usage:
        - Events are configured in the Inspector by assigning conditions and next events.
        - BoolConditions are set dynamically during gameplay (e.g., when the player interacts 
          with an object, presses a button, or completes a task).
        - The Timeline system listens for these conditions and determines which event should 
          execute next, based on the game logic defined by the developer.

        ✅ This system allows for a fully modular and adaptable game flow, where actions, 
        choices, and outcomes dynamically shape the experience.
    */





    //public static TimeLine Instance;

    [Header("Timeline Events")]
    public List<TimelineEventBase> events = new List<TimelineEventBase>();
    public Dictionary<string, TimelineEventBase> eventLookup = new Dictionary<string, TimelineEventBase>();

    private TimelineEventBase currentEvent;

    void Awake()
    {
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject); // Destroy duplicate
        //    return;
        //}

        //Instance = this;
        //DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    void Start()
    {
        if (events.Count > 0) TriggerEvent(events[0]);
    }

    public void RegisterEvents()
    {
        eventLookup.Clear();
        foreach (var e in events)
        {
            if (e != null && !eventLookup.ContainsKey(e.eventName))
                eventLookup.Add(e.eventName, e);
        }
    }

    public void AddEvent(TimelineEventBase newEvent)
    {
        if (newEvent != null && !events.Contains(newEvent))
        {
            events.Add(newEvent);
            Debug.Log("Manually added event: " + newEvent.eventName);
        }
    }

    public void TriggerEvent(TimelineEventBase newEvent)
    {
        if (newEvent == null) return;

        currentEvent = newEvent;
        currentEvent.Execute();
    }
}
