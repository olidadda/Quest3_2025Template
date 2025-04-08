using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public FadeScreen fadeScreen;
    [SerializeField] float delayBeforeFadeOut;

    private void Start()
    {
        //Load Scene 1 (Menu) additively alongside persistent scene (which contains Scene Manager, Game State, Player, and persistent world objects)
        StartCoroutine(StartInitialSceneLoad());
    }

    private IEnumerator StartInitialSceneLoad()
    {
       
        //yield return FadeOutAndWait();

        // Load Scene 1 (Menu) additively alongside persistent scene
        SceneManager.LoadScene(1, LoadSceneMode.Additive);

        // Fade in once the scene is loaded, adjust fadeinDuration as needed
        yield return FadeInAndWait();
    }

    public void GoToScene(int newSceneIndex, int oldSceneIndex)
    {        
        StartCoroutine(FadeTransitionBetweenAdditiveScenes(newSceneIndex, oldSceneIndex));        
    }
    

    IEnumerator FadeTransitionBetweenAdditiveScenes(int newSceneIndex, int oldSceneIndex)
    {
        yield return new WaitForSeconds(delayBeforeFadeOut);
        // Fade out, wait for completion
        yield return FadeOutAndWait();

        //Wait until it's (almost) fully loaded, then activate
        yield return LoadAdditiveSceneAndWait(newSceneIndex);

        // Set the newly loaded scene as the active scene (helpful for lighting, references, etc.)
        //SetActiveScene(newSceneIndex);

        //Unload the old scene 
        yield return UnloadOldSceneAndWait(oldSceneIndex);

        // Fade in after new scene is loaded
        yield return FadeInAndWait();
    }

    private IEnumerator UnloadOldSceneAndWait(int oldSceneIndex)
    {
        if (oldSceneIndex != -1)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(oldSceneIndex);
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }
    }

    private static void SetActiveScene(int newSceneIndex)
    {
        UnityEngine.SceneManagement.Scene newlyLoadedScene = SceneManager.GetSceneByBuildIndex(newSceneIndex);
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator WaitForSecondsRoutine(float duration)
    {
        float timer = 0;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOutAndWait()
    {
        fadeScreen.FadeOut();
        yield return WaitForSecondsRoutine(fadeScreen.fadeoutDuration);
    }

    private IEnumerator FadeInAndWait()
    {
        fadeScreen.FadeIn();
        yield return WaitForSecondsRoutine(fadeScreen.fadeinDuration);
    }

    private IEnumerator LoadAdditiveSceneAndWait(int newSceneIndex)
    {
        // Begin loading the new scene in the background, additively
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(newSceneIndex, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;

        // Wait until the scene is (almost) fully loaded
        while (loadOperation.progress < 0.9f)
        {
            // You could display a progress bar or loading UI here
            yield return null;
        }

        // Scene is ready to be activated, but won't show until we allow it
        loadOperation.allowSceneActivation = true;


        //Optionally wait until the scene is actually done activating
        while (!loadOperation.isDone)
        {
            yield return null;
        }

    }

}
