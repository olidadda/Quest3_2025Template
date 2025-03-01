using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransitionEvent : TimelineEventBase
{
    [Header("Transition Settings")]
    public string sceneToLoad;
    public GameObject menuToShow; // Optional menu activation
    public GameObject menuToHide;
    public float transitionDelay = 0f;

    public override void Execute()
    {
        Debug.Log("Scene transition event triggered.");
        SetupPhase();
        StartCoroutine(HandleTransition());
    }

    private IEnumerator HandleTransition()
    {
        if (transitionDelay > 0)
            yield return new WaitForSeconds(transitionDelay);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            if (menuToShow != null) menuToShow.SetActive(true);
            if (menuToHide != null) menuToHide.SetActive(false);
        }

        Transition(); // Move to the next event in sequence
    }

    public override bool CheckCondition() => true; // Always progresses after execution
}
