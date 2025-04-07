using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TransformRecorder))]
public class TransformRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields (sourceTransform, start/end state variables)
        DrawDefaultInspector();

        TransformRecorder recorder = (TransformRecorder)target;

        EditorGUILayout.Space(10);

        // --- Buttons to Record This Object's Current State ---
        EditorGUILayout.LabelField("Record THIS Object's Current Transform:", EditorStyles.boldLabel);
        if (GUILayout.Button("Record Current as Start State"))
        {
            recorder.RecordCurrentAsStartState(); // Use the renamed method
        }

        if (GUILayout.Button("Record Current as End State"))
        {
            recorder.RecordCurrentAsEndState(); // Use the renamed method
        }

        EditorGUILayout.Space(10);

        // --- Button to Set Source From Selection ---
        if (GUILayout.Button("Set Source From Selection"))
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject != recorder.gameObject)
            {
                Undo.RecordObject(recorder, "Set Source Transform"); // Record change to recorder component
                recorder.sourceTransform = Selection.activeGameObject.transform;
                EditorUtility.SetDirty(recorder); // Mark recorder as dirty
            }
            else if (Selection.activeGameObject == recorder.gameObject)
            {
                EditorUtility.DisplayDialog("Set Source Failed", "Cannot set the source to be the same GameObject this script is on.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Set Source Failed", "Please select a valid GameObject in the Hierarchy view (other than this one) to set as the source.", "OK");
            }
        }

        // --- Buttons to Copy From Source Object ---
        EditorGUILayout.LabelField("Copy SOURCE Object's Transform:", EditorStyles.boldLabel);

        // Disable buttons if source is not valid
        bool sourceIsValid = recorder.sourceTransform != null && recorder.sourceTransform != recorder.transform;
        EditorGUI.BeginDisabledGroup(!sourceIsValid);

        if (GUILayout.Button("Copy Source to Start State"))
        {
            recorder.CopySourceToStartState(); // NEW method call
        }

        if (GUILayout.Button("Copy Source to End State"))
        {
            recorder.CopySourceToEndState(); // NEW method call
        }

        EditorGUI.EndDisabledGroup(); // Re-enable GUI

        // Show help message if source is invalid
        if (recorder.sourceTransform == null)
        {
            EditorGUILayout.HelpBox("Assign a Source Transform to enable copying.", MessageType.Info);
        }
        else if (recorder.sourceTransform == recorder.transform)
        {
            EditorGUILayout.HelpBox("Source cannot be the same as this object.", MessageType.Warning);
        }


        EditorGUILayout.Space(10);

        // --- Buttons to Apply Recorded States ---
        EditorGUILayout.LabelField("Apply Recorded State To THIS Object:", EditorStyles.boldLabel);
        if (GUILayout.Button("Apply Start State"))
        {
            recorder.ApplyStartState();
        }
        if (GUILayout.Button("Apply End State"))
        {
            recorder.ApplyEndState();
        }
    }
}