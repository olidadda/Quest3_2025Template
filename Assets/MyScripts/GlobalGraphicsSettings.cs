using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGraphicsSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        OVRPlugin.systemDisplayFrequency = 120.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
