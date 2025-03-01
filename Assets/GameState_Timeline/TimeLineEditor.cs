using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeLine))]
public class TimelineManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        TimeLine manager = (TimeLine)target;

        EditorGUILayout.LabelField("Timeline Events", EditorStyles.boldLabel);

        SerializedProperty eventList = serializedObject.FindProperty("events");

        if (eventList.isArray)
        {
            for (int i = 0; i < eventList.arraySize; i++)
            {
                SerializedProperty element = eventList.GetArrayElementAtIndex(i);
                TimelineEventBase eventRef = element.objectReferenceValue as TimelineEventBase;

                string eventName = eventRef != null ? eventRef.eventName : $"Element {i} (Unassigned)";
                GUIContent content = new GUIContent(eventName); // Show event name as label

                EditorGUILayout.PropertyField(element, content, true);
            }
        }

        // Keep standard Unity add/remove buttons
        EditorGUILayout.PropertyField(eventList, true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Manual Event Management", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Event Names"))
        {
            manager.RegisterEvents();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(manager);
            Repaint();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
