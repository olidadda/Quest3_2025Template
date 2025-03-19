using UnityEngine;
using DG.Tweening;

public class EmissionTweener : MonoBehaviour
{
    [Header("Material & Property")]
    public Material gridMaterial;   // Assign in Inspector
    [Tooltip("How bright to get at the peak of the tween.")]
    public float targetEmission = 5f;
    [Tooltip("Time (seconds) to reach the targetEmission.")]
    public float tweenDuration = 2f;

    private void OnEnable()
    {
        // Force the material's emission to zero
        if (gridMaterial != null)
        {
            gridMaterial.SetFloat("_EmissionIntensity", 0f);
        }
    }
    void Start()
    {
        // Make sure the material is valid, and your shader actually has _EmissionIntensity
        if (gridMaterial != null)
        {
            // 1) Tween the emission intensity from its current value to targetEmission
            float startValue = gridMaterial.GetFloat("_EmissionIntensity");
            DOTween.To(
                () => startValue,
                x => {
                    startValue = x;
                    gridMaterial.SetFloat("_EmissionIntensity", x);
                },
                targetEmission,
                tweenDuration
            ).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            // ^ This will oscillate forever, going up then back down

            //// 2) Tween the emission color if you want
            //Color startColor = gridMaterial.GetColor("_EmissionColor");
            //DOTween.To(
            //    () => startColor,
            //    x => {
            //        startColor = x;
            //        gridMaterial.SetColor("_EmissionColor", x);
            //    },
            //    targetEmissionColor,
            //    tweenDuration
            //).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
