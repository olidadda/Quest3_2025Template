using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

// Ensure this target type matches the exact name of your ScriptedEvent script file
[CustomEditor(typeof(ScriptedEvent_WithORConditions))]
public class ScriptedEvent_WithORConditionsEditor : Editor
{
    // --- Properties from Base Class ---
    private SerializedProperty eventNameProp;
    private SerializedProperty objectsToActivateProp;
    private SerializedProperty objectsToDeactivateProp;

    // --- Properties from Derived Class (ScriptedEvent_WithORConditions) ---
    private SerializedProperty conditionsToResetProp;
    private SerializedProperty subEventsProp;
    //private SerializedProperty allowSkipProp;
    //private SerializedProperty requireAllSubEventsToFinishProp;
    private SerializedProperty completionConditionsProp;
    private SerializedProperty nextEventProp;

    // --- Read-only display properties ---
    private SerializedProperty resetConditionNamesProp;

    private bool showSubEvents = true; // Foldout state for sub-events list

    private void OnEnable()
    {
        // --- Cache Base Class Properties ---
        eventNameProp = serializedObject.FindProperty("eventName");
        objectsToActivateProp = serializedObject.FindProperty("objectsToActivate");
        objectsToDeactivateProp = serializedObject.FindProperty("objectsToDeactivate");

        // --- Cache Derived Class Properties ---
        conditionsToResetProp = serializedObject.FindProperty("conditionsToReset");
        subEventsProp = serializedObject.FindProperty("subEvents");
        //allowSkipProp = serializedObject.FindProperty("allowSkip");
        //requireAllSubEventsToFinishProp = serializedObject.FindProperty("requireAllSubEventsToFinish");
        completionConditionsProp = serializedObject.FindProperty("completionConditions");
        nextEventProp = serializedObject.FindProperty("nextEvent");
        resetConditionNamesProp = serializedObject.FindProperty("resetConditionNames");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Always start with this

        // --- Draw Base Class Fields ---
        EditorGUILayout.PropertyField(eventNameProp);
        EditorGUILayout.Space(5);
        DrawGameObjectList(objectsToActivateProp, "Objects To Activate On Execute");
        EditorGUILayout.Space(2);
        DrawGameObjectList(objectsToDeactivateProp, "Objects To Deactivate On Execute");
       

        // --- Draw Derived Class Fields ---
        //EditorGUILayout.LabelField("Resets Conditions and Bools Before Passing to Next Event Script", EditorStyles.boldLabel);
        // Use default drawer for this array, usually okay
        EditorGUILayout.PropertyField(conditionsToResetProp, true);
        DrawReadOnlyStringArray(resetConditionNamesProp, "Conditions to Reset Names");
        EditorGUILayout.Space(5);

        // Sub-Events Foldout
        showSubEvents = EditorGUILayout.Foldout(showSubEvents, "Scripted Sub-Events", true, EditorStyles.foldoutHeader);
        if (showSubEvents)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(subEventsProp, true); // Default drawer for list of sub-events
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(10);

        //// Skipping Options
        //EditorGUILayout.LabelField("Skipping Options", EditorStyles.boldLabel);
        //EditorGUILayout.PropertyField(allowSkipProp);
        //EditorGUILayout.PropertyField(requireAllSubEventsToFinishProp);
        //EditorGUILayout.Space(10);

        // Custom Drawer for Completion Conditions
        DrawCompletionConditionsList();
        EditorGUILayout.Space(10);

        // Next Event
        EditorGUILayout.LabelField("Next Event", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(nextEventProp);

        // Apply changes back to the object
        serializedObject.ApplyModifiedProperties();
    }

    // --- Helper method to draw GameObject Lists (Copied from proposed Base Editor) ---
    private void DrawGameObjectList(SerializedProperty listProperty, string label)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

        if (listProperty.arraySize == 0)
        {
            EditorGUILayout.HelpBox("List is empty.", MessageType.Info);
        }
        EditorGUI.indentLevel++;
        for (int i = 0; i < listProperty.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty elementProp = listProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(elementProp, GUIContent.none);
            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(25)))
            {
                elementProp.objectReferenceValue = null;
                listProperty.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Object", GUILayout.Width(100)))
        {
            listProperty.arraySize++;
            listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1).objectReferenceValue = null;
        }
        EditorGUILayout.EndHorizontal();
    }

    // --- Helper method to draw Completion Conditions List ---
    private void DrawCompletionConditionsList()
    {
        EditorGUILayout.LabelField("Event Completion Conditions (OR Logic)", EditorStyles.boldLabel);

        if (completionConditionsProp.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No completion conditions defined. Event will complete after sub-events finish.", MessageType.Info);
        }
        for (int i = 0; i < completionConditionsProp.arraySize; i++)
        {
            SerializedProperty elementProp = completionConditionsProp.GetArrayElementAtIndex(i);
            SerializedProperty conditionRefProp = elementProp.FindPropertyRelative("condition");
            SerializedProperty allowEarlyProp = elementProp.FindPropertyRelative("allowEarlyCompletion");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(conditionRefProp, GUIContent.none, GUILayout.ExpandWidth(true));
            BoolCondition referencedCondition = conditionRefProp.objectReferenceValue as BoolCondition;
            if (referencedCondition != null)
            {
                GUILayout.Label($"({referencedCondition.conditionName})", EditorStyles.miniLabel, GUILayout.Width(100), GUILayout.ExpandWidth(false));
            }
            else
            {
                GUILayout.Label("(None)", EditorStyles.miniLabel, GUILayout.Width(100), GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.LabelField(new GUIContent("Can Skip Early?", "If checked, this condition can complete the event during sub-events."), GUILayout.Width(100));
            allowEarlyProp.boolValue = EditorGUILayout.Toggle(GUIContent.none, allowEarlyProp.boolValue, GUILayout.Width(20));
            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(25)))
            {
                conditionRefProp.objectReferenceValue = null;
                completionConditionsProp.DeleteArrayElementAtIndex(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Completion Condition", GUILayout.Width(200)))
        {
            completionConditionsProp.arraySize++;
            SerializedProperty newElement = completionConditionsProp.GetArrayElementAtIndex(completionConditionsProp.arraySize - 1);
            newElement.FindPropertyRelative("condition").objectReferenceValue = null;
            newElement.FindPropertyRelative("allowEarlyCompletion").boolValue = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    // --- Helper to draw read-only string arrays nicely ---
    private void DrawReadOnlyStringArray(SerializedProperty arrayProp, string label)
    {
        if (arrayProp == null || !arrayProp.isArray || arrayProp.arraySize == 0) return;
        EditorGUI.BeginDisabledGroup(true);
        string displayString = "";
        for (int i = 0; i < arrayProp.arraySize; ++i)
        {
            displayString += arrayProp.GetArrayElementAtIndex(i).stringValue;
            if (i < arrayProp.arraySize - 1) displayString += ", ";
        }
        EditorGUILayout.TextField(label, displayString);
        EditorGUI.EndDisabledGroup();
    }
}
#endif
