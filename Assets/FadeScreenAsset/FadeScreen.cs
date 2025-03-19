using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public bool fadeOnStart = true;
    
    public float fadeoutDuration = 1f;
    public float fadeinDuration = 1f;

    public Color fadeColor;
    Color nextPhaseOfColor;
    Renderer rend;
    
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (fadeOnStart)
        {
            FadeIn();
        }
    }

   public void FadeIn()
   {
        Fade(1, 0, fadeinDuration);
   }

   public void FadeOut()
   {
        Fade(0, 1, fadeoutDuration);
    }
   public void Fade (float alphaIn, float alphaOut, float duration)
   {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut, duration));

   }

    public IEnumerator FadeRoutine (float alphaIn, float alphaOut, float duration)
    {
        float timer = 0;
        while (timer <= duration)
        {
            nextPhaseOfColor = fadeColor;
            nextPhaseOfColor.a = Mathf.Lerp(alphaIn, alphaOut, timer/ duration);

            rend.material.SetColor("_Color", nextPhaseOfColor);

            timer += Time.deltaTime;
            yield return null;
        }

        Color finalColor = fadeColor;
        finalColor.a = alphaOut;
        rend.material.SetColor("_Color", finalColor);
    }
}
