using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // Needed for Undo and SetDirty
#endif

[ExecuteInEditMode]
public class TransformRecorder : MonoBehaviour
{
    [Header("Source Object (Optional)")]
    [Tooltip("Drag another GameObject here to copy its transform data.")]
    public Transform sourceTransform; // Assign the object to copy FROM

    [Header("Recorded Start State")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation = Quaternion.identity;
    [SerializeField] private Vector3 startScale = Vector3.one;

    [Header("Recorded End State")]
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private Quaternion endRotation = Quaternion.identity;
    [SerializeField] private Vector3 endScale = Vector3.one;

    // --- Public properties ---
    public Vector3 StartPosition => startPosition;
    public Quaternion StartRotation => startRotation;
    public Vector3 StartScale => startScale;
    public Vector3 EndPosition => endPosition;
    public Quaternion EndRotation => endRotation;
    public Vector3 EndScale => endScale;

    // --- Methods called by the Editor Script ---

    public void RecordCurrentAsStartState() // Renamed for clarity
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Record Current as Start State");
        startPosition = transform.position; // Current object's world position
        startRotation = transform.rotation; // Current object's world rotation
        startScale = transform.localScale;    // Current object's local scale
        EditorUtility.SetDirty(this);
        Debug.Log($"Start state recorded from THIS object ({gameObject.name})", this);
#endif
    }

    public void RecordCurrentAsEndState() // Renamed for clarity
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Record Current as End State");
        endPosition = transform.position;
        endRotation = transform.rotation;
        endScale = transform.localScale;
        EditorUtility.SetDirty(this);
        Debug.Log($"End state recorded from THIS object ({gameObject.name})", this);
#endif
    }

    // --- NEW Methods to copy from Source ---
    public void CopySourceToStartState()
    {
        if (!ValidateSourceTransform()) return; // Check if source is valid

#if UNITY_EDITOR
        Undo.RecordObject(this, "Copy Source to Start State");
        startPosition = sourceTransform.position; // Source's world position
        startRotation = sourceTransform.rotation; // Source's world rotation
        startScale = sourceTransform.localScale;    // Source's local scale
        EditorUtility.SetDirty(this);
        Debug.Log($"Start state recorded from SOURCE object ({sourceTransform.name})", this);
#endif
    }

    public void CopySourceToEndState()
    {
        if (!ValidateSourceTransform()) return; // Check if source is valid

#if UNITY_EDITOR
        Undo.RecordObject(this, "Copy Source to End State");
        endPosition = sourceTransform.position;
        endRotation = sourceTransform.rotation;
        endScale = sourceTransform.localScale;
        EditorUtility.SetDirty(this);
        Debug.Log($"End state recorded from SOURCE object ({sourceTransform.name})", this);
#endif
    }

    // --- Helper validation ---
    private bool ValidateSourceTransform()
    {
        if (sourceTransform == null)
        {
            Debug.LogWarning("Source Transform is not assigned. Cannot copy.", this);
            return false;
        }
        if (sourceTransform == this.transform)
        {
            Debug.LogWarning("Source Transform cannot be the same object this script is on. Use 'Record Current...' buttons instead.", this);
            return false;
        }
        return true;
    }


    // --- Optional: Helper methods to apply recorded states back ---
    public void ApplyStartState()
    {
#if UNITY_EDITOR
        Undo.RecordObject(transform, "Apply Start State");
#endif
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = startScale;
#if UNITY_EDITOR
        if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabInstance(transform))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }
        EditorUtility.SetDirty(transform);
#endif
        Debug.Log($"Applied recorded Start state to {gameObject.name}", this);
    }

    public void ApplyEndState()
    {
#if UNITY_EDITOR
        Undo.RecordObject(transform, "Apply End State");
#endif
        transform.position = endPosition;
        transform.rotation = endRotation;
        transform.localScale = endScale;
#if UNITY_EDITOR
        if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabInstance(transform))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }
        EditorUtility.SetDirty(transform);
#endif
        Debug.Log($"Applied recorded End state to {gameObject.name}", this);
    }

    // --- Gizmos ---
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!this.enabled || startScale == Vector3.zero || endScale == Vector3.zero) return;

        // Draw Start State
        Handles.color = Color.green;
        Handles.matrix = Matrix4x4.TRS(startPosition, startRotation, Vector3.one);
        Handles.DrawWireCube(Vector3.zero, startScale);
        Handles.Label(startPosition + Vector3.up * 0.5f, "Start State");

        // Draw End State
        Handles.color = Color.red;
        Handles.matrix = Matrix4x4.TRS(endPosition, endRotation, Vector3.one);
        Handles.DrawWireCube(Vector3.zero, endScale);
        Handles.Label(endPosition + Vector3.up * 0.5f, "End State");

        // Draw line between states
        Handles.matrix = Matrix4x4.identity;
        Handles.color = Color.yellow;
        Handles.DrawLine(startPosition, endPosition);
        Handles.color = Color.white;
    }
#endif
}
