using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    public FadeScreen fadeScreen;   

    public void GoToHomeScene()
    {
        //FindObjectOfType<DelayAmbienceTrack>().FadeOutAudio();

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            StartCoroutine(EndGameAndLoadSceneFromGame2());
        }
        else
        {
            StartCoroutine(GotoSceneTransitionRoutine(0));
            //SceneManager.LoadScene(0);
        }
    }

    private IEnumerator EndGameAndLoadSceneFromGame2()
    {        

        //yield return new WaitUntil(() => StateManager.ins.scoring_Game2.endGameCompleted); //wait until bool true 

        StartCoroutine(GotoSceneTransitionRoutine(0));
        yield return null;
    }


    
    public void GoToMiniGame(int sceneIndex)
    {        
        StartCoroutine(GotoSceneTransitionRoutine(sceneIndex));        
    }   

    IEnumerator GotoSceneTransitionRoutine(int sceneIndex)
    {
        //this is outside the fade script because probably this object was persistent but it doesn't really make sense, the player should be persistent too
        fadeScreen.FadeOut();

        float timer1 = 0;
        while (timer1 <= fadeScreen.fadeoutDuration)
        {
            timer1 += Time.deltaTime;
            yield return null;
        }

        //yield return new WaitForSeconds(fadeScreen.fadeDuration);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float timer = 0; 
        while (timer <= fadeScreen.fadeoutDuration && !operation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        operation.allowSceneActivation = true;

    }
}
