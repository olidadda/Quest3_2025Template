#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoolCondition))]
public class BoolConditionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BoolCondition boolCondition = (BoolCondition)target;

        // Draw default inspector excluding the neededConditions field
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "neededConditions");

        // Custom field for needed conditions with better info
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Prerequisite Conditions", EditorStyles.boldLabel);

        SerializedProperty neededConditions = serializedObject.FindProperty("neededConditions");

        if (neededConditions.arraySize == 0)
        {
            EditorGUILayout.HelpBox("No prerequisites required for this condition.", MessageType.Info);
        }

        for (int i = 0; i < neededConditions.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Draw the object field
            SerializedProperty element = neededConditions.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(element, new GUIContent($"Prerequisite {i + 1}"));

            // Show condition name if assigned
            BoolCondition reference = (BoolCondition)element.objectReferenceValue;
            if (reference != null)
            {
                EditorGUILayout.LabelField(reference.conditionName, EditorStyles.boldLabel, GUILayout.Width(150));
            }

            // Remove button
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                neededConditions.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        // Add button
        if (GUILayout.Button("Add Prerequisite"))
        {
            neededConditions.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif