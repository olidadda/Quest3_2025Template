
using UnityEngine;

public class FestoObjectInfo : MonoBehaviour
{
    [Tooltip("Audio clip to play when this object is touched after migration.")]
    public AudioClip explanationAudio;

    [Tooltip("Text to display when this object is touched after migration.")]
    [TextArea(3, 5)]
    public string explanationText;
}