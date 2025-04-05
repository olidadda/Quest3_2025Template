using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactSceneLoader : MonoBehaviour
{
   // [SerializeField] int sceneToLoad, sceneToUnload = 0;

    enum SceneNames 
    {
        notLoaded,
        Menu = 1,
        sceneA_WaterTreatment = 2,
        sceneB_WasteByproduct = 3,
        BackToMenu = 4
    }

    [SerializeField] SceneNames sceneToLoad;
    [SerializeField] SceneNames sceneToUnLoad;

      
    public void FindSceneLoaderAndChangeScene()

    {
        int toLoad = (int)sceneToLoad;
        int toUnload = (int)sceneToUnLoad;

        if (toLoad == 0) { Debug.LogError("scene To Load is not set"); return; }
        if (toUnload == 0) { Debug.LogError("scene To Unload is not set"); return; }        

        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader != null) 
        {
            sceneLoader.GoToScene(toLoad, toUnload);
        }
        else { Debug.LogError("SceneLoader Not Found"); }
        
    }
}
