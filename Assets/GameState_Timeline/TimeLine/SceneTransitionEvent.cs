using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionEvent : TimelineEventBase
{
    [Header("Scene Transition Settings")]
    public string sceneToLoad;
    public float transitionDelay = 0f;

    [Header("Next Event After Transition")]
    public TimelineEventBase nextEvent; // Define the next event after transition

    public override void Execute()
    {
        Debug.Log($"Scene transition event triggered: {sceneToLoad}");
        StartCoroutine(HandleSceneTransition());
    }

    private IEnumerator HandleSceneTransition()
    {
        if (transitionDelay > 0)
            yield return new WaitForSeconds(transitionDelay);

        if (!string.IsNullOrEmpty(sceneToLoad) && SceneExists(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneToLoad);
        }
        else
        {
            Debug.LogWarning($" Scene '{sceneToLoad}' does not exist or is not set!");
        }

        // Transition to the next event in the timeline
        if (nextEvent != null)
        {
            Debug.Log($" Transitioning to next event: {nextEvent.eventName}");
            nextEvent.Execute(); //Directly execute the next event
        }
        else
        {
            Debug.LogWarning($" No next event assigned after scene transition.");
        }
    }

    public override bool CheckCondition() => true; // Always progresses after execution

    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneFileName == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}
