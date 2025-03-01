//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(TimeLine))]
//public class TimelineManagerEditor : Editor
//{
//    private TimelineEventBase eventToAdd; // Selected event to add
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        TimeLine manager = (TimeLine)target;

//        EditorGUILayout.LabelField("Timeline Events", EditorStyles.boldLabel);

//        SerializedProperty eventList = serializedObject.FindProperty("events");

//        for (int i = 0; i < eventList.arraySize; i++)
//        {
//            SerializedProperty element = eventList.GetArrayElementAtIndex(i);
//            TimelineEventBase eventRef = (TimelineEventBase)element.objectReferenceValue;

//            string eventName = eventRef != null ? eventRef.eventName : "Unnamed Event";
//            element.objectReferenceValue = (TimelineEventBase)EditorGUILayout.ObjectField(eventName, eventRef, typeof(TimelineEventBase), true);
//        }

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Add Event to Timeline", EditorStyles.boldLabel);

//        eventToAdd = (TimelineEventBase)EditorGUILayout.ObjectField("Event to Add", eventToAdd, typeof(TimelineEventBase), true);

//        if (GUILayout.Button("Add Event") && eventToAdd != null)
//        {
//            manager.AddEvent(eventToAdd);
//            eventToAdd = null;
//        }

//        if (GUILayout.Button("Refresh Event Names"))
//        {
//            manager.RegisterEvents();
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}

//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(TimeLine))]
//public class TimelineManagerEditor : Editor
//{
//    private TimelineEventBase eventToAdd; // Selected event to add

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        TimeLine manager = (TimeLine)target;

//        EditorGUILayout.LabelField("Timeline Events", EditorStyles.boldLabel);

//        SerializedProperty eventList = serializedObject.FindProperty("events");

//        for (int i = 0; i < eventList.arraySize; i++)
//        {
//            SerializedProperty element = eventList.GetArrayElementAtIndex(i);
//            TimelineEventBase eventRef = (TimelineEventBase)element.objectReferenceValue;

//            string eventName = eventRef != null ? eventRef.eventName : "Unnamed Event";
//            EditorGUILayout.BeginHorizontal();
//            element.objectReferenceValue = (TimelineEventBase)EditorGUILayout.ObjectField(eventName, eventRef, typeof(TimelineEventBase), true);
//            EditorGUILayout.EndHorizontal();
//        }

//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Add Event to Timeline", EditorStyles.boldLabel);

//        eventToAdd = (TimelineEventBase)EditorGUILayout.ObjectField("Event to Add", eventToAdd, typeof(TimelineEventBase), true);

//        if (GUILayout.Button("Add Event") && eventToAdd != null)
//        {
//            manager.AddEvent(eventToAdd);
//            eventToAdd = null;
//            serializedObject.ApplyModifiedProperties();
//            EditorUtility.SetDirty(manager);  // Force Unity to recognize a change
//            Repaint();  // Refresh the UI immediately
//        }

//        if (GUILayout.Button("Refresh Event Names"))
//        {
//            manager.RegisterEvents();
//            serializedObject.ApplyModifiedProperties();
//            EditorUtility.SetDirty(manager);  // Ensure the change is recognized
//            Repaint();  // Force UI refresh
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}

