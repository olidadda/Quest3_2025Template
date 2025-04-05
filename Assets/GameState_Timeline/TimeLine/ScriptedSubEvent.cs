using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] // Allows sub-events to be editable in Inspector
public class ScriptedSubEvent 
{
    public string message;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public GameObject[] activateObjects;
    public GameObject[] deactivateObjects;
    public float delayBeforeNext = 0f; // Time before executing next subevent

    [Header("Custom Actions")]
    [Tooltip("Events triggered when this subevent occurs")]
    public UnityEngine.Events.UnityEvent onSubeventTriggered;
}
