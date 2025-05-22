using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoicePlaybackInitialMenu : MonoBehaviour
{
    [SerializeField] AudioSource audioSource; 

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartAudio());
    }

   IEnumerator StartAudio()
   {
     yield return new WaitForSeconds(1.5f);

        audioSource.Play();

   }
}
