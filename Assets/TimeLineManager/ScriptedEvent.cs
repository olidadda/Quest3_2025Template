using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Allows sub-events to be editable in Inspector
public class ScriptedSubEvent
{
    public string message;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public GameObject[] activateObjects;
    public GameObject[] deactivateObjects;
    public float delayBeforeNext; // Time before executing next subevent
}

public class ScriptedEvent : TimelineEventBase
{
    [Header("Scripted Sub-Events")]
    public List<ScriptedSubEvent> subEvents = new List<ScriptedSubEvent>();

    public override void Execute()
    {
        Debug.Log("Starting scripted event: " + eventName);
        SetupPhase();
        StartCoroutine(PlaySubEvents());
    }

    private IEnumerator PlaySubEvents()
    {
        foreach (var subEvent in subEvents)
        {
            if (!string.IsNullOrEmpty(subEvent.message))
                Debug.Log(subEvent.message);

            if (subEvent.audioClip != null && subEvent.audioSource != null)
                subEvent.audioSource.PlayOneShot(subEvent.audioClip);

            foreach (var obj in subEvent.activateObjects) obj.SetActive(true);
            foreach (var obj in subEvent.deactivateObjects) obj.SetActive(false);

            yield return new WaitForSeconds(subEvent.delayBeforeNext);
        }

        Transition();
    }

    public override bool CheckCondition() => true; // Auto-progress after last subevent
}

