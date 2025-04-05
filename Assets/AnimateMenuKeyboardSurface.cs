using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimateMenuKeyboardSurface : MonoBehaviour
{
    [SerializeField] float timeToShrink = 1.0f;
    public void ShrinkAndDisable()
    {
        // Start the scaling animation
        transform.DOScale(Vector3.zero,timeToShrink) // Scale to zero over 1 second
            .OnComplete(() => gameObject.SetActive(false)); // Disable the GameObject once scaling is complete
    }
}

