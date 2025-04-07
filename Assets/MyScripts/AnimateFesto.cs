using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimateFesto : MonoBehaviour
{
    [Tooltip("Duration of the migration animation in seconds.")]
    public float migrationDuration = 1.0f;

    [Tooltip("Easing function for the animation.")]
    public Ease easeType = Ease.InOutQuad; // Choose an easing type

    [Header("Optional Settings")]
    [Tooltip("Delay before the animation starts.")]
    public float startDelay = 0f;

    [Tooltip("Animate rotation as well?")]
    public bool includeRotation = false;

    [SerializeField] List<GameObject> festoComponents = new List<GameObject>();

   public void DisassembleFesto()
   {
        foreach (GameObject obj in festoComponents)
        {
            if (obj == null)
            {
                Debug.LogWarning("Found a null object in the migration list. Skipping.", this);
                continue; 
            }


            Transform objTransform = obj.transform; // Cache transform
            TransformRecorder recorder = obj.GetComponent<TransformRecorder>();

            if (recorder == null)
            {
                Debug.LogWarning($"Object '{obj.name}' is missing the TransformRecorder component. Skipping animation for it.", obj);
                continue; // Skip objects without the recorder
            }

            // Get target values from the recorder
            Vector3 targetPosition = recorder.EndPosition;
            Vector3 targetScale = recorder.EndScale;
            Quaternion targetRotation = recorder.EndRotation; // Get rotation even if not used immediately

            // Create the position tween
            objTransform.DOMove(targetPosition, migrationDuration)
                .SetEase(easeType)
                .SetDelay(startDelay); // Apply delay

            // Create the scale tween
            objTransform.DOScale(targetScale, migrationDuration)
                .SetEase(easeType)
                .SetDelay(startDelay); // Apply delay

            // Optionally create the rotation tween (World Space)
            if (includeRotation)
            {
                // DORotate uses world space Eulers. Use DORotateQuaternion for potentially smoother rotations.
                objTransform.DORotateQuaternion(targetRotation, migrationDuration)
                    .SetEase(easeType)
                    .SetDelay(startDelay);

                // Alternative using Euler angles (can suffer from gimbal lock):
                // objTransform.DORotate(targetRotation.eulerAngles, migrationDuration)
                //    .SetEase(easeType)
                //    .SetDelay(startDelay);
            }
        }           
   }


   public void RecombineFesto()
   {
        foreach (GameObject obj in festoComponents)
        {
            if (obj == null)
            {
                Debug.LogWarning("Found a null object in the migration list. Skipping.", this);
                continue;
            }


            Transform objTransform = obj.transform; // Cache transform
            TransformRecorder recorder = obj.GetComponent<TransformRecorder>();

            if (recorder == null)
            {
                Debug.LogWarning($"Object '{obj.name}' is missing the TransformRecorder component. Skipping animation for it.", obj);
                continue; // Skip objects without the recorder
            }

            // Get target values from the recorder
            Vector3 targetPosition = recorder.StartPosition;
            Vector3 targetScale = recorder.StartScale;
            Quaternion targetRotation = recorder.StartRotation; // Get rotation even if not used immediately

            // Create the position tween
            objTransform.DOMove(targetPosition, migrationDuration)
                .SetEase(easeType)
                .SetDelay(startDelay); // Apply delay

            // Create the scale tween
            objTransform.DOScale(targetScale, migrationDuration)
                .SetEase(easeType)
                .SetDelay(startDelay); // Apply delay

            // Optionally create the rotation tween (World Space)
            if (includeRotation)
            {
                // DORotate uses world space Eulers. Use DORotateQuaternion for potentially smoother rotations.
                objTransform.DORotateQuaternion(targetRotation, migrationDuration)
                    .SetEase(easeType)
                    .SetDelay(startDelay);

                // Alternative using Euler angles (can suffer from gimbal lock):
                // objTransform.DORotate(targetRotation.eulerAngles, migrationDuration)
                //    .SetEase(easeType)
                //    .SetDelay(startDelay);
            }
        }
    }
}
