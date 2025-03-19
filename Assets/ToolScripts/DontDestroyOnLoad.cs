using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    private void Awake()
    {
        // This ensures that this GameObject (and its children) 
        // will not be destroyed when loading a new scene.
        DontDestroyOnLoad(gameObject);
    }
}
